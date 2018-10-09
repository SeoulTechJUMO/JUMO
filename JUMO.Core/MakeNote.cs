﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiToolKit = Sanford.Multimedia.Midi;
using System.Diagnostics;

namespace JUMO
{
    //채널 메시지 임시저장 클래스
    class ChannelMessage
    {
        public string command;
        public long AbsoluteTick;
        public long Value;
        public long velocuty;
    }

    public class MakeNote
    {
        public List<Note> MakeScore(string FilePath)
        {
            List<Note> Score = new List<Note>();

            MidiToolKit.Sequence sq = new MidiToolKit.Sequence();
            sq.Load(FilePath);

            Score = NoteMake(sq);

            return Score;
        }

        public List<Note> NoteMake(MidiToolKit.Sequence sq)
        {
            List<ChannelMessage> list_cm = new List<ChannelMessage>();
            List<Note> list_note = new List<Note>();

            ChannelMessage listcm = new ChannelMessage();

            //시퀀스를 트랙화
            foreach (MidiToolKit.Track track in sq)
            {
                //트렉안의 이벤트
                foreach(MidiToolKit.MidiEvent ev in track.Iterator())
                {
                    if (ev.MidiMessage is MidiToolKit.ChannelMessage cm)
                    {   
                        switch (cm.Command)
                        {
                            case MidiToolKit.ChannelCommand.NoteOn:
                                if (cm.Data2 != 0)
                                {
                                    ChannelMessage CM_struct = new ChannelMessage
                                    {
                                        command = "NoteOn",
                                        AbsoluteTick = ev.AbsoluteTicks,
                                        Value = cm.Data1,
                                        velocuty = cm.Data2
                                    };

                                    list_cm.Add(CM_struct);
                                }
                                else
                                {
                                    listcm = list_cm.Find(x => x.Value.Equals(cm.Data1));
                                    list_cm.Remove(new ChannelMessage() { Value = cm.Data1, AbsoluteTick = listcm.AbsoluteTick, command = "NoteOn", velocuty = listcm.velocuty });

                                    list_note.Add(new Note((Byte)listcm.Value, (Byte)listcm.velocuty, listcm.AbsoluteTick, ev.AbsoluteTicks - listcm.AbsoluteTick));
                                }

                                break;

                            case MidiToolKit.ChannelCommand.NoteOff:
                                listcm = list_cm.Find(x => x.Value.Equals(cm.Data1));
                                list_cm.Remove(new ChannelMessage() { Value = cm.Data1, AbsoluteTick = listcm.AbsoluteTick, command = "NoteOn", velocuty = listcm.velocuty });

                                list_note.Add(new Note((Byte)listcm.Value, (Byte)listcm.velocuty, listcm.AbsoluteTick, ev.AbsoluteTicks - listcm.AbsoluteTick));                     
                                break;
                        }
                    }
                }
            }

            //magenta에서 생성하는 미디의 PPQN을 현재 Song의 PPQN에 맞춰서 길이변화
            int Count = 0;
            foreach (Note i in list_note)
            {
                list_note[Count].Start = (long)((double)i.Start * ((double)sq.Division / (double)Song.Current.TimeResolution));
                list_note[Count].Length = (long)((double)i.Length * ((double)sq.Division / (double)Song.Current.TimeResolution));
                Count++;
            }

            return list_note;
        }

    }
}
