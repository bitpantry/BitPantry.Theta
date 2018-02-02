using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public interface IBufferedWriter
    {
        IWriterCollection Out { get; }
        void Clear();
        void Flush();
    }
}
