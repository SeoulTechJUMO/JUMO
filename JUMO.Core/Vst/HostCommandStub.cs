using System;
using System.Collections.Generic;
using System.Diagnostics;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;

namespace JUMO.Vst
{
    class HostCommandStub : IVstHostCommandStub
    {
        private readonly Dictionary<string, VstCanDoResult> _canDoAnswers = new Dictionary<string, VstCanDoResult>()
        {
            { "sendVstEvents", VstCanDoResult.Unknown }, //< Host supports send of Vst events to plug-in
            { "sendVstMidiEvent", VstCanDoResult.Yes }, //< Host supports send of MIDI events to plug-in
            { "sendVstTimeInfo", VstCanDoResult.Yes }, //< Host supports send of VstTimeInfo to plug-in
            { "receiveVstEvents", VstCanDoResult.No }, //< Host can receive Vst events from plug-in
            { "receiveVstMidiEvent", VstCanDoResult.No }, //< Host can receive MIDI events from plug-in
            { "reportConnectionChanges", VstCanDoResult.No }, //< Host will indicates the plug-in when something change in plug-in�s routing/connections with #suspend/#resume/#setSpeakerArrangement
            { "acceptIOChanges", VstCanDoResult.Unknown }, //< Host supports #ioChanged ()
            { "sizeWindow", VstCanDoResult.Unknown }, //< used by VSTGUI
            { "offline", VstCanDoResult.Unknown }, //< Host supports offline feature
            { "openFileSelector", VstCanDoResult.Unknown }, //< Host supports function #openFileSelector ()
            { "closeFileSelector", VstCanDoResult.Unknown }, //< Host supports function #closeFileSelector ()
            { "startStopProcess", VstCanDoResult.Yes }, //< Host supports functions #startProcess () and #stopProcess ()
            { "shellCategory", VstCanDoResult.Unknown }, //< 'shell' handling via uniqueID. If supported by the Host and the Plug-in has the category #kPlugCategShell
            { "sendVstMidiEventFlagIsRealtime", VstCanDoResult.Yes }, //< Host supports flags for #VstMidiEvent
        };

        private readonly PluginBase _plugin;

        private IVstPluginContext _pluginContext;
        private bool _isInitialized = false;

        public HostCommandStub(PluginBase plugin)
        {
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
        }

        #region IVstHostCommands10 Members

        public int GetCurrentPluginID() => PluginContext.PluginInfo.PluginID;

        public int GetVersion() => 1;

        public void ProcessIdle() { }

        // Parameter automation is not yet supported.
        public void SetParameterAutomated(int index, float value) { }

        #endregion

        #region IVstHostCommands20 Members

        public IVstPluginContext PluginContext
        {
            get => _pluginContext;
            set
            {
                if (_isInitialized)
                {
                    throw new InvalidOperationException($"{nameof(PluginContext)} has already been set.");
                }

                _pluginContext = value;
                _isInitialized = true;
            }
        }

        // Parameter automation is not yet supported.
        public bool BeginEdit(int index) => true;

        // Parameter automation is not yet supported.
        public bool EndEdit(int index) => true;

        public VstCanDoResult CanDo(string cando)
        {
            if (_canDoAnswers.TryGetValue(cando, out VstCanDoResult answer))
            {
                return answer;
            }

            return VstCanDoResult.Unknown;
        }

        public bool CloseFileSelector(VstFileSelect fileSelect)
        {
            throw new NotImplementedException();
        }

        public VstAutomationStates GetAutomationState()
        {
            throw new NotImplementedException();
        }

        public int GetBlockSize() => PluginBase.PluginBlockSize;

        public string GetDirectory()
        {
            throw new NotImplementedException();
        }

        public int GetInputLatency()
        {
            throw new NotImplementedException();
        }

        public VstHostLanguage GetLanguage() => VstHostLanguage.English;

        public int GetOutputLatency()
        {
            throw new NotImplementedException();
        }

        public VstProcessLevels GetProcessLevel() => VstProcessLevels.Unknown;

        public string GetProductString() => "JUMO";

        public float GetSampleRate() => 44100.0f;

        public VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags)
        {
            Song song = Song.Current;
            int tickPosition = Playback.MasterSequencer.Instance.Position;
            double sampleRate = 44100.0;
            VstTimeInfoFlags flags = 0;

            VstTimeInfo timeInfo = new VstTimeInfo()
            {
                SamplePosition = tickPosition * song.SecondsPerTick * sampleRate,
                SampleRate = sampleRate
            };

            if (filterFlags.HasFlag(VstTimeInfoFlags.PpqPositionValid))
            {
                timeInfo.PpqPosition = (double)tickPosition / song.TimeResolution;
                flags |= VstTimeInfoFlags.PpqPositionValid;
            }

            if (filterFlags.HasFlag(VstTimeInfoFlags.TempoValid))
            {
                timeInfo.Tempo = song.Tempo;
                flags |= VstTimeInfoFlags.TempoValid;
            }

            //if (filterFlags.HasFlag(VstTimeInfoFlags.BarStartPositionValid)) { }

            //if (filterFlags.HasFlag(VstTimeInfoFlags.CyclePositionValid)) { }

            if (filterFlags.HasFlag(VstTimeInfoFlags.TimeSignatureValid))
            {
                timeInfo.TimeSignatureNumerator = song.Numerator;
                timeInfo.TimeSignatureDenominator = song.Denominator;
                flags |= VstTimeInfoFlags.TimeSignatureValid;
            }

            // if (filterFlags.HasFlag(VstTimeInfoFlags.SmpteValid)) { }

            if (filterFlags.HasFlag(VstTimeInfoFlags.NanoSecondsValid))
            {
                timeInfo.NanoSeconds = 100.0 * Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
                flags |= VstTimeInfoFlags.NanoSecondsValid;
            }

            // if (filterFlags.HasFlag(VstTimeInfoFlags.ClockValid)) { }

            timeInfo.Flags = flags;

            return timeInfo;
        }

        public string GetVendorString() => "Team JUMAK";

        public int GetVendorVersion() => 1;

        public bool IoChanged()
        {
            throw new NotImplementedException();
        }

        public bool OpenFileSelector(VstFileSelect fileSelect)
        {
            throw new NotImplementedException();
        }

        public bool ProcessEvents(VstEvent[] events)
        {
            throw new NotImplementedException();
        }

        public bool SizeWindow(int width, int height)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDisplay()
        {
            Debug.WriteLine($"{PluginContext.PluginCommandStub?.GetEffectName()}: UpdateDisplay");
            _plugin.RequestUpdateDisplay();

            return true;
        }

        #endregion
    }
}
