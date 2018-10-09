using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ChordMagicianModel
{
    public static class CreateMelody
    {
        public static string StartUp = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
        public static string MelodyPath = StartUp + "/ChordMagicianModel/Melody ";

        //JUMO 프로젝트 안에 마젠타를 포함시킨 경우
        public static void RunMagenta(string Progress, int NumOfFiles)
        {
            string FileName = StartUp + "/improv_rnn_generate/improv_rnn_generate.exe";
            string Args = "--config=chord_pitches_improv " +
                "--bundle_file=" + StartUp + "/improv_rnn_generate/chord_pitches_improv.mag " +
                "--output_dir=" + MelodyPath +
                "--num_outputs=" + NumOfFiles + " " +
                "--backing_chords=\"" + Progress + "\" "+
                //이부분은 추후 삭제
                "--render_chords";

            //이미 생성된 멜로디 파일이 있다면 지워준다
            if (System.IO.Directory.Exists(MelodyPath))
            {
                string[] files = GetMelodyPath();

                foreach (string s in files)
                {
                    System.IO.File.Delete(s);
                }
            }

            RunProcess(FileName, Args);
        }

        //생성된 미디파일 경로 얻기
        public static string[] GetMelodyPath()
        {
            string[] files = System.IO.Directory.GetFiles(MelodyPath);
            return files;
        }

        public static int RunProcess(String FileName, String Args)
        {
            Process p = new Process();

            p.StartInfo.FileName = FileName;
            p.StartInfo.Arguments = Args;

            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.Start();
            p.WaitForExit();

            return p.ExitCode;
        }

        
    }
}
