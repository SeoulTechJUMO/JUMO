using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordMagicianTest
{
    public class ChordTools
    {
        public static string GetChordName(string id, string key, string mode)
        {
            string ChordName = "";

            // 계산할 스케일 
            List<byte> Scale = Naming.CalScale(key, Naming.Scale[mode]);

            //코드 구성음을 담은 리스트
            List<byte> Chord = new List<byte>();

            bool First = true;
            bool FlatFlag = false;
            bool SlashFlag = false;
            sbyte num = 0;

            //코드 구성음 만들기
            foreach (char i in id)
            {
                // 0~9에 해당하는 아스키코드만 숫자로 인식
                // num은 부호있는 byte로 다용도로 쓰임
                if (i > 47 & i < 58)
                {
                    num = (sbyte)((byte)i - 49);
                }
                switch (i)
                {
                    case 'b':
                        FlatFlag = true;
                        break;
                    case '/':
                        SlashFlag = true;
                        break;
                    default:
                        if (First == true)
                        {
                            // b가 붙어있는 경우
                            if (FlatFlag == true)
                            {
                                num -= 1;
                                FlatFlag = false;
                            }
                            // 0보다 작으면 스케일의 상단으로 올림
                            if (num < 0)
                            {
                                num += 7;
                            }
                            // Build Triad
                            Chord.Add(Scale[num]);
                            for (int k = 0; k < 2; k++)
                            {
                                num += 2;
                                if (num > 6)
                                {
                                    num -= 7;
                                }
                                Chord.Add(Scale[num]);
                            }
                            First = false;
                        }
                        else if (SlashFlag == true)
                        {
                            num -= 1;
                            // slash 처리, slash 이후에 나오는 숫자-1 만큼 이동한다. (pitch 이동)
                            for (int k = 0; k < Chord.Count; k++)
                            {
                                if (Chord[k] + num > 11)
                                {
                                    Chord[k] += (byte)num;
                                    Chord[k] -= 12;
                                }
                                else
                                {
                                    Chord[k] += (byte)num;
                                }
                            }
                            SlashFlag = false;
                        }
                        else
                        {
                            //추가 구성음 처리
                            //코드 인버전, 7도음, 하프 디미니쉬 등 처리

                            if (i == '7')
                            {
                                if (FlatFlag == true)
                                {

                                    FlatFlag = false;
                                }
                                else
                                {

                                }
                                continue;
                            }
                            else if (i == '4')
                            {

                            }
                            else if (i == '5')
                            {

                            }
                            else if (i == '6')
                            {

                            }


                        }
                        break;
                }
            }

            return ChordName;
        }
    }
}
