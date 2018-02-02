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

        public int OptionsBoxHeight { get { return this.lstAutoCompleteOptions.Height; } }
        public int OptionsBoxWidth { get { return this.lstAutoCompleteOptions.Width; } }

        public frmAutoComplete()
        {
            InitializeComponent();

            this.SelectedValue = null;
            base.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.lstAutoCompleteOptions.MouseClick += lstAutoCompleteOptions_MouseClick;
            this.lstAutoCompleteOptions.MouseDoubleClick += lstAutoCompleteOptions_MouseDoubleClick;

            this.lstAutoCompleteOptions.ForeColor = Constants.COLOR_STANDARD_FOREGROUND;
            this.lstAutoCompleteOptions.BackColor = Constants.COLOR_BACKGROUND;
            this.lstAutoCompleteOptions.Font = Constants.FONT_STANDARD;

            this.lstAutoCompleteOptions.BorderStyle = BorderStyle.FixedSingle;
            
        }

        public void SetAutoCompleteOptions(List<string> options)
        {
            this.lstAutoCompleteOptions.Items.Clear();

            // load options

            foreach (var value in options)
                this.lstAutoCompleteOptions.Items.Add(value);

            // set size

            string longestValue = options.Select(o => o).OrderByDescending(o => o.Length).First();
            var size = TextRenderer.MeasureText(longestValue, this.lstAutoCompleteOptions.Font);

            this.lstAutoCompleteOptions.Width = size.Width + 20;
            this.FixSize();

            // initialize variables

            this.SelectedValue = null;
            this.lstAutoCompleteOptions.SelectedIndex = 0;
        }

        internal void FilterAutoCompleteOptions(string byValue)
        {
            if (byValue != null)
            {
                for (int i = 0; i < this.lstAutoCompleteOptions.Items.Count; i++)
                {
                    if (((string)this.lstAutoCompleteOptions.Items[i]).StartsWith(byValue, StringComparison.OrdinalIgnoreCase))
                    {
                        this.lstAutoCompleteOptions.SelectedIndex = i;
                        this.lstAutoCompleteOptions.TopIndex = i;
                        break;
                    }
                }
            }
        }

        void lstAutoCompleteOptions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.SelectItemAtCursorPosition())
                this.SubmitCurrentSelection();
        }

        void lstAutoCompleteOptions_MouseClick(object sender, MouseEventArgs e)
        {
            this.SelectItemAtCursorPosition();
        }

        private bool SelectItemAtCursorPosition()
        {
            var pointToScreen = this.lstAutoCompleteOptions.PointToClient(Cursor.Position);
            int index = this.lstAutoCompleteOptions.IndexFromPoint(pointToScreen);
            if (index > -1 && index < this.lstAutoCompleteOptions.Items.Count)
            {
                this.lstAutoCompleteOptions.SelectedIndex = index;
                return true;
            }

            return false;
        }

        // have to do this in order to resize form, if not, there is always a little form visible behind the list control
        protected override void OnShown(EventArgs e)
        {
            this.FixSize();
        }

        private void FixSize()
        {
            this.SuspendLayout();
            this.lstAutoCompleteOptions.Location = new Point(0, 0);
            this.ClientSize = new Size(this.lstAutoCompleteOptions.Width, this.lstAutoCompleteOptions.Height);
            this.ResumeLayout();
        }

        public void Cancel()
        {
            this.SelectedValue = null;
            base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Hide();
            this.lstAutoCompleteOptions.Items.Clear();
        }

        public void SubmitCurrentSelection()
        {
            this.SelectedValue = (string)this.lstAutoCompleteOptions.SelectedItem;
            base.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Hide();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!this.Visible && this.AutoCompleteComplete != null)
                this.AutoCompleteComplete(this.DialogResult);

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
            if (this.lstAutoCompleteOptions.Items.Count > this.lstAutoCompleteOptions.SelectedIndex + 1)
                this.lstAutoCompleteOptions.SelectedIndex = this.lstAutoCompleteOptions.SelectedIndex + 1;
        }

        internal void SelectPreviousOption()
        {
            if (this.lstAutoCompleteOptions.SelectedIndex - 1 > -1)
                this.lstAutoCompleteOptions.SelectedIndex = this.lstAutoCompleteOptions.SelectedIndex - 1;
        }

        // hides the form instead of realling closing it

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
