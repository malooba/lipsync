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
    /// 2kHz 4th order elliptic highpass filter
    /// Fs = 48kHz
    /// </summary>
    public class Hpf
    {
        private readonly Biquad bq1;
        private readonly Biquad bq2;
        private readonly double g;

        /// <summary>
        /// Construct a fourth order filter from two Biquad elements
        /// </summary>
        public Hpf()
        {
            // 2 kHz filter
            bq1 = new Biquad(1.0, -1.973416440674660, 1.0, -1.450197273018358, 0.596145071222633);
            bq2 = new Biquad(1.0, -1.994429119281506, 1.0, -1.880440530016805, 0.946873330944877);
            g = 0.654717549190572;

            // 1 kHz filter
            //bq1 = new Biquad(1.0, -1.993377987075336, 1.0, -1.727679913361968, 0.769204050974404);
            //bq2 = new Biquad(1.0, -1.998617772299150, 1.0, -1.955947061007005, 0.972849469079112);
            //g = 0.766814731321591;
        }

        /// <summary>
        /// Filter a single sample
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double Filter(double input)
        {
            return g * bq2.Filter(bq1.Filter(input));
        }
    }
}
