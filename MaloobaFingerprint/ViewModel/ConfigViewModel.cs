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

using System.Collections.ObjectModel;
using DeckLinkAPI;

namespace MaloobaFingerprint.ViewModel
{
    public class ConfigViewModel : ConfigViewModelBase
    {
        public ConfigViewModel(string[] args) : base(args)
        {
            Devices = new ObservableCollection<InputDevice>();
            var dli = new CDeckLinkIterator();
            while(true)
            {
                IDeckLink dl;
                dli.Next(out dl);
                if(dl == null) break;
                var chans = GetAudioChannelCount(dl);
                if(dl is IDeckLinkInput)
                    Devices.Add(new InputDevice(dl, chans));
            }
            RestoreConfiguration();
        }

        private int GetAudioChannelCount(IDeckLink dl)
        {
            var attr = dl as IDeckLinkAttributes;
            if(attr == null)
                return 0;
            long chans;
            attr.GetInt(_BMDDeckLinkAttributeID.BMDDeckLinkMaximumAudioChannels, out chans);
            return (int)chans;
        }
    }
}
