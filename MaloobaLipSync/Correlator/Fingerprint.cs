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
using System.Linq;

namespace MaloobaLipSync.Correlator
{
    /// <summary>
    /// A single fingerprint record from an analyser
    /// </summary>
    public class Fingerprint : IComparable<Fingerprint>
    {
        /// <summary>
        /// UDP packet version identifier
        /// </summary>
        private const int PACKET_VERSION = 1;

        public readonly uint Timecode;                       // BCD timecode
        public readonly int AudioSize;                       // Number of bits in each audio fingerprint 0-48
        public readonly ulong[] AudioFingerprints;           // Audio fingerprints

        /// <summary>
        /// Read a fingerprint from text (e.g. a line in a text file)
        /// The input fields are converted to a byte array to simulate an incoming UDP packet
        /// This approach means it is valid to test fingerprint creation using this method
        /// </summary>
        /// <param name="line"></param>
        public static Fingerprint Parse(string line)
        {
            var fields = line.Split(' ');
            var version = Convert.ToByte(fields[0], 16);
            var audioSize = Convert.ToByte(fields[1], 16);
            var timecode = Convert.ToUInt32(fields[2], 16);
            var audioFingerprints = fields.Skip(3).SelectMany(f => BitConverter.GetBytes(Convert.ToUInt64(f, 16))).ToArray();

            var bytes = new byte[audioFingerprints.Length + 6];
            bytes[0] = version;
            bytes[1] = audioSize;
            BitConverter.GetBytes(timecode).CopyTo(bytes, 2);
            audioFingerprints.CopyTo(bytes, 6);

            return new Fingerprint(bytes);
        }

        /// <summary>
        /// Read a fingerprint from a byte array (e.g. a UDP packet)
        /// </summary>
        /// <param name="b"></param>
        public Fingerprint(byte[] b)
        {
            // Returning now will create a Fingerprint with a zero timecode which should normally be ignored
            if(b[0] != PACKET_VERSION)
                return;

            // size must be 2 + 4 + 8 * f where f is 1, 2, 8 or 16
            // 2, 8, and 16 are the standard channel counts from decklink cards
            // 1 channel is used for merged L & R (mono) monitoring
            int chans;
            switch(b.Length)
            {
                case 2 + sizeof(uint) + 1 * sizeof(ulong):
                    chans = 1;
                    break;

                case 2 + sizeof(uint) + 2 * sizeof(ulong):
                    chans = 2;
                    break;

                // This is currently the only packet size used
                case 2 + sizeof(uint) + 8 * sizeof(ulong):
                    chans = 8;
                    break;

                case 2 + sizeof(uint) + 16 * sizeof(ulong):
                    chans = 16;
                    break;

                default:
                    // Returning now will create a Fingerprint with a zero timecode which should normally be ignored
                    return;
            }

            AudioSize = b[1];
            Timecode = BitConverter.ToUInt32(b, 2);
            AudioFingerprints = new ulong[Correlator.CHANNELS];
            for(var i = 0; i < chans; i++)
                AudioFingerprints[i] = BitConverter.ToUInt64(b, 2 + sizeof(uint) + i * sizeof(ulong));
        }

        private string TimecodeString
        {
            get
            {
                var s = Timecode.ToString("X8");
                return $"{s.Substring(0, 2)}:{s.Substring(2, 2)}:{s.Substring(4, 2)}:{s.Substring(6, 2)}";
            }
        }

        public override string ToString()
        {
            return TimecodeString;
        }

        /// <summary>
        /// Comparison function used by SyncZip to merge streams 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Fingerprint other)
        {
            return Math.Sign(Timecode - other.Timecode);
        }
    }
}
