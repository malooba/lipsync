//Copyright 2016 Malooba Ltd

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

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
