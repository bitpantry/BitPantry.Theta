using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.API;

namespace BitPantry.Theta.Modules.Core.Commands
{
    [Command("Help")]
    [Alias("?")]
    [Synopsis("Provides help for registered commands. Help is printed to the console for the given command.")]
    class HelpCommand : InputCommand
    {
        private const string TAB = "  ";

        [Parameter(ordinalPosition:1, autoCompleteValuesFunction:"GetCommandNameValues", useAutoCompleteForValidation:true)]
        [Synopsis("The command name to get help for")]
        public string CommandName { get; set; }

        [Switch]
        [Synopsis("Whether or not to display detailed help (if available) for the given command")]
        public SwitchParameter Details { get; set; }

        [Switch]
        [Synopsis("Whether or not to display examples (if available) help for the given command")]
        public SwitchParameter Examples { get; set; }

        public override void Invoke(CommandInvocationContext context)
        {
            var def = base.Host.Commands.FirstOrDefault(c => c.CommandName.Equals(CommandName, StringComparison.OrdinalIgnoreCase)
                || c.Aliases.Contains(CommandName, StringComparer.OrdinalIgnoreCase));

            WriteHeaderHelp(def);
            WriteSyntax(def);
            WriteParameters(def);
            WriteSwitches(def);

            var cmd = Host.CommandActivatorContainer.Get(def.InputCommandType);

            if (Details.IsPresent)
                WriteDetails(cmd);
            if (Examples.IsPresent)
                WriteExamples(cmd);

            base.Out.Standard.WriteLine();
        }

        private void WriteExamples(InputCommand cmd)
        {
            base.Out.Standard.WriteLine();
            base.Out.Standard.WriteLine("EXAMPLES");
            base.Out.Standard.WriteLine();

            var docs = cmd.GetExamplesDocumentation() ?? new List<string>() { "NONE" };

            foreach (var line in docs)
                base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, line));
        }

        private void WriteDetails(InputCommand cmd)
        {
            base.Out.Standard.WriteLine();
            base.Out.Standard.WriteLine("DETAILS");
            base.Out.Standard.WriteLine();

            var docs = cmd.GetDetailsDocumentation() ?? new List<string>() { "NONE" };

            foreach (var line in docs)
                base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, line));
        }

        private void WriteSwitches(InputCommandDef def)
        {
            base.Out.Standard.WriteLine();
            base.Out.Standard.WriteLine("SWITCHES");

            foreach (var item in def.Switches)
            {
                base.Out.Standard.WriteLine();

                // write name

                base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, item.Name));

                // write attributes

                base.Out.Standard.WriteLine();

                WriteAttribute("Synopsis:", item.Synopsis);
                WriteAttribute("Aliases:", item.Aliases.Count() == 0 ? string.Empty : string.Join(", ", item.Aliases));
            }

        }

        private void WriteParameters(InputCommandDef def)
        {
            base.Out.Standard.WriteLine();
            base.Out.Standard.WriteLine("PARAMETERS");

            foreach (var param in def.Parameters)
            {
                base.Out.Standard.WriteLine();

                // write name

                base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, param.Name));

                // write attributes

                base.Out.Standard.WriteLine();

                WriteAttribute("Synopsis:", param.Synopsis);
                WriteAttribute("Aliases:", param.Aliases.Count() == 0 ? string.Empty : string.Join(", ", param.Aliases));
                WriteAttribute("IsRequired:", param.IsRequired);
                WriteAttribute("OrdinalPosition:", param.OrdinalPosition < 1 ? string.Empty : param.OrdinalPosition.ToString());
                WriteAttribute("ParameterSet:", param.ParameterSet);
                WriteAttribute("AutoComplete:", string.IsNullOrWhiteSpace(param.AutoCompleteValuesFunction));
            }
        }

        private void WriteAttribute(string label, object value)
        {
            int attributePadding = 20;

            base.Out.Standard.WriteLine("{0}{1}{2}{3}", TAB, TAB,
                label.PadRight(attributePadding),
                value);
        }

        private void WriteSyntax(InputCommandDef def)
        {
            base.Out.Standard.WriteLine();
            base.Out.Standard.WriteLine("SYNTAX");

            if (!string.IsNullOrWhiteSpace(def.DefaultParameterSet))
            {
                var noSetParameters = def.Parameters.Where(p => string.IsNullOrWhiteSpace(p.ParameterSet));
                foreach (var parameterSet in def.Parameters.Where(p => !string.IsNullOrWhiteSpace(p.ParameterSet)).Select(p => p.ParameterSet).Distinct())
                {
                    base.Out.Standard.WriteLine();
                    base.Out.Standard.WriteLine("{0}For parameter set '{1}':", TAB, parameterSet);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0} ", def.CommandName);

                    foreach (var param in def.Parameters.Where(p => !string.IsNullOrWhiteSpace(p.ParameterSet) 
                        && p.ParameterSet.Equals(parameterSet)).Union(noSetParameters))
                        sb.AppendFormat("-{0} <value> ", param.Name);

                    foreach (var param in def.Switches)
                        sb.AppendFormat("/{0} ", param.Name);

                    base.Out.Standard.WriteLine();
                    base.Out.Standard.WriteLine(string.Format("{0}{1}{2}", TAB, TAB, sb.ToString()));
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} ", def.CommandName);

                foreach (var param in def.Parameters)
                    sb.AppendFormat("-{0} <value> ", param.Name);

                foreach (var param in def.Switches)
                    sb.AppendFormat("/{0} ", param.Name);

                base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, sb.ToString()));
            }
        }

        private void WriteHeaderHelp(InputCommandDef def)
        {
            // write command name

            base.Out.Standard.WriteLine();
            base.Out.Standard.WriteLine("NAME");
            base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, def.CommandName));

            // write synopsis

            if (!string.IsNullOrWhiteSpace(def.Synopsis))
            {
                base.Out.Standard.WriteLine();
                base.Out.Standard.WriteLine("SYNOPSIS");
                base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, def.Synopsis));
            }

            // write module

            var module = base.Host.Modules.Where(m => m.CommandTypes.Contains(def.InputCommandType)).FirstOrDefault();
            if (module != null)
            {
                base.Out.Standard.WriteLine();
                base.Out.Standard.WriteLine("MODULE");
                base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, module.Name));
            }
            
            // write aliases

            if (def.Aliases.Count() > 0)
            {
                base.Out.Standard.WriteLine();
                base.Out.Standard.WriteLine("ALIASES");
                base.Out.Standard.WriteLine(string.Format("{0}{1}", TAB, string.Join(", ", def.Aliases)));
            }  
           
            
        }

        public void GetCommandNameValues(AutoCompleteValuesFunctionContext context)
        {
            foreach (var cmd in base.Host.Commands)
            {
                context.Values.Add(cmd.CommandName);
                context.Values.AddRange(cmd.Aliases);
            }
        }

    }
}
