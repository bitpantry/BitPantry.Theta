using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Component.Writers
{
    

    class TableColumn
    {
        public Table Table { get; private set; }
        public string HeaderText { get; set; }
        
        public int Index { get { return this.Table.Columns.IndexOf(this); } }

        public int ColumnWidth
        {
            get
            {
                int length = this.Table.Rows.Select(r => r.Values[this.Index].CellWidth).Max();
                if (length < this.HeaderText.Length)
                    length = this.HeaderText.Length;
                return length;
            }
        }

        public TableColumn(Table table, string headerText)
        {
            this.Table = table;
            this.HeaderText = headerText;
        }
    }
}
