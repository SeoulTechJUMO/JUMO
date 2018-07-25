using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IEnumerable<Score> Scores { get; set; }
        public IEnumerable<VstChannel> Channels { get; }

        /// <summary>
        /// 패턴의 길이를 가져오거나 설정합니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length { get; set; }

        // TODO
        public Pattern() => throw new NotImplementedException();
    }
}
