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
