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
        
        public int Index { get { return Table.Columns.IndexOf(this); } }

        public int ColumnWidth
        {
            get
            {
                int length = Table.Rows.Select(r => r.Values[Index].CellWidth).Max();
                if (length < HeaderText.Length)
                    length = HeaderText.Length;
                return length;
            }
        }

        public TableColumn(Table table, string headerText)
        {
            Table = table;
            HeaderText = headerText;
        }
    }
}
