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
                if(dl is IDeckLinkInput)
                    Devices.Add(new InputDevice(dl));
            }
            RestoreConfiguration();
        }
    }
}
