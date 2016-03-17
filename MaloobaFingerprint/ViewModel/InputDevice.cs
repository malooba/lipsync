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

        /// <summary>
        /// Display name of device
        /// </summary>
        public string Name => name;

        private readonly IDeckLinkInput device;
        private readonly string name;

        /// <summary>
        /// Construct a physical Mini-recorder device
        /// </summary>
        /// <param name="d"></param>
        public InputDevice(IDeckLink d)
        {
            device = d as IDeckLinkInput;
            if(device == null)
                throw new ApplicationException("Not a Decklink input device");
            d.GetDisplayName(out name);
        }

        /// <summary>
        /// Construct a dummy device
        /// </summary>
        /// <param name="name"></param>
        public InputDevice(string name)
        {
            this.name = name;
        }
    }
}
