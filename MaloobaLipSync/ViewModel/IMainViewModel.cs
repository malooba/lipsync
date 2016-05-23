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

using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;

namespace MaloobaLipSync.ViewModel 
{
    public interface IMainViewModel
    {
        /// <summary>
        /// Entering Config mode
        /// </summary>
        RelayCommand ConfigCommand { get; }

        /// <summary>
        /// Entering Off mode
        /// </summary>
        RelayCommand OffCommand { get; }

        /// <summary>
        /// Entering Run mode
        /// </summary>
        RelayCommand RunCommand { get; }

        /// <summary>
        /// Programatically set Off mode (e.g. when Config window closes)
        /// </summary>
        bool OffMode { get; set; }

        /// <summary>
        /// Current timecode formatted as 00:00:00:00
        /// </summary>
        string Timecode { get; set; }

        /// <summary>
        /// Audio channel status controls
        /// </summary>
        List<ChannelViewModel> Channels { get; set; }
    }
}
