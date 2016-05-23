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
    /// Biquad digital filter
    /// </summary>
    public class Biquad
    {
        // Filter coefficients
        private readonly double b0;
        private readonly double b1;
        private readonly double b2;
        private readonly double a1;
        private readonly double a2;

        // Delay buffers
        private double z1;
        private double z2;

        public Biquad(double b0, double b1, double b2, double a1, double a2)
        {
            this.b0 = b0;
            this.b1 = b1;
            this.b2 = b2;
            this.a1 = a1;
            this.a2 = a2;
        }

        /// <summary>
        /// Filter a single input sample
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double Filter(double input)
        {
            var z0 = input - a1 * z1 - a2 * z2;
            var output = b0 * z0 + b1 * z1 + b2 * z2;
            z2 = z1;
            z1 = z0;
            return output;
        }
    }
}
