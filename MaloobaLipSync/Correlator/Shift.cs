using System;
using System.Text;

namespace MaloobaLipSync.Correlator
{
    public class Shift : EventArgs
    {
        public readonly uint Timecode;
        public readonly double[] Delay;
        public readonly double[] Confidence;
        public readonly bool[] AudioPresentA;
        public readonly bool[] AudioPresentB;

        public Shift(uint timecode)
        {
            Timecode = timecode;
            Delay = new double[16];
            Confidence = new double[16];
            AudioPresentA = new bool[16];
            AudioPresentB = new bool[16];

            for(var i = 0; i < 16; i++)
                Delay[i] = double.NaN;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Timecode.ToString("X8"));
            for(var i = 0; i < 16; i++)
            {
                if(double.IsNaN(Delay[i])) continue;
                sb.Append(" ");
                sb.Append(i.ToString("00"));
                sb.Append(":");
                sb.Append(Delay[i]);
                sb.Append(" ");
                sb.Append(Confidence[i].ToString("0.00"));
            }
            return sb.ToString();
        }
    }
}