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

using System.Linq;
using MaloobaLipSync.Correlator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameCorrelatorTest
{
    [TestClass]
    public class AudioCorrelatorTest
    {
        private string[] audioFingerprints =
        {
            "C09E545F86",
            "B5D03E8B2D",
            "F1B6FFA582",
            "69CD20901F",
            "A67A2FEF4D",
            "8578CC9C3C",
            "38739C176C",
            "E3307FB424",
            "92C582E4C7",
            "FD547A7966",
            "7C52607972",
            "51F255730C",
            "94A0B1B785",
            "48BE0DEA82",
            "B16ABD779A",
            "261EB9F7C2",
            "1046CF4FD8",
            "4292CF7065",
            "51D958F2B4",
            "9E9DDE391E",
            "BFF8E6B689",
            "F54004A204",
            "49346A3BB4",
            "9AD7270ED6",
            "7D8C08EEA7"
        };

        [TestMethod]
        public void TestExactMatch()
        {
            var expected = new[]
            {  -52,  -46,  -47,  -46,  -65,  -62,  -74,  -66,  -64,  -63,
               -57,  -55,  -62,  -65,  -70,  -60,  -68,  -64,  -63,  -58,
               -59,  -54,  -74,  -45,  -58,  -63,  -63,  -44,  -75,  -72,
               -62,  -56,  -61,  -53,  -78,  -67,  -62,  -67,  -57,  -59,
               -74,  -66,  -65,  -58,  -101, -72,  -66,  -56,  -62,  -77,
               -71,  -69,  -52,  -63,  -84,  -64,  -58,  -58,  -59,  -66,
               -83,  -86,  -86,  -73,  -76,  -75,  -79,  -74,  -73,  -62,
               -86,  -70,  -63,  -67,  -56,  -73,  -60,  -79,  -81,  -49,
               -87,  -63,  -68,  -51,  -64,  -47,  -81,  -71,  -63,  -72,
               -70,  -74,  -53,  -66,  -75,  -67,  -97,  -71,  -58,  -73,
               -76,  -65,  -81,  -82,  -83,  -84,  -84,  -77,  -68,  -63,
               -67,  -55,  -96,  -82,  -63,  -60,  -67,  -58,  -56,  -62,
               -55,  -55,  -64,  -69,  -70,  -71,  -41,  -67,  -63,  -60,
               -56,  -66,  -71,  -94,  -79,  -65,  -67,  -59,  -48,  -91,
               -68,  -77,  -57,  -80,  -61,  -82,  -76,  -61,  -64,  -71,
               -65,  -57,  -74,  -62,  -45,  -46,  -49,  -50,  -60,  -74,
               -40,  -74,  -51,  -62,  -61,  -70,  -72,  -42,  -54,  -53,
               -85,  -55,  -88,  -47,  -48,  -78,  -66,  -78,  -71,  -86,
               -59,  -78,  -74,  -73,  -38,  -85,  -79,  -66,  -89,  -72,
               -68,  -76,  -65,  -51,  -58,  -77,  -86,  -83,  -73,  -67,
               256,  -42,  -51,  -64,  -63,  -48,  -34,  -24,  -40,  -47,  // Correlation spike at start of line
               -45,  -47,  -70,  -45,  -58,  -58,  -10,  -52,  -53,  -60,
               -39,  -68,  -48,  -53,  -46,  -57,  -29,  -30,  -71,  -36,
               -64,  -36,  -27,  -17,  -48,  -51,  -42,  -39,  -37,  -57,
               -39,  -63,  -52,  -49,  -44,  -41,  -53,  -69,  -65,  -54,
               -60,  -70,  -63,  -60,  -79,  -87,  -55,  -75,  -52,  -67,
               -62,  -85,  -55,  -56,  -57,  -66,  -70,  -81,  -66,  -61,
               -55,  -59,  -58,  -66,  -37,  -54,  -63,  -66,  -70,  -48,
               -62,  -60,  -47,  -62,  -65,  -54,  -56,  -86, -108,  -57,
               -57,  -63,  -70,  -71,  -82,  -64,  -90,  -72,  -77,  -70,
               -75,  -64,  -58,  -63,  -88,  -63,  -75,  -68,  -59,  -68,
               -68,  -76,  -65,  -77,  -78,  -53,  -70,  -59,  -81,  -59,
               -80,  -56,  -77,  -88,  -57,  -64,  -46,  -58,  -66,  -73,
               -75,  -67,  -80,  -69,  -64,  -52,  -62,  -68,  -79,  -82,
               -77,  -56,  -46,  -49,  -48,  -61,  -73,  -60,  -47,  -66,
               -62,  -60,  -61,  -49,  -52,  -65,  -88,  -49,  -67,  -61,
               -67,  -47,  -62,  -71,  -68,  -73,  -77,  -49,  -69,  -64,
               -56,  -72,  -89,  -56,  -71,  -67,  -65,  -41,  -74,  -67,
               -74,  -63,  -79,  -56,  -51,  -50,  -68,  -69,  -76,  -55,
               -71,  -67,  -66,  -64,  -71,  -76,  -63,  -64,  -48,  -74 };

            bool presentA, presentB;
            // Fingerprints for second channel have data (A5A5A5) in the unused bits of the audio fingerprint
            var fingerprints =
                audioFingerprints.Select(a => Fingerprint.Parse($"01 28 12345612 800000{a} A5A5A5{a}")).ToList();

            var corr1 = AudioCorrelator.Correlate(0, fingerprints.Skip(5), 15, fingerprints, 25, out presentA, out presentB);

            CollectionAssert.AreEqual(expected, corr1);
            Assert.IsTrue(presentA);
            Assert.IsTrue(presentB);

            // Check that unused fingerprint bits are ignored 
            var corr2 = AudioCorrelator.Correlate(1, fingerprints.Skip(5), 15, fingerprints, 25, out presentA, out presentB);

            CollectionAssert.AreEqual(expected, corr2);
            Assert.IsTrue(presentA);
            Assert.IsTrue(presentB);
        }

        [TestMethod]
        public void TestEmptyMatch()
        {
            bool presentA, presentB;
            var fingerprints = audioFingerprints.Select(a => Fingerprint.Parse($"01 28 12345612 800000{a}")).ToList();
            var empty = audioFingerprints.Select(a => Fingerprint.Parse($"01 28 12345612 0000000000000000")).ToList();

            var corr1 = AudioCorrelator.Correlate(0, empty.Skip(5), 15, fingerprints, 25, out presentA, out presentB);
            CollectionAssert.AreEqual(null, corr1);
            Assert.IsFalse(presentA);
            Assert.IsTrue(presentB);

            var corr2 = AudioCorrelator.Correlate(0, fingerprints.Skip(5), 15, empty, 25, out presentA, out presentB);
            CollectionAssert.AreEqual(null, corr2);
            Assert.IsTrue(presentA);
            Assert.IsFalse(presentB);
        }
    }
}
