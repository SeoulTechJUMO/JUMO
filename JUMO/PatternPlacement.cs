using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    /// <summary>
    /// 트랙 상에 배치된 패턴을 나타냅니다.
    /// </summary>
    class PatternPlacement
    {
        /// <summary>
        /// 배치된 패턴 인스턴스를 가져옵니다.
        /// </summary>
        public Pattern Pattern { get; }

        /// <summary>
        /// 배치된 패턴의 시작 시점을 가져오거나 설정합니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// 배치된 패턴의 길이를 가져옵니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length => Pattern.Length;

        /// <summary>
        /// 새로운 PatternPlacement 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="pattern">배치된 패턴</param>
        /// <param name="start">배치된 패턴의 시작 지점 (PPQN 기반)</param>
        public PatternPlacement(Pattern pattern, long start)
        {
            Pattern = pattern;
            Start = start;
        }
    }
}
