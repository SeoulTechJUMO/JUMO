using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ChordMagicianModel
{
    public class CreateMelody
    {
        //JUMO 프로젝트 안에 마젠타를 포함시킨 경우
        public void RunMagenta(string Progress)
        {
            string StartUp = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

            string FileName = StartUp + "/improv_rnn_generate/improv_rnn_generate.exe";
            string Args = "--config=chord_pitches_improv " +
                "--bundle_file=" + StartUp + "/improv_rnn_generate/chord_pitches_improv.mag " +
                "--output_dir=" + StartUp + "/ChordMagicianModel/Melody " +
                "--num_outputs=5 " +
                "--backing_chords=\"" + Progress + "\" "+
                //이부분은 추후 삭제
                "--render_chords";

            RunProcess(FileName, Args);
        }

        public int RunProcess(String FileName, String Args)
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
