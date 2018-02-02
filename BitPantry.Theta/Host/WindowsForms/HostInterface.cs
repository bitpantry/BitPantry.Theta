using BitPantry.Theta.Component;
using BitPantry.Theta.Component.Modules;
using BitPantry.Theta.Component.Writers;
using BitPantry.Theta.Host.WindowsForms.AutoComplete;
using BitPantry.Theta.Host.WindowsForms.InputEventsFilter;
using BitPantry.Theta.Processing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BitPantry.Parsing.Strings;
using BitPantry.Theta.API;

namespace BitPantry.Theta.Host.WindowsForms
{
    public class HostInterface : UserControl, IHostInterface
    {
        public delegate void OnCopyEventHandler(CopyEventArgs e);
        public event OnCopyEventHandler OnCopy;
        
        private HostInterfaceMode _mode = HostInterfaceMode.Output;
        public HostInterfaceMode Mode
        {
            get { return this._mode; }
            set
            {
                if (this._mode == value)
                    return;

                this._mode = value;
                if (this._mode == HostInterfaceMode.Interactive)
                {
                    this.WritePrompt();
                    this.InputFilters.SetFilterMode(FilterMode.Standard);
                }
                else
                {
                    this.RTB.Clear();
                    this.InputFilters.SetFilterMode(FilterMode.Execution);
                }
            }
        }

        private Action _writePromptAction = null;
        private AutoCompleteController _autoComplete = null;
        private CommandExecutionPrompt _commandExecutionPrompt = null;
        private CommandInvoker _invoker = null;

        private int _inputStartPosition = 0;

        private List<string> _submitHistory = null;
        private int _submitHistoryIndex = -1;
        

        public HostWriterCollection Out { get; private set; }

        /// <summary>
        /// Captures specific user input and translates to events for registered subscribers
        /// </summary>
        internal InputEventsFilterCollection InputFilters { get; private set; }
        
        /// <summary>
        /// Gets the commands collection
        /// </summary>
        public CommandCollection Commands { get; private set; }

        /// <summary>
        /// Gets the modules collection
        /// </summary>
        public ModuleCollection Modules { get; private set; }

        /// <summary>
        /// Gets the cursor position of relative to the current input start position
        /// </summary>
        internal int InputPosition
        {
            get
            {
                if (this.RTB.SelectionStart < this._inputStartPosition)
                    return -1; // the caret is before the input field
                return this.RTB.SelectionStart - this._inputStartPosition; 
            }
        }

        /// <summary>
        /// Gets the screen coordinates of the caret
        /// </summary>
        public Point CaretLocation
        {
            get
            {
                Point caretScreenLocation = this.RTB.GetPositionFromCharIndex(this.RTB.SelectionStart);
                return new Point()
                {
                    X = this.RTB.PointToScreen(caretScreenLocation).X,
                    Y = this.RTB.PointToScreen(caretScreenLocation).Y + this.RTB.SelectionFont.Height
                };

            }
        }

        /// <summary>
        /// Gets the line number of the current selection start
        /// </summary>
        public int LineNumber { get { return this.RTB.GetLineFromCharIndex(this.RTB.SelectionStart); } }

        /// <summary>
        /// Gets the column index of the current selection start
        /// </summary>
        public int ColumnIndex { get { return this.RTB.SelectionStart - this.RTB.GetFirstCharIndexFromLine(this.LineNumber); } }

        /// <summary>
        /// Gets the rich text box that serves as the host interfaces's user interface
        /// </summary>
        internal HostRtb RTB { get; private set; }

        private bool _inputEnabled = true;

        public ICommandActivatorContainer CommandActivatorContainer { get; private set; }

