//Copyright 2016 Malooba Ltd

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
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
        private readonly Queue<T> q;           // Queue of items from leading input
        private int qid;                       // Id of queued items
        private readonly bool[] done;          // Flag completion of sources
        private readonly Func<T, T, U> f;      // Combining function
        private readonly Subject<U> subject;   // Subject to manage subscriptions
        private readonly IDisposable[] subs;   // Our subscriptions

        public int QueueMax { get; set; } = 10000;

        /// <summary>
        /// Combine a stream of ordered messages, x with another stream of ordered messages y using function f
        /// Messages are combined only if their order parameters match.  Messages which do not have a corresponding 
        /// message in the other stream are discarded.
        /// Both streams of messages are assumed to have strictly increasing order parameters and messages are 
        /// IComparable (which compares their order parameters)
        /// 
        /// The specific use case here is to compare audio/video fingerprints that have matching timecode
        /// where the timecode is the order parameter.
        /// </summary>
        /// <param name="x">First stream of messages</param>
        /// <param name="y">Second stream of messages</param>
        /// <param name="f">Combining function</param>
        /// <returns></returns>
        public SyncZip(IObservable<T> x, IObservable<T> y, Func<T, T, U> f)
        {
            this.f = f;
            done = new bool[2];         
            q = new Queue<T>();        

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
                if(q.Count == 0) qid = index;

                if(qid == index)
                {
                    q.Enqueue(value);

                    // If the queue overflows then just dump it - presumably we only have one input
                    if(q.Count > QueueMax)
                        q.Clear();
                    return;
                }

                while(q.Count > 0)
                {
                    var comp = value.CompareTo(q.Peek());

                    // If the new value is earlier then discard it
                    if(comp < 0)
                        return;

                    // The queued item will now be matched or discarded so dequeue it
                    var otherItem = q.Dequeue();

                    // We have a match!
                    if(comp == 0)
                    {
                        // call function with (item0, item 1)
                        var t = index == 0 ? f(value, otherItem) : f(otherItem, value);
                        subject.OnNext(t);
                        return;
                    }
                    // Loop until value is matched, discarded or queue is empty
                }
                // Not matched - put value on the queue
                qid = index;
                q.Enqueue(value);
            }
        }

        private void OnCompleted(int index)
        {
            subs[index].Dispose();
            done[index] = true;
            if(done[0] && done[1])
            {
                q.Clear();
                subject.OnCompleted();
            }
        }

        private void OnError(Exception ex)
        {
            subs[0]?.Dispose();
            subs[1]?.Dispose();
            q.Clear();
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