using System;
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

        public ChannelMessage()
        {

        }
    }

    class MakeNote
    {
        public void NoteMake(MidiToolKit.Sequence sq)
        {
            List<ChannelMessage> list_cm = new List<ChannelMessage>();
            List<Note> list_note = new List<Note>();

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
                                ChannelMessage CM_struct = new ChannelMessage
                                {
                                    command = "NoteOn",
                                    AbsoluteTick = ev.AbsoluteTicks,
                                    Value = cm.Data1,
                                    velocuty = cm.Data2
                                };

                                list_cm.Add(CM_struct);
                                break;

                            case MidiToolKit.ChannelCommand.NoteOff:
                                ChannelMessage listcm = new ChannelMessage();
                                listcm = list_cm.Find(x => x.Value.Equals(cm.Data1));
                                list_cm.Remove(new ChannelMessage() { Value = cm.Data1, AbsoluteTick = listcm.AbsoluteTick, command = "NoteOn", velocuty = listcm.velocuty });

                                list_note.Add(new Note((Byte)listcm.Value, (Byte)listcm.velocuty, listcm.AbsoluteTick, ev.AbsoluteTicks - listcm.AbsoluteTick));                     
                                break;
                        }
                    }
                }
            }
            
            for (int i = 0; i < list_note.Count; i++) {
                Debug.WriteLine("Value : {0}, Start : {1}, Length : {2}, Velocity : {3}", list_note[i].Value, list_note[i].Start, list_note[i].Length, list_note[i].Velocity);
            }
            
        }

    }
}
