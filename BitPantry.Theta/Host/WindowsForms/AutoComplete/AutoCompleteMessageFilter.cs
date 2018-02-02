using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitPantry.Theta.Host.WindowsForms.AutoComplete
{
    /// <summary>
    /// A Message Loop filter which detect mouse events when the auto complete form is shown
    /// and closes the auto complete form when a mouse click outside the auto complete popup occurs.
    /// </summary>
    class AutoCompleteMessageFilter : IMessageFilter, IDisposable
    {
        private object locker = new object();

        /// <summary>
        /// Has the message filter been disposed
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The auto complete form
        /// </summary>
        public frmAutoComplete AutoCompleteForm { get; set; }

        /// <summary>
        /// Installs a new auto complete message filter into the message loop for the given auto complete form
        /// </summary>
        /// <param name="autoCompleteForm">The auto complete form for which to install the message filter</param>
        public static void Install(frmAutoComplete autoCompleteForm)
        {
            Application.AddMessageFilter(new AutoCompleteMessageFilter(autoCompleteForm));
        }

        /// <summary>
        /// Creates a new instance of the AutoCompleteMessageFilter
        /// </summary>
        /// <param name="autoCompleteForm">The auto complete form associated with the host rich text box control</param>
        /// <remarks>The message filter is created and added to the message loop using the install function. The filter
        /// is automatically disposed once the auto complete form is closed - the filter is automatically removed
        /// from the message loop at this time.</remarks>
        private AutoCompleteMessageFilter(frmAutoComplete autoCompleteForm)
        {
            AutoCompleteForm = autoCompleteForm;
            AutoCompleteForm.Disposed += AutoCompleteForm_Disposed;

            IsDisposed = false;
        }

        void AutoCompleteForm_Disposed(object sender, EventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// The implementation of the IMessageFilter interface
        /// </summary>
        /// <param name="m">The message to filter</param>
        /// <returns></returns>
        public bool PreFilterMessage(ref Message m)
        {
            if (!IsDisposed && AutoCompleteForm != null)
            {
                switch (m.Msg)
                {
                    case Win32.WM_LBUTTONDOWN:
                    case Win32.WM_RBUTTONDOWN:
                    case Win32.WM_MBUTTONDOWN:
                    case Win32.WM_NCLBUTTONDOWN:
                    case Win32.WM_NCRBUTTONDOWN:
                    case Win32.WM_NCMBUTTONDOWN:
                        OnMouseDown();
                        break;
                }
            }

            return false; // allows the message to continue and be dispatched
        }

        /// <summary>
        /// Checks the mouse location and cancels the auto complete form if a mouse click occurs
        /// outside of the auto complete control
        /// </summary>
        private void OnMouseDown()
        {
            if (!AutoCompleteForm.Bounds.Contains(Cursor.Position))
                AutoCompleteForm.Cancel();
        }

        /// <summary>
        /// Disposes the message filter
        /// </summary>
        public void Dispose()
        {
            lock (locker)
            {
                if (IsDisposed)
                    return;

                Application.RemoveMessageFilter(this);
                IsDisposed = true;
            }
        }
    }
}
