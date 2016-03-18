using System;

namespace MaloobaFingerprint.FingerprintAnalyser
{
    /// <summary>
    /// The fingerprint event outputs the fingerprint data
    /// </summary>
    public class FingerprintEventArgs : EventArgs
    {
        public readonly uint Timecode;
        public readonly byte SlotsPerFrame;
        public ushort VideoFingerprint { get; private set; }
        public ulong[] AudioFingerprints { get; private set; }
        public int[] VideoSegments { get; private set; }

        public FingerprintEventArgs(uint timecode, byte slotsPerFrame, ushort videoFingerprint, ulong[] audioFingerprints, int[] videoSegments = null)
        {
            Timecode = timecode;
            SlotsPerFrame = slotsPerFrame;
            VideoFingerprint = videoFingerprint;
            AudioFingerprints = audioFingerprints;
            VideoSegments = videoSegments;
        }
    }
}