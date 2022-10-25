using BRAGI.Util;
using NAudio.CoreAudioApi;
using Newtonsoft.Json.Linq;
using OdinNative.Odin;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BRAGI.Bragi.Commands
{
    enum InitializeError
    {
        BRAGI_INITIALIZATION_ERROR,
        INVALID_INPUT_DEVICE,
        INVALID_OUTPUT_DEVICE
    }
    public interface InitializeParameters
    {
        SetAudioSettingsParameter AudioSettings { get; set; }
        [OptionalParameter]
        string OdinServer { get; set; }
        [OptionalParameter]
        string AccessKey { get; set; }
    }

    public static class CInitialize
    {
        public static async Task<object> Initialize(JObject parameters)
        {
            if (!JsonValidator.CheckParameters<InitializeParameters>(parameters)) return "";
            if (!JsonValidator.CheckParameters<SetAudioSettingsParameter>((JObject)parameters["AudioSettings"])) return "";
            if (Bragi.Instance.State == BRAGISTATE.INITIALIZED) throw new CommandException((int)InitializeError.BRAGI_INITIALIZATION_ERROR, "Bragi already initialized");
            MMDevice inDevice = Audio.GetAudioDeviceByID((string)parameters["AudioSettings"]["InDeviceId"], DEVICETYPE.IN);
            if (inDevice == null) throw new CommandException((int)InitializeError.INVALID_INPUT_DEVICE, string.Format("{0}", (string)parameters["AudioSettings"]["InDeviceId"]));
            MMDevice outDevice = Audio.GetAudioDeviceByID((string)parameters["AudioSettings"]["OutDeviceId"], DEVICETYPE.OUT);
            if (outDevice == null) throw new CommandException((int)InitializeError.INVALID_OUTPUT_DEVICE, string.Format("{0}", (string)parameters["AudioSettings"]["OutDeviceId"]));
            if (parameters.ContainsKey("OdinServer"))
            {
                OdinDefaults.Server = (string)parameters.GetValue("OdinServer");
            }
            if (parameters.ContainsKey("AccessKey"))
            {
                OdinDefaults.AccessKey = (string)parameters.GetValue("AccessKey");
            }
            Bragi.Instance.Initialize();
            Bragi.Instance.Settings.ApplySettings(
                inDevice,
                outDevice,
                (parameters["AudioSettings"]?["Volume"] != null) ? (int)parameters["AudioSettings"]?["Volume"] : 100,
                (parameters["AudioSettings"]?["PushToTalkKey"] != null) ? (Key)(int)parameters["AudioSettings"]?["PushToTalkKey"] : Key.None);
            return "";
            
        }

    }
}
