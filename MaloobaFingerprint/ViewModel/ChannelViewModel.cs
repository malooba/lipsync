using GalaSoft.MvvmLight;

namespace MaloobaFingerprint.ViewModel
{
    public class ChannelViewModel : ViewModelBase
    {
        public int Index
        {
            get { return index; }
            set { Set(nameof(Index), ref index, value); }
        }
        private int index;

        public bool AudioIndicator
        {
            get { return audioIndicator; }
            set
            {
                var old = audioIndicator;
                audioIndicator = value;
                RaisePropertyChanged(nameof(AudioIndicator), old, audioIndicator);
            }
        }

        private bool audioIndicator;

        public ulong Fingerprint
        {
            get { return fingerprint; }
            set
            {
                var old = fingerprint;
                fingerprint = value;
                RaisePropertyChanged(nameof(Fingerprint), old, fingerprint);
            }
        }

        private ulong fingerprint;

        public int Enabled
        {
            get { return enabled; }
            set
            {
                var old = enabled;
                enabled = value;
                RaisePropertyChanged(nameof(Enabled), old, enabled);
            }
        }

        private int enabled;

        public ChannelViewModel(int index)
        {
            Index = index + 1;
        }

    }
}