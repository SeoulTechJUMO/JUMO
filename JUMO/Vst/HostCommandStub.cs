using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;

namespace JUMO.Vst
{
    class HostCommandStub : IVstHostCommandStub
    {
        #region IVstHostCommands10 Members

        public int GetCurrentPluginID() => PluginContext.PluginInfo.PluginID;

        public int GetVersion() => 1;

        public void ProcessIdle()
        {
            throw new NotImplementedException();
        }

        public void SetParameterAutomated(int index, float value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVstHostCommands20 Members

        public IVstPluginContext PluginContext { get; set; }

        public bool BeginEdit(int index)
        {
            throw new NotImplementedException();
        }

        public VstCanDoResult CanDo(string cando)
        {
            throw new NotImplementedException();
        }

        public bool CloseFileSelector(VstFileSelect fileSelect)
        {
            throw new NotImplementedException();
        }

        public bool EndEdit(int index)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
