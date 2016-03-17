using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;

namespace MaloobaFingerprint.ViewModel
{
    public interface IMainViewModel
    {
        /// <summary>
        /// Entering Config mode
        /// </summary>
        RelayCommand ConfigCommand { get; }

        /// <summary>
        /// Entering Off mode
        /// </summary>
        RelayCommand OffCommand { get; }

        /// <summary>
        /// Entering Run mode
        /// </summary>
        RelayCommand RunCommand { get; }

        /// <summary>
        /// Programatically set Off mode (e.g. when Config window closes)
        /// </summary>
        bool OffMode { get; set; }

        /// <summary>
        /// Current timecode formatted as 00:00:00:00
        /// </summary>
        string Timecode { get; set; }

        /// <summary>
        /// Audio channel status controls
        /// </summary>
        List<ChannelViewModel> Channels { get; set; }
    }
}