        public bool InputEnabled
        {
            get { return this._inputEnabled; }
            set
            {
                if (this._inputEnabled != value)
                {
                    this._inputEnabled = value;
                    this.RTB.Text = string.Empty;
                    if (value)
                    {
                        this.InputFilters.SetFilterMode(FilterMode.Standard);
                        this.WritePrompt();
                    }
                    else
                    {
                        this.InputFilters.SetFilterMode(FilterMode.Execution);
                    }
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) 
        {
            if(keyData == (Keys.Control | Keys.V))
            {
                var args = new CopyEventArgs() { Content = Clipboard.GetText() };
                if (this.OnCopy != null) 
                    this.OnCopy(args);

                if (!args.Handled)
                {
                    string[] lines = args.Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    if(lines.Count() > 0)
                        this.Out.Standard.Write(lines[0]);
                }

                return true;
            } 
            else 
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }


        public delegate IBufferedWriter bw();

        /// <summary>
        /// Gets the buffered writer
        /// </summary>
        /// <returns></returns>
        public IBufferedWriter GetBuffer()
        {
            return new BufferedRtfWriter(this.RTB);
        }

        /// <summary>
        /// instantiates a new instance of the host interface control
        /// </summary>
        public HostInterface(HostInterfaceMode mode = HostInterfaceMode.Interactive)
        {
            // initialize user control

            this.BackColor = Constants.COLOR_BACKGROUND;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Height = 200;
            this.Width = 200;

            // initialize RTB

            this.RTB = new HostRtb();
            this.RTB.Dock = DockStyle.Fill;
            this.RTB.Font = Constants.FONT_STANDARD;
            this.RTB.BackColor = Constants.COLOR_BACKGROUND;
            this.RTB.ForeColor = Constants.COLOR_STANDARD_FOREGROUND;
            this.Controls.Add(this.RTB);

            // initialize input filters

            this.InputFilters = new InputEventsFilterCollection(this);
            this.InputFilters.SetFilterMode(FilterMode.Execution);
            this.InputFilters.RegisterSubscription(this.HandleKeyInputResultAction);

            // initialize the commands collection

            this.Commands = new CommandCollection();

            // initialize host writers

            this.Out = new HostWriterCollection(this.OnHostWriterActionHandler, this.GetBuffer());

            // initialize prompt

            this._writePromptAction = () => { this.Out.Standard.Write("> "); };

            // initialize mode

            this.Mode = mode;

            // initialize modules collection

            this.Modules = new ModuleCollection(this.Commands);
            this.Modules.Install(typeof(BitPantry.Theta.Modules.Core.Module), this.Out);

            // initialize auto complete

            this._autoComplete = new AutoCompleteController(this);

            // initialize submit history

            this._submitHistory = new List<string>();

            // initialize command execution prompt

            this._commandExecutionPrompt = new CommandExecutionPrompt(this);

            // initialize invoker

            this._invoker = new CommandInvoker(this);

        }

        public void SetCommandActivatorContainer(ICommandActivatorContainer container)
        {
            CommandActivatorContainer = container;
        }

        private void OnHostWriterActionHandler(string str, HostWriterContext context)
        {
            if (this.InvokeRequired) { this.Invoke((MethodInvoker)delegate { OnHostWriterActionHandler(str, context); }); }
            else
            {
                System.Drawing.Color backupForecolor = this.RTB.SelectionColor;
                System.Drawing.Color backupHighlight = this.RTB.SelectionBackColor;

                if (context.ForeColor != System.Drawing.Color.Empty)
                    this.RTB.SelectionColor = context.ForeColor;

                if (context.HighlightColor != System.Drawing.Color.Empty)
                    this.RTB.SelectionBackColor = context.HighlightColor;

                this.RTB.AppendText(str);

                this.RTB.SelectionFont = Constants.FONT_STANDARD;
                this.RTB.SelectionColor = backupForecolor;
                this.RTB.SelectionBackColor = backupHighlight;
            }
        }

        // key input filter listener
        private void HandleKeyInputResultAction(InputEventsFilterHandlerArgs args)
        {
            switch (args.Result)
            {
                case KeyInputFilterResult.Input_Submit:
                    this.SubmitInput();
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_ShowPreviousHistory:
                    this.ShowPreviousSubmit();
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_ShowNextHistory:
                    this.ShowNextSubmit();
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_Focus:
                    this.RTB.ScrollToCaret();
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_MoveToStart:
                    this.RTB.SelectionStart = this._inputStartPosition;
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_SelectToStart:
                    int endSelect = this.RTB.SelectionStart;
                    this.RTB.SelectionStart = this._inputStartPosition;
                    this.RTB.SelectionLength = endSelect - this._inputStartPosition;
                    break;
                case KeyInputFilterResult.Exec_CancelCommandExecution:
                    args.IsHandled = true;
                    if(this._invoker.IsExecuting) // if the invoker is not executing, then assume the system is executing (e.g., on startup) and cannot be canceled
                        this._invoker.CancelExecution();
                    break;
            }
        }

        

        private void ShowNextSubmit()
        {
            if (this._submitHistoryIndex > -1 && this._submitHistoryIndex < this._submitHistory.Count - 1)
            {
                this._submitHistoryIndex++;
                this.ReplaceCurrentInput(this._submitHistory[this._submitHistoryIndex]);
            }
        }

        private void ShowPreviousSubmit()
        {
            if (this._submitHistoryIndex > 0)
            {
                this._submitHistoryIndex--;
                this.ReplaceCurrentInput(this._submitHistory[this._submitHistoryIndex]);
            }
        }

        public string Prompt(string prompt)
        {
            return this._commandExecutionPrompt.Prompt(prompt);
        }

        internal void WritePrompt() { this.WritePrompt(null); }
        internal void WritePrompt(string prompt)
        {
            if (this.InvokeRequired) { this.Invoke((MethodInvoker)delegate { this.WritePrompt(prompt); }); }
            else
            {
                if (this.ColumnIndex > 0)
                    this.Out.Standard.WriteLine();

                if (prompt == null)
                    this._writePromptAction();
                else
                    this.Out.Standard.Write(prompt);

                this._inputStartPosition = this.RTB.Text.Length;
            }
        }

        private void SubmitInput()
        {
            // get current input

            Input input = this.GetCurrentInput();

            // update command history

            if (_submitHistory.Count == 0 || !_submitHistory.LastOrDefault().Equals(input.ToString().Trim()))
            {
                this._submitHistory.Add(input.ToString().TrimEnd());
                this._submitHistoryIndex = this._submitHistory.Count;
            }

            this.Out.Standard.WriteLine();

            // resolve command

            CommandResolver resolver = new CommandResolver(input, this.Commands);
            if (resolver.HasErrors) // if resolution failed with errors, output errors here
            {
                foreach (var err in resolver.Errors)
                    this.Out.Error.WriteLine(err);
                this.CommandExecutionComplete(null);
            }
            else
            {
                var cmd = new CommandActivator().Create(resolver, this);
                this.InputFilters.SetFilterMode(FilterMode.Execution);
                this._invoker.Invoke(cmd, false, this.CommandExecutionComplete);
            }


        }

        private void CommandExecutionComplete(CommandInvokerResponse result)
        {
            if (this.InvokeRequired) { this.Invoke((MethodInvoker)delegate { this.CommandExecutionComplete(result); }); }
            else 
            {
                this.InputFilters.SetFilterMode(FilterMode.Standard);
                this.WritePrompt(); 
            }
        }

        internal Input GetCurrentInput()
        {
            return new Input(this.GetCurrentInputString());
        }

        public void Clear()
        {
            if (this.RTB.InvokeRequired)
                this.RTB.Invoke((MethodInvoker)delegate { this.Clear(); });
            else
                this.RTB.Clear();
        }

        internal string GetCurrentInputString()
        {
            return this.RTB.Text.Substring(this._inputStartPosition);
        }

        private void ReplaceCurrentInput(string value)
        {
            this.RTB.SelectionStart = this._inputStartPosition;
            this.RTB.SelectionLength = this.RTB.Text.Length - this._inputStartPosition;
            this.RTB.SelectedText = value;
            this.RTB.SelectionStart = this.RTB.Text.Length;
        }

        internal void ReplaceCurrentNode(string value)
        {
            Input input = this.GetCurrentInput();
            if (string.IsNullOrEmpty(input.ToString()))
            {
                this.RTB.SelectedText = value;
            }
            else
            {
                InputNode node = input.GetNodeAtPosition(this.InputPosition);

                // build tailing space count

                int tailingSpaceCount = 0;
                if(!string.IsNullOrWhiteSpace(node.Element))
                     tailingSpaceCount = node.Element.Length - node.Element.TrimEnd().Length;               

                //if(string.IsNullOrWhiteSpace(node.Element))

                StringBuilder sb = new StringBuilder();
                if (node.ElementType == InputNodeType.NamedParameter)
                    sb.Append("-");
                else if (node.ElementType == InputNodeType.Switch)
                    sb.Append("/");

                sb.Append(value);

                sb.Append(string.Empty.PadLeft(tailingSpaceCount, ' '));

                // make some room for replacement if it is up against another element

                if (input.InputNodes.Exists(n => n.Index == node.Index + 1)
                    && input.InputNodes[node.Index + 1].StartPosition == this.RTB.SelectionStart - 1)
                    sb.Append(" ");

                // select node if node has value for replacing

                if (!string.IsNullOrWhiteSpace(node.Element)) 
                {
                    this.RTB.SelectionStart = this._inputStartPosition + node.StartPosition - 1;
                    this.RTB.SelectionLength = node.EndPosition - node.StartPosition + 1;
                }

                this.RTB.SelectedText = sb.ToString();
            }
        }

    }
}
