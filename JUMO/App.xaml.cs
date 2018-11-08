using System.Threading.Tasks;
using System.Windows;
using JUMO.UI.Layouts;

namespace JUMO
{
    public partial class App : Application
    {
        private volatile Audio.AudioManager audioManager;
        private volatile Vst.PluginManager pluginManager;
        private volatile Playback.MasterSequencer masterSequencer;
        private volatile MixerManager mixerManager;

        protected override async void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            SplashWindow splashWindow = new SplashWindow();

            splashWindow.Show();

            await Task.Delay(2000);

            PrepareApp();
            mainWindow.Show();
            splashWindow.Close();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Audio.AudioManager.Instance.Dispose();
            Vst.PluginManager.Instance.Dispose();
            Playback.MasterSequencer.Instance.Dispose();

            base.OnExit(e);
        }

        private void PrepareApp()
        {
            audioManager = Audio.AudioManager.Instance;
            pluginManager = Vst.PluginManager.Instance;
            masterSequencer = Playback.MasterSequencer.Instance;
            mixerManager = MixerManager.Instance;
        }
    }
}
