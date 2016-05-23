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

using GalaSoft.MvvmLight;

namespace MaloobaFingerprint.ViewModel
{
    public class ChannelViewModel : ViewModelBase
    {
        public int Index
        {
            get { return index; }
            set { Set(nameof(Index), ref index, value); }
        }
        private int index;

        public bool AudioIndicator
        {
            get { return audioIndicator; }
            set
            {
                var old = audioIndicator;
                audioIndicator = value;
                RaisePropertyChanged(nameof(AudioIndicator), old, audioIndicator);
            }
        }

        private bool audioIndicator;

        public ulong Fingerprint
        {
            get { return fingerprint; }
            set
            {
                var old = fingerprint;
                fingerprint = value;
                RaisePropertyChanged(nameof(Fingerprint), old, fingerprint);
            }
        }

        private ulong fingerprint;

        public int Enabled
        {
            get { return enabled; }
            set
            {
                var old = enabled;
                enabled = value;
                RaisePropertyChanged(nameof(Enabled), old, enabled);
            }
        }

        private int enabled;

        public ChannelViewModel(int index)
        {
            Index = index + 1;
        }

    }
}