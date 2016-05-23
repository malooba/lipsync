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
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Input;
using DeckLinkAPI;
using GalaSoft.MvvmLight;

namespace MaloobaFingerprint.ViewModel
{
    public class ConfigViewModelBase : ViewModelBase, IConfigViewModel
    {

        /// <summary>
        /// Available Black Magic Mini-Recorder devices
        /// </summary>
        public ObservableCollection<InputDevice> Devices { get; set; }

        /// <summary>
        /// Currently selected Black Magic Mini-Recorder device
        /// </summary>
        public InputDevice Device
        {
            get { return device; }
            set { Set(nameof(Device), ref device, value); Validate(); }
        }
        private InputDevice device;

        /// <summary>
        /// Correlator host IP address
        /// </summary>
        public string Host
        {
            get { return host; }
            set { Set(nameof(Host), ref host, value); Validate(); }
        }
        private string host = "127.0.0.1";

        /// <summary>
        /// Correlator host port
        /// </summary>
        public string Port
        {
            get { return port; }
            set { Set(nameof(Port), ref port, value); Validate(); }
        }
        private string port = "11000";

        /// <summary>
        /// Available timecode mode settings
        /// </summary>
        public ObservableCollection<TimecodeMode> TimecodeModes { get; set; }

        /// <summary>
        /// Selected timecode mode
        /// </summary>
        public TimecodeMode TimecodeMode { get; set; }

        /// <summary>
        /// Available video input mode settings
        /// </summary>
        public ObservableCollection<VideoMode> VideoModes { get; set; }

        /// <summary>
        /// Selected video input mode
        /// </summary>
        public VideoMode VideoMode { get; set; }

        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                selectedTabIndex = value;
                Validate();
            }
        }

        /// <summary>
        /// True if settings are valid
        /// </summary>
        public bool Valid => Validate();

        public string FirLength { get; set; }

        /// <summary>
        /// TODO:  Get rid of this?
        /// </summary>
        public ICommand OkCommand { get; set; }

        /// <summary>
        /// Failed validation rules
        /// </summary>
        private readonly Dictionary<string, string> brokenRules;

        /// <summary>
        /// JSON file of configuration settings
        /// </summary>
        private ConfigFile configFile;

        private int selectedTabIndex;

        /// <summary>
        /// Constructor
        /// </summary>
        protected ConfigViewModelBase(string[] args)
        {
            brokenRules = new Dictionary<string, string>();
            InitVideoModes();
            InitTimecodeModes();
            var configFileName = "Settings";
            if(args != null && args.Length != 0)
                configFileName = args[0];
            configFile = new ConfigFile("MaloobaFingerprint", configFileName + ".txt");
        }

        /// <summary>
        /// Restore and validate configuration 
        /// </summary>
        public void RestoreConfiguration()
        {
            Device = Devices.SingleOrDefault(d => d.Name == configFile["Device"]);
            VideoMode = VideoModes.SingleOrDefault(v => v.Name == configFile["VideoMode"]) ?? VideoModes.First();
            TimecodeMode = TimecodeModes.SingleOrDefault(t => t.Name == configFile["TimecodeMode"]) ?? TimecodeModes.First();
            Host = configFile["Host"];
            Port = configFile["Port"];
            FirLength = configFile["FirLength"];
            Validate();
        }

        /// <summary>
        /// Clear, update and save current configuration
        /// </summary>
        public void SaveConfiguration()
        {
            configFile.Clear();
            configFile["Device"] = Device?.Name;
            configFile["VideoMode"] = VideoMode.Name;
            configFile["TimecodeMode"] = TimecodeMode.Name;
            configFile["Host"] = Host;
            configFile["Port"] = Port;
            configFile.Save();
        }

        /// <summary>
        /// Validate current configuration settings
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            IPAddress ip;
            ushort p;
            brokenRules["Device"] = device == null ? "No input device selected" : "";
            brokenRules["Host"] = !IPAddress.TryParse(Host, out ip) ? "Invalid IP Address" : "";
            brokenRules["Port"] = !ushort.TryParse(Port, out p) ? "Invalid port number" : "";

         return brokenRules.All(r => string.IsNullOrEmpty(r.Value));
        }


        /// <summary>
        /// Initialise the configuration timecode mode drop-down list collection
        /// </summary>
        private void InitTimecodeModes()
        {
            TimecodeModes = new ObservableCollection<TimecodeMode>
            {
                new TimecodeMode {Mode = _BMDTimecodeFormat.bmdTimecodeRP188Any, Name = "RP188 Any"},
                new TimecodeMode {Mode = _BMDTimecodeFormat.bmdTimecodeRP188LTC, Name = "RP188 LTC"},
                new TimecodeMode {Mode = _BMDTimecodeFormat.bmdTimecodeRP188VITC1, Name = "RP188 VITC1"},
                new TimecodeMode {Mode = _BMDTimecodeFormat.bmdTimecodeRP188VITC2, Name = "RP188 VITC2"},
                new TimecodeMode {Mode = _BMDTimecodeFormat.bmdTimecodeVITC, Name = "VITC"},
                new TimecodeMode {Mode = _BMDTimecodeFormat.bmdTimecodeVITCField2, Name = "VITC Field 2"},
                new TimecodeMode {Mode = _BMDTimecodeFormat.bmdTimecodeSerial, Name = "Serial"},
                new TimecodeMode {Mode = _BMDTimecodeFormat.bmdTimecodeLTC, Name = "LTC"}
            };
        }

        /// <summary>
        /// Initialise the configuration vide mode drop-down list collection
        /// </summary>
        private void InitVideoModes()
        {
            VideoModes = new ObservableCollection<VideoMode>
            {
                new VideoMode {Mode = _BMDDisplayMode.bmdModeHD1080p25, Name = "1080p25", Fps = 25000},
                new VideoMode {Mode = _BMDDisplayMode.bmdModeHD1080i50, Name = "1080i25", Fps = 25000},
                new VideoMode {Mode = _BMDDisplayMode.bmdModeHD1080p24, Name = "1080i24", Fps = 24000},
                new VideoMode {Mode = _BMDDisplayMode.bmdModeHD720p50,  Name = "720p25",  Fps = 25000},
                new VideoMode {Mode = _BMDDisplayMode.bmdModePAL,       Name = "PAL",     Fps = 25000},
                new VideoMode {Mode = _BMDDisplayMode.bmdModePALp,      Name = "PALp",    Fps = 25000}
            };
        }

        // IDataErrorInfo implementation
        public string this[string columnName] => brokenRules.ContainsKey(columnName) ? brokenRules[columnName] : "";
        public string Error => null;
    }
}
