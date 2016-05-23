//Copyright 2016 Malooba Ltd

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using DeckLinkAPI;

namespace MaloobaFingerprint.FingerprintAnalyser
{
    /// <summary>
    /// Callback object for Decklink Monitor card
    /// </summary>
    internal class Callback : IDeckLinkInputCallback
    {
        private readonly AnalyserConfig config;

        public event EventHandler<FingerprintEventArgs> FingerprintCreated;

        private readonly Channel[] channelData;

        // Three slot triangular window 
        private readonly double[] window0;
        private readonly double[] window1;
        private readonly double[] window2;

        /// <summary>
        /// Hadamard masks
        /// </summary>
        private readonly int[,] masks =
        {
            { 1,  1,  1,  1,  1,  1,  1,  1, -1, -1, -1, -1, -1, -1, -1, -1},
            { 1,  1, -1, -1,  1,  1, -1, -1,  1,  1, -1, -1,  1,  1, -1, -1},
            { 1,  1,  1,  1, -1, -1, -1, -1, -1, -1, -1, -1,  1,  1,  1,  1},
            { 1,  1, -1, -1,  1,  1, -1, -1, -1, -1,  1,  1, -1, -1,  1,  1},
            { 1, -1, -1,  1,  1, -1, -1,  1,  1, -1, -1,  1,  1, -1, -1,  1},
            { 1,  1,  1,  1, -1, -1, -1, -1,  1,  1,  1,  1, -1, -1, -1, -1},
            { 1,  1, -1, -1, -1, -1,  1,  1, -1, -1,  1,  1,  1,  1, -1, -1},
            { 1, -1, -1,  1,  1, -1, -1,  1, -1,  1,  1, -1, -1,  1,  1, -1},
            { 1, -1,  1, -1,  1, -1,  1, -1,  1, -1,  1, -1,  1, -1,  1, -1},
            { 1,  1, -1, -1, -1, -1,  1,  1,  1,  1, -1, -1, -1, -1,  1,  1},
            { 1, -1, -1,  1, -1,  1,  1, -1, -1,  1,  1, -1,  1, -1, -1,  1},
            { 1, -1,  1, -1,  1, -1,  1, -1, -1,  1, -1,  1, -1,  1, -1,  1},
            { 1, -1, -1,  1, -1,  1,  1, -1,  1, -1, -1,  1, -1,  1,  1, -1},
            { 1, -1,  1, -1, -1,  1, -1,  1, -1,  1, -1,  1,  1, -1,  1, -1},
            { 1, -1,  1, -1, -1,  1, -1,  1,  1, -1,  1, -1, -1,  1, -1,  1}
        };

        /// <summary>
        /// Construct with configuration and output stream writer
        /// </summary>
        /// <param name="config"></param>
        public Callback(AnalyserConfig config)
        {
            this.config = config;

            // sps is assumed to be even
            var sps = config.SamplesPerSlot; 

            channelData = new Channel[Analyser.CHANNELS];
            for(var c = 0; c < Analyser.CHANNELS; c++)
                channelData[c] = new Channel(config);

            window0 = new double[sps];
            window1 = new double[sps];
            window2 = new double[sps];

            // Build a triangular window across 3 slots
            var half = sps * 3 >> 1;
            var end = (sps << 1) - 1;
            var step = 1.0 / (half - 1);

            for(var i = 0; i < sps; i++)
                window0[i] = window2[sps - 1 - i] = i * step;

            for(var i = sps; i < half; i++)
                window1[i - sps] = window1[end - i] = i * step;
        }

        /// <summary>
        /// Calback function for input format change (unused)
        /// </summary>
        /// <param name="notificationEvents"></param>
        /// <param name="newDisplayMode"></param>
        /// <param name="detectedSignalFlags"></param>
        public void VideoInputFormatChanged(_BMDVideoInputFormatChangedEvents notificationEvents, IDeckLinkDisplayMode newDisplayMode,
            _BMDDetectedVideoInputFormatFlags detectedSignalFlags)
        {
            
        }

        /// <summary>
        /// Calback function for new frame
        /// </summary>
        /// <param name="videoFrame"></param>
        /// <param name="audioPacket"></param>
        public void VideoInputFrameArrived(IDeckLinkVideoInputFrame videoFrame, IDeckLinkAudioInputPacket audioPacket)
        {
            // The conditions under which either of these cases occur are unclear but an overstreched processor doesn't help
            if(videoFrame == null || audioPacket == null) return;

            IDeckLinkTimecode timecode;
            videoFrame.GetTimecode(config.TimecodeFormat, out timecode);

            if(audioPacket.GetSampleFrameCount() != config.SamplesPerSlot * config.SlotsPerFrame)
                throw new ApplicationException("Wrong buffer size");

            IntPtr buffer;
            audioPacket.GetBytes(out buffer);
            var audioFingerprints = GetAudioFingerprints(buffer, config);

            var timecodeBcd = timecode?.GetBCD() ?? 0;

            FingerprintCreated?.Invoke(this, new FingerprintEventArgs(timecodeBcd, (byte)config.SlotsPerFrame, 0, audioFingerprints));

            // The documentation suggests that neither of these are necessary
            // BM's own code does the former
            // Including these doesn't make anything go bang so, in for a penny...
            Marshal.ReleaseComObject(videoFrame);
            Marshal.ReleaseComObject(audioPacket);
        }

        /// <summary>
        /// Generate a 15-bit video fingerprint from the input YUV244 buffer
        /// This is currently unused but could be used to regenerate timecode on the destination video 
        /// if this has been lost in the trancode process.  There are many other potential uses for this fingerprint data.
        /// 
        /// The algorithm divides the frame into 16 blocks (4x4) and applies the Hadamard masks to divide these blocks 
        /// into two sets in 15 different and orthogonal ways.  The relative brightness of each pair of sets yields one
        /// bit of the fingerprint.  The entire fingerprint is comprised of 15 bits; one from each Hadamard mask.
        /// The whole algorithm is resilient to changes of resolution, aspect, brightness, contrast and gamma that might occur in the
        /// transcode process.  It would be a simple matter to automatically detect letterboxing/pillarboxing and to only analyse the
        /// active video area if this were required. 
        /// Testing with transcoded broadcast video has shown the fingerprint to be robust and reliable.
        /// </summary>
        /// <param name="videoFrame"></param>
        /// <param name="sums"></param>
        /// <returns></returns>
        private unsafe int VideoFingerprint(IDeckLinkVideoInputFrame videoFrame, int[] sums)
        {
            var qheight = videoFrame.GetHeight() >> 2;
            var qwidth = videoFrame.GetWidth() >> 2;

            IntPtr buffer;

            videoFrame.GetBytes(out buffer);

            var ptr = (byte*)buffer + 1; // Skip over first U byte

            for(var r = 0; r < qheight; r++)
            {
                for(var c = 0; c < qwidth; c++)
                {
                    sums[0] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[1] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[2] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[3] += *ptr;
                    ptr += 2;
                }
            }

            for(var r = 0; r < qheight; r++)
            {
                for(var c = 0; c < qwidth; c++)
                {
                    sums[4] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[5] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[6] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[7] += *ptr;
                    ptr += 2;
                }
            }

            for(var r = 0; r < qheight; r++)
            {
                for(var c = 0; c < qwidth; c++)
                {
                    sums[8] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[9] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[10] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[11] += *ptr;
                    ptr += 2;
                }
            }

            for(var r = 0; r < qheight; r++)
            {
                for(var c = 0; c < qwidth; c++)
                {
                    sums[12] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[13] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[14] += *ptr;
                    ptr += 2;
                }

                for(var c = 0; c < qwidth; c++)
                {
                    sums[15] += *ptr;
                    ptr += 2;
                }
            }

            for(var i = 0; i < 16; i++)
            {
                sums[i] /= qwidth * qheight;
            }

            var videoFingerprint = 0;
            var bit = 1;
            for(var i = 0; i < 15; i++)
            {
                var sum = 0;
                for(var j = 0; j < 16; j++)
                    sum += sums[j] * masks[i, j];

                if(sum > 0)
                    videoFingerprint |= bit;

                bit <<= 1;
            }
            return videoFingerprint;
        }

        /// <summary>
        /// Generate an audio fingerprint for each selected input audio channel in the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        internal unsafe ulong[] GetAudioFingerprints(IntPtr buffer, AnalyserConfig config)
        {
            var signatures = new ulong[Analyser.CHANNELS];

            // window support
            var support = config.SamplesPerSlot * 3;

            for(var channel = 0; channel < Analyser.CHANNELS; channel++)
            {
                var signature = 0UL;
                var signalPresent = false;
                var cd = channelData[channel];

                // Initialise sptr to the first sample for the channel
                var sptr = (short*)(buffer) + channel;
                var bit = 1UL << config.SlotsPerFrame;
                for(var slot = 0; slot < config.SlotsPerFrame; slot++)
                {
                    // Sum of squares of windowed samples
                    var ssqr = 0.0;
                    for(var i = 0; i < config.SamplesPerSlot; i++)
                    {
                        var sample = *sptr;
                        // Step to the same channel in the next sample
                        sptr += Analyser.CHANNELS;
                        var fsample = cd.Filter.Filter(sample);
                        cd.Slot2[i] = fsample * fsample;

                        ssqr += window0[i] * cd.Slot0[i] + window1[i] * cd.Slot1[i] + window2[i] * cd.Slot2[i];
                    }
                    signalPresent = signalPresent || (ssqr > 0);

                    // Convert to logarithmic RMS
                    // The log scaling ensures that we are working with perceived loudness
                    var rms = signalPresent ? Math.Log10(ssqr / support) : 0.0;

                    // Rotate the slot buffers
                    var tmp = cd.Slot0;
                    cd.Slot0 = cd.Slot1;
                    cd.Slot1 = cd.Slot2;
                    cd.Slot2 = tmp;

                    bit >>= 1;
                    var rmsa = cd.RmsBuffer.Average();
                    if(rms > rmsa)
                        signature |= bit;

                    Buffer.BlockCopy(cd.RmsBuffer, sizeof(double), cd.RmsBuffer, 0, (config.FirLength - 1) * sizeof(double));
                    cd.RmsBuffer[config.FirLength - 1] = rms;
                }
                // MSB is a signal present flag
                if(signalPresent)
                    signature |= 0x8000000000000000UL;

                signatures[channel] = signature;
            }
            return signatures;
        }
    }
}