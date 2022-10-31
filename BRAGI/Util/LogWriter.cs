using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BRAGI.Util;

public class LogWriter
{
    private string m_exePath = string.Empty;
    public LogWriter(string logMessage)
    {
        LogWrite(logMessage);
    }
    public void LogWrite(string logMessage)
    {
        string? m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (m_exePath == null) return;
        try
        {
            using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
            {
                Log(logMessage, w);
            }
        }
        catch
        {
        }
    }

    public void Log(string logMessage, TextWriter txtWriter)
    {
        try
        {
            txtWriter.Write("{0}", logMessage);
        }
        catch
        {
        }
    }
}
