using DeckLinkAPI;

namespace MaloobaFingerprint.Analyser
{
    public class AnalyserConfig
    {
        public IDeckLinkInput Recorder { get; set; }
        public _BMDTimecodeFormat TimecodeFormat { get; set; }
        public ushort AudioChannelMask { get; set; }
        public int FirLength { get; set; }
        public int SamplesPerSlot { get; set; }
        public int SlotsPerFrame { get; set; }
        public _BMDDisplayMode VideoMode { get; set; }

        public int GetBufferChannels()
        {
            // Get the number of channels in the decklink buffer
            if(AudioChannelMask > 0x00FF)
                return 16;

            if(AudioChannelMask > 0x0003)
                return 8;

            return 2;
        }
    }
}
