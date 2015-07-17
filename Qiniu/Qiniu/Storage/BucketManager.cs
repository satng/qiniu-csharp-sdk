using Newtonsoft.Json;
using Qiniu.Common;
using Qiniu.Http;
using Qiniu.Storage.Model;
using Qiniu.Util;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Qiniu.Storage
{
    public class BucketManager
    {
        private Mac mac;

        public BucketManager()
        {
        }

        public BucketManager(Mac mac)
        {
            this.mac = mac;
        }

        public StatResult stat(string bucket, string key)
        {
            StatResult statResult = null;
            string url = string.Format("{0}{1}", Config.RS_HOST, statOp(bucket, key));
            string token = Auth.createManageToken(url, null, this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                if (respInfo.isOk())
                {
                    statResult = StringUtils.jsonDecode<StatResult>(response);
                }
                else
                {
                    statResult = new StatResult();
                }
                statResult.Response = response;
                statResult.ResponseInfo = respInfo;
            });
            httpManager.get(url);
            return statResult;
        }

        public HttpResult delete(string bucket, string key)
        {
            HttpResult deleteResult = null;
            string url = string.Format("{0}{1}", Config.RS_HOST, deleteOp(bucket, key));
            string token = Auth.createManageToken(url, null, this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                deleteResult = new HttpResult();
                deleteResult.Response = response;
                deleteResult.ResponseInfo = respInfo;
            });
            httpManager.post(url);
            return deleteResult;
        }

        public HttpResult copy(string srcBucket, string srcKey, string destBucket, string destKey)
        {
            HttpResult copyResult = null;
            string url = string.Format("{0}{1}", Config.RS_HOST, copyOp(srcBucket, srcKey, destBucket, destKey));
            string token = Auth.createManageToken(url, null, this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                copyResult = new HttpResult();
                copyResult.Response = response;
                copyResult.ResponseInfo = respInfo;
            });
            httpManager.post(url);
            return copyResult;
        }

        public HttpResult move(string srcBucket, string srcKey, string destBucket, string destKey)
        {
            HttpResult moveResult = null;
            string url = string.Format("{0}{1}", Config.RS_HOST, moveOp(srcBucket, srcKey, destBucket, destKey));
            string token = Auth.createManageToken(url, null, this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                moveResult = new HttpResult();
                moveResult.Response = response;
                moveResult.ResponseInfo = respInfo;
            });
            httpManager.post(url);
            return moveResult;
        }

        public HttpResult chgm(string bucket, string key, string mimeType)
        {
            HttpResult chgmResult = null;
            string url = string.Format("{0}{1}", Config.RS_HOST, chgmOp(bucket, key, mimeType));
            string token = Auth.createManageToken(url, null, this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                chgmResult = new HttpResult();
                chgmResult.Response = response;
                chgmResult.ResponseInfo = respInfo;
            });
            httpManager.post(url);
            return chgmResult;
        }

        public HttpResult batch(string ops)
        {
            HttpResult batchResult = null;
            string url = string.Format("{0}{1}", Config.RS_HOST, "/batch");
            string token = Auth.createManageToken(url, Encoding.UTF8.GetBytes(ops), this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                batchResult = new FetchResult();
                batchResult.Response = response;
                batchResult.ResponseInfo = respInfo;
            });
            PostArgs postArgs = new PostArgs();
            postArgs.Data = Encoding.UTF8.GetBytes(ops);
            httpManager.Headers.Set(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            httpManager.PostArgs = postArgs;
            httpManager.postData(url);
            return batchResult;
        }

        public FetchResult fetch(string remoteResUrl, string bucket, string key)
        {
            FetchResult fetchResult = null;
            string url = string.Format("{0}{1}", Config.IOVIP_HOST, fetchOp(remoteResUrl, bucket, key));
            string token = Auth.createManageToken(url, null, this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                if (respInfo.isOk())
                {
                    fetchResult = StringUtils.jsonDecode<FetchResult>(response);
                }
                else
                {
                    fetchResult = new FetchResult();
                }
                fetchResult.Response = response;
                fetchResult.ResponseInfo = respInfo;
            });
            httpManager.post(url);
            return fetchResult;
        }

        public HttpResult prefetch(string bucket, string key)
        {
            HttpResult prefetchResult = null;
            string url = string.Format("{0}{1}", Config.IOVIP_HOST, prefetchOp(bucket, key));
            string token = Auth.createManageToken(url, null, this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                prefetchResult = new FetchResult();
                prefetchResult.Response = response;
                prefetchResult.ResponseInfo = respInfo;
            });
            httpManager.post(url);
            return prefetchResult;
        }

        public BucketsResult buckets()
        {
            BucketsResult bucketsResult = null;
            List<string> buckets = new List<string>();
            string url = string.Format("{0}/buckets", Config.RS_HOST);
            string token = Auth.createManageToken(url, null, this.mac);
            HttpManager httpManager = new HttpManager();
            httpManager.setAuthHeader(token);
            httpManager.CompletionHandler = new CompletionHandler(delegate(ResponseInfo respInfo, string response)
            {
                bucketsResult = new BucketsResult();
                bucketsResult.Response = response;
                bucketsResult.ResponseInfo = respInfo;
                if (respInfo.isOk())
                {
                    buckets = JsonConvert.DeserializeObject<List<string>>(response);
                    bucketsResult.Buckets = buckets;
                }
            });
            httpManager.post(url);
            return bucketsResult;
        }

        public string statOp(string bucket, string key)
        {
            return string.Format("/stat/{0}", StringUtils.encodedEntry(bucket, key));
        }

        public string deleteOp(string bucket, string key)
        {
            return string.Format("/delete/{0}", StringUtils.encodedEntry(bucket, key));
        }

        public string copyOp(string srcBucket, string srcKey, string destBucket, string destKey)
        {
            return string.Format("/copy/{0}/{1}", StringUtils.encodedEntry(srcBucket, srcKey),
                StringUtils.encodedEntry(destBucket, destKey));
        }

        public string moveOp(string srcBucket, string srcKey, string destBucket, string destKey)
        {
            return string.Format("/move/{0}/{1}", StringUtils.encodedEntry(srcBucket, srcKey),
                StringUtils.encodedEntry(destBucket, destKey));
        }

        public string chgmOp(string bucket, string key, string mimeType)
        {
            return string.Format("/chgm/{0}/mime/{1}", StringUtils.encodedEntry(bucket, key),
                StringUtils.urlSafeBase64Encode(mimeType));
        }

        public string fetchOp(string url, string bucket, string key)
        {
            return string.Format("/fetch/{0}/to/{1}", StringUtils.urlSafeBase64Encode(url),
                StringUtils.encodedEntry(bucket, key));
        }

        public string prefetchOp(string bucket, string key)
        {
            return string.Format("/prefetch/{0}", StringUtils.encodedEntry(bucket, key));
        }
    }
}
