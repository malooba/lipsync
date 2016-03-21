﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;

namespace MaloobaLipSync.Correlator
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    internal class SyncZip<T, U> : IDisposable, IObservable<U> where T : IComparable<T>
    {
        private readonly Queue<T>[] q;         // Source queues
        private readonly bool[] done;          // Flag completion of sources
        private readonly Func<T, T, U> f;      // Combining function
        private readonly Subject<U> subject;   // Subject to manage subscriptions
        private readonly IDisposable[] subs;   // Our subscriptions

        public int QueueMax { get; set; } = 10000;

        /// <summary>
        /// Combine a stream of ordered events, x with another stream of events y using function f
        /// Events are combined only if their order parameters match.  Events which do not have a corresponding 
        /// event in the other stream are discarded.
        /// Both streams of events are assumed to have strictly increasing order parameters and events are 
        /// IComparable (which compares their order parameters)
        /// 
        /// The specific use case here is to compare audio/video fingerprints that have matching timecode
        /// where the timecode is the order parameter.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public SyncZip(IObservable<T> x, IObservable<T> y, Func<T, T, U> f)
        {
            this.f = f;
            done = new bool[2];         
            q = new Queue<T>[2];        
            q[0] = new Queue<T>();
            q[1] = new Queue<T>();

            subject = new Subject<U>();

            // Identify and merge sources
            subs = new IDisposable[2];
            subs[0] = x.Subscribe(v => OnNext(v, 0), OnError, () => OnCompleted(0));
            subs[1] = y.Subscribe(v => OnNext(v, 1), OnError, () => OnCompleted(1));
        }

        private void OnNext(T value, int index)
        {
            lock(q)
            {
                var other = q[1 - index];
                while(true)
                {
                    // Always enque if the other queue is empty
                    if(other.Count == 0)
                    {
                        q[index].Enqueue(value);

                        if(q.Length > QueueMax)
                            OnError(new ApplicationException("SyncZip sources are overly misaligned"));
                        return;
                    }

                    var comp = value.CompareTo(other.Peek());

                    // If the new value is smaller then discard it
                    if(comp < 0)
                        return;

                    // The other item will now be matched or discarded so dequeue it
                    var otherItem = other.Dequeue();

                    // We have a match!
                    if(comp == 0)
                    {
                        // call function with (item0, item 1)
                        var t = index == 0 ? f(value, otherItem) : f(otherItem, value);
                        subject.OnNext(t);
                        return;
                    }
                    // Loop until matched or value is enqueued
                }
            }
        }

        private void OnCompleted(int index)
        {
            subs[index].Dispose();
            done[index] = true;
            if(done[0] && done[1])
            {
                q[0].Clear();
                q[1].Clear();
                subject.OnCompleted();
            }
        }

        private void OnError(Exception ex)
        {
            subs[0]?.Dispose();
            subs[1]?.Dispose();
            q[0].Clear();
            q[1].Clear();
            subject.OnError(ex);
        }

        public IDisposable Subscribe(IObserver<U> obs)
        {
            return subject.Subscribe(obs);
        }

        public void Dispose()
        {
            subs[0].Dispose();
            subs[1].Dispose();
            subject.OnCompleted();
        }
    }
}