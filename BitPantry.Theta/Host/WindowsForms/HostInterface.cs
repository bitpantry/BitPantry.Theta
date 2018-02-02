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
            get { return _mode; }
            set
            {
                if (_mode == value)
                    return;

                _mode = value;
                if (_mode == HostInterfaceMode.Interactive)
                {
                    WritePrompt();
                    InputFilters.SetFilterMode(FilterMode.Standard);
                }
                else
                {
                    RTB.Clear();
                    InputFilters.SetFilterMode(FilterMode.Execution);
                }
            }
        }

        private Action<HostWriterCollection> _writePromptAction = null;
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
                if (RTB.SelectionStart < _inputStartPosition)
                    return -1; // the caret is before the input field
                return RTB.SelectionStart - _inputStartPosition; 
            }
        }

        /// <summary>
        /// Gets the screen coordinates of the caret
        /// </summary>
        public Point CaretLocation
        {
            get
            {
                Point caretScreenLocation = RTB.GetPositionFromCharIndex(RTB.SelectionStart);
                return new Point()
                {
                    X = RTB.PointToScreen(caretScreenLocation).X,
                    Y = RTB.PointToScreen(caretScreenLocation).Y + RTB.SelectionFont.Height
                };

            }
        }

        /// <summary>
        /// Gets the line number of the current selection start
        /// </summary>
        public int LineNumber { get { return RTB.GetLineFromCharIndex(RTB.SelectionStart); } }

        /// <summary>
        /// Gets the column index of the current selection start
        /// </summary>
        public int ColumnIndex { get { return RTB.SelectionStart - RTB.GetFirstCharIndexFromLine(LineNumber); } }

        /// <summary>
        /// Gets the rich text box that serves as the host interfaces's user interface
        /// </summary>
        internal HostRtb RTB { get; private set; }

        private bool _inputEnabled = true;

        public ICommandActivatorContainer CommandActivatorContainer { get; private set; }

        public bool InputEnabled
        {
            get { return _inputEnabled; }
            set
            {
                if (_inputEnabled != value)
                {
                    _inputEnabled = value;
                    RTB.Text = string.Empty;
                    if (value)
                    {
                        InputFilters.SetFilterMode(FilterMode.Standard);
                        WritePrompt();
                    }
                    else
                    {
                        InputFilters.SetFilterMode(FilterMode.Execution);
                    }
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) 
        {
            if(keyData == (Keys.Control | Keys.V))
            {
                var args = new CopyEventArgs() { Content = Clipboard.GetText() };
                if (OnCopy != null)
                    OnCopy(args);

                if (!args.Handled)
                {
                    string[] lines = args.Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    if(lines.Count() > 0)
                        Out.Standard.Write(lines[0]);
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
            return new BufferedRtfWriter(RTB);
        }

        /// <summary>
        /// instantiates a new instance of the host interface control
        /// </summary>
        public HostInterface(HostInterfaceMode mode = HostInterfaceMode.Interactive)
        {
            // initialize user control

            BackColor = Constants.COLOR_BACKGROUND;
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            Height = 200;
            Width = 200;

            // initialize RTB

            RTB = new HostRtb();
            RTB.Dock = DockStyle.Fill;
            RTB.Font = Constants.FONT_STANDARD;
            RTB.BackColor = Constants.COLOR_BACKGROUND;
            RTB.ForeColor = Constants.COLOR_STANDARD_FOREGROUND;
            Controls.Add(RTB);

            // initialize input filters

            InputFilters = new InputEventsFilterCollection(this);
            InputFilters.SetFilterMode(FilterMode.Execution);
            InputFilters.RegisterSubscription(HandleKeyInputResultAction);

            // initialize the commands collection

            Commands = new CommandCollection();

            // initialize host writers

            Out = new HostWriterCollection(OnHostWriterActionHandler, GetBuffer());

            // initialize prompt

            _writePromptAction = outWriters => { outWriters.Standard.Write("> "); };

            // initialize mode

            Mode = mode;

            // initialize modules collection

            Modules = new ModuleCollection(Commands);
            Modules.Install(typeof(BitPantry.Theta.Modules.Core.Module), Out);

            // initialize auto complete

            _autoComplete = new AutoCompleteController(this);

            // initialize submit history

            _submitHistory = new List<string>();

            // initialize command execution prompt

            _commandExecutionPrompt = new CommandExecutionPrompt(this);

            // initialize invoker

            _invoker = new CommandInvoker(this);

        }

        public void SetCommandActivatorContainer(ICommandActivatorContainer container)
        {
            CommandActivatorContainer = container;
        }

        private void OnHostWriterActionHandler(string str, HostWriterContext context)
        {
            if (InvokeRequired) { Invoke((MethodInvoker)delegate { OnHostWriterActionHandler(str, context); }); }
            else
            {
                System.Drawing.Color backupForecolor = RTB.SelectionColor;
                System.Drawing.Color backupHighlight = RTB.SelectionBackColor;

                if (context.ForeColor != System.Drawing.Color.Empty)
                    RTB.SelectionColor = context.ForeColor;

                if (context.HighlightColor != System.Drawing.Color.Empty)
                    RTB.SelectionBackColor = context.HighlightColor;

                RTB.AppendText(str);

                RTB.SelectionFont = Constants.FONT_STANDARD;
                RTB.SelectionColor = backupForecolor;
                RTB.SelectionBackColor = backupHighlight;
            }
        }

        // key input filter listener
        private void HandleKeyInputResultAction(InputEventsFilterHandlerArgs args)
        {
            switch (args.Result)
            {
                case KeyInputFilterResult.Input_Submit:
                    SubmitInput();
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_ShowPreviousHistory:
                    ShowPreviousSubmit();
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_ShowNextHistory:
                    ShowNextSubmit();
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_Focus:
                    RTB.ScrollToCaret();
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_MoveToStart:
                    RTB.SelectionStart = _inputStartPosition;
                    args.IsHandled = true;
                    break;
                case KeyInputFilterResult.Input_SelectToStart:
                    int endSelect = RTB.SelectionStart;
                    RTB.SelectionStart = _inputStartPosition;
                    RTB.SelectionLength = endSelect - _inputStartPosition;
                    break;
                case KeyInputFilterResult.Exec_CancelCommandExecution:
                    args.IsHandled = true;
                    if(_invoker.IsExecuting) // if the invoker is not executing, then assume the system is executing (e.g., on startup) and cannot be canceled
                        _invoker.CancelExecution();
                    break;
            }
        }

        

        private void ShowNextSubmit()
        {
            if (_submitHistoryIndex > -1 && _submitHistoryIndex < _submitHistory.Count - 1)
            {
                _submitHistoryIndex++;
                ReplaceCurrentInput(_submitHistory[_submitHistoryIndex]);
            }
        }

        private void ShowPreviousSubmit()
        {
            if (_submitHistoryIndex > 0)
            {
                _submitHistoryIndex--;
                ReplaceCurrentInput(_submitHistory[_submitHistoryIndex]);
            }
        }

        public string Prompt(string prompt)
        {
            return _commandExecutionPrompt.Prompt(prompt);
        }

        internal void WritePrompt() { WritePrompt(null); }
        internal void WritePrompt(string prompt)
        {
            if (InvokeRequired) { Invoke((MethodInvoker)delegate { WritePrompt(prompt); }); }
            else
            {
                if (ColumnIndex > 0)
                    Out.Standard.WriteLine();

                if (prompt == null)
                    _writePromptAction(Out);
                else
                    Out.Standard.Write(prompt);

                _inputStartPosition = RTB.Text.Length;
            }
        }

        private void SubmitInput()
        {
            // get current input

            Input input = GetCurrentInput();

            // update command history

            if (_submitHistory.Count == 0 || !_submitHistory.LastOrDefault().Equals(input.ToString().Trim()))
            {
                _submitHistory.Add(input.ToString().TrimEnd());
                _submitHistoryIndex = _submitHistory.Count;
            }

            Out.Standard.WriteLine();

            // resolve command

            CommandResolver resolver = new CommandResolver(input, Commands);
            if (resolver.HasErrors) // if resolution failed with errors, output errors here
            {
                foreach (var err in resolver.Errors)
                    Out.Error.WriteLine(err);
                CommandExecutionComplete(null);
            }
            else
            {
                var cmd = new CommandActivator().Create(resolver, this);
                InputFilters.SetFilterMode(FilterMode.Execution);
                _invoker.Invoke(cmd, false, CommandExecutionComplete);
            }


        }

        private void CommandExecutionComplete(CommandInvokerResponse result)
        {
            if (InvokeRequired) { Invoke((MethodInvoker)delegate { CommandExecutionComplete(result); }); }
            else 
            {
                InputFilters.SetFilterMode(FilterMode.Standard);
                WritePrompt(); 
            }
        }

        internal Input GetCurrentInput()
        {
            return new Input(GetCurrentInputString());
        }

        public void Clear()
        {
            if (RTB.InvokeRequired)
                RTB.Invoke((MethodInvoker)delegate { Clear(); });
            else
                RTB.Clear();
        }

        internal string GetCurrentInputString()
        {
            return RTB.Text.Substring(_inputStartPosition);
        }

        private void ReplaceCurrentInput(string value)
        {
            RTB.SelectionStart = _inputStartPosition;
            RTB.SelectionLength = RTB.Text.Length - _inputStartPosition;
            RTB.SelectedText = value;
            RTB.SelectionStart = RTB.Text.Length;
        }

        internal void ReplaceCurrentNode(string value)
        {
            Input input = GetCurrentInput();
            if (string.IsNullOrEmpty(input.ToString()))
            {
                RTB.SelectedText = value;
            }
            else
            {
                InputNode node = input.GetNodeAtPosition(InputPosition);

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
                    && input.InputNodes[node.Index + 1].StartPosition == RTB.SelectionStart - 1)
                    sb.Append(" ");

                // select node if node has value for replacing

                if (!string.IsNullOrWhiteSpace(node.Element)) 
                {
                    RTB.SelectionStart = _inputStartPosition + node.StartPosition - 1;
                    RTB.SelectionLength = node.EndPosition - node.StartPosition + 1;
                }

                RTB.SelectedText = sb.ToString();
            }
        }

        public void SetWritePromptAction(Action<HostWriterCollection> del) { _writePromptAction = del; }

    }
}
