using System;
using System.Linq;

namespace MaloobaLipSync.Correlator
{
    /// <summary>
    /// A single fingerprint record from an analyser
    /// </summary>
    internal class Fingerprint : IComparable<Fingerprint>
    {
        /// <summary>
        /// UDP packet version identifier
        /// </summary>
        private const int PACKET_VERSION = 1;

        public readonly uint Timecode;                       // BCD timecode
        public readonly int AudioSize;                       // Number of bits in each audio fingerprint 0-48
        public readonly ulong[] AudioFingerprints;           // Audio fingerprints

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

                case 2 + sizeof(uint) + 8 * sizeof(ulong):
                    chans = 8;
                    break;

                // This is currently the only packet size used
                case 2 + sizeof(uint) + 16 * sizeof(ulong):
                    chans = 16;
                    break;

                default:
                    // Returning now will create a Fingerprint with a zero timecode which should normally be ignored
                    return;
            }

            AudioSize = b[1];
            Timecode = BitConverter.ToUInt32(b, 2);
            AudioFingerprints = new ulong[16];
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
