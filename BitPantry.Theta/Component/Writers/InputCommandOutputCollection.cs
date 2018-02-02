using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public class InputCommandOutputCollection : IWriterCollection
    {
        public InterceptWriter Standard { get; private set; }
        public InterceptWriter Warning { get; private set; }
        public InterceptWriter Error { get; private set; }
        public InterceptWriter Debug { get; private set; }
        public InterceptWriter Verbose { get; private set; }

        public InterceptWriter Accent1 { get; private set; }
        public InterceptWriter Accent2 { get; private set; }
        public InterceptWriter Accent3 { get; private set; }
        public InterceptWriter Accent4 { get; private set; }
        public InterceptWriter Accent5 { get; private set; }

        public IObjectWriter Object { get; private set; }

        public InputCommandOutputCollection(InterceptWriter standard, InterceptWriter warning
            , InterceptWriter error, InterceptWriter debug, InterceptWriter verbose
            , InterceptWriter accent1, InterceptWriter accent2, InterceptWriter accent3, InterceptWriter accent4
            , InterceptWriter accent5, IObjectWriter objectWriter)
        {
            Standard = standard;
            Warning = warning;
            Error = error;
            Debug = debug;
            Verbose = verbose;
            Accent1 = accent1;
            Accent2 = accent2;
            Accent3 = accent3;
            Accent4 = accent4;
            Accent5 = accent5;
            Object = objectWriter;
        }
    }
}
