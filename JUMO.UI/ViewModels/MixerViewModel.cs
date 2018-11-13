using JUMO.Vst;
using System.Collections.Generic;

namespace JUMO.UI.ViewModels
{
    public class MixerViewModel : ViewModelBase
    {
        private MixerChannel _currentChannel;
        private RelayCommand _soloCommand;
        private RelayCommand _addPluginCommand;
        private RelayCommand _removePluginCommand;
        private RelayCommand _replacePluginCommand;
        private RelayCommand _moveUpPluginCommand;
        private RelayCommand _moveDownPluginCommand;

        public override string DisplayName => "믹서";

        #region Properties

        //믹서 채널
        public IEnumerable<MixerChannel> MixerChannels => MixerManager.Instance.MixerChannels;

        //현재 선택중인 채널
        public MixerChannel CurrentChannel
        {
            get => _currentChannel;
            set
            {
                _currentChannel = value;
                OnPropertyChanged(nameof(CurrentChannel));
            }
        }

        //사용 커맨드
        public RelayCommand SoloCommand
            => _soloCommand ?? (_soloCommand = new RelayCommand(current => MixerManager.Instance.ToggleSolo(current as MixerChannel)));

        public RelayCommand AddPluginCommand => _addPluginCommand ?? (_addPluginCommand = new RelayCommand(_ => ExecuteAddPlugin()));

        public RelayCommand ReplacePluginCommand => _replacePluginCommand ?? (_replacePluginCommand = new RelayCommand(oldPlugin => ExecuteReplacePlugin(oldPlugin as EffectPlugin)));

        public RelayCommand MoveUpPluginCommand => _moveUpPluginCommand ?? (_moveUpPluginCommand = new RelayCommand(idx => MoveUp((int)idx)));
        public RelayCommand MoveDownPluginCommand => _moveDownPluginCommand ?? (_moveDownPluginCommand = new RelayCommand(idx => MoveDown((int)idx)));

        public RelayCommand OpenPluginEditorCommand { get; } =
            new RelayCommand(
                plugin => PluginEditorManager.Instance.OpenEditor(plugin as PluginBase),
                plugin => true // TODO: VST 플러그인이 에디터 UI를 제공하는지 확인해야 함. (Flag, CanDo 등을 조사)
            );

        public RelayCommand RemovePluginCommand
            => _removePluginCommand ?? (_removePluginCommand = new RelayCommand(plugin => CurrentChannel.RemoveEffect(plugin as EffectPlugin)));

        #endregion

        public MixerViewModel()
        {
            _currentChannel = MixerManager.Instance.MixerChannels[0];
        }

        private void MoveUp(int current)
        {
            CurrentChannel.MoveUp(current);
        }

        private void MoveDown(int current)
        {
            CurrentChannel.MoveDown(current);
        }

        private string ShowOpenFileDialog()
        {
            FileDialogViewModel fdvm = new FileDialogViewModel()
            {
                Title = "플러그인 열기",
                Extension = ".dll",
                Filter = "VST 플러그인|*.dll|모든 파일|*.*"
            };

            fdvm.ShowOpenCommand.Execute(null);

            return fdvm.FileName;
        }

        private void ExecuteAddPlugin()
        {
            string fileName = ShowOpenFileDialog();

            if (!string.IsNullOrEmpty(fileName))
            {
                CurrentChannel.AddEffect(fileName);
            }
        }

        private void ExecuteReplacePlugin(EffectPlugin oldPlugin)
        {
            string fileName = ShowOpenFileDialog();

            if (!string.IsNullOrEmpty(fileName))
            {
                CurrentChannel.ReplaceEffect(fileName, oldPlugin);
            }
        }
    }
}
