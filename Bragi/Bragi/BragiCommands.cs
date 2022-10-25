using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using BRAGI.Util;
using BRAGI.Bragi.Commands;
using Bragi.Bragi.Commands;

namespace BRAGI.Bragi
{
    public class OptionalParameter : Attribute
    {
        public OptionalParameter() { }
    }

    public static class BragiCommands
    {
        public static IDictionary<string, Func<JObject, Task<object>>> Commands
            = new Dictionary<string, Func<JObject, Task<object>>>();

        public static void AssociateCommands(Valhalla.Valhalla valhalla)
        {
            // General Valhalla Commands
            Commands["Ping"] = CPong.Pong;
            // Bragi Commands
            // -- General
            Commands["GetAudioDevices"] = CGetAudioDevices.GetAudioDevices;
            Commands["Initialize"] = CInitialize.Initialize;
            Commands["JoinRoom"] = CJoinRoom.JoinRoom;
            Commands["LeaveRoom"] = CLeaveRoom.LeaveRoom;
            // Commands["Initialize"]
            // -- Odin Specific
        }

        public static Func<JObject, object> GetFunction(string key)
        {
            return Commands[key];
        }

        public static Task<object> CallFunction(string key, JObject parameters)
        {
            return Commands[key](parameters);
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

        private readonly string _text;
        public string Text { get { return _text; } }

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
        private readonly string _missingKey;
        public string MissingKey { get { return _missingKey; } }

        protected InvalidParameterException() : base()
        { }

        public InvalidParameterException(string missingKey)
            : base(string.Format("Provided parameter object is missing key {0}", missingKey))
        {
            _missingKey = missingKey;
        }

        // Ensure Exception is Serializable
        protected InvalidParameterException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        { }
    }
}
