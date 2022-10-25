using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands
{
    public static class CPong
    {
        public static async Task<object> Pong(JObject parameters)
        {
            return "Pong";
        }

    }
}
