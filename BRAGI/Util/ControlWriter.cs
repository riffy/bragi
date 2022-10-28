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
    private readonly StringBuilder logStringBuilder;
    public ControlWriter(TextBox textbox)
    {
        this.textbox = textbox;
#if DEBUG
        logStringBuilder = new StringBuilder();
        Console.WriteLine("Mode=Debug");
#endif
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
        string text = "[" + timestamp + "]: " + value + "\r\n";
        Dispatcher.UIThread.InvokeAsync(delegate
        {
            textbox.Text += text;
            textbox.Focus();
            textbox.CaretIndex = textbox.Text.Length;
        });
#if DEBUG
        LogWriter log = new(text);
#endif
    }

    public override Encoding Encoding
    {
        get { return Encoding.ASCII; }
    }
}
