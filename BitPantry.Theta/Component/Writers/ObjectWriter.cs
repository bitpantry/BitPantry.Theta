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
            _writers = writers;
            _buffer = buffer;
        }

        public void Write(object obj)
        {
            if (obj is System.Collections.IList)
            {
                var table = new Table((System.Collections.IList)obj);
                if (table.TableWidth < 200)
                    table.WriteTable(_buffer);
                else
                    JSON(obj);
            }
            else
            {
                JSON(obj);
            }
        }

        public void JSON(object obj)
        {
            try
            {
                _buffer.Out.Standard.WriteLine();

                Queue<string> lines = new Queue<string>(JsonConvert.SerializeObject(obj, Formatting.Indented)
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

                int maxWidth = lines.Select(l => l.Length).Max() + 2;

                foreach (var line in lines)
                {
                    _buffer.Out.Standard.Write(Constants.TAB);
                    _buffer.Out.Accent1.WriteLine(string.Format(" {0} ", line.PadRight(maxWidth)));
                }

                _buffer.Out.Standard.WriteLine();
                _buffer.Flush();
            }
            finally
            {
                _buffer.Clear();
            }
        }

        public void Table(System.Collections.IList data)
        {
            try
            {
                var table = new Table(data);
                table.WriteTable(_buffer);
            }
            finally
            {
                _buffer.Clear();
            }
        }

    }
}
