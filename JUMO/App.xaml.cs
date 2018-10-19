using System.Windows;

namespace JUMO
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            Audio.AudioManager.Instance.CurrentOutputDevice = null;
            Vst.PluginManager.Instance.Dispose();
            Playback.MasterSequencer.Instance.Dispose();

            base.OnExit(e);
        }
    }
}
