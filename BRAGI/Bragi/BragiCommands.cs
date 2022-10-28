using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using BRAGI.Util;
using BRAGI.Bragi.Commands;
using Bragi.Bragi.Commands;
using System.Text.Json.Nodes;

namespace BRAGI.Bragi;

public abstract class BragiCommand
{
    public abstract Task<object> Execute(JsonObject? param);
}

public abstract class BragiCommand<T> : BragiCommand where T : BragiParameter
{
    public static string Command = "";
    public abstract bool CheckParameters(JsonObject? parameters);
    public abstract T? ParseParameters(JsonObject? parameters);
    public override async Task<object> Execute(JsonObject? param)
    {
        return await ExecuteInternal(ParseParameters(param));
    }
    public abstract Task<object> ExecuteInternal(T? param);
}

public abstract class BragiParameter { }

public class NoParameter : BragiParameter { }

public class OptionalParameter : Attribute
{
    public OptionalParameter() { }
}

public static class BragiCommands
{
    //public static Dictionary<string, Func<JsonObject?, Task<object>>> Commands = new();
    public static Dictionary<string, BragiCommand> Commands = new();
    public static void AssociateCommands()
    {
        // General Valhalla Commands
        Commands.Add(nameof(Ping), new Ping());
        // Bragi Commands
        // -- General
        Commands.Add(nameof(Initialize), new Initialize());
        Commands.Add(nameof(GetAudioDevices), new GetAudioDevices());
        Commands.Add(nameof(GetAudioSettings), new GetAudioSettings());
        Commands.Add(nameof(SetAudioSettings), new SetAudioSettings());
        // -- Odin Specific
        Commands.Add(nameof(JoinRoom), new JoinRoom());
        Commands.Add(nameof(LeaveRoom), new LeaveRoom());
    }

    public static BragiCommand GetFunction(string key)
    {
        return Commands[key];
    }

    public static Task<object> CallFunction(string key, JsonObject? parameters)
    {
        return Commands[key].Execute(parameters);
    }
}

public enum CommandError
{
    INVALID_PARAMETER
}

[Serializable]
public class CommandException : Exception
{
    private readonly int _errorCode;
    public int ErrorCode { get { return _errorCode; } }

    private readonly string? _text;
    public string? Text { get { return _text; } }

    protected CommandException() : base()
    { }
    public CommandException(int errorCode, string text)
        : base(string.Format(text))
    {
        _errorCode = errorCode;
        _text = text;
    }

    // Ensure Exception is Serializable
    protected CommandException(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
    { }
}

[Serializable]
public class InvalidParameterException : Exception
{
    private readonly string? _missingKey;
    public string? MissingKey { get { return _missingKey; } }

    protected InvalidParameterException() : base()
    { }

    public InvalidParameterException(string missingKey)
        : base(
            missingKey == "" ?
            "Message is missing 'params' attribute." :
            string.Format("Provided parameters is missing key {0}", missingKey)
              )
    {
        _missingKey = missingKey;
    }

    // Ensure Exception is Serializable
    protected InvalidParameterException(SerializationInfo info, StreamingContext ctxt)
        : base(info, ctxt)
    { }
}
