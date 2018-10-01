﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using MidiToolkit = Sanford.Multimedia.Midi;

namespace JUMO
{
    /// <summary>
    /// 음표(Note)를 배치할 수 있는 악보를 나타냅니다.
    /// </summary>
    public class Score : ObservableCollection<Note>
    {
        private readonly MidiToolkit.Track _track = new MidiToolkit.Track();

        private long _length = 0;
        private bool _isStale = true;

        /// <summary>
        /// 악보에 배치된 임의의 음표의 속성이 변경되었을 때 발생하는 이벤트입니다.
        /// </summary>
        public event EventHandler NotePropertyChanged;

        /// <summary>
        /// 이 악보가 속해 있는 패턴의 인스턴스를 가져옵니다.
        /// </summary>
        public Pattern Pattern { get; }

        /// <summary>
        /// 이 악보의 길이를 가져옵니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length
        {
            get => _length;
            private set
            {
                if (_length != value)
                {
                    _length = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Length)));
                }
            }
        }

        /// <summary>
        /// 이 악보로부터 컴파일된 MIDI 트랙을 가져옵니다.
        /// </summary>
        public MidiToolkit.Track MidiTrack
        {
            get
            {
                if (_isStale)
                {
                    CompileTrack();
                }

                return _track;
            }
        }

        /// <summary>
        /// 새로운 Score 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="pattern">이 악보가 속하게 될 패턴. null은 사용할 수 없습니다.</param>
        public Score(Pattern pattern)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Note note in e.OldItems)
                {
                    note.PropertyChanged -= OnNotePropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (Note note in e.NewItems)
                {
                    note.PropertyChanged += OnNotePropertyChanged;
                }
            }

            UpdateLength();
            base.OnCollectionChanged(e);

            _isStale = true;
        }

        private void UpdateLength() => Length = this.Select(note => note.Start + note.Length).DefaultIfEmpty(0L).Max();

        private void CompileTrack()
        {
            _track.Clear();

            foreach (Note note in this)
            {
                int start = (int)note.Start;
                int end = start + (int)note.Length;

                _track.Insert(start, new MidiToolkit.ChannelMessage(MidiToolkit.ChannelCommand.NoteOn, 0, note.Value, note.Velocity));
                _track.Insert(end, new MidiToolkit.ChannelMessage(MidiToolkit.ChannelCommand.NoteOff, 0, note.Value, 64));
            }

            _isStale = false;
        }

        private void OnNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateLength();
            NotePropertyChanged?.Invoke(this, EventArgs.Empty);

            _isStale = true;
        }
    }
}
