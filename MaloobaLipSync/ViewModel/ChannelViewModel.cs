using GalaSoft.MvvmLight;

namespace MaloobaLipSync.ViewModel
{
    public class ChannelViewModel : ViewModelBase
    {
        public int Index
        {
            get { return index; }
            set { Set(nameof(Index), ref index, value); }
        }
        private int index;

        public string Delay
        {
            get { return delay; }
            set { Set(nameof(Delay), ref delay, value); }
        }
        private string delay = "-  ";

        public bool AudioPresentA
        {
            get { return audioPresentA; }
            set { Set(nameof(AudioPresentA), ref audioPresentA, value); }
        }
        private bool audioPresentA = true;

        public bool AudioPresentB
        {
            get { return audioPresentB; }
            set { Set(nameof(AudioPresentB), ref audioPresentB, value); }
        }
        private bool audioPresentB = true;

        public ChannelViewModel(int index)
        {
            this.index = index;
        }
    }
}
