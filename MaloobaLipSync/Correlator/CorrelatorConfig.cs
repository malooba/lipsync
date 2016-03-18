namespace MaloobaLipSync.Correlator
{
    class CorrelatorConfig
    {
        public const int DEFAULT_HAYSTACK_FRAMES = 51;
        public const int DEFAULT_NEEDLE_FRAMES = 13;
        public const int DEFAULT_STEP_FRAMES = 5;
        public const int DEFAULT_CLEANUP_FRAMES = 20;
        public const double DEFAULT_CONFIDENCE_THRESHOLD = 0.1;

        public string HostA;
        public string PortA;

        public string HostB;
        public string PortB;

        // Correlations are performed on overlapping windows of size HaystackFrames
        // The offset between two succesive correlation windows is given by Step
        // Step thus determines the frequency of correlations.
        // i.e. An output is produced every Step frames
        // NeedleFrames is the size of the search buffer
        // NeedleFrames and HaystackFrames should both be odd numbers
        public int HaystackFrames = DEFAULT_HAYSTACK_FRAMES;      // Size of search window in frames
        public int NeedleFrames = DEFAULT_NEEDLE_FRAMES;          // Size of search buffer in frames
        public int StepFrames = DEFAULT_STEP_FRAMES;              // Step from window to window in frames
        public int CleanupFrames = DEFAULT_CLEANUP_FRAMES;        // Size of median filter buffer
    }
}
