using System;
using System.Text;
using System.Collections.Generic;
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
