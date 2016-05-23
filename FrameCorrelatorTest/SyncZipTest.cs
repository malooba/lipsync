using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MaloobaLipSync.Correlator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameCorrelatorTest
{
    [TestClass]
    public class SyncZipTest
    {
        /// <summary>
        /// Test normal operation
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            var ev1 = new [] { 1, 2,    4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,         17, 18, 19, 20 };
            var ev2 = new [] { 1, 2, 3, 4, 5,    7, 8, 9,             13, 14, 15, 16, 17, 18 };
            var expected = new [] { "1-1", "2-2", "4-4", "5-5", "7-7", "8-8", "9-9", "13-13", "14-14", "17-17", "18-18" };

            var s1 = new Subject<int>();
            var s2 = new Subject<int>();

            var observable = new SyncZip<int, string>(s1, s2, (x, y) => $"x-y");

            var results = observable.ToArray();
            for(var i = 0; i < 99; i++)
            {
                if(i < ev1.Length) s1.OnNext(ev1[i]);
                if(i < ev2.Length) s2.OnNext(ev2[i]);
            }
            s1.OnCompleted();
            s2.OnCompleted();

            results.Subscribe(r => CollectionAssert.AreEqual(expected, r));
        }

        /// <summary>
        /// Test unsubscription
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            var ev1 = new[] { 1, 2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 17, 18, 19, 20 };
            var ev2 = new[] { 1, 2, 3, 4, 5, 7, 8, 9, 13, 14, 15, 16, 17, 18 };
            var expected = new[] { "1-1", "2-2", "4-4", "5-5", "7-7", "8-8", "9-9", "13-13", "14-14", "17-17", "18-18" };

            var s1 = new Subject<int>();
            var s2 = new Subject<int>();

            var observable = new SyncZip<int, string>(s1, s2, (x, y) => $"x-y");

            IDisposable subs = null;
            subs = observable.Subscribe(x =>
            {
                if(x == "7-7") subs.Dispose();  // unsubscribe here
                if(x == "8-8") Assert.Fail();   // We should now be unsubscribed
            });

            var results = observable.ToArray();

            for(var i = 0; i < 99; i++)
            {
                if(i < ev1.Length) s1.OnNext(ev1[i]);
                if(i < ev2.Length) s2.OnNext(ev2[i]);
            }
            subs.Dispose();                     // Should be harmless
            s1.OnCompleted();
            s2.OnCompleted();
            subs.Dispose();                     // Should be harmless

            // This ahould still have worked
            results.Subscribe(r => CollectionAssert.AreEqual(expected, r));
        }
    }
}
