using System;
using System.IO;
using System.Diagnostics;

namespace ChordMagicianModel
{
    public static class CreateMelody
    {
        public static string startup = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
        public static string melodyPath = startup + "/ChordMagicianModel/Melody ";

        //JUMO 프로젝트 안에 마젠타를 포함시킨 경우
        public static void RunMagenta(string progress, int numOfFiles)
        {
            string fileName = startup + "/improv_rnn_generate/improv_rnn_generate.exe";

            string args =
                "--config=chord_pitches_improv " +
                $"--bundle_file={startup}/improv_rnn_generate/chord_pitches_improv.mag " +
                $"--output_dir={melodyPath} " +
                $"--num_outputs={numOfFiles} " +
                $"--backing_chords=\"{progress}\" " +
                //이부분은 추후 삭제
                "--render_chords";

            //이미 생성된 멜로디 파일이 있다면 지워준다
            if (Directory.Exists(melodyPath))
            {
                string[] files = GetMelodyPath();

                foreach (string s in files)
                {
                    File.Delete(s);
                }
            }

            RunProcess(fileName, args);
        }

        //생성된 미디파일 경로 얻기
        public static string[] GetMelodyPath()
        {
            string[] files = Directory.GetFiles(melodyPath);
            return files;
        }

        public static int RunProcess(String fileName, String args)
        {
            Process p = new Process();

            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = args;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.Start();
            p.WaitForExit();

            return p.ExitCode;
        }
    }
}
