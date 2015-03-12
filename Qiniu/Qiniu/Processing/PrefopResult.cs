using Newtonsoft.Json;
using Qiniu.Http;
using System.Collections.Generic;

namespace Qiniu.Processing
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PrefopResult : HttpResult
    {
        [JsonProperty("id")]
        public string Id { set; get; }
        [JsonProperty("code")]
        public int Code { set; get; }
        [JsonProperty("desc")]
        public string Desc { set; get; }
        [JsonProperty("inputBucket")]
        public string InputBucket { set; get; }
        [JsonProperty("inputKey")]
        public string InputKey { set; get; }
        [JsonProperty("items")]
        public List<PfopItem> Items { set; get; }
        [JsonProperty("keys")]
        public List<string> Keys { set; get; }

        public PrefopResult()
        {

        }

        public void copyResult(PrefopResult prefopResult)
        {
            this.Id = prefopResult.Id;
            this.Code = prefopResult.Code;
            this.Desc = prefopResult.Desc;
            this.InputBucket = prefopResult.InputBucket;
            this.InputKey = prefopResult.InputKey;
            this.Items = prefopResult.Items;
            this.Keys = prefopResult.Keys;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class PfopItem
    {
        [JsonProperty("cmd")]
        public string Cmd { set; get; }
        [JsonProperty("code")]
        public string Code { set; get; }
        [JsonProperty("desc")]
        public string Desc { set; get; }
        [JsonProperty("error")]
        public string Error { set; get; }
        [JsonProperty("hash")]
        public string Hash { set; get; }
        [JsonProperty("key")]
        public string Key { set; get; }
    }
}
