using DeckLinkAPI;

namespace MaloobaFingerprint.ViewModel
{
    /// <summary>
    /// Timecode recovery modes
    /// </summary>
    public class TimecodeMode
    {
        /// <summary>
        /// Mode name displayed on the UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Black Magic timecode mode 
        /// </summary>
        public _BMDTimecodeFormat Mode { get; set; }
    }
}