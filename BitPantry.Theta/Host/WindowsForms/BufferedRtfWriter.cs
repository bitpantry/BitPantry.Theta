using BitPantry.Theta.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.Component.Writers;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms
{
    class BufferedRtfWriter : IBufferedWriter, IDisposable
    {
        private RichTextBox _rtb = null;
        private RichTextBox _buffer = null;

        public IWriterCollection Out { get; private set; }

        internal BufferedRtfWriter(RichTextBox rtb)
        {
            this._rtb = rtb;
            this.CreateBuffer();
            this.Out = new HostWriterCollection(this.OnHostWriterActionHandler, this);
        }

        private void CreateBuffer()
        {
            if (this._rtb.InvokeRequired) { this._rtb.BeginInvoke((MethodInvoker)delegate { this.CreateBuffer(); }, null); }
            else
            {
                this._buffer = new RichTextBox();
                this._buffer.Font = Constants.FONT_STANDARD;
                this._buffer.BackColor = Constants.COLOR_BACKGROUND;
                this._buffer.ForeColor = Constants.COLOR_STANDARD_FOREGROUND;
                this._buffer.CreateControl();
                this._buffer.Enabled = false;
            }
        }

        private void OnHostWriterActionHandler(string str, HostWriterContext context)
        {
            if (this._buffer.InvokeRequired) { this._buffer.Invoke((MethodInvoker)delegate { OnHostWriterActionHandler(str, context); }); }
            else
            {
                System.Drawing.Color backupForecolor = this._buffer.SelectionColor;
                System.Drawing.Color backupHighlight = this._buffer.SelectionBackColor;

                if (context.ForeColor != System.Drawing.Color.Empty)
                    this._buffer.SelectionColor = context.ForeColor;

                if (context.HighlightColor != System.Drawing.Color.Empty)
                    this._buffer.SelectionBackColor = context.HighlightColor;

                this._buffer.AppendText(str);

                this._buffer.SelectionFont = Constants.FONT_STANDARD;
                this._buffer.SelectionColor = backupForecolor;
                this._buffer.SelectionBackColor = backupHighlight;
            }
        }

        public void Clear()
        {
            if (this._buffer.InvokeRequired) 
                this._buffer.Invoke((MethodInvoker)delegate { this.Clear(); }, null); 
            else
                this._buffer.Clear();
        }

        public void Flush()
        {
            if (this._rtb.InvokeRequired) { this._rtb.Invoke((MethodInvoker)delegate { this.Flush(); }, null); }
            else
            {
                this._rtb.SelectedRtf = this._buffer.Rtf;
                this.Clear();
            }
        }

        public void Dispose()
        {
            if (this._buffer != null && this._buffer.InvokeRequired) { this._buffer.BeginInvoke((MethodInvoker)delegate { this.Dispose(); }, null); }
            else
            {
                if (this._buffer != null && !this._buffer.IsDisposed)
                    this._buffer.Dispose();
            }
        }
    }
}
