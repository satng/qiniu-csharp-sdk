using Qiniu.Common;
using Qiniu.Http;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qiniu.Storage
{
    public class BucketManager
    {
        private HttpManager httpManager;
        private Mac mac;
        public BucketManager(Mac mac)
        {
            this.httpManager = new HttpManager();
            this.mac = mac;
        }

        public void stat(string bucket, string key, CompletionHandler completionHandler)
        {
            string url = string.Format("{0}{1}", Config.RS_HOST, statOp(bucket, key));
            string token = Auth.createManageToken(url, null, this.mac);
            this.httpManager.setAuthHeader(string.Format("QBox {0}", token));
            this.httpManager.CompletionHandler = completionHandler;
            this.httpManager.get(url);
        }

        public string statOp(string bucket, string key)
        {
            return string.Format("/stat/{0}", StringUtils.encodedEntry(bucket, key));
        }
    }
}
