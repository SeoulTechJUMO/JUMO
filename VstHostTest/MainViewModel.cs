﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Jacobi.Vst.Interop.Host;
using JUMO.Vst;
using JUMO.Media.Audio;

namespace VstHostTest
{
    class MainViewModel : INotifyPropertyChanged
    {
        private Plugin _selectedPlugin = null;
        private bool _canOpenPluginEditor = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Plugin> Plugins => PluginManager.Instance.Plugins;

        public Plugin SelectedPlugin
        {
            get => _selectedPlugin;
            set
            {
                _selectedPlugin = value;
                CanOpenPluginEditor = _selectedPlugin != null;
                OnPropertyChanged(nameof(SelectedPlugin));
            }
        }

        public bool CanOpenPluginEditor
        {
            get => _canOpenPluginEditor;
            private set
            {
                _canOpenPluginEditor = value;
                OnPropertyChanged(nameof(CanOpenPluginEditor));
            }
        }

        public bool AddPlugin(Action<Exception> onError)
            => PluginManager.Instance.AddPlugin(onError);

        public ICollectionView AudioOutputDevices => AudioManager.Instance.OutputDevices;

        public IAudioOutputDevice CurrentAudioOutputDevice
        {
            get => AudioManager.Instance.CurrentOutputDevice;
            private set
            {
                AudioManager.Instance.CurrentOutputDevice = value;
                OnPropertyChanged(nameof(CurrentAudioOutputDevice));
            }
        }

        public IAudioOutputDevice SelectedAudioOutputDevice { get; set; }

        public void ChangeCurrentAudioOutputDevice()
        {
            CurrentAudioOutputDevice = SelectedAudioOutputDevice;
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainViewModel()
        {
        }
    }
}
