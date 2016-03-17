using System;
using System.Collections;
using System.Collections.Generic;

namespace MaloobaFingerprint.Analyser
{
    /// <summary>
    /// Create BCD timecode
    /// </summary>
    class TimecodeGenerator : IEnumerable<uint>
    {
        private readonly byte [] bcd = new byte[4];
        private readonly int fps;

        public TimecodeGenerator(int fps)
        {
            if(fps > 99)
                throw new ArgumentException("fps must be < 100", nameof(fps));
            this.fps = ((fps / 10) << 4) + fps % 10;
        }

        public IEnumerator<uint> GetEnumerator()
        {
            while(true)
            {
                yield return BitConverter.ToUInt32(bcd, 0);
                IncBcd(ref bcd[0]);
                if(bcd[0] < fps) continue;
                bcd[0] = 0;
                IncBcd(ref bcd[1]);
                if(bcd[1] < 0x60) continue;
                bcd[1] = 0;
                IncBcd(ref bcd[2]);
                if(bcd[2] < 0x60) continue;
                bcd[2] = 0;
                IncBcd(ref bcd[3]);
            }
        }

        void IncBcd(ref byte b)
        {
            b++;
            if((b & 0x0F) == 0xA)
                b += 6;
            if((b & 0xF0) == 0xA0)
                b = 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
