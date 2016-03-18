using DeckLinkAPI;

namespace MaloobaFingerprint.FingerprintAnalyser
{
    public class AnalyserConfig
    {
        public IDeckLinkInput Recorder { get; set; }
        public _BMDTimecodeFormat TimecodeFormat { get; set; }
        public int FirLength { get; set; }
        public int SamplesPerSlot { get; set; }
        public int SlotsPerFrame { get; set; }
        public _BMDDisplayMode VideoMode { get; set; }
    }
}
