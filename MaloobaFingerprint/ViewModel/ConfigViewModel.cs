using System.Collections.ObjectModel;
using DeckLinkAPI;

namespace MaloobaFingerprint.ViewModel
{
    public class ConfigViewModel : ConfigViewModelBase
    {
        public ConfigViewModel()
        {
            Devices = new ObservableCollection<InputDevice>();
            var dli = new CDeckLinkIterator();
            while(true)
            {
                IDeckLink dl;
                dli.Next(out dl);
                if(dl == null) break;
                var chans = GetAudioChannelCount(dl);
                if(dl is IDeckLinkInput)
                    Devices.Add(new InputDevice(dl, chans));
            }
            RestoreConfiguration();
        }

        private int GetAudioChannelCount(IDeckLink dl)
        {
            var attr = dl as IDeckLinkAttributes;
            if(attr == null)
                return 0;
            long chans;
            attr.GetInt(_BMDDeckLinkAttributeID.BMDDeckLinkMaximumAudioChannels, out chans);
            return (int)chans;
        }
    }
}
