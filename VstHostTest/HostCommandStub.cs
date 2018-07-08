using System;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;

namespace VstHostTest
{
    class HostCommandStub : IVstHostCommandStub
    {
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

        public int GetCurrentPluginID()
        {
            return PluginContext.PluginInfo.PluginID;
        }

        public string GetDirectory()
        {
            throw new NotImplementedException();
        }

        public int GetInputLatency()
        {
            throw new NotImplementedException();
        }

        public VstHostLanguage GetLanguage()
        {
            throw new NotImplementedException();
        }

        public int GetOutputLatency()
        {
            throw new NotImplementedException();
        }

        public VstProcessLevels GetProcessLevel()
        {
            throw new NotImplementedException();
        }

        public string GetProductString()
        {
            throw new NotImplementedException();
        }

        public float GetSampleRate()
        {
            throw new NotImplementedException();
        }

        public VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags)
        {
            return null;
        }

        public string GetVendorString()
        {
            throw new NotImplementedException();
        }

        public int GetVendorVersion()
        {
            throw new NotImplementedException();
        }

        public int GetVersion()
        {
            return 1000;
        }

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

        public void ProcessIdle()
        {
        }

        public void SetParameterAutomated(int index, float value)
        {
        }

        public bool SizeWindow(int width, int height)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDisplay()
        {
            return false;
        }
    }
}
