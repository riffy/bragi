using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace BRAGI.Util;

public class ControlWriter : TextWriter
{
    private readonly TextBox textbox;
    public ControlWriter(TextBox textbox)
    {
        this.textbox = textbox;
    }

    public override void Write(string? value)
    {
        WriteToTextBox(value == null ? "NULL" : value.ToString());
    }

    public override void WriteLine(string? value)
    {
        WriteToTextBox(value == null ? "NULL" : value.ToString());
    }

    private void WriteToTextBox(string value)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                CultureInfo.InvariantCulture);
        Dispatcher.UIThread.InvokeAsync(delegate
        {
            textbox.Text += "[" + timestamp + "]: " + value + "\r\n";
            textbox.Focus();
            textbox.CaretIndex = textbox.Text.Length;
        });
        /*
        textbox.Text += "[" + timestamp + "]: " + value + "\r\n";
        textbox.Focus();
        textbox.CaretIndex = textbox.Text.Length;
        */
        /*
        textbox.Invoke(delegate
        {
            textbox.AppendText("[" + timestamp + "]: " + value + "\r\n");
            textbox.Focus();
            textbox.CaretIndex = textbox.Text.Length;
            textbox.ScrollToEnd();
        });
        */
    }

    public override Encoding Encoding
    {
        get { return Encoding.ASCII; }
    }
}
