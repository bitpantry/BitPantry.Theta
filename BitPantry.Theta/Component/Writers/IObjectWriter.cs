using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public interface IObjectWriter
    {
        void Write(object obj);
        void JSON(object obj);
        void Table(System.Collections.IList obj);
    }
}
