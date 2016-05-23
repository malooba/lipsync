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
using System.Runtime.InteropServices;
using MaloobaFingerprint.FingerprintAnalyser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AudioFingerprintTest
{
    [TestClass]
    public class AudioFingerprintTest
    {
        [TestMethod]
        public void Test1()
        {
            var config = new AnalyserConfig
            {
                FirLength = 20,
                SamplesPerSlot = 48,
                SlotsPerFrame = 40
            };
            var samplesPerFrame = config.SamplesPerSlot * config.SlotsPerFrame;
            var sampleBuffer = new short[samplesPerFrame * Analyser.CHANNELS];

            for(var i = 0; i < samplesPerFrame; i++)
            {
                var sin4k = (short)(10000 * Math.Sin(2 * Math.PI * i / 12));
                var sin1k = (short)(10000 * Math.Sin(2 * Math.PI * i / 48));

                var l = ((i / 48) & 1) == 0 ? sin4k : sin1k;
                var r = ((i / 48) & 1) != 0 ? sin4k : sin1k;

                sampleBuffer[i * Analyser.CHANNELS] = l;
                sampleBuffer[(i * Analyser.CHANNELS) + 1] = r;
            }

            var cb = new Callback(config);

            var handle = GCHandle.Alloc(sampleBuffer, GCHandleType.Pinned);
            var buf = handle.AddrOfPinnedObject();
            var fingerprints1 = cb.GetAudioFingerprints(buf, config);
            var fingerprints2 = cb.GetAudioFingerprints(buf, config);

            Assert.AreEqual(0x800000FFFFF55555UL, fingerprints1[0]);
            Assert.AreEqual(0x800000FFFFFAAAAAUL, fingerprints1[1]);
            Assert.AreEqual(0x8000005555555555UL, fingerprints2[0]);
            Assert.AreEqual(0x800000AAAAAAAAAAUL, fingerprints2[1]);
            handle.Free();
        }
    }
}
