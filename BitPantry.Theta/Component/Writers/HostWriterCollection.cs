using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public class HostWriterCollection : IWriterCollection
    {
        private readonly Action<string, HostWriterContext> onWriteAction;

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

        public HostWriter CreateWriter(Color foregroundColor, Color highlightColor)
        {
            return new HostWriter(onWriteAction, foregroundColor, highlightColor);
        }

        public HostWriterCollection(Action<string, HostWriterContext> onWriteAction, IBufferedWriter buffer)
        {
            this.onWriteAction = onWriteAction;

            Standard = new HostWriter(onWriteAction, Constants.COLOR_STANDARD_FOREGROUND, Constants.COLOR_STANDARD_HIGHLIGHT);
            Warning = new HostWriter(onWriteAction, Constants.COLOR_WARNING_FOREGROUND, Constants.COLOR_WARNING_HIGHLIGHT);
            Error = new HostWriter(onWriteAction, Constants.COLOR_ERROR_FOREGROUND, Constants.COLOR_ERROR_HIGHLIGHT);
            Debug = new HostWriter(onWriteAction, Constants.COLOR_DEBUG_FOREGROUND, Constants.COLOR_DEBUG_HIGHLIGHT);
            Verbose = new HostWriter(onWriteAction, Constants.COLOR_VERBOSE_FOREGROUND, Constants.COLOR_VERBOSE_HIGHLIGHT);

            Accent1 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT1_FOREGROUND, Constants.COLOR_ACCENT1_HIGHLIGHT);
            Accent2 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT2_FOREGROUND, Constants.COLOR_ACCENT2_HIGHLIGHT);
            Accent3 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT3_FOREGROUND, Constants.COLOR_ACCENT3_HIGHLIGHT);
            Accent4 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT4_FOREGROUND, Constants.COLOR_ACCENT4_HIGHLIGHT);
            Accent5 = new HostWriter(onWriteAction, Constants.COLOR_ACCENT5_FOREGROUND, Constants.COLOR_ACCENT5_HIGHLIGHT);

            Object = new ObjectWriter(this, buffer);
        }
    }
}
