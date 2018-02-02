using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public class NullWriter : InterceptWriter
    {
        protected override void OnWrite(string str)
        {
            // do nothing - this is a null writer
        }
    }
}
