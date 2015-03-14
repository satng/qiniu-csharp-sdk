using Qiniu.Http;
using Qiniu.Util;
using System.Collections.Generic;
using System.Text;

namespace Qiniu.Processing
{
    public class Pfop
    {
        private HttpManager httpManager;
        private string url = "http://api.qiniu.com/pfop/";
        public string Bucket { set; get; }
        public string Key { set; get; }
        public string Fops { set; get; }
        public string NotifyURL { set; get; }
        public bool Force { set; get; }
        public string Pipeline { set; get; }
        public Mac Mac { set; get; }

        public Pfop(Mac mac, string bucket, string key, string fops)
        {
            this.httpManager = new HttpManager();
            this.Mac = mac;
            this.Bucket = bucket;
            this.Key = key;
            this.Fops = fops;
        }

        public PfopResult pfop()
        {
            PfopResult pfopResult = null;

            PostArgs postArgs = new PostArgs();
            postArgs.Params.Add("bucket", this.Bucket);
            postArgs.Params.Add("key", this.Key);
            postArgs.Params.Add("fops", this.Fops);
            if (!string.IsNullOrEmpty(this.NotifyURL))
            {
                postArgs.Params.Add("notifyURL", this.NotifyURL);
            }
            if (this.Force)
            {
                postArgs.Params.Add("force", "1");
            }
            if (!string.IsNullOrEmpty(this.Pipeline))
            {
                postArgs.Params.Add("pipeline", this.Pipeline);
            }

            string auth = Auth.createManageToken(url, Encoding.UTF8.GetBytes(StringUtils.urlParamsJoin(postArgs.Params)), this.Mac);

            //set http manager
            this.httpManager.PostArgs = postArgs;
            this.httpManager.setAuthHeader(auth);
            this.httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                if (respInfo.isOk())
                {
                    pfopResult = StringUtils.jsonDecode<PfopResult>(response);
                }
                else
                {
                    pfopResult = new PfopResult();
                }
                pfopResult.ResponseInfo = respInfo;
                pfopResult.Response = response;
            });
            this.httpManager.post(url);
            return pfopResult;
        }
    }
}