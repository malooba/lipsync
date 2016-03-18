using System;
using DeckLinkAPI;

namespace MaloobaFingerprint.ViewModel
{
    /// <summary>
    /// Represents a Black Magic Mini-recorder or a dummy device for design mode
    /// </summary>
    public class InputDevice
    {
        /// <summary>
        /// Mini-recorder device
        /// </summary>
        public IDeckLinkInput Device => device;

        public int Channels => channels;

        /// <summary>
        /// Display name of device
        /// </summary>
        public string Name => name;

        private readonly IDeckLinkInput device;
        private readonly string name;
        private readonly int channels;

        /// <summary>
        /// Construct a physical Mini-recorder device
        /// </summary>
        /// <param name="d"></param>
        /// <param name="channels"></param>
        public InputDevice(IDeckLink d, int channels)
        {
            device = d as IDeckLinkInput;
            if(device == null)
                throw new ApplicationException("Not a Decklink input device");
            this.channels = channels;
            d.GetDisplayName(out name);
 }

        /// <summary>
        /// Construct a dummy device
        /// </summary>
        /// <param name="name"></param>
        public InputDevice(string name)
        {
            this.name = name;
            channels = 8;
        }
    }
}
