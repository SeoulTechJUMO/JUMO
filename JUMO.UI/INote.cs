using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUMO.UI
{
    public interface INote
    {
        int Value { get; }
        int Velocity { get; }
        int Start { get; }
        int Length { get; }
    }
}
