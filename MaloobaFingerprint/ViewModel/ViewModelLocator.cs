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
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static string[] Args { get; set; }
        public IMainViewModel Main => main ?? (main = new MainViewModel());

        private static IMainViewModel main;

        public IConfigViewModel Configuration
        {
            get
            {
                if(configuration == null)
                {
                    if(ViewModelBase.IsInDesignModeStatic)
                        configuration = new DesignConfigViewModel(null);
                    else
                        configuration = new ConfigViewModel(Args);
                }
                return configuration;
            }
        }

        private static IConfigViewModel configuration;

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}