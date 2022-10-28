using Avalonia.Input;
using BRAGI.Util;
using NAudio.CoreAudioApi;
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

    public AudioSettingsParameter(string inDeviceId, string outDeviceId, int volume, Key pushToTalkKey)
    {
        InDeviceId = inDeviceId;
        OutDeviceId = outDeviceId;
        Volume = volume;
        PushToTalkKey = pushToTalkKey;
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
        MMDevice? outDevice = Audio.GetAudioDeviceByID(parameters!.OutDeviceId, DEVICETYPE.OUT);
        if (outDevice == null) throw new CommandException((int)SetAudioError.INVALID_OUTPUT_DEVICE, string.Format("{0}", parameters!.OutDeviceId));

        Bragi.Instance.Settings.ApplySettings(
            inDevice,
            outDevice,
            parameters.Volume,
            parameters.PushToTalkKey);
        return "";
    }

    public override AudioSettingsParameter? ParseParameters(JsonObject? parameters)
    {
        if (!CheckParameters(parameters)) return null;
        int currentVol = (Bragi.Instance == null) ? BragiSettings.defaultVolume : Bragi.Instance.Settings.Volume;
        Key P2TKey = (Bragi.Instance == null) ? BragiSettings.defaultP2TKey : Bragi.Instance.Settings.P2TKey;
        return new AudioSettingsParameter(
            (string)parameters!["InDeviceId"]!,
            (string)parameters!["OutDeviceId"]!,
            (parameters!["AudioSettings"]?["Volume"] != null) ? (int)parameters!["AudioSettings"]!["Volume"]! : currentVol,
            (parameters!["AudioSettings"]?["PushToTalkKey"] != null) ? (Key)(int)parameters!["AudioSettings"]!["PushToTalkKey"]! : P2TKey);
    }
}
