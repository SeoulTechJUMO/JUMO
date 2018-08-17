using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUMO.Vst;

namespace JUMO
{
    /// <summary>
    /// 음악을 구성하는 각각의 패턴을 나타냅니다.
    /// </summary>
    public class Pattern
    {
        /// <summary>
        /// 패턴의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 각각의 VST 플러그인에 대한 악보 정보를 저장하는 Dictionary를 가져옵니다.
        /// </summary>
        public IDictionary<Plugin, IEnumerable<Note>> Scores { get; } = new Dictionary<Plugin, IEnumerable<Note>>();

        /// <summary>
        /// 패턴의 길이를 가져오거나 설정합니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 새로운 Pattern 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="name">패턴의 이름</param>
        public Pattern(string name) => Name = name;
    }
}
