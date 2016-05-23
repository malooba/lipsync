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

namespace MaloobaFingerprint.FingerprintAnalyser
{
    /// <summary>
    /// All of the storage required to fingerprint an audio channel
    /// </summary>
    public class Channel
    {
        // Three slot audio buffer
        public double[] Slot0;
        public double[] Slot1;
        public double[] Slot2;

        // Rms slot values
        public readonly double[] RmsBuffer;

        // Filter
        public readonly Hpf Filter;

        /// <summary>
        /// Construct the storage for an audio channel
        /// </summary>
        /// <param name="config"></param>
        public Channel(AnalyserConfig config)
        {
            Slot0 = new double[config.SamplesPerSlot];
            Slot1 = new double[config.SamplesPerSlot];
            Slot2 = new double[config.SamplesPerSlot];
            RmsBuffer = new double[config.FirLength];
            Filter = new Hpf();
        }
    }
}