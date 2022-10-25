using BRAGI.Util;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Bragi.Bragi.Commands
{
    public static class CGetAudioDevices
    {
        public static async Task<object> GetAudioDevices(JObject parameters)
        {
            return await Audio.GetAudioDevices();
        }
    }
}
