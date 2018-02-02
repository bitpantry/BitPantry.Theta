using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BitPantry.Theta.Modules.Variables
{
    public class VariableContextLogic
    {
        public const string DEFAULT_VARIABLE_CONTEXT_NAME = "Default";
        public const string FILE_PATH_VARIABLES = "variables.xml";

        public VariableContextCollection VariableContextCollection { get; private set; }

        public VariableContext CurrentContext { get; set; }

        #region INITIALIZATION LOGIC

        private static VariableContextLogic _instance = null;
        public static VariableContextLogic Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VariableContextLogic();
                return _instance;
            }
        }

        private VariableContextLogic() 
        {
            this.DiscardChanges();
            if (this.CurrentContext == null)
            {
                if (!this.VariableContextCollection.Contexts.Any(c => c.Name.Equals(DEFAULT_VARIABLE_CONTEXT_NAME)))
                {
                    this.VariableContextCollection.Contexts.Add(new VariableContext()
                    {
                        Name = DEFAULT_VARIABLE_CONTEXT_NAME,
                        Description = "Default variable context. This context is loaded by default when the module is initialized"
                    });
                
                    this.Save();
                }

                this.CurrentContext = this.VariableContextCollection.Contexts.FirstOrDefault(c => c.Name.Equals(DEFAULT_VARIABLE_CONTEXT_NAME));
            }
        }

        #endregion

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(VariableContextCollection));
            using (FileStream stream = File.Create(FILE_PATH_VARIABLES))
                serializer.Serialize(stream, this.VariableContextCollection);
        }

        public void DiscardChanges()
        {
            if (new FileInfo(FILE_PATH_VARIABLES).Exists)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VariableContextCollection));
                using (FileStream stream = File.OpenRead(FILE_PATH_VARIABLES))
                    this.VariableContextCollection = (VariableContextCollection)serializer.Deserialize(stream);
            }
            else
            {
                this.VariableContextCollection = new VariableContextCollection();
                this.Save();
            }
        }
    }
}
