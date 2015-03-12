
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
            PrefopResult prefopResult = new PrefopResult();

            this.httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                prefopResult.ResponseInfo = respInfo;
                prefopResult.Response = response;
                if (respInfo.isDone())
                {
                    PrefopResult result = StringUtils.jsonDecode<PrefopResult>(response);
                    prefopResult.copyResult(result);
                }
            });
            string qUrl = string.Format(urlPattern, this.PersistentId);
            this.httpManager.get(qUrl);
            return prefopResult;
        }
    }
}
