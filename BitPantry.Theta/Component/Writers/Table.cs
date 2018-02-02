using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    enum DataType
    {
        Reference,
        Value
    }

    class Table
    {
        private int _leftPadding = 5;

        public IList Data { get; set; }
        public List<TableColumn> Columns { get; set; }
        public DataType DataType { get; set; }
        public List<TableRow> Rows { get; set; }

        public int TableWidth { get { return this._leftPadding + this.Columns.Select(c => c.ColumnWidth).Sum(); } }

        public Table(IList data)
        {
            this.Data = data;
            this.Columns = new List<TableColumn>();
            this.Rows = new List<TableRow>();

            if (data != null && data.Count > 0) // is there any data to render
            {
                // find the first not null item in the list to create columns from

                object firstNotNullItem = null;
                foreach (var item in this.Data)
                {
                    if (item != null)
                    {
                        firstNotNullItem = item;
                        break;
                    }
                }

                // build table if there is any non null items in the list
                
                if (firstNotNullItem != null)
                {
                    if (data[0].GetType().IsValueType || data[0].GetType() == typeof(string))
                        this.DataType = Writers.DataType.Value;
                    else
                        this.DataType = Writers.DataType.Reference;

                    this.LoadColumns();
                    this.LoadRows();
                }
            }
        }

        

        private void LoadRows()
        {
            foreach (var item in this.Data)
            {
                if (item != null)
                {
                    if (this.DataType == Writers.DataType.Value)
                    {
                        this.Rows.Add(new TableRow(this));
                        this.Rows.Last().Values[0].Value = item.ToString();
                    }
                    else
                    {
                        var row = new TableRow(this);

                        foreach (var column in this.Columns)
                        {
                            var value = item.GetType().GetProperty(column.HeaderText).GetValue(item);
                            row.Values[column.Index].Value = value == null ? string.Empty : value.ToString();
                        }

                        this.Rows.Add(row);
                    }
                }
            }
        }

        private void LoadColumns()
        {
            if (this.DataType == Writers.DataType.Value)
            {
                this.Columns.Add(new TableColumn(this, "Value"));
            }
            else
            {
                foreach (var prop in this.Data[0].GetType().GetProperties())
                    this.Columns.Add(new TableColumn(this, prop.Name));
            }
        }

        public void WriteTable(IBufferedWriter buffer)
        {
            int padding = 5;

            // write header text row

            if (this.Columns.Count > 0)
            {

                buffer.Out.Standard.WriteLineAsync();

                buffer.Out.Standard.WriteAsync(string.Empty.PadLeft(this._leftPadding));

                foreach (var column in this.Columns)
                    buffer.Out.Standard.WriteAsync(column.HeaderText.PadRight(column.ColumnWidth + padding));

                buffer.Out.Standard.WriteLineAsync();

                // write header underline row

                buffer.Out.Standard.WriteAsync(string.Empty.PadLeft(this._leftPadding));

                foreach (var column in this.Columns)
                    buffer.Out.Standard.WriteAsync(string.Empty.PadRight(column.ColumnWidth, '=').PadRight(column.ColumnWidth + padding));

                buffer.Out.Standard.WriteLineAsync();

                // write rows

                var writer = buffer.Out.Accent1;

                foreach (var row in this.Rows)
                {
                    if (writer == buffer.Out.Accent1)
                        writer = buffer.Out.Standard;
                    else
                        writer = buffer.Out.Accent1;

                    for (int i = 0; i < row.RowHeight; i++)
                    {
                        buffer.Out.Standard.WriteAsync(string.Empty.PadLeft(this._leftPadding));

                        foreach (var column in this.Columns)
                        {
                            if (row.Values[column.Index].Lines.Count < i + 1)
                                writer.WriteAsync(string.Empty.PadRight(column.ColumnWidth + padding));
                            else
                                writer.WriteAsync(row.Values[column.Index].Lines[i].PadRight(column.ColumnWidth + padding));
                        }

                        buffer.Out.Standard.WriteLineAsync();
                    }

                }

                buffer.Flush();
            }

        }
        
    }
}
