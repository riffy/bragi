using Avalonia.Input;
using BRAGI.Util;
using NAudio.CoreAudioApi;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

enum SetAudioError
{
    BRAGI_INITIALIZATION_ERROR,
    INVALID_INPUT_DEVICE,
    INVALID_OUTPUT_DEVICE
}

public class AudioSettingsParameter : BragiParameter
{
    public string InDeviceId { get; set; }
    public string OutDeviceId { get; set; }
    [OptionalParameter]
    public int Volume { get; set; }
    [OptionalParameter]
    public Key PushToTalkKey { get; set; }
    [OptionalParameter]
    public float InputGain { get; set; }
    [OptionalParameter]
    public float InputGate { get; set; }
    public AudioSettingsParameter(string inDeviceId, string outDeviceId, int volume, Key pushToTalkKey, float inputGain, float inputGate)
    {
        InDeviceId = inDeviceId;
        OutDeviceId = outDeviceId;
        Volume = volume;
        PushToTalkKey = pushToTalkKey;
        InputGain = inputGain;
        InputGate = inputGate;
    }   
}

public class SetAudioSettings : BragiCommand<AudioSettingsParameter>
{
    public override bool CheckParameters(JsonObject? parameters)
    {
        return JsonValidator.CheckParameters<AudioSettingsParameter>(parameters);
    }

    public async override Task<object> ExecuteInternal(AudioSettingsParameter? parameters)
    {
        if (Bragi.Instance?.State != BRAGISTATE.INITIALIZED) throw new CommandException((int)SetAudioError.BRAGI_INITIALIZATION_ERROR, "Bragi not initialized");
        MMDevice? inDevice = Audio.GetAudioDeviceByID(parameters!.InDeviceId, DEVICETYPE.IN);
        if (inDevice == null) throw new CommandException((int)SetAudioError.INVALID_INPUT_DEVICE, string.Format("{0}", parameters!.InDeviceId));
        MMDevice? outDevice = Audio.GetAudioDeviceByID(parameters.OutDeviceId, DEVICETYPE.OUT);
        if (outDevice == null) throw new CommandException((int)SetAudioError.INVALID_OUTPUT_DEVICE, string.Format("{0}", parameters!.OutDeviceId));
        BragiAudio.ApplySettings(
            parameters.InDeviceId,
            parameters.OutDeviceId,
            parameters.Volume,
            parameters.PushToTalkKey,
            parameters.InputGain,
            parameters.InputGate);
        return "";
    }

    public override AudioSettingsParameter? ParseParameters(JsonObject? parameters)
    {
        if (!CheckParameters(parameters)) return null;
        int currentVol = (Bragi.Instance == null) ? BragiAudio.DefaultVolume : BragiAudio.Volume;
        Key P2TKey = (Bragi.Instance == null) ? BragiAudio.DefaultP2TKey : BragiAudio.P2TKey;
        float inputGain = (Bragi.Instance == null) ? BragiAudio.DefaultInputGain : BragiAudio.InputGain;
        float inputGate = (Bragi.Instance == null) ? BragiAudio.DefaultInputGate : BragiAudio.InputGate;
        return new AudioSettingsParameter(
            (string)parameters!["InDeviceId"]!,
            (string)parameters!["OutDeviceId"]!,
            (parameters!["Volume"] != null) ? (int)parameters!["Volume"]! : currentVol,
            (parameters!["PushToTalkKey"] != null) ? (Key)(int)parameters!["PushToTalkKey"]! : P2TKey,
            (parameters!["InputGain"] != null) ? (float)parameters!["InputGain"]! : inputGain,
            (parameters!["InputGate"] != null) ? (float)parameters!["InputGate"]! : inputGate);
    }
}
