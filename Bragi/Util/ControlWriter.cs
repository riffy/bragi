using System.Globalization;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Threading;
using MahApps.Metro.Controls;

namespace BRAGI.Util
{

    public class ControlWriter : TextWriter
    {
        private TextBox textbox;
        public ControlWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(string value)
        {
            WriteToTextBox(value == null ? "NULL" : value.ToString());

        }

        public override void WriteLine(string value)
        {
            WriteToTextBox(value == null ? "NULL" : value.ToString());
        }

        private void WriteToTextBox(string value)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture);
            textbox.Invoke(delegate
            {
                textbox.AppendText("[" + timestamp + "]: " + value + "\r\n");
                textbox.Focus();
                textbox.CaretIndex = textbox.Text.Length;
                textbox.ScrollToEnd();
            });
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
