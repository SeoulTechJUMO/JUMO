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

            Task loadTask = PrepareApp();
            Task delayTask = Task.Delay(3000);

            await loadTask;
            await delayTask;

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

        private async Task PrepareApp()
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() => audioManager = Audio.AudioManager.Instance);
                Dispatcher.Invoke(() => pluginManager = Vst.PluginManager.Instance);
                Dispatcher.Invoke(() => masterSequencer = Playback.MasterSequencer.Instance);
                Dispatcher.Invoke(() => mixerManager = MixerManager.Instance);
            });
        }
    }
}
