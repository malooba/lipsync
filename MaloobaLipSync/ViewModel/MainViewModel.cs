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

using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaloobaLipSync.Correlator;

namespace MaloobaLipSync.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        /// <summary>
        /// UDP packet version identifier
        /// </summary>
        private const int PACKET_VERSION = 1;

        public RelayCommand ConfigCommand { get; }
        public RelayCommand OffCommand { get; }
        public RelayCommand RunCommand { get; }

        public bool OffMode
        {
            get { return offMode; }
            set { Set(nameof(OffMode), ref offMode, value); }
        }
        private bool offMode = true;

        /// <summary>
        /// Timecode string 
        /// </summary>
        public string Timecode
        {
            get { return timecode; }
            set { Set(nameof(Timecode), ref timecode, value); }
        }
        private string timecode = "00:00:00:00";

        public List<ChannelViewModel> Channels { get; set; }

        private readonly ViewModelLocator locator;
        private bool configuring;
        private Correlator.Correlator correlator;
        private double confidenceThreshold;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            locator = new ViewModelLocator();
            ConfigCommand = new RelayCommand(DoConfig);
            OffCommand = new RelayCommand(DoOff);
            RunCommand = new RelayCommand(DoStart, () => locator.Configuration.Valid);
            Channels = new List<ChannelViewModel>();
            for(var i = 0; i < Correlator.Correlator.CHANNELS; i++)
                Channels.Add(new ChannelViewModel(i + 1));
        }

        private void DoConfig()
        {
            if(configuring) return;
            DoOff();
            configuring = true;
        }

        /// <summary>
        /// Exit the configuration state
        /// </summary>
        private void EndConfig()
        {
            configuring = false;
            locator.Configuration.SaveConfiguration();
            RunCommand.RaiseCanExecuteChanged();
            OffMode = true;
        }

        private void DoOff()
        {
            if(configuring)
                EndConfig();
            if(correlator != null)
                StopRunning();
            configuring = false;
            RunCommand.RaiseCanExecuteChanged();
        }

        private void DoStart()
        {
            if(correlator != null) return;
            DoOff();
            
            var configuration = locator.Configuration;
            var cconfig = new CorrelatorConfig
            {
                HostA = configuration.HostA,
                PortA = configuration.PortA,
                HostB = configuration.HostB,
                PortB = configuration.PortB,
                HaystackFrames = int.Parse(configuration.HaystackFrames),
                NeedleFrames = int.Parse(configuration.NeedleFrames),
                CleanupFrames = int.Parse(configuration.CleanupFrames),
                StepFrames = int.Parse(configuration.StepFrames)
            };

            confidenceThreshold = double.Parse(configuration.ConfidenceThreshold);
            correlator = new Correlator.Correlator(cconfig);
            correlator.OutputCreated += OutputCreated;
            correlator.Start();
        }

        private void OutputCreated(object sender, Shift s)
        {
            Timecode = TimecodeString(s.Timecode);
            for(var i = 0; i < Correlator.Correlator.CHANNELS; i++)
            {
                var ch = Channels[i];
                ch.AudioPresentA = s.AudioPresentA[i];
                ch.AudioPresentB = s.AudioPresentB[i];
                var delay = s.Delay[i];
                var confidence = s.Confidence[i];
                ch.Delay = confidence >= confidenceThreshold ? ((int)(delay + 0.499)).ToString() : "-  ";
            }
        }

        /// <summary>
        /// Stop the analyser
        /// </summary>
        private void StopRunning()
        {
            correlator.OutputCreated -= OutputCreated;
            correlator.Stop();
            correlator = null;
        }

        /// <summary>
        /// Convert a BCD timecode integer to a string
        /// </summary>
        /// <param name="tc"></param>
        /// <returns></returns>
        private string TimecodeString(uint tc)
        {
            var b = BitConverter.GetBytes(tc);
            return $"{b[3]:X2}:{b[2]:X2}:{b[1]:X2}:{b[0]:X2}";
        }
    }
}