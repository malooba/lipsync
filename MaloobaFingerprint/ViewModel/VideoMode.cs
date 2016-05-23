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