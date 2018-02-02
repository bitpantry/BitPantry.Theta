using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BitPantry.Theta.Component.Writers
{
    public class HostWriter : InterceptWriter
    {
        private object _locker = new object();

        private Action<string, HostWriterContext> _onWriteAction = null;

        private System.Drawing.Color _foreColor = System.Drawing.Color.Empty;
        private System.Drawing.Color _highlightColor = System.Drawing.Color.Empty;

        public HostWriter(Action<string, HostWriterContext> onWriteAction) : this(onWriteAction, System.Drawing.Color.Empty, System.Drawing.Color.Empty) { }
        public HostWriter(Action<string, HostWriterContext> onWriteAction, System.Drawing.Color foreColor) : this(onWriteAction, foreColor, System.Drawing.Color.Empty) { }
        public HostWriter(Action<string, HostWriterContext> onWriteAction, System.Drawing.Color foreColor, System.Drawing.Color highlightColor)
        {
            this._onWriteAction = onWriteAction;
            this._foreColor = foreColor;
            this._highlightColor = highlightColor;
        }

        protected override void OnWrite(string str)
        {
            lock (this._locker)
            {
                // split multiline output string

                string splitterToken = Guid.NewGuid().ToString().Substring(0, 5);
                str = str.Replace("|", splitterToken);
                str = str.Replace(Environment.NewLine, "|");

                List<string> lines = new List<string>(str.Split('|'));

                // apply formatting

                for (int i = 0; i < lines.Count - 1; i++)
                    lines[i] = string.Format("{0}{1}", lines[i].Replace(splitterToken, "|"), Environment.NewLine);

                // remove trailing empty line (not a line break)

                if (string.IsNullOrEmpty(lines.Last())) lines.Remove(lines.Last());

                var context = new HostWriterContext()
                {
                    ForeColor = this._foreColor,
                    HighlightColor = this._highlightColor
                };

                foreach (var ln in lines)
                    this._onWriteAction(ln, context);

            }
        }


    }
}
