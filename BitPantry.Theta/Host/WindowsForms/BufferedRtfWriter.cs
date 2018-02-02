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
            _rtb = rtb;
            CreateBuffer();
            Out = new HostWriterCollection(OnHostWriterActionHandler, this);
        }

        private void CreateBuffer()
        {
            if (_rtb.InvokeRequired) { _rtb.BeginInvoke((MethodInvoker)delegate { CreateBuffer(); }, null); }
            else
            {
                _buffer = new RichTextBox();
                _buffer.Font = Constants.FONT_STANDARD;
                _buffer.BackColor = Constants.COLOR_BACKGROUND;
                _buffer.ForeColor = Constants.COLOR_STANDARD_FOREGROUND;
                _buffer.CreateControl();
                _buffer.Enabled = false;
            }
        }

        private void OnHostWriterActionHandler(string str, HostWriterContext context)
        {
            if (_buffer.InvokeRequired) { _buffer.Invoke((MethodInvoker)delegate { OnHostWriterActionHandler(str, context); }); }
            else
            {
                System.Drawing.Color backupForecolor = _buffer.SelectionColor;
                System.Drawing.Color backupHighlight = _buffer.SelectionBackColor;

                if (context.ForeColor != System.Drawing.Color.Empty)
                    _buffer.SelectionColor = context.ForeColor;

                if (context.HighlightColor != System.Drawing.Color.Empty)
                    _buffer.SelectionBackColor = context.HighlightColor;

                _buffer.AppendText(str);

                _buffer.SelectionFont = Constants.FONT_STANDARD;
                _buffer.SelectionColor = backupForecolor;
                _buffer.SelectionBackColor = backupHighlight;
            }
        }

        public void Clear()
        {
            if (_buffer.InvokeRequired)
                _buffer.Invoke((MethodInvoker)delegate { Clear(); }, null); 
            else
                _buffer.Clear();
        }

        public void Flush()
        {
            if (_rtb.InvokeRequired) { _rtb.Invoke((MethodInvoker)delegate { Flush(); }, null); }
            else
            {
                _rtb.SelectedRtf = _buffer.Rtf;
                Clear();
            }
        }

        public void Dispose()
        {
            if (_buffer != null && _buffer.InvokeRequired) { _buffer.BeginInvoke((MethodInvoker)delegate { Dispose(); }, null); }
            else
            {
                if (_buffer != null && !_buffer.IsDisposed)
                    _buffer.Dispose();
            }
        }
    }
}
