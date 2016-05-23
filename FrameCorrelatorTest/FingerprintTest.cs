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
using MaloobaFingerprint.FingerprintAnalyser;
using MaloobaLipSync.Correlator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameCorrelatorTest
{
    [TestClass]
    public class FingerprintTest
    {
        [TestMethod]
        public void TestFingerprintConstruction1()
        {
           
            var fingerprint = Fingerprint.Parse("01 28 12345612 800000123456789A 800000ABCDEF0123");
            Assert.AreEqual(0x12345612U, fingerprint.Timecode);
            Assert.AreEqual("12:34:56:12", fingerprint.ToString());
            Assert.AreEqual(40, fingerprint.AudioSize);
            Assert.AreEqual(Analyser.CHANNELS, fingerprint.AudioFingerprints.Length);
            Assert.AreEqual(0x800000123456789AUL, fingerprint.AudioFingerprints[0]);
            Assert.AreEqual(0x800000ABCDEF0123UL, fingerprint.AudioFingerprints[1]);
            Assert.IsTrue(fingerprint.AudioFingerprints.Skip(2).All(f => f == 0));
        }
    }
}
