using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using BitPantry.Parsing.Strings;

namespace BitPantry.Theta.Component
{
    class CommandValidator
    {
        public static void Validate(InputCommandDef def)
        {
            ValidateAliases(def);
            ValidateParameterSets(def);
            ValidateAutoCompleteFunctions(def);
            ValidateValidationFunctions(def);
            ValidateParameterTypes(def);
        }

        private static void ValidateParameterTypes(InputCommandDef def)
        {
            foreach (var param in def.Parameters)
            {
                if (param.PropertyInfo.PropertyType != typeof(string)
                    && StringParsing.GetParser(param.PropertyInfo.PropertyType) == null)
                {
                    throw new InvalidCommandException(def.InputCommandType
                        , string.Format("The parameter '{0}' for command {1} has a property type of {2}. There are no available property input TypeParsers available for that type."
                        , param.Name, def.InputCommandType, param.PropertyInfo.PropertyType));
                }
            }
        }

        private static void ValidateValidationFunctions(InputCommandDef def)
        {
            foreach (var param in def.Parameters)
            {
                if (param.ValidationFunction != null)
                {
                    if (def.InputCommandType.GetMethod(param.ValidationFunction) == null)
                    {
                        throw new InvalidCommandException(def.InputCommandType
                           , string.Format("The parameter '{0}' for command {1} defines a validation function '{2}' which cannot be found"
                           , param.Name, def.InputCommandType, param.ValidationFunction));
                    }
                    else
                    {
                        var methodInfo = def.InputCommandType.GetMethod(param.ValidationFunction);
                        var parameters = methodInfo.GetParameters();

                        if (parameters.Count() != 1 && parameters[0].ParameterType != typeof(ValidationFunctionContext))
                            throw new InvalidCommandException(def.InputCommandType
                                , string.Format("The parameter '{0}' for command {1} must define an validation function {2} which accepts a single input parameter of type {3}"
                                , param.Name, def.InputCommandType, param.ValidationFunction, typeof(ValidationFunctionContext).FullName));

                        if(param.UseAutoCompleteForValidation)
                            throw new InvalidCommandException(def.InputCommandType
                                , string.Format("The parameter '{0}' specifies that the auto complete function '{1}' should be used for validation, but a validation function '{2} is also defined."
                                , param.Name, param.AutoCompleteValuesFunction, param.ValidationFunction));
                    }
                }
            }
        }

        private static void ValidateAutoCompleteFunctions(InputCommandDef def)
        {
            foreach (var param in def.Parameters)
            {
                if (param.AutoCompleteValuesFunction != null)
                {
                    if (def.InputCommandType.GetMethod(param.AutoCompleteValuesFunction) == null)
                    {
                        throw new InvalidCommandException(def.InputCommandType
                           , string.Format("The parameter '{0}' for command {1} defines an auto complete values function '{2}' which cannot be found"
                           , param.Name, def.InputCommandType, param.AutoCompleteValuesFunction));
                    }
                    else
                    {
                        var methodInfo = def.InputCommandType.GetMethod(param.AutoCompleteValuesFunction);
                        var parameters = methodInfo.GetParameters();

                        if (parameters.Count() != 1 && parameters[0].ParameterType != typeof(AutoCompleteValuesFunctionContext))
                            throw new InvalidCommandException(def.InputCommandType
                                , string.Format("The parameter '{0}' for command {1} must define an auto complete values function {2} which defines a single input parameter of type {3}"
                                , param.Name, def.InputCommandType, param.AutoCompleteValuesFunction, typeof(AutoCompleteValuesFunctionContext).FullName));
                    }
                }
                else if (param.UseAutoCompleteForValidation)
                {
                    throw new InvalidCommandException(def.InputCommandType
                        , string.Format("The parameter '{0}' specifies that the auto complete function should be used for validation, but no auto complete function could be found"
                        , param.Name));
                }
            }
        }

        private static void ValidateAliases(InputCommandDef def)
        {
            // validate parameters

            List<string> uniqueParameterNames = new List<string>();
            foreach (var param in def.Parameters)
            {
                if (uniqueParameterNames.Contains(param.Name, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidCommandException(def.InputCommandType
                        , string.Format("The command {0} contains a duplicate parameter name or alias '{1}'", param.Name));

                uniqueParameterNames.Add(param.Name);

                foreach (var alias in param.Aliases)
                {
                    if(uniqueParameterNames.Contains(alias, StringComparer.OrdinalIgnoreCase))
                        throw new InvalidCommandException(def.InputCommandType
                            , string.Format("The command {0} contains a duplicate parameter name or alias '{1}'", param.Name));

                    uniqueParameterNames.Add(alias);
                }
            }

            // validate command aliases

            List<string> commandNames = new List<string>();
            commandNames.Add(def.CommandName);

            foreach (var alias in def.Aliases)
            {
                if(commandNames.Contains(alias, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidCommandException(def.InputCommandType
                        , string.Format("The command {0} contains a duplicate command name or alias '{1}'", alias));
            }
        }

        private static void ValidateParameterSets(InputCommandDef info)
        {
            var usedParameterSets = info.Parameters.Where(p => !string.IsNullOrEmpty(p.ParameterSet)).Select(p => p.ParameterSet).Distinct().ToList();
            if (usedParameterSets.Count > 0 && string.IsNullOrEmpty(info.DefaultParameterSet))
                throw new InvalidCommandException(info.InputCommandType
                    , string.Format("The input command {0} has parameters that define parameter sets, but a default parameter set is not defined",
                        info.InputCommandType.FullName));

            if (!string.IsNullOrEmpty(info.DefaultParameterSet) && !usedParameterSets.Contains(info.DefaultParameterSet))
                    throw new InvalidCommandException(info.InputCommandType
                        , string.Format("The input command {0} defines a default parameter set, but no parameters are in the defined set"
                            , info.InputCommandType.FullName));
        }
    }
}
