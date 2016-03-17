using DeckLinkAPI;

namespace MaloobaFingerprint.ViewModel
{
    /// <summary>
    /// Video input modes
    /// </summary>
    public class VideoMode
    {
        /// <summary>
        /// Mode name displayed on the UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Black Magic recorder video mode
        /// </summary>
        public _BMDDisplayMode Mode { get; set; }

        /// <summary>
        /// Frames per second * 1000
        /// </summary>
        public int Fps;

        /// <summary>
        /// Number of audio slots per frame
        /// </summary>
        public int SlotsPerFrame => 40;

        /// <summary>
        /// Number of audio samples per slot
        /// This will be zero for unimplemented frame rates
        /// </summary>
        public int SamplesPerSlot
        {
            get
            {
                if(Fps == 25000)
                    return 48;
                if(Fps == 24000)
                    return 50;
                return 0;
            }
        }
    }
}