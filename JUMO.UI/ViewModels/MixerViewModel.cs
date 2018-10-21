using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI.ViewModels
{
    public class MixerViewModel : ViewModelBase
    {
        public override string DisplayName => "믹서";

        public MixerViewModel()
        {
            _MixerChannels = MixerManager.Instance.MixerChannels;
            _CurrentChannel = _MixerChannels[0];
        }

        //믹서 채널
        private ObservableCollection<MixerChannel> _MixerChannels = new ObservableCollection<MixerChannel>();
        public ObservableCollection<MixerChannel> MixerChannels
        {
            get => _MixerChannels;
            set
            {
                _MixerChannels = value;
                OnPropertyChanged(nameof(MixerChannels));
            }
        }

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
    }
}
