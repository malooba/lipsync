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
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MaloobaLipSync.Correlator
{
    class Correlator
    {
        public const int CHANNELS = 8;

        public EventHandler<Shift> OutputCreated;
        private readonly CorrelatorConfig config;
        private readonly IPEndPoint inputA;
        private readonly IPEndPoint inputB;
        private readonly double[] medianBuffer;

        // Needle offset is the offset from the start of the haystack to the start of the needle
        // when aligned with zero delay
        private readonly int needleOffset;


        private SyncZip<Fingerprint, Tuple<Fingerprint, Fingerprint>> pairs;
        private IDisposable subs;
        private Task task;

        public Correlator(CorrelatorConfig config)
        {
            this.config = config;
            inputA = new IPEndPoint(IPAddress.Parse(config.HostA), ushort.Parse(config.PortA));
            inputB = new IPEndPoint(IPAddress.Parse(config.HostB), ushort.Parse(config.PortB));
            medianBuffer = new double[config.CleanupFrames];
            needleOffset = (config.HaystackFrames - config.NeedleFrames) >> 1;
        }

        public void Start()
        {
            if(task == null)
                task = Task.Run((Action)ProcessInputs);
        }

        /// <summary>
        /// The main task that processes fingerprints and produces shift values
        /// </summary>
        private void ProcessInputs()
        {
            // Create a sequence of fingerprints from the first source
            var s1 = UdpObservable.Create(inputA).Select(b => new Fingerprint(b));

            // Create a sequence of fingerprints from the second source
            var s2 = UdpObservable.Create(inputB).Select(b => new Fingerprint(b));

            // Create a sequence of paired fingerprints with matching timecodes
            pairs = new SyncZip<Fingerprint, Tuple<Fingerprint, Fingerprint>>(s1, s2, Tuple.Create);

            // Correlate on overlapping windows of pairs
            var shifts = pairs.Window(config.HaystackFrames, config.StepFrames).SelectMany(x => x.ToArray().Select(Correlate));

            // Median filter on overlapping windows of shift results
            var cleanShifts = shifts.Window(config.CleanupFrames, 1).SelectMany(x => x.ToArray().Select(Cleanup));

            // Send the results to the output
            subs = cleanShifts.Subscribe(s => OutputCreated?.Invoke(this, s));
        }

        public void Stop()
        {
            if(task == null) return;
            subs.Dispose();
            pairs.Dispose();
            task.Wait();
        }

        /// <summary>
        /// Correlate the fingerprints
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private Shift Correlate(Tuple<Fingerprint, Fingerprint>[] a)
        {
            var chans = a[0].Item1.AudioFingerprints.Length;
            var shift = new Shift(a[needleOffset].Item1.Timecode);
            for(var ch = 0; ch < chans; ch++)
            {
                bool chPresentA;
                bool chPresentB;

                var corr = AudioCorrelator.Correlate(ch, 
                                                     a.Select(i => i.Item2).Skip(needleOffset), config.NeedleFrames, 
                                                     a.Select(i => i.Item1), config.HaystackFrames, 
                                                     out chPresentA, 
                                                     out chPresentB);
                shift.AudioPresentA[ch] = chPresentA;
                shift.AudioPresentB[ch] = chPresentB;
                if(corr == null)
                    continue;

                double confidence;
                var delay = GetOffset(corr, out confidence);
                shift.Delay[ch] = delay;
                shift.Confidence[ch] = confidence;
            }
            return shift;
        }

        /// <summary>
        /// Median filter output shifts and select the appropriate confidence value
        /// </summary>
        /// <param name="shiftBlock"></param>
        /// <returns></returns>
        private Shift Cleanup(Shift[] shiftBlock)
        {
            var shift = new Shift(shiftBlock[0].Timecode);

            for(var ch = 0; ch < 16; ch++)
            {
                shift.AudioPresentA[ch] = shiftBlock.All(s => s.AudioPresentA[ch]);
                shift.AudioPresentB[ch] = shiftBlock.All(s => s.AudioPresentB[ch]);

                if(double.IsNaN(shiftBlock[0].Delay[ch]))
                    continue;
                for(var j = 0; j < config.CleanupFrames; j++)
                    medianBuffer[j] = shiftBlock[j].Delay[ch];
                Array.Sort(medianBuffer);
                shift.Delay[ch] = medianBuffer[config.CleanupFrames >> 1];
                if(double.IsNaN(shift.Delay[ch]))
                    continue;
                // Select the maximum confidence in the input block with a delay matching the median delay
                shift.Confidence[ch] = shiftBlock.Where(s => Math.Abs(s.Delay[ch] - shift.Delay[ch]) < 0.4).Select(s => s.Confidence[ch]).Max();
            }
            return shift;
        }

        /// <summary>
        /// Find a single narrow peak in the correlation results and derive a confidence measure
        /// Return int.MaxValue if there is no single narrow peak.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="confidence"></param>
        /// <returns></returns>
        private double GetOffset(int[] result, out double confidence)
        {
            confidence = 0.0;
            if(result == null)
                return int.MaxValue;

            var peak = result.Max();
            if(peak < 10.0)
                return int.MaxValue;

            var first = -1;
            var last = -1;
            for(var i = 0; i < result.Length; i++)
            {
                if(result[i] == peak)
                {
                    if(first == -1)
                        first = i;
                    last = i;
                }
            }

            if(last - first < 3)
            {
                var pk1 = 1.0;
                var pk2 = 1.0;
                if(first - 5 > 0)
                    pk1 = result.Take(first - 5).Max();
                if(last + 5 < result.Length - 1)
                    pk2 = result.Skip(last + 5).Max();

                confidence = (peak - Math.Max(pk1, pk2)) / peak;
                return ((double)last + first - result.Length) / 2.0;
            }

            return int.MaxValue;
        }
    }
}
