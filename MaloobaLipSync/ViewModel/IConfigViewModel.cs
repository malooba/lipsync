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

using System.ComponentModel;

namespace MaloobaLipSync.ViewModel
{
    public interface IConfigViewModel : IDataErrorInfo
    {
        string HostA { get; set; }
        string PortA { get; set; }

        string HostB { get; set; }
        string PortB { get; set; }

        string HaystackFrames { get; set; }
        string NeedleFrames { get; set; }
        string CleanupFrames { get; set; }
        string StepFrames { get; set; }

        string ConfidenceThreshold { get; set; }

        void SaveConfiguration();
        void RestoreConfiguration();

        bool Valid { get; }

    }
}
