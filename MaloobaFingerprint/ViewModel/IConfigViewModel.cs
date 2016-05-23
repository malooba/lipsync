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
using System.ComponentModel;

namespace MaloobaFingerprint.ViewModel
{
    public interface IConfigViewModel : IDataErrorInfo
    {
        ObservableCollection<InputDevice> Devices { get; }
        InputDevice Device { get; set; }
        string Host { get; set; }
        string Port { get; set; }
        ObservableCollection<TimecodeMode> TimecodeModes { get; }
        TimecodeMode TimecodeMode { get; set; }
        ObservableCollection<VideoMode> VideoModes { get; }
        VideoMode VideoMode { get; set; }
        int SelectedTabIndex { get; set; }

        void SaveConfiguration();
        void RestoreConfiguration();

        
        bool Valid { get; }
        string FirLength { get; }
    }
}