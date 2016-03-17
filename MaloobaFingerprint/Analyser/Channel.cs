namespace MaloobaFingerprint.Analyser
{
    /// <summary>
    /// All of the storage required to fingerprint an audio channel
    /// </summary>
    public class Channel
    {
        // Three slot audio buffer
        public double[] Slot0;
        public double[] Slot1;
        public double[] Slot2;

        // Rms slot values
        public readonly double[] RmsBuffer;

        // Filter
        public readonly Hpf Filter;

        /// <summary>
        /// Construct the storage for an audio channel
        /// </summary>
        /// <param name="config"></param>
        public Channel(AnalyserConfig config)
        {
            Slot0 = new double[config.SamplesPerSlot];
            Slot1 = new double[config.SamplesPerSlot];
            Slot2 = new double[config.SamplesPerSlot];
            RmsBuffer = new double[config.FirLength];
            Filter = new Hpf();
        }
    }
}