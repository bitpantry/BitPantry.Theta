using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public class NullObjectWriter : IObjectWriter
    {
        public void Write(object obj)
        {
            // do nothing
        }

        public void JSON(object obj)
        {
            // do nothing
        }

        public void Table(System.Collections.IList data)
        {
            // do nothing
        }
    }
}
