using BRAGI.Util;
using NAudio.CoreAudioApi;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BRAGI.Bragi.Commands
{
    enum SetAudioError
    {
        BRAGI_INITIALIZATION_ERROR,
        INVALID_INPUT_DEVICE,
        INVALID_OUTPUT_DEVICE
    }
    public interface SetAudioSettingsParameter
    {
        string InDeviceId { get; set; }
        string OutDeviceId { get; set; }
        [OptionalParameter]
        int Volume { get; set; }
        [OptionalParameter]
        Key PushToTalkKey { get; set; }
    }
    public class CSetAudioSettings
    {
        public static async Task<object> SetAudioSettings(JObject parameters)
        {
            if (!JsonValidator.CheckParameters<SetAudioSettingsParameter>(parameters)) return "";
            if (Bragi.Instance.State != BRAGISTATE.INITIALIZED) throw new CommandException((int)SetAudioError.BRAGI_INITIALIZATION_ERROR, "Bragi not initialized");
            MMDevice inDevice = Audio.GetAudioDeviceByID((string)parameters["InDeviceId"], DEVICETYPE.IN);
            if (inDevice == null) throw new CommandException((int)SetAudioError.INVALID_INPUT_DEVICE, string.Format("{0}", (string)parameters["InDeviceId"]));
            MMDevice outDevice = Audio.GetAudioDeviceByID((string)parameters["OutDeviceId"], DEVICETYPE.OUT);
            if (outDevice == null) throw new CommandException((int)SetAudioError.INVALID_OUTPUT_DEVICE, string.Format("{0}", (string)parameters["OutDeviceId"]));
            Bragi.Instance.Settings.ApplySettings(
                inDevice,
                outDevice,
                (parameters["AudioSettings"]?["Volume"] != null) ? (int)parameters["AudioSettings"]?["Volume"] : Bragi.Instance.Settings.Volume,
                (parameters["AudioSettings"]?["PushToTalkKey"] != null) ? (Key)(int)parameters["AudioSettings"]?["PushToTalkKey"] : Bragi.Instance.Settings.PushToTalkHotkey);
            return "";
        }
    }
}
