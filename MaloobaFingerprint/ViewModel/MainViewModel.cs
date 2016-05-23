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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaloobaFingerprint.FingerprintAnalyser;

namespace MaloobaFingerprint.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        /// <summary>
        /// UDP packet version identifier
        /// </summary>
        private const int PACKET_VERSION = 1;

        /// <summary>
        /// Default length of moving average filter
        /// </summary>
        private const string FIR_LENGTH_DEFAULT = "20";

        /// <summary>
        /// Timecode string 
        /// </summary>
        public string Timecode
        {
            get { return timecode; }
            set { Set(nameof(Timecode), ref timecode, value); }
        }
        private string timecode = "00:00:00:00";

        /// <summary>
        /// Audio channel status
        /// </summary>
        public List<ChannelViewModel> Channels { get; set; }

        /// <summary>
        /// Enter Configuration mode
        /// </summary>
        public RelayCommand ConfigCommand { get; }

        /// <summary>
        /// Enter Off mode
        /// </summary>
        public RelayCommand OffCommand { get; }

        /// <summary>
        /// Enter Run mode
        /// </summary>
        public RelayCommand RunCommand { get; }

        /// <summary>
        /// Permit setting of Off mode
        /// </summary>
        public bool OffMode
        {
            get { return offMode; }
            set { Set(nameof(OffMode), ref offMode, value); }
        }
        private bool offMode = true;

        /// <summary>
        /// When set, the fingerprinter is running and transmitting
        /// </summary>
        private bool running;

        /// <summary>
        /// When set, the configuration panel is open
        /// </summary>
        private bool configuring;
        
        private readonly ViewModelLocator locator;

        /// <summary>
        /// Fingerprint analyser
        /// </summary>
        private Analyser analyser;

        /// <summary>
        /// Correlator host address
        /// </summary>
        private IPEndPoint hostAddress;

        /// <summary>
        /// Correlator port
        /// </summary>
        private Socket client;

        private CancellationTokenSource dummyTaskCancel;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            locator = new ViewModelLocator();
            ConfigCommand = new RelayCommand(DoConfig);
            OffCommand = new RelayCommand(DoOff);
            RunCommand = new RelayCommand(DoStart, () => locator.Configuration.Valid);
            Channels = new List<ChannelViewModel>();
            for(var i = 0; i < Analyser.CHANNELS; i++)
                Channels.Add(new ChannelViewModel(i));
        }

        /// <summary>
        /// Start the analyser
        /// If we are configure with a dummy recorder device then simulate the analyser (design mode) 
        /// </summary>
        private void DoStart()
        {
            if(running) return;
            DoOff();
            running = true;
            var configuration = locator.Configuration;

            AnalyserConfig analyserConfig;
            try
            {
                hostAddress = new IPEndPoint(IPAddress.Parse(configuration.Host), Int32.Parse(configuration.Port));
                analyserConfig = new AnalyserConfig
                {
                    Recorder = configuration.Device.Device,
                    FirLength = int.Parse(configuration.FirLength ?? FIR_LENGTH_DEFAULT),
                    SamplesPerSlot = configuration.VideoMode.SamplesPerSlot,
                    SlotsPerFrame = configuration.VideoMode.SlotsPerFrame,
                    TimecodeFormat = configuration.TimecodeMode.Mode,
                    VideoMode = configuration.VideoMode.Mode
                };
            }
            catch(Exception)
            {
                MessageBox.Show("Invalid configuration");
                return;
            }

            if(analyserConfig.Recorder == null)
            {
                MessageBox.Show("Starting in design mode", "Design Mode", MessageBoxButton.OK);
                dummyTaskCancel = new CancellationTokenSource();
                var token = dummyTaskCancel.Token;
                Task.Run(async () =>
                {
                    var tcg = new TimecodeGenerator(configuration.VideoMode.Fps / 1000).GetEnumerator();
                    var delay = 1000000 / configuration.VideoMode.Fps;
                    while(!token.IsCancellationRequested)
                    {
                        tcg.MoveNext();
                        var t = Task.Delay(delay, token);
                        Application.Current.Dispatcher.Invoke(() => Timecode = TimecodeString(tcg.Current), DispatcherPriority.Background);
                        await t;
                    }
                }, token);
            }
            else
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                analyser = new Analyser(analyserConfig);
                analyser.FingerprintCreated += FingerprintCreated;
                analyser.Start();
            }
        }

        /// <summary>
        /// A fingerprint has been generated
        /// Send a UDP packet and update the GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void FingerprintCreated(object sender, FingerprintEventArgs ea)
        {
            SendPacket(ea);
            Timecode = TimecodeString(ea.Timecode);
            for(var i = 0; i < ea.AudioFingerprints.Length; i++)
                Channels[i].AudioIndicator = (ea.AudioFingerprints[i] & 0x8000000000000000UL) != 0;
        }

        private readonly byte[] fp = new byte[2 + sizeof(int) + Analyser.CHANNELS * sizeof(ulong)];

        /// <summary>
        /// Send a fingerprint packet over UDP
        /// </summary>
        /// <param name="ea"></param>
        private void SendPacket(FingerprintEventArgs ea)
        {
            Array.Clear(fp, 0, fp.Length);
            fp[0] = PACKET_VERSION;
            fp[1] = ea.SlotsPerFrame;
            Array.Copy(BitConverter.GetBytes(ea.Timecode), 0, fp, 2, 4);
            for(var ch = 0; ch < ea.AudioFingerprints.Length; ch++)
                Array.Copy(BitConverter.GetBytes(ea.AudioFingerprints[ch]), 0, fp, 2 + sizeof(int) + ch * sizeof(ulong), sizeof(ulong));

            client.SendTo(fp, hostAddress);
        }

        /// <summary>
        /// Stop the analyser
        /// </summary>
        private void StopRunning()
        {
            analyser?.Stop();
            analyser = null;
            client?.Close();
            client = null;
            dummyTaskCancel?.Cancel();
            dummyTaskCancel = null;
            running = false;
        }

        /// <summary>
        /// Enter the off state by terminating the existing state
        /// </summary>
        void DoOff()
        {
            if(configuring)
                EndConfig();
            if(running)
                StopRunning();
            configuring = false;
            running = false;
            RunCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Enter the configuration state
        /// </summary>
        private void DoConfig()
        {
            if(configuring) return;
            DoOff();
            configuring = true;
        }

        /// <summary>
        /// Exit the configuration state
        /// </summary>
        private void EndConfig()
        {
            configuring = false;
            locator.Configuration.SaveConfiguration();
            RunCommand.RaiseCanExecuteChanged();
            OffMode = true;
        }

        /// <summary>
        /// Convert a BCD timecode integer to a string
        /// </summary>
        /// <param name="tc"></param>
        /// <returns></returns>
        private string TimecodeString(uint tc)
        {
            var b = BitConverter.GetBytes(tc);
            return $"{b[3]:X2}:{b[2]:X2}:{b[1]:X2}:{b[0]:X2}";
        }
    }
}