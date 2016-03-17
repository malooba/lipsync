using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DeckLinkAPI;

namespace MaloobaFingerprint.Analyser
{
    /// <summary>
    /// Callback object for Decklink Monitor card
    /// </summary>
    internal class Callback : IDeckLinkInputCallback
    {
        private readonly AnalyserConfig config;

        private readonly IEnumerator<uint> tcg;
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

            tcg = new TimecodeGenerator(25).GetEnumerator();

            // sps is assumed to be even
            var sps = config.SamplesPerSlot; 
            var ch = config.GetBufferChannels();

            channelData = new Channel[ch];
            for(var c = 0; c < ch; c++)
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
            IDeckLinkTimecode timecode;
            videoFrame.GetTimecode(config.TimecodeFormat, out timecode);

            if(audioPacket.GetSampleFrameCount() != config.SamplesPerSlot * config.SlotsPerFrame)
                throw new ApplicationException("Wrong buffer size");

            IntPtr buffer;
            audioPacket.GetBytes(out buffer);
            var audioFingerprints = GetAudioFingerprints(buffer);

            var sums = new int[16];
            var videoFingerprint = VideoFingerprint(videoFrame, sums);

            uint timecodeBcd;
            if(timecode != null)
            {
                timecodeBcd = timecode.GetBCD();
            }
            else
            {
                tcg.MoveNext();
                timecodeBcd = tcg.Current;
            }
            FingerprintCreated?.Invoke(this,
                new FingerprintEventArgs(timecodeBcd, (byte)config.SlotsPerFrame, (ushort)videoFingerprint,
                    audioFingerprints, sums));

            Marshal.ReleaseComObject(videoFrame);

            // Is this one required?
            Marshal.ReleaseComObject(audioPacket);
        }

        /// <summary>
        /// Generate a video fingerprint from the input YUV244 buffer
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
        /// <param name="config"></param>
        /// <returns></returns>
        private unsafe ulong[] GetAudioFingerprints(IntPtr buffer)
        {
            var channelMask = config.AudioChannelMask;

            // Get number of channels in buffer
            var channels = config.GetBufferChannels();

            var signatures = new ulong[channels];
            
            for(var channel = 0; channel < channels; channel++)
            {
                ulong signature = 0;
                bool signalPresent = false;
                var cd = channelData[channel];
                // Skip masked channels
                if((channelMask & 1 << channel) == 0)
                    continue;

                // Initialise sptr to the first sample for the channel
                var sptr = (short*)(buffer) + channel;
                var bit = 1UL << config.SlotsPerFrame;
                for(var slot = 0; slot < config.SlotsPerFrame; slot++)
                {
                    var msqr = 0.0;
                    for(var i = 0; i < config.SamplesPerSlot; i++)
                    {
                        var sample = *sptr;
                        // Step to the same channel in the next sample
                        sptr += channels;
                        var fsample = cd.Filter.Filter(sample);
                        cd.Slot2[i] = fsample * fsample;

                        msqr += window0[i] * cd.Slot0[i] + window1[i] * cd.Slot1[i] + window2[i] * cd.Slot2[i];
                    }
                    var rms = Math.Sqrt(msqr / 150);

                    signalPresent = signalPresent || (msqr > 0);

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