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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using GalaSoft.MvvmLight;
using MaloobaLipSync.Correlator;

namespace MaloobaLipSync.ViewModel
{
    public class ConfigViewModel : ViewModelBase, IConfigViewModel
    {
        /// <summary>
        /// Correlator host IP address for port A
        /// </summary>
        public string HostA
        {
            get { return hostA; }
            set { Set(nameof(HostA), ref hostA, value); Validate(); }
        }
        private string hostA = "0.0.0.0";

        /// <summary>
        /// Correlator host port A
        /// </summary>
        public string PortA
        {
            get { return portA; }
            set { Set(nameof(PortA), ref portA, value); Validate(); }
        }
        private string portA = "11000";

        /// <summary>
        /// Correlator host IP address for port B
        /// </summary>
        public string HostB
        {
            get { return hostB; }
            set { Set(nameof(HostB), ref hostB, value); Validate(); }
        }
        private string hostB = "0.0.0.0";

        /// <summary>
        /// Correlator host port B
        /// </summary>
        public string PortB
        {
            get { return portB; }
            set { Set(nameof(PortB), ref portB, value); Validate(); }
        }

        public string HaystackFrames { get; set; }
        public string NeedleFrames { get; set; }
        public string CleanupFrames { get; set; }
        public string StepFrames { get; set; }
        public string ConfidenceThreshold { get; set; }

        private string portB = "11001";

        public bool Valid => Validate();

        /// <summary>
        /// Failed validation rules
        /// </summary>
        private readonly Dictionary<string, string> brokenRules;

        /// <summary>
        /// JSON file of configuration settings
        /// </summary>
        private readonly ConfigFile configFile;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigViewModel(string[] args)
        {
            brokenRules = new Dictionary<string, string>();
            var configFileName = "Settings";
            if(args != null && args.Length != 0)
                configFileName = args[0];
            configFile = new ConfigFile("MaloobaLipSync", configFileName + ".txt");
            RestoreConfiguration();
        }

        /// <summary>
        /// Restore and validate configuration 
        /// </summary>
        public void RestoreConfiguration()
        {
            HaystackFrames = configFile["HaystackFrames"] ?? CorrelatorConfig.DEFAULT_HAYSTACK_FRAMES.ToString();
            NeedleFrames = configFile["NeedleFrames"] ?? CorrelatorConfig.DEFAULT_NEEDLE_FRAMES.ToString();
            StepFrames = configFile["StepFrames"] ?? CorrelatorConfig.DEFAULT_STEP_FRAMES.ToString();
            CleanupFrames = configFile["CleanupFrames"] ?? CorrelatorConfig.DEFAULT_CLEANUP_FRAMES.ToString();
            ConfidenceThreshold = configFile["ConfidenceThreshold"] ?? CorrelatorConfig.DEFAULT_CONFIDENCE_THRESHOLD.ToString(CultureInfo.InvariantCulture);
            hostA = configFile["HostA"] ?? "0.0.0.0";
            portA = configFile["PortA"] ?? "11000";
            hostB = configFile["HostB"] ?? "0.0.0.0";
            portB = configFile["PortB"] ?? "11001";
            SaveConfiguration();   // Save any defaults that were applied
            Validate();
        }

        /// <summary>
        /// Clear, update and save current configuration
        /// </summary>
        public void SaveConfiguration()
        {
            configFile.Clear();
            configFile["HostA"] = HostA;
            configFile["PortA"] = PortA;
            configFile["HostB"] = HostB;
            configFile["PortB"] = PortB;
            configFile["HaystackFrames"] = HaystackFrames;
            configFile["NeedleFrames"] = NeedleFrames;
            configFile["StepFrames"] = StepFrames;
            configFile["CleanupFrames"] = CleanupFrames;
            configFile["ConfidenceThreshold"] = ConfidenceThreshold;
            configFile.Save();
        }

        private bool Validate()
        {
            IPAddress ip;
            ushort p;
            brokenRules["HostA"] = !IPAddress.TryParse(HostA, out ip) ? "Invalid IP Address" : "";
            brokenRules["PortA"] = !ushort.TryParse(PortA, out p) ? "Invalid port number" : "";
            brokenRules["HostB"] = !IPAddress.TryParse(HostB, out ip) ? "Invalid IP Address" : "";
            brokenRules["PortB"] = !ushort.TryParse(PortB, out p) ? "Invalid port number" : "";

            uint h, n, s, c;
            double t;
            var ok = uint.TryParse(HaystackFrames, out h) && h <= 100 &&
                     uint.TryParse(NeedleFrames, out n) && n < h &&
                     uint.TryParse(StepFrames, out s) && s <= 10 &&
                     uint.TryParse(CleanupFrames, out c) && c < 40 &&
                     double.TryParse(ConfidenceThreshold, out t) && t > 0.0 && t < 0.5;
            if(!ok)
            {
                MessageBox.Show("Configuration invalid - reverting to defaults", "Warning", MessageBoxButton.OK);
                HaystackFrames = CorrelatorConfig.DEFAULT_HAYSTACK_FRAMES.ToString();
                NeedleFrames = CorrelatorConfig.DEFAULT_NEEDLE_FRAMES.ToString();
                StepFrames = CorrelatorConfig.DEFAULT_STEP_FRAMES.ToString();
                CleanupFrames = CorrelatorConfig.DEFAULT_CLEANUP_FRAMES.ToString();
                ConfidenceThreshold = CorrelatorConfig.DEFAULT_CONFIDENCE_THRESHOLD.ToString(CultureInfo.InvariantCulture);
            }
            return brokenRules.All(r => string.IsNullOrEmpty(r.Value));
        }

        // IDataErrorInfo implementation
        public string this[string columnName] => brokenRules.ContainsKey(columnName) ? brokenRules[columnName] : "";
        public string Error => null;
    }
}
