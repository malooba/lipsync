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
using System.Collections.Generic;
using System.Linq;

namespace MaloobaLipSync.Correlator
{
    internal static class AudioCorrelator
    {
        /// <summary>
        /// Perform an audio fingerprint correlation
        /// n signatures, taken from s1 are compared with m signatures taken from s2
        /// m must be greater than n.
        /// The output is an array of coincident bits for each offset of the (n - m) * NBITS possible
        /// where NBITS is the number of bits per signature (frame)
        /// 
        /// The expected alignment should be with s1 located centrally in s2
        /// e.g. if n = 9, m = 19, the expected alignment should look like this:
        ///       #########
        ///  ###################
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="s1"></param>
        /// <param name="n"></param>
        /// <param name="s2"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int[] Correlate(int channel, IEnumerable<Fingerprint> s1, int n, IEnumerable<Fingerprint> s2, int m, out bool chPresent1, out bool chPresent2)
        {
            int nbits = 0;
            chPresent1 = false;
            chPresent2 = false;
            var bufn = new ulong[n + 1];
            var mask = 0UL;
            var i = 0;
            foreach(var s in s1)
            {
                if(nbits == 0)
                {
                    nbits = s.AudioSize;
                    mask = (1UL << nbits) - 1UL;
                }
                else if(nbits != s.AudioSize)
                    throw new ApplicationException("Incompatible audio fingerprints");

                bufn[i++] = s.AudioFingerprints[channel] & mask;
                if((s.AudioFingerprints[channel] & 0x8000000000000000) != 0)
                    chPresent1 = true;

                if(i > n) break;
            }

            var bufm = new ulong[m];
            i = 0;
            foreach(var s in s2)
            {
                if(nbits != s.AudioSize)
                    throw new ApplicationException("Incompatible audio fingerprints");

                bufm[i++] = s.AudioFingerprints[channel] & mask;
                if((s.AudioFingerprints[channel] & 0x8000000000000000) != 0)
                    chPresent2 = true;

                if(i >= m) break;
            }

            // If either buffer is empty then just return null
            // Note: This is principally checking the "signal present" bit, the MSB of the fingerprint
            if(bufn.All(b => b == 0) || bufm.All(b => b == 0))
                return null;

            var frameShifts = m - n;
            if(frameShifts < 0)
                throw new ApplicationException("needle bigger than haystack");

            // Set up the correlation results
            var result = new int[nbits * frameShifts];

            // Do the correlation
            for(var bitShift = 0; bitShift < nbits; bitShift++)
            {
                for(var frameShift = 0; frameShift < frameShifts; frameShift++)
                {
                    var count = 0;
                    for(var frame = 0; frame <= n; frame++)
                        count += 16 - BitCounter.Count(bufn[frame] ^ bufm[frame + frameShift]);
                    result[frameShift * nbits + bitShift] = count;
                }
                // Now shift bufn by one bit
                // Note that the signatures run in time from MSB to LSB so we shift right
                var carry = 0UL;
                for(var j = 0; j < n; j++)
                {
                    var carryout = bufn[j] & 1;
                    bufn[j] = (bufn[j] >> 1) | carry << (nbits - 1);
                    carry = carryout;
                }
            }
            return result;
        }
    }
}
