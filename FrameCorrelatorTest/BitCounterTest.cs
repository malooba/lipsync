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

using MaloobaLipSync.Correlator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameCorrelatorTest
{
    [TestClass]
    public class BitCounterTest
    {
        [TestMethod]
        public void CountUlongBits()
        {
            Assert.AreEqual(0, BitCounter.Count(0x0000000000000000L));
            Assert.AreEqual(64, BitCounter.Count(0xFFFFFFFFFFFFFFFFL));
            Assert.AreEqual(32, BitCounter.Count(0x5A5A5A5A5A5A5A5AL));
            Assert.AreEqual(32, BitCounter.Count(0xA5A5A5A5A5A5A5A5L));
            Assert.AreEqual(1, BitCounter.Count(0x0000000000000001L));
            Assert.AreEqual(2, BitCounter.Count(0x0000000000000003L));
            Assert.AreEqual(3, BitCounter.Count(0x0000000000000007L));
            Assert.AreEqual(4, BitCounter.Count(0x000000000000000FL));
            Assert.AreEqual(5, BitCounter.Count(0x000000000000001FL));
            Assert.AreEqual(6, BitCounter.Count(0x000000000000003FL));
            Assert.AreEqual(7, BitCounter.Count(0x000000000000007FL));
            Assert.AreEqual(8, BitCounter.Count(0x00000000000000FFL));
            Assert.AreEqual(9, BitCounter.Count(0x00000000000001FFL));
            Assert.AreEqual(10, BitCounter.Count(0x00000000000003FFL));
            Assert.AreEqual(11, BitCounter.Count(0x00000000000007FFL));
            Assert.AreEqual(12, BitCounter.Count(0x0000000000000FFFL));
            Assert.AreEqual(13, BitCounter.Count(0x0000000000001FFFL));
            Assert.AreEqual(14, BitCounter.Count(0x0000000000003FFFL));
            Assert.AreEqual(15, BitCounter.Count(0x0000000000007FFFL));
            Assert.AreEqual(16, BitCounter.Count(0x000000000000FFFFL));
            Assert.AreEqual(17, BitCounter.Count(0x000000000001FFFFL));
            Assert.AreEqual(18, BitCounter.Count(0x000000000003FFFFL));
            Assert.AreEqual(19, BitCounter.Count(0x000000000007FFFFL));
            Assert.AreEqual(20, BitCounter.Count(0x00000000000FFFFFL));
            Assert.AreEqual(21, BitCounter.Count(0x00000000001FFFFFL));
            Assert.AreEqual(22, BitCounter.Count(0x00000000003FFFFFL));
            Assert.AreEqual(23, BitCounter.Count(0x00000000007FFFFFL));
            Assert.AreEqual(24, BitCounter.Count(0x0000000000FFFFFFL));
            Assert.AreEqual(25, BitCounter.Count(0x0000000001FFFFFFL));
            Assert.AreEqual(26, BitCounter.Count(0x0000000003FFFFFFL));
            Assert.AreEqual(27, BitCounter.Count(0x0000000007FFFFFFL));
            Assert.AreEqual(28, BitCounter.Count(0x000000000FFFFFFFL));
            Assert.AreEqual(29, BitCounter.Count(0x000000001FFFFFFFL));
            Assert.AreEqual(30, BitCounter.Count(0x000000003FFFFFFFL));
            Assert.AreEqual(31, BitCounter.Count(0x000000007FFFFFFFL));
            Assert.AreEqual(32, BitCounter.Count(0x00000000FFFFFFFFL));
            Assert.AreEqual(1 + 32, BitCounter.Count(0x00000001FFFFFFFFL));
            Assert.AreEqual(2 + 32, BitCounter.Count(0x00000003FFFFFFFFL));
            Assert.AreEqual(3 + 32, BitCounter.Count(0x00000007FFFFFFFFL));
            Assert.AreEqual(4 + 32, BitCounter.Count(0x0000000FFFFFFFFFL));
            Assert.AreEqual(5 + 32, BitCounter.Count(0x0000001FFFFFFFFFL));
            Assert.AreEqual(6 + 32, BitCounter.Count(0x0000003FFFFFFFFFL));
            Assert.AreEqual(7 + 32, BitCounter.Count(0x0000007FFFFFFFFFL));
            Assert.AreEqual(8 + 32, BitCounter.Count(0x000000FFFFFFFFFFL));
            Assert.AreEqual(9 + 32, BitCounter.Count(0x000001FFFFFFFFFFL));
            Assert.AreEqual(10 + 32, BitCounter.Count(0x000003FFFFFFFFFFL));
            Assert.AreEqual(11 + 32, BitCounter.Count(0x000007FFFFFFFFFFL));
            Assert.AreEqual(12 + 32, BitCounter.Count(0x00000FFFFFFFFFFFL));
            Assert.AreEqual(13 + 32, BitCounter.Count(0x00001FFFFFFFFFFFL));
            Assert.AreEqual(14 + 32, BitCounter.Count(0x00003FFFFFFFFFFFL));
            Assert.AreEqual(15 + 32, BitCounter.Count(0x00007FFFFFFFFFFFL));
            Assert.AreEqual(16 + 32, BitCounter.Count(0x0000FFFFFFFFFFFFL));
            Assert.AreEqual(17 + 32, BitCounter.Count(0x0001FFFFFFFFFFFFL));
            Assert.AreEqual(18 + 32, BitCounter.Count(0x0003FFFFFFFFFFFFL));
            Assert.AreEqual(19 + 32, BitCounter.Count(0x0007FFFFFFFFFFFFL));
            Assert.AreEqual(20 + 32, BitCounter.Count(0x000FFFFFFFFFFFFFL));
            Assert.AreEqual(21 + 32, BitCounter.Count(0x001FFFFFFFFFFFFFL));
            Assert.AreEqual(22 + 32, BitCounter.Count(0x003FFFFFFFFFFFFFL));
            Assert.AreEqual(23 + 32, BitCounter.Count(0x007FFFFFFFFFFFFFL));
            Assert.AreEqual(24 + 32, BitCounter.Count(0x00FFFFFFFFFFFFFFL));
            Assert.AreEqual(25 + 32, BitCounter.Count(0x01FFFFFFFFFFFFFFL));
            Assert.AreEqual(26 + 32, BitCounter.Count(0x03FFFFFFFFFFFFFFL));
            Assert.AreEqual(27 + 32, BitCounter.Count(0x07FFFFFFFFFFFFFFL));
            Assert.AreEqual(28 + 32, BitCounter.Count(0x0FFFFFFFFFFFFFFFL));
            Assert.AreEqual(29 + 32, BitCounter.Count(0x1FFFFFFFFFFFFFFFL));
            Assert.AreEqual(30 + 32, BitCounter.Count(0x3FFFFFFFFFFFFFFFL));
            Assert.AreEqual(31 + 32, BitCounter.Count(0x7FFFFFFFFFFFFFFFL));
            Assert.AreEqual(32 + 32, BitCounter.Count(0xFFFFFFFFFFFFFFFFL));
        }

        [TestMethod]
        public void CountIntBits()
        {
            Assert.AreEqual(0, BitCounter.Count(0x00000000L));
            Assert.AreEqual(32, BitCounter.Count(0xFFFFFFFFL));
            Assert.AreEqual(16, BitCounter.Count(0x5A5A5A5AL));
            Assert.AreEqual(16, BitCounter.Count(0xA5A5A5A5L));
            Assert.AreEqual(1, BitCounter.Count(0x00000001L));
            Assert.AreEqual(2, BitCounter.Count(0x00000003L));
            Assert.AreEqual(3, BitCounter.Count(0x00000007L));
            Assert.AreEqual(4, BitCounter.Count(0x0000000FL));
            Assert.AreEqual(5, BitCounter.Count(0x0000001FL));
            Assert.AreEqual(6, BitCounter.Count(0x0000003FL));
            Assert.AreEqual(7, BitCounter.Count(0x0000007FL));
            Assert.AreEqual(8, BitCounter.Count(0x000000FFL));
            Assert.AreEqual(9, BitCounter.Count(0x000001FFL));
            Assert.AreEqual(10, BitCounter.Count(0x000003FFL));
            Assert.AreEqual(11, BitCounter.Count(0x000007FFL));
            Assert.AreEqual(12, BitCounter.Count(0x00000FFFL));
            Assert.AreEqual(13, BitCounter.Count(0x00001FFFL));
            Assert.AreEqual(14, BitCounter.Count(0x00003FFFL));
            Assert.AreEqual(15, BitCounter.Count(0x00007FFFL));
            Assert.AreEqual(16, BitCounter.Count(0x0000FFFFL));
            Assert.AreEqual(17, BitCounter.Count(0x0001FFFFL));
            Assert.AreEqual(18, BitCounter.Count(0x0003FFFFL));
            Assert.AreEqual(19, BitCounter.Count(0x0007FFFFL));
            Assert.AreEqual(20, BitCounter.Count(0x000FFFFFL));
            Assert.AreEqual(21, BitCounter.Count(0x001FFFFFL));
            Assert.AreEqual(22, BitCounter.Count(0x003FFFFFL));
            Assert.AreEqual(23, BitCounter.Count(0x007FFFFFL));
            Assert.AreEqual(24, BitCounter.Count(0x00FFFFFFL));
            Assert.AreEqual(25, BitCounter.Count(0x01FFFFFFL));
            Assert.AreEqual(26, BitCounter.Count(0x03FFFFFFL));
            Assert.AreEqual(27, BitCounter.Count(0x07FFFFFFL));
            Assert.AreEqual(28, BitCounter.Count(0x0FFFFFFFL));
            Assert.AreEqual(29, BitCounter.Count(0x1FFFFFFFL));
            Assert.AreEqual(30, BitCounter.Count(0x3FFFFFFFL));
            Assert.AreEqual(31, BitCounter.Count(0x7FFFFFFFL));
            Assert.AreEqual(32, BitCounter.Count(0xFFFFFFFFL));
        }
    }
}
