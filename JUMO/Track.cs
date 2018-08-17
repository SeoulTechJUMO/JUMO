using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    /// <summary>
    /// 패턴을 배치할 수 있는 트랙을 나타냅니다.
    /// </summary>
    public class Track
    {
        /// <summary>
        /// 트랙의 이름을 가져오거나 설정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 트랙에 배치된 패턴을 저장하는 컬렉션을 가져옵니다.
        /// </summary>
        public ICollection<PatternPlacement> Patterns { get; } = new List<PatternPlacement>();

        /// <summary>
        /// 트랙의 총 길이를 가져옵니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length => Patterns.Count == 0 ? 0 : Patterns.Max(pp => pp.Start + pp.Length);

        /// <summary>
        /// 새로운 Track 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="name">새 트랙의 이름</param>
        public Track(string name) => Name = name;
    }
}
