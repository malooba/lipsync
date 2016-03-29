using System.Collections.ObjectModel;

namespace MaloobaFingerprint.ViewModel
{
    public class DesignConfigViewModel : ConfigViewModelBase
    {
        /// <summary>
        /// Create some dummy devices for design mode
        /// </summary>
        public DesignConfigViewModel(string[] args) : base(args)
        {
            Devices = new ObservableCollection<InputDevice>
            {
                new InputDevice("Recorder 1"),
                new InputDevice("Recorder 2")
            };
            Device = null;
            RestoreConfiguration();
        }
    }
}
