using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MaloobaFingerprint.ViewModel
{
    public interface IConfigViewModel : IDataErrorInfo
    {
        ObservableCollection<InputDevice> Devices { get; }
        InputDevice Device { get; set; }
        string Host { get; set; }
        string Port { get; set; }
        ObservableCollection<TimecodeMode> TimecodeModes { get; }
        TimecodeMode TimecodeMode { get; set; }
        ObservableCollection<VideoMode> VideoModes { get; }
        VideoMode VideoMode { get; set; }

        void SaveConfiguration();
        void RestoreConfiguration();

        /// <summary>
        /// TODO:  Get rid of this?
        /// </summary>
        ICommand OkCommand { get; set; }
        bool Valid { get; }
        string FirLength { get; }
    }
}