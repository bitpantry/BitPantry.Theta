using BitPantry.Theta.API;
using BitPantry.Theta.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta.Extensions
{
    public static class InputCommandDescriptorExtension
    {
        private static Dictionary<Type, InputCommandDef> definitionCache = new Dictionary<Type, InputCommandDef>();

        /// <summary>
        /// Returns an <typeparamref name="BitPantry.Console.API.InputCommandInfo" object/>
        /// </summary>
        /// <param name="inputCommandType">The type to define</param>
        /// <returns>Returns an <typeparamref name="BitPantry.Console.API.InputCommandInfo" object. Or NULL if the type is not an Input Command</returns>
        public static InputCommandDef DescribeInputCommand(this Type inputCommandType)
        {
            // check base type

            if (!inputCommandType.IsSubclassOf(typeof(InputCommand)))
                throw new ArgumentException(string.Format("The input command {0} does not extend {1}"
                    , inputCommandType.FullName, typeof(InputCommand).FullName));

            // check for command attribute

            Command commandAttr = GetAttributes<Command>(inputCommandType).FirstOrDefault();
            if (commandAttr == null)
                throw new ArgumentException(string.Format("The input command {0} does not have the {1} attribute"
                    , inputCommandType.FullName, typeof(Command).FullName));

            // return if already cached

            if (!definitionCache.Keys.Any(k => k.Equals(inputCommandType)))
            {

                // initialize info for new

                InputCommandDef info = new InputCommandDef();
                info.InputCommandType = inputCommandType;
                info.CommandName = commandAttr.Name ?? inputCommandType.Name;
                info.DefaultParameterSet = commandAttr.DefaultParameterSet;

                // parse aliases

                info.AddAliases(GetAttributes<Alias>(inputCommandType).Select(a => a.Value));

                // parse parameter definitions

                info.Add(GetParameterDefinitions(inputCommandType));

                // parse switches

                info.Add(GetSwitchDefinitions(inputCommandType));

                // parse synopsis

                Synopsis synopsisAttribute = GetAttributes<Synopsis>(inputCommandType).FirstOrDefault();
                info.Synopsis = synopsisAttribute == null ? null : synopsisAttribute.Value;

                CommandValidator.Validate(info);

                // cache new definition

                definitionCache.Add(inputCommandType, info);
            }

            return definitionCache[inputCommandType];
        }


        private static List<SwitchDef> GetSwitchDefinitions(Type type)
        {
            List<SwitchDef> switches = new List<SwitchDef>();

            PropertyInfo[] propertiesList = type.GetProperties();

            foreach (var property in propertiesList)
            {
                Switch switchAttribute = GetAttributes<Switch>(property).FirstOrDefault();
                Synopsis synopsisAttribute = GetAttributes<Synopsis>(property).FirstOrDefault();

                if (switchAttribute != null)
                {
                    SwitchDef newSwitch = new SwitchDef();
                    newSwitch.PropertyInfo = property;
                    newSwitch.Name = switchAttribute.Name ?? property.Name;
                    newSwitch.AddAliases(GetAttributes<Alias>(property).Select(a => a.Value));
                    newSwitch.Synopsis = synopsisAttribute == null ? null : synopsisAttribute.Value;

                    switches.Add(newSwitch);
                }
            }

            return switches;
        }

        /// <summary>
        /// Returns information about the parameters found on the given type
        /// </summary>
        /// <param name="type">The type to find Parameter attributes for</param>
        /// <returns>A list of ParameterDef objects which describe the InputCommandNamed arameters
        /// found on in the type</returns>
        private static List<ParameterDef> GetParameterDefinitions(Type type)
        {
            List<ParameterDef> infoList = new List<ParameterDef>();

            PropertyInfo[] propertiesList = type.GetProperties();
            foreach (var property in propertiesList)
            {
                Parameter parameterAttribute = GetAttributes<Parameter>(property).FirstOrDefault();
                Synopsis synopsisAttribute = GetAttributes<Synopsis>(property).FirstOrDefault();

                if (parameterAttribute != null)
                {
                    ParameterDef info = new ParameterDef();
                    info.PropertyInfo = property;
                    info.Name = parameterAttribute.ParameterName ?? property.Name;
                    info.AddAliases(GetAttributes<Alias>(property).Select(a => a.Value));
                    info.IsRequired = parameterAttribute.IsRequired;
                    info.OrdinalPosition = parameterAttribute.OrdinalPosition;
                    info.Synopsis = synopsisAttribute == null ? null : synopsisAttribute.Value;
                    info.ParameterSet = parameterAttribute.ParameterSet;
                    info.AutoCompleteValuesFunction = parameterAttribute.AutoCompleteValuesFunction;
                    info.UseAutoCompleteForValidation = parameterAttribute.UseAutoCompleteForValidation;
                    info.ValidationFunction = parameterAttribute.ValidationFunction;

                    infoList.Add(info);
                }
            }

            return infoList;
        }

        // internal helper function to return attributes from a type
        private static List<T> GetAttributes<T>(Type type)
        {
            object[] attributeList = type.GetCustomAttributes(typeof(T), true);
            return attributeList.Select(p => (T)p).ToList();
        }

        // internal helper function to return attributes from a property
        private static List<T> GetAttributes<T>(PropertyInfo property)
        {
            object[] attributeList = property.GetCustomAttributes(typeof(T), true);
            return attributeList.Select(p => (T)p).ToList();
        }
    }
}
