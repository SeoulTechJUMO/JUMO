using System;
using System.Collections.Generic;

namespace ChordMagicianModel
{
    public static class ChordTools
    {
        public static Progress GetChordName(Progress p, string key, string mode)
        {
            string id = p.Id;
            string ChordName = "";

            // 계산할 스케일 
            List<byte> Scale = Naming.CalScale(key, mode);

            // 코드 구성음을 담은 리스트
            List<byte> Chord = new List<byte>();

            bool First = true;
            bool SlashFlag = false;
            bool FlatFlag = false;
            byte num = 0;
            byte last = 0;
            string inversion = "";

            // 다른 스케일의 코드일 경우 계산
            if(Char.IsLetter(id[0]))
            {
                if (id[0] == 'b')
                {
                    if (mode == "Major")
                    {
                        Scale = Naming.CalScale(key, "Minor");
                    }
                    else
                    {
                        Scale = Naming.CalScale(key, "Major");
                    }
                }
                else
                {
                    Scale = Naming.CalScale(key, Naming.Scale[Char.ToString(id[0])]);
                }
            }

            // 코드 구성음 만들기
            foreach (char i in id)
            {
                // 0~9에 해당하는 아스키코드만 숫자로 인식
                if (Char.IsDigit(i))
                {
                    num = (byte)(i - '0');

                    num--;
                }
                else if (i == '/')
                {
                    SlashFlag = true;

                    continue;
                }
                else if (i == 'b' & !First)
                {
                    FlatFlag = true;

                    continue;
                }
                else
                {
                    continue;
                }

                if (First == true)
                {
                    // Triad 코드 만들기
                    Chord.Add(Scale[num]);
                    Chord.Add(Scale[(byte)((num + 2) % 7)]);
                    Chord.Add(Scale[(byte)((num + 4) % 7)]);

                    last = (byte)((num + 4) % 7);
                    First = false;
                }
                else if (SlashFlag == true)
                {
                    // 실제 움직여야 하는 pitch를 스케일로 계산
                    // Scale[(byte)((Scale.IndexOf(Chord[0]) + num) % 7)] => 가야하는 타겟 음정 (pitch)
                    // Chord[0] => 원래 음정 (pitch)
                    num = Scale[(byte)((Scale.IndexOf(Chord[0]) + num) % 7)];

                    // pitch 간격 계산
                    for (int k = 0; ; k++)
                    {
                        if ((Chord[0] + k) % 12 == num)
                        {
                            num = (byte)k;

                            break;
                        }
                    }

                    for (int k = 0; k < Chord.Count; k++)
                    {
                        // pitch 이동

                        Chord[k] = (byte)((Chord[k] + num) % 12);
                    }

                    SlashFlag = false;
                }
                else
                {
                    // 추가 구성음 처리
                    // 인버전이 7th인경우 7th 추가
                    if (inversion == "65" || inversion == "43" || inversion == "42")
                    {
                        Chord.Add(Scale[(byte)((last + 2) % 7)]);
                    }

                    if (i == '7')
                    {
                        // 7th 노트 추가

                        if (FlatFlag == true)
                        {
                            // 반음 내린 경우

                            Chord.Add(Scale[(byte)((last + 2) % 7)]);

                            if (Chord[Chord.Count - 1] != 0)
                            {
                                Chord[Chord.Count - 1] -= 1;
                            }
                            else
                            {
                                Chord[Chord.Count - 1] = 11;
                            }

                            FlatFlag = false;
                        }
                        else
                        {
                            Chord.Add(Scale[(byte)((last + 2) % 7)]);
                        }
                    }
                    else
                    {
                        // 인버전

                        inversion += i;
                    }
                }
            }

            // 인버전이 7th인경우 7th 추가, 추가가 안된경우
            if (inversion == "65" || inversion == "43" || inversion == "42")
            {
                // 7th가 있어야 하는데 없는 경우 추가
                if (Chord.Count < 4)
                {
                    Chord.Add(Scale[(byte)((last + 2) % 7)]);
                }
            }

            // 코드명 입력
            ChordName = CalChord(Chord);

            if (inversion != "")
            {
                // 코드 인버전 처리

                if (inversion == "6")
                {
                    // first inversion

                    for(int k = 0; k < 1; k++)
                    {
                        Chord.Add(Chord[0]);
                        Chord.RemoveAt(0);
                    }

                    ChordName += "/" + Naming.KeyName[Chord[0]];
                }
                else if (inversion == "64")
                {
                    // second inversion

                    for (int k = 0; k < 2; k++)
                    {
                        Chord.Add(Chord[0]);
                        Chord.RemoveAt(0);
                    }

                    ChordName += "/" + Naming.KeyName[Chord[0]];
                }
                else if (inversion == "65")
                {
                    // 7th first inversion

                    for (int k = 0; k < 1; k++)
                    {
                        Chord.Add(Chord[0]);
                        Chord.RemoveAt(0);
                    }

                    ChordName += "/" + Naming.KeyName[Chord[0]];
                }
                else if (inversion == "43")
                {
                    // 7th second inversion

                    for (int k = 0; k < 2; k++)
                    {
                        Chord.Add(Chord[0]);
                        Chord.RemoveAt(0);
                    }

                    ChordName += "/" + Naming.KeyName[Chord[0]];
                }
                else if (inversion == "42")
                {
                    // 7th Third inversion

                    for (int k = 0; k < 3; k++)
                    {
                        Chord.Add(Chord[0]);
                        Chord.RemoveAt(0);
                    }

                    ChordName += "/" + Naming.KeyName[Chord[0]];
                }
            }

            p.Chord = ChordName;
            p.ChordNotes = Chord;

            return p;
        }

        // 코드 이름 정하는 함수 (인버전 되기 전)
        public static string CalChord(List<byte> Chords)
        {
            string Name = "";
            Name += Naming.KeyName[Chords[0]];

            // 코드 형식, Triad
            if ((byte)((Chords[0]+4)%12) == Chords[1])
            {
                // Major
            }
            else
            {
                if((byte)((Chords[1] + 3) % 12) == Chords[2])
                {
                    // Diminished

                    Name += "dim";
                }
                else
                {
                    // Minor

                    Name += "m";
                }
            }

            if (Chords.Count > 3)
            {
                // 7th

                if((byte)((Chords[2] + 3) % 12) == Chords[3]) 
                {
                    // dim7, m7, 7 etc.

                    Name += "7";  
                }
                else if ((byte)((Chords[2] + 4) % 12) == Chords[3])
                {
                    // Maj7, m7b5

                    if (Name.Contains("dim"))
                    {
                        // Half diminished

                        Name = Name[0].ToString();
                        Name += "m7b5";
                    }
                    else
                    {
                        // 나머지

                        Name += "M7";
                    }
                }
            }

            return Name;
        }
    }
}
