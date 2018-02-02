using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Modules.Variables
{
    class TableRecords
    {
        #region VARIABLE CONTEXT

        public static List<dynamic> CreateVariableContextRecordList(params VariableContext[] contexts)
        {
            List<dynamic> records = new List<dynamic>();

            if (contexts != null && contexts.Count() > 0)
            {
                foreach (var item in contexts)
                {
                    records.Add(CreateVariableContextTableRecord(
                        item.Name,
                        item.Description,
                        VariableContextLogic.Instance.CurrentContext == item ? "Loaded" : "Not Loaded"
                        ));
                }
            }
            else
            {
                records.Add(CreateVariableContextTableRecord(string.Empty, string.Empty, string.Empty));
            }

            return records;
        }

        private static dynamic CreateVariableContextTableRecord(string name, string description, string status)
        {
            return new
            {
                Name = name,
                Description = description,
                Status = status
            };
        }

        #endregion

        #region VARIABLE

        public static List<dynamic> CreateVariableRecordList(params VariableContextVariable[] variables)
        {
            List<dynamic> records = new List<dynamic>();

            if (variables != null && variables.Count() > 0)
            {
                foreach (var item in variables)
                {
                    records.Add(CreateVariableTableRecord(
                        item.Name,
                        item.Value));
                }
            }
            else
            {
                records.Add(CreateVariableTableRecord(string.Empty, string.Empty));
            }

            return records;
        }

        private static dynamic CreateVariableTableRecord(string name, string value)
        {
            return new
            {
                Name = name,
                Value = value
            };
        }

        #endregion



    }
}
