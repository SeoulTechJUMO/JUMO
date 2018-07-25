using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO
{
    /// <summary>
    /// 악보 상의 음표 하나를 나타냅니다.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// 음표의 값을 가져오거나 설정합니다. 이 값은 건반 상의 위치나 퍼커션 종류 등을 의미합니다.
        /// </summary>
        public byte Value { get; set; }

        /// <summary>
        /// 음표의 velocity 값을 가져오거나 설정합니다.
        /// </summary>
        public byte Velocity { get; set; }

        /// <summary>
        /// 음표의 시작 시점을 가져오거나 설정합니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// 음표의 길이를 가져오거나 설정합니다. PPQN에 의한 상대적인 단위를 사용합니다.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 새로운 Note 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="value">음표의 값</param>
        /// <param name="velocity">음표의 velocity 값</param>
        /// <param name="start">음표의 시작 시점 (PPQN 기반)</param>
        /// <param name="length">음표의 길이 (PPQN 기반)</param>
        public Note(byte value, byte velocity, long start, long length)
        {
            Value = value;
            Velocity = velocity;
            Start = start;
            Length = length;
        }
    }
}
