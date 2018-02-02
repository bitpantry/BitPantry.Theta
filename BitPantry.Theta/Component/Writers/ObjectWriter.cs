using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    public class ObjectWriter : IObjectWriter
    {
        private IWriterCollection _writers = null;
        private IBufferedWriter _buffer = null;

        internal ObjectWriter(IWriterCollection writers, IBufferedWriter buffer)
        {
            this._writers = writers;
            this._buffer = buffer;
        }

        public void Write(object obj)
        {
            if (obj is System.Collections.IList)
            {
                var table = new Table((System.Collections.IList)obj);
                if (table.TableWidth < 200)
                    table.WriteTable(this._buffer);
                else
                    this.JSON(obj);
            }
            else
            {
                this.JSON(obj);
            }
        }

        public void JSON(object obj)
        {
            try
            {
                this._buffer.Out.Standard.WriteLine();

                Queue<string> lines = new Queue<string>(JsonConvert.SerializeObject(obj, Formatting.Indented)
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

                int maxWidth = lines.Select(l => l.Length).Max() + 2;

                foreach (var line in lines)
                {
                    this._buffer.Out.Standard.Write(Constants.TAB);
                    this._buffer.Out.Accent1.WriteLine(string.Format(" {0} ", line.PadRight(maxWidth)));
                }

                this._buffer.Out.Standard.WriteLine();
                this._buffer.Flush();
            }
            finally
            {
                this._buffer.Clear();
            }
        }

        public void Table(System.Collections.IList data)
        {
            try
            {
                var table = new Table(data);
                table.WriteTable(this._buffer);
            }
            finally
            {
                this._buffer.Clear();
            }
        }

    }
}
