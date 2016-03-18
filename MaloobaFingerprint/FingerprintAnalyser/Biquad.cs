namespace MaloobaFingerprint.FingerprintAnalyser
{
    /// <summary>
    /// Biquad didgital filter
    /// </summary>
    public class Biquad
    {
        // Filter coefficients
        private readonly double b0;
        private readonly double b1;
        private readonly double b2;
        private readonly double a1;
        private readonly double a2;

        // Delay buffers
        private double z1;
        private double z2;

        public Biquad(double b0, double b1, double b2, double a1, double a2)
        {
            this.b0 = b0;
            this.b1 = b1;
            this.b2 = b2;
            this.a1 = a1;
            this.a2 = a2;
        }

        /// <summary>
        /// Filter a single input sample
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double Filter(double input)
        {
            var z0 = input - a1 * z1 - a2 * z2;
            var output = b0 * z0 + b1 * z1 + b2 * z2;
            z2 = z1;
            z1 = z0;
            return output;
        }
    }
}
