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
using System.Windows;
using DeckLinkAPI;

namespace MaloobaFingerprint.FingerprintAnalyser
{
    public class Analyser
    {
        public const int CHANNELS = 8;

        public EventHandler<FingerprintEventArgs> FingerprintCreated;

        private IDeckLinkInput recorder;
        private readonly AnalyserConfig config;

        public Analyser(AnalyserConfig config)
        {
            this.config = config;
        }

        public void Start()
        {
            try
            {
                var cb = new Callback(config);
                recorder = config.Recorder;
                cb.FingerprintCreated += OnFingerprintCreated;
                recorder.SetCallback(cb);
                recorder.EnableVideoInput(config.VideoMode, _BMDPixelFormat.bmdFormat8BitYUV, _BMDVideoInputFlags.bmdVideoInputFlagDefault);
                recorder.EnableAudioInput(_BMDAudioSampleRate.bmdAudioSampleRate48kHz, _BMDAudioSampleType.bmdAudioSampleType16bitInteger, CHANNELS);
                recorder.StartStreams();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Cannot start fingerprinting: {ex.Message}", "Error", MessageBoxButton.OK);
                Stop(false);
            }
        }

        private void OnFingerprintCreated(object sender, FingerprintEventArgs e)
        {
            FingerprintCreated?.Invoke(sender, e);
        }

        public void Stop(bool reportError = true)
        {
            try
            {
                recorder.StopStreams();
                recorder.DisableAudioInput();
                recorder.DisableVideoInput();
            }
            catch(Exception ex)
            {
                if(reportError)
                    MessageBox.Show($"Cannot stop fingerprinting: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }
    }
}
