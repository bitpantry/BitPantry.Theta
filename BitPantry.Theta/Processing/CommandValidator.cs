using BitPantry.Theta.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPantry.Theta.Extensions;
using BitPantry.Theta.Component;
using BitPantry.Theta.Host;

namespace BitPantry.Theta.Processing
{
    class CommandValidator
    {
        public InputCommand Command { get; set; }
        public List<CommandValidatorError> Errors { get; set; }
        public IHostInterface Host { get; private set; }

        public CommandValidator(InputCommand command, IHostInterface host)
        {
            this.Command = command;
            this.Host = host;
            this.Errors = new List<CommandValidatorError>();

            this.EvaluateParameterAnalysis();
            this.ValidateParameterValues();
        }

        private void ValidateParameterValues()
        {
            var definition = this.Command.GetType().DescribeInputCommand();

            foreach (var param in definition.Parameters.Where(p => 
                (p.UseAutoCompleteForValidation && !string.IsNullOrWhiteSpace(p.AutoCompleteValuesFunction)) 
                || !string.IsNullOrWhiteSpace(p.ValidationFunction)))
            {
                // get parameter value

                string value = param.PropertyInfo.GetValue(this.Command)?.ToString();
                
                if (value != null) // compare to validation values
                {
                    // get validation values

                    try
                    {
                        bool isValid = false;
                        string validationMessage = null;

                        if (param.UseAutoCompleteForValidation)
                        {
                            var methodInfo = this.Command.GetType().GetMethod(param.AutoCompleteValuesFunction);

                            var context = new AutoCompleteValuesFunctionContext();
                            methodInfo.Invoke(this.Command, new object[] { context });
                            isValid = context.Values.Contains(value, StringComparer.OrdinalIgnoreCase);
                        }
                        else
                        {
                            var methodInfo = this.Command.GetType().GetMethod(param.ValidationFunction);

                            var context = new ValidationFunctionContext(value);
                            methodInfo.Invoke(this.Command, new object[] { context });
                            isValid = context.IsValid;
                            validationMessage = context.Message;
                        }




                        // evaluate

                        if (!isValid)
                        {
                            this.Errors.Add(new CommandValidatorError()
                            {
                                Type = CommandValidationType.InvalidParameterValue,
                                Parameters = new List<ParameterDef>() { param },
                                Message = validationMessage ?? string.Format("The value of parameter {0} is invalid", param.Name)
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                        this.Errors.Add(new CommandValidatorError()
                        {
                            Type = CommandValidationType.InvalidValidationValuesFunction,
                            Parameters = new List<ParameterDef>() { param },
                            Message = string.Format("An error occured during the invocation of the validation values function for parameter {0}", param.Name),
                            Error = ex
                        });
                    }
                }
            }
        }

        private void ValidateUsingValidationFunction()
        {

        }

        private void EvaluateParameterAnalysis()
        {
            CommandParameterAnalysis analysis = new CommandParameterAnalysis(this.Command);

            if (analysis.MissingRequiredParameters.Count() > 0)
                this.Errors.Add(new CommandValidatorError()
                {
                    Type = CommandValidationType.MissingRequiredParameters,
                    Parameters = analysis.MissingRequiredParameters,
                    Message = string.Format("Required parameters are missing [{0}]"
                        , string.Join(", ", analysis.MissingRequiredParameters.Select(p => p.Name).ToArray()))
                });

            if (analysis.SuperflousParameters.Count() > 0)
                this.Errors.Add(new CommandValidatorError()
                {
                    Type = CommandValidationType.SuperflousParametersPresent,
                    Parameters = analysis.SuperflousParameters,
                    Message = string.Format("Parameters not found in active set are present [{0}]"
                    , string.Join(", ", analysis.SuperflousParameters.Select(p => p.Name).ToArray()))
                });
        }
    }
}
