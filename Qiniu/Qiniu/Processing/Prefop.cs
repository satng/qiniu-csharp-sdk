
using Qiniu.Http;
using Qiniu.Util;
namespace Qiniu.Processing
{
    public class Prefop
    {
        private string urlPattern = "http://api.qiniu.com/status/get/prefop?id={0}";
        private HttpManager httpManager;
        public string PersistentId { set; get; }
        public Prefop(string persistentId)
        {
            this.httpManager = new HttpManager();
            this.PersistentId = persistentId;
        }

        public PrefopResult prefop()
        {
            PrefopResult prefopResult = null;

            this.httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                if (respInfo.isOk())
                {
                    prefopResult = StringUtils.jsonDecode<PrefopResult>(response);
                }
                else
                {
                    prefopResult = new PrefopResult();
                }
                prefopResult.ResponseInfo = respInfo;
                prefopResult.Response = response;
            });
            string qUrl = string.Format(urlPattern, this.PersistentId);
            this.httpManager.get(qUrl);
            return prefopResult;
        }
    }
}
