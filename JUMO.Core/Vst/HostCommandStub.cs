using System;
using System.Collections.Generic;
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

        #region IVstHostCommands10 Members

        public int GetCurrentPluginID() => PluginContext.PluginInfo.PluginID;

        public int GetVersion() => 1;

        public void ProcessIdle()
        {
            throw new NotImplementedException();
        }

        public void SetParameterAutomated(int index, float value)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region IVstHostCommands20 Members

        public IVstPluginContext PluginContext { get; set; }

        public bool BeginEdit(int index)
        {
            //throw new NotImplementedException();
            return true;
        }

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

        public bool EndEdit(int index)
        {
            //throw new NotImplementedException();
            return true;
        }

        public VstAutomationStates GetAutomationState()
        {
            throw new NotImplementedException();
        }

        public int GetBlockSize()
        {
            throw new NotImplementedException();
        }

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

        public float GetSampleRate()
        {
            // TODO: 실제 데이터를 제공할 것
            return 44100.0f;
        }

        public VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags)
        {
            // TODO: 실제 데이터를 제공할 것
            return new VstTimeInfo()
            {
                TimeSignatureNumerator = 4,
                TimeSignatureDenominator = 4,
                SampleRate = 44100.0,
                Tempo = 120,
                BarStartPosition = 0,
                NanoSeconds = 0,
                PpqPosition = 0.0
            };
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
            //throw new NotImplementedException();
            return true;
        }

        #endregion
    }
}
