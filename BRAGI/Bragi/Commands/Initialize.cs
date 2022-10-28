using Avalonia.Input;
using BRAGI.Util;
using NAudio.CoreAudioApi;
using OdinNative.Odin;
using OdinNative.Odin.Room;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

enum InitializeError
{
    BRAGI_INITIALIZATION_ERROR,
    INVALID_INPUT_DEVICE,
    INVALID_OUTPUT_DEVICE
}

public class InitializeParameter : BragiParameter
{
    public AudioSettingsParameter AudioSettings { get; private set; }
    [OptionalParameter]
    public string OdinServer { get; private set; }
    [OptionalParameter]
    public string AccessKey { get; private set; }
    public InitializeParameter(AudioSettingsParameter audioSettings, string odinServer, string accessKey)
    {
        AudioSettings = audioSettings;
        OdinServer = odinServer;
        AccessKey = accessKey;
    }
}

public class Initialize : BragiCommand<InitializeParameter>
{
    public override bool CheckParameters(JsonObject? parameters)
    {
        return JsonValidator.CheckParameters<InitializeParameter>(parameters);
    }

    public async override Task<object> ExecuteInternal(InitializeParameter? parameters)
    {
        if (Bragi.Instance!.State == BRAGISTATE.INITIALIZED) throw new CommandException((int)InitializeError.BRAGI_INITIALIZATION_ERROR, "Bragi already initialized");
        OdinDefaults.AccessKey = parameters!.AccessKey;
        OdinDefaults.Server = parameters.OdinServer;
        Bragi.Instance.Initialize();
        try
        {
            SetAudioSettings set = new();
            await set.ExecuteInternal(parameters.AudioSettings);
            return "";
        }
        catch
        {
            // If an error occurs on post-initialization, perform a cleanup.
            Bragi.Instance.CleanUp();
            throw;
        }
    }

    public override InitializeParameter? ParseParameters(JsonObject? parameters)
    {
        if (!CheckParameters(parameters)) return null;
        AudioSettingsParameter? audioSettings = new SetAudioSettings().ParseParameters((JsonObject)parameters!["AudioSettings"]!);
        if (audioSettings == null) return null;
        return new InitializeParameter(
            audioSettings,
            parameters.ContainsKey("OdinServer") ? (string)parameters["OdinServer"]! : OdinDefaults.Server,
            parameters.ContainsKey("AccessKey") ? (string)parameters["AccessKey"]! : OdinDefaults.AccessKey);
    }
}