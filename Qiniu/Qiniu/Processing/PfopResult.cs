
using Qiniu.Http;
namespace Qiniu.Processing
{
    public class PfopResult : HttpResult
    {
        public string PersistentId { set; get; }
        public PfopResult()
        {
        }

        public PfopResult(ResponseInfo respInfo, string response, string persistentId)
        {
            base.ResponseInfo = respInfo;
            base.Response = response;
            this.PersistentId = persistentId;
        }
    }
}
