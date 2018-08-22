using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JUMO.UI
{
    public static class UICommands
    {
        /// <summary>
        /// 지정한 VST 플러그인과 패턴에 대한 피아노 롤 창을 열도록 하는 명령입니다.
        /// </summary>
        public static RoutedCommand OpenPianoRollCommand = new RoutedCommand();
    }
}
