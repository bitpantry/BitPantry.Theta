using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public class HostWriterCollection : IWriterCollection
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

        public HostWriterCollection(Action<string, HostWriterContext> onWriteAction, IBufferedWriter buffer)
        {
            this.Standard = new HostWriter(onWriteAction, Constants.COLOR_STANDARD_FOREGROUND, Constants.COLOR_STANDARD_HIGHLIGHT);
            this.Warning = new HostWriter(onWriteAction, Constants.COLOR_WARNING_FOREGROUND, Constants.COLOR_WARNING_HIGHLIGHT);
            this.Error = new HostWriter(onWriteAction, Constants.COLOR_ERROR_FOREGROUND, Constants.COLOR_ERROR_HIGHLIGHT);
            this.Debug = new HostWriter(onWriteAction, Constants.COLOR_DEBUG_FOREGROUND, Constants.COLOR_DEBUG_HIGHLIGHT);
            this.Verbose = new HostWriter(onWriteAction, Constants.COLOR_VERBOSE_FOREGROUND, Constants.COLOR_VERBOSE_HIGHLIGHT);

            this.Accent1 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT1_FOREGROUND, Constants.COLOR_ACCENT1_HIGHLIGHT);
            this.Accent2 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT2_FOREGROUND, Constants.COLOR_ACCENT2_HIGHLIGHT);
            this.Accent3 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT3_FOREGROUND, Constants.COLOR_ACCENT3_HIGHLIGHT);
            this.Accent4 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT4_FOREGROUND, Constants.COLOR_ACCENT4_HIGHLIGHT);
            this.Accent5 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT5_FOREGROUND, Constants.COLOR_ACCENT5_HIGHLIGHT);

            this.Object = new ObjectWriter(this, buffer);
        }
    }
}
