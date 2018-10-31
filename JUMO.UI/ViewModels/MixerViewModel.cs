using JUMO.Vst;
using System.Collections.Generic;

namespace JUMO.UI.ViewModels
{
    public class MixerViewModel : ViewModelBase
    {
        public override string DisplayName => "믹서";

        public MixerViewModel()
        {
            _CurrentChannel = MixerManager.Instance.MixerChannels[0];
        }

        //믹서 채널
        public IEnumerable<MixerChannel> MixerChannels => MixerManager.Instance.MixerChannels;

        //현재 선택중인 채널
        private MixerChannel _CurrentChannel;
        public MixerChannel CurrentChannel
        {
            get => _CurrentChannel;
            set
            {
                _CurrentChannel = value;
                OnPropertyChanged(nameof(CurrentChannel));
            }
        }

        //사용 커맨드
        private RelayCommand _Solo;
        public RelayCommand Solo
            => _Solo ?? (_Solo = new RelayCommand(current => MixerManager.Instance.ToggleSolo(current as MixerChannel)));

        private RelayCommand _AddPluginCommand;
        public RelayCommand AddPluginCommand => _AddPluginCommand ?? (_AddPluginCommand = new RelayCommand(ExecuteAddPlugin));

        public RelayCommand OpenPluginEditorCommand { get; } =
            new RelayCommand(
                plugin => PluginEditorManager.Instance.OpenEditor(plugin as PluginBase),
                plugin => true // TODO: VST 플러그인이 에디터 UI를 제공하는지 확인해야 함. (Flag, CanDo 등을 조사)
            );

        private void ExecuteAddPlugin(object _)
        {
            FileDialogViewModel fdvm = new FileDialogViewModel()
            {
                Title = "플러그인 열기",
                Extension = ".dll",
                Filter = "VST 플러그인|*.dll|모든 파일|*.*"
            };

            fdvm.ShowOpenCommand.Execute(null);

            if (fdvm.FileName != null)
            {
                CurrentChannel.AddEffect(fdvm.FileName);
            }
        }
    }
}
