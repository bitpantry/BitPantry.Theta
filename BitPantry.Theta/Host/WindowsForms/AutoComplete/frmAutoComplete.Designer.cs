namespace BitPantry.Theta.Host.WindowsForms.AutoComplete
{
    partial class frmAutoComplete
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstAutoCompleteOptions = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstAutoCompleteOptions
            // 
            this.lstAutoCompleteOptions.FormattingEnabled = true;
            this.lstAutoCompleteOptions.ItemHeight = 16;
            this.lstAutoCompleteOptions.Location = new System.Drawing.Point(-1, -3);
            this.lstAutoCompleteOptions.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.lstAutoCompleteOptions.Name = "lstAutoCompleteOptions";
            this.lstAutoCompleteOptions.Size = new System.Drawing.Size(160, 164);
            this.lstAutoCompleteOptions.TabIndex = 0;
            // 
            // frmAutoComplete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 173);
            this.Controls.Add(this.lstAutoCompleteOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmAutoComplete";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstAutoCompleteOptions;

    }
}