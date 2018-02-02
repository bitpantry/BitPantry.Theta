using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.AutoComplete
{
    delegate void AutoCompleteCompleteHandler(DialogResult result);

    partial class frmAutoComplete : Form
    {
        public event AutoCompleteCompleteHandler AutoCompleteComplete;

        public string SelectedValue { get; set; }

        public int OptionsBoxHeight { get { return lstAutoCompleteOptions.Height; } }
        public int OptionsBoxWidth { get { return lstAutoCompleteOptions.Width; } }

        public frmAutoComplete()
        {
            InitializeComponent();

            SelectedValue = null;
            base.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            lstAutoCompleteOptions.MouseClick += lstAutoCompleteOptions_MouseClick;
            lstAutoCompleteOptions.MouseDoubleClick += lstAutoCompleteOptions_MouseDoubleClick;

            lstAutoCompleteOptions.ForeColor = Constants.COLOR_STANDARD_FOREGROUND;
            lstAutoCompleteOptions.BackColor = Constants.COLOR_BACKGROUND;
            lstAutoCompleteOptions.Font = Constants.FONT_STANDARD;

            lstAutoCompleteOptions.BorderStyle = BorderStyle.FixedSingle;
            
        }

        public void SetAutoCompleteOptions(List<string> options)
        {
            lstAutoCompleteOptions.Items.Clear();

            // load options

            foreach (var value in options)
                lstAutoCompleteOptions.Items.Add(value);

            // set size

            string longestValue = options.Select(o => o).OrderByDescending(o => o.Length).First();
            var size = TextRenderer.MeasureText(longestValue, lstAutoCompleteOptions.Font);

            lstAutoCompleteOptions.Width = size.Width + 20;
            FixSize();

            // initialize variables

            SelectedValue = null;
            lstAutoCompleteOptions.SelectedIndex = 0;
        }

        internal void FilterAutoCompleteOptions(string byValue)
        {
            if (byValue != null)
            {
                for (int i = 0; i < lstAutoCompleteOptions.Items.Count; i++)
                {
                    if (((string)lstAutoCompleteOptions.Items[i]).StartsWith(byValue, StringComparison.OrdinalIgnoreCase))
                    {
                        lstAutoCompleteOptions.SelectedIndex = i;
                        lstAutoCompleteOptions.TopIndex = i;
                        break;
                    }
                }
            }
        }

        void lstAutoCompleteOptions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectItemAtCursorPosition())
                SubmitCurrentSelection();
        }

        void lstAutoCompleteOptions_MouseClick(object sender, MouseEventArgs e)
        {
            SelectItemAtCursorPosition();
        }

        private bool SelectItemAtCursorPosition()
        {
            var pointToScreen = lstAutoCompleteOptions.PointToClient(Cursor.Position);
            int index = lstAutoCompleteOptions.IndexFromPoint(pointToScreen);
            if (index > -1 && index < lstAutoCompleteOptions.Items.Count)
            {
                lstAutoCompleteOptions.SelectedIndex = index;
                return true;
            }

            return false;
        }

        // have to do this in order to resize form, if not, there is always a little form visible behind the list control
        protected override void OnShown(EventArgs e)
        {
            FixSize();
        }

        private void FixSize()
        {
            SuspendLayout();
            lstAutoCompleteOptions.Location = new Point(0, 0);
            ClientSize = new Size(lstAutoCompleteOptions.Width, lstAutoCompleteOptions.Height);
            ResumeLayout();
        }

        public void Cancel()
        {
            SelectedValue = null;
            base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Hide();
            lstAutoCompleteOptions.Items.Clear();
        }

        public void SubmitCurrentSelection()
        {
            SelectedValue = (string)lstAutoCompleteOptions.SelectedItem;
            base.DialogResult = System.Windows.Forms.DialogResult.OK;
            Hide();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!Visible && AutoCompleteComplete != null)
                AutoCompleteComplete(DialogResult);

            base.OnVisibleChanged(e);
        }

        // Creates a form that doesn't steal focus from the main form
        // http://www.codeproject.com/Articles/71808/Creating-a-Form-That-Doesn-t-Take-Focus
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams ret = base.CreateParams;
                ret.Style = (int)Win32.WindowStyles.WS_CHILD; // form cannot be activated
                return ret;
            }
        }


        internal void SelectNextOption()
        {
            if (lstAutoCompleteOptions.Items.Count > lstAutoCompleteOptions.SelectedIndex + 1)
                lstAutoCompleteOptions.SelectedIndex = lstAutoCompleteOptions.SelectedIndex + 1;
        }

        internal void SelectPreviousOption()
        {
            if (lstAutoCompleteOptions.SelectedIndex - 1 > -1)
                lstAutoCompleteOptions.SelectedIndex = lstAutoCompleteOptions.SelectedIndex - 1;
        }

        // hides the form instead of realling closing it

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
