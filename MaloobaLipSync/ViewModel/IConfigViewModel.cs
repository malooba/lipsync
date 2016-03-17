using System.ComponentModel;

namespace MaloobaLipSync.ViewModel
{
    public interface IConfigViewModel : IDataErrorInfo
    {
        string HostA { get; set; }
        string PortA { get; set; }

        string HostB { get; set; }
        string PortB { get; set; }

        string HaystackFrames { get; set; }
        string NeedleFrames { get; set; }
        string CleanupFrames { get; set; }
        string StepFrames { get; set; }

        string ConfidenceThreshold { get; set; }

        void SaveConfiguration();
        void RestoreConfiguration();

        bool Valid { get; }

    }
}
