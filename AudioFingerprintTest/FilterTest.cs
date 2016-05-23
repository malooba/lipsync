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
using MaloobaFingerprint.FingerprintAnalyser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AudioFingerprintTest
{
    [TestClass]
    public class FilterTest
    {
        [TestMethod]
        public void ImpulseResponseTest()
        {
            var impulseResponse = new double[]
            {
                 0.65471754919057201,
                -0.41719110093654077,
                -0.29887670227185281,
                -0.17887455470804403,
                -0.07495271913491164,
                 0.00420364664357312,
                 0.05661585665848813,
                 0.08464252576943176,
                 0.09295574981587507,
                 0.08701260448316661,
                 0.07203835103345660,
                 0.05245619317269765,
                 0.03165979792384682,
                 0.01201734200381667,
                -0.00499199754056533,
                -0.01858620452338062,
                -0.02848579851610977,
                -0.03474661966479594,
                -0.03763251723486896,
                -0.03752819499746086,
                -0.03488535222082659
            };

            var bq1 = new Biquad(1.0, -1.973416440674660, 1.0, -1.450197273018358, 0.596145071222633);
            var bq2 = new Biquad(1.0, -1.994429119281506, 1.0, -1.880440530016805, 0.946873330944877);
            var g = 0.654717549190572;

            var input = 1.0;
            for(var i = 0; i < 20; i++)
            {
                var output = g * bq2.Filter(bq1.Filter(input));
                // This should be near enough :D
                Assert.IsTrue(Math.Abs(output - impulseResponse[i]) < 0.00000000000001);
                input = 0.0;
            }
        }
    }
}
