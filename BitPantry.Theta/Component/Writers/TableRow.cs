using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    class TableRow
    {
        public Table Table { get; private set; }
        public int Index { get { return this.Table.Rows.IndexOf(this); } }

        public List<TableCell> Values { get; set; }

        public int RowHeight { get { return this.Values.Select(v => v.Lines.Count()).Max(); } }

        public TableRow(Table table)
        {
            this.Table = table;
            this.Values = new List<TableCell>();
            foreach (var column in this.Table.Columns)
                this.Values.Add(new TableCell());
        }
    }
}
