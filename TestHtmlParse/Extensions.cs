using System;
using System.Windows.Forms;

namespace TestHtmlMonkey
{
    public static class Extensions
    {
        public static void ShowError(this Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
