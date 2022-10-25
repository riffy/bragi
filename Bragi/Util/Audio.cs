using BRAGI.Bragi;
using NAudio.CoreAudioApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRAGI.Util
{
    public static class Audio
    {
        /// <summary>
        /// Returns the sound devices connected in a dictionary with an ID (string) and their name.
        /// in: List of Soundinputs (micrphone)
        /// out: List of Soundoutputs (speaker, headset,...)
        /// </summary>
        /// <returns></returns>
        public static async Task<object> GetAudioDevices()
        {
            Dictionary<string, List<Dictionary<string, string>>> result = new Dictionary<string, List<Dictionary<string, string>>>
            {
                ["In"] = new List<Dictionary<string, string>>(),
                ["Out"] = new List<Dictionary<string, string>>()
            };
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                result["Out"].Add(new Dictionary<string, string>
                {
                    ["Id"] = endpoint.ID,
                    ["Name"] = endpoint.DeviceFriendlyName
                });
            }
            enumerator.Dispose();
            enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                result["In"].Add(new Dictionary<string, string>
                {
                    ["Id"] = endpoint.ID,
                    ["Name"] = endpoint.DeviceFriendlyName
                });
            }
            enumerator.Dispose();
            return result;
        }

        /// <summary>
        /// Returns an MMDevice based on the provided ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static MMDevice GetAudioDeviceByID(string id, DEVICETYPE dt)
        {
            MMDevice result = null;
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints((dt == DEVICETYPE.IN) ? DataFlow.Capture : DataFlow.Render, DeviceState.Active))
            {
                if (endpoint.ID == id)
                {
                    result = endpoint;
                    break;
                }
            }

            enumerator.Dispose();
            return result;
        }

    }
}
