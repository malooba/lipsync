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

namespace MaloobaLipSync.ViewModel
{
    public class ChannelViewModel : ViewModelBase
    {
        public int Index
        {
            get { return index; }
            set { Set(nameof(Index), ref index, value); }
        }
        private int index;

        public string Delay
        {
            get { return delay; }
            set { Set(nameof(Delay), ref delay, value); }
        }
        private string delay = "-  ";

        public bool AudioPresentA
        {
            get { return audioPresentA; }
            set { Set(nameof(AudioPresentA), ref audioPresentA, value); }
        }
        private bool audioPresentA;

        public bool AudioPresentB
        {
            get { return audioPresentB; }
            set { Set(nameof(AudioPresentB), ref audioPresentB, value); }
        }
        private bool audioPresentB;

        public ChannelViewModel(int index)
        {
            this.index = index;
        }
    }
}
