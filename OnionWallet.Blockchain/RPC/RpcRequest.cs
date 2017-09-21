using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionWallet.Blockchain.RPC
{
    public class RpcRequest
    {
        public RpcRequest(int id, string method, params object[] parameters)
        {
            this.Id = id;
            this.Method = method.ToLower();
            this.Parameters = parameters?.ToList() ?? new List<object>();
        }

        [JsonProperty(PropertyName = "method", Order = 0)]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "params", Order = 1)]
        public IList<object> Parameters { get; set; }

        [JsonProperty(PropertyName = "id", Order = 2)]
        public int Id { get; set; }

        public byte[] GetBytes()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
