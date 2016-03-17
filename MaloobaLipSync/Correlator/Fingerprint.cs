using System;
using System.Linq;

namespace MaloobaLipSync.Correlator
{
    /// <summary>
    /// A single fingerprint record from an analyser
    /// </summary>
    internal class Fingerprint : IComparable<Fingerprint>
    {
        public readonly uint Timecode;                       // BCD timecode
        public readonly ushort VideoFingerprint;             // 15 bits of video fingerprint
        public readonly int AudioSize;                       // Number of bits in each audio fingerprint 0-64
        public readonly ulong[] AudioFingerprints;           // Audio fingerprints

        /// <summary>
        /// Read a fingerprint from text (e.g. a line in a text file)
        /// </summary>
        /// <param name="line"></param>
        public Fingerprint(string line)
        {
            var fields = line.Split(' ');
            Timecode = Convert.ToUInt32(fields[0], 10);
            VideoFingerprint = Convert.ToUInt16(fields[1], 16);
            AudioSize = Convert.ToInt32(fields[2], 16);
            AudioFingerprints = fields.Skip(3).Select(f => Convert.ToUInt64(f, 16)).ToArray();
        }

        /// <summary>
        /// Read a fingerprint from a byte array (e.g. a UDP packet)
        /// </summary>
        /// <param name="b"></param>
        public Fingerprint(byte[] b)
        {
            // size must be 2 + 4 + 8 * f where f is 1, 2, 8 or 16
            // 2, 8, and 16 are the standard channel counts from decklink cards
            // 1 channel is used for merged L & R (mono) monitoring
            int chans;
            switch(b.Length)
            {
                case 6 + 1 * 8:
                    chans = 1;
                    break;

                case 6 + 2 * 8:
                    chans = 2;
                    break;

                case 6 + 8 * 8:
                    chans = 8;
                    break;

                case 6 + 16 * 8:
                    chans = 16;
                    break;

                default:
                    // Returning now will create a Fingerprint with a zero timecode which should normally be ignored
                    return;
            }

            Timecode = BitConverter.ToUInt32(b, 2);
            //VideoFingerprint = BitConverter.ToUInt16(b, 4);
            AudioSize = b[1];
            AudioFingerprints = new ulong[16];
            for(var i = 0; i < chans; i++)
                AudioFingerprints[i] = BitConverter.ToUInt64(b, 6 + i * 8);
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
