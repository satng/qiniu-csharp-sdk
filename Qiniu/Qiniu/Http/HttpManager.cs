﻿using System;
using System.Collections.Generic;
using System.Text;
using Qiniu.Common;
using System.Net;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
namespace Qiniu.Http
{
    /// <summary>
    /// HTTP请求管理器，SDK通过该HttpManager发送各类HTTP请求
    /// </summary>
    public class HttpManager
    {
        private HttpWebRequest webRequest;
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private const string APPLICATION_OCTET_STREAM = "application/octet-stream";
        private const string APPLICATION_FORM_URLENCODED = "application/x-www-form-urlencoded";
        private const string APPLICATION_MULTIPART_FORM = "multipart/form-data";
        private const string MULTIPART_BOUNDARY = "-------CSharpSDKBoundaryjEdoki6WbQVQuakI";
        private const string MULTIPART_BOUNDARY_SEP_TAG = "--";
        private const string MULTIPART_SEP_LINE = "\r\n";
        private const int BUFFER_SIZE = 4096;//4KB
        public PostContentType FileContentType { set; get; }
        public PostArgs PostArgs { set; get; }
        public WebHeaderCollection Headers { set; get; }
        public ProgressHandler ProgressHandler { set; get; }
        public CompletionHandler CompletionHandler { set; get; }
        public CancellationSignal CancellationSignal { set; get; }
        private MemoryStream postDataMemoryStream;
        private DateTime startTime;

        /// <summary>
        /// 生成一个随机数
        /// </summary>
        /// <returns>随机数</returns>
        private string genId()
        {
            Random r = new Random();
            return System.DateTime.Now.Millisecond + "" + r.Next(999);
        }

        /// <summary>
        /// 生成HTTP UserAgent
        /// </summary>
        /// <returns>UserAgent</returns>
        private string getUserAgent()
        {
            return string.Format("QiniuCSharpSDK/{0} ({1}; {2}; {3}; {4})",
                Config.VERSION,
                Environment.MachineName,
                Environment.OSVersion.Platform,
                Environment.OSVersion.Version,
                genId());
        }

        /// <summary>
        /// 创建一个默认的HttpManager对象
        /// </summary>
        public HttpManager()
        {
            this.Headers = new WebHeaderCollection();
        }

        /// <summary>
        /// 设置Authorization头部信息
        /// </summary>
        /// <param name="upToken">上传/管理凭证</param>
        public void setAuthHeader(string token)
        {
            this.Headers[HttpRequestHeader.Authorization] = token;
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url"></param>
        public void get(string url)
        {
            try
            {
                this.webRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (Exception ex)
            {
                if (this.CompletionHandler != null)
                {
                    this.CompletionHandler(ResponseInfo.invalidRequest(ex.Message), "");
                    return;
                }
            }

            try
            {
                this.webRequest.UserAgent = this.getUserAgent();
                this.webRequest.AllowAutoRedirect = false;
                this.webRequest.Method = "GET";
                if (this.webRequest.Headers == null)
                {
                    this.webRequest.Headers = new WebHeaderCollection();
                }
                foreach (string headerKey in this.Headers.AllKeys)
                {
                    this.webRequest.Headers[headerKey] = this.Headers[headerKey];
                }
                this.webRequest.ContentLength = 0;
                this.webRequest.Timeout = Config.TIMEOUT_INTERVAL * 1000;

                this.webRequest.BeginGetResponse(new AsyncCallback(handleResponse), this.webRequest);
                this.allDone.WaitOne();
            }
            catch (Exception ex)
            {
                if (CompletionHandler != null)
                {
                    CompletionHandler(ResponseInfo.networkError(ex.Message), "");
                }
            }
        }


        /// <summary>
        /// 发送格式为application/x-www-form-urlencoded的POST请求
        /// 该请求的Body参数由PostArgs.Params来指定
        /// </summary>
        /// <param name="url">请求Url</param>
        public void post(string url)
        {
            try
            {
                this.webRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (Exception ex)
            {
                if (this.CompletionHandler != null)
                {
                    this.CompletionHandler(ResponseInfo.invalidRequest(ex.Message), "");
                    return;
                }
            }

            try
            {
                this.webRequest.UserAgent = this.getUserAgent();
                this.webRequest.AllowAutoRedirect = false;
                this.webRequest.Method = "POST";
                this.webRequest.ContentType = APPLICATION_FORM_URLENCODED;
                if (this.webRequest.Headers == null)
                {
                    this.webRequest.Headers = new WebHeaderCollection();
                }
                foreach (string headerKey in this.Headers.AllKeys)
                {
                    this.webRequest.Headers[headerKey] = this.Headers[headerKey];
                }
                //设置请求Body参数
                StringBuilder postParams = new StringBuilder();
                if (this.PostArgs != null && this.PostArgs.Params != null)
                {
                    foreach (KeyValuePair<string, string> kvp in this.PostArgs.Params)
                    {
                        postParams.Append(Uri.EscapeDataString(kvp.Key)).Append("=")
                            .Append(Uri.EscapeDataString(kvp.Value)).Append("&");
                    }
                }
                byte[] postData = new byte[0];
                if (postParams.Length > 0)
                {
                    postData = Encoding.UTF8.GetBytes(postParams.ToString().Substring(0, postParams.Length - 1));
                }
                this.postDataMemoryStream = new MemoryStream(postData);
                //设置ContentLength头部
                this.webRequest.ContentLength = this.postDataMemoryStream.Length;
                this.webRequest.Timeout = Config.TIMEOUT_INTERVAL * 1000;
                this.webRequest.AllowWriteStreamBuffering = true;
                this.webRequest.BeginGetRequestStream(new AsyncCallback(firePostRequest),
                    this.webRequest);
                allDone.WaitOne();
            }
            catch (Exception ex)
            {
                if (CompletionHandler != null)
                {
                    CompletionHandler(ResponseInfo.networkError(ex.Message), "");
                }
            }
        }

        /// <summary>
        /// 发送异步请求，并获取处理回复
        /// </summary>
        /// <param name="asyncResult">异步状态</param>
        private void firePostRequest(IAsyncResult asyncResult)
        {
            try
            {
                this.startTime = DateTime.Now;
                HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
                if (postDataMemoryStream.Length > 0)
                {
                    Stream postStream = request.EndGetRequestStream(asyncResult);
                    postDataMemoryStream.WriteTo(postStream);
                    postDataMemoryStream.Close();
                    postStream.Flush();
                    postStream.Close();
                }
                request.BeginGetResponse(new AsyncCallback(handleResponse), request);
            }
            catch (Exception ex)
            {
                if (CompletionHandler != null)
                {
                    CompletionHandler(ResponseInfo.networkError(ex.Message), "");
                    this.allDone.Set();
                }
            }
        }

        /// <summary>
        /// 发送格式为application/octet-stream的POST请求，主要用于分片上传中发送二进制数据
        /// </summary>
        /// <param name="url">请求Url</param>
        public void postData(string url)
        {
            try
            {
                this.webRequest = (HttpWebRequest)WebRequest.Create(url);

            }
            catch (Exception ex)
            {
                if (this.CompletionHandler != null)
                {
                    this.CompletionHandler(ResponseInfo.invalidRequest(ex.Message), "");
                    return;
                }
            }

            try
            {
                this.webRequest.UserAgent = this.getUserAgent();
                this.webRequest.AllowAutoRedirect = false;
                this.webRequest.Method = "POST";
                this.webRequest.ContentType = APPLICATION_OCTET_STREAM;
                this.webRequest.ContentLength = this.PostArgs.Data.Length;
                this.webRequest.Timeout = Config.TIMEOUT_INTERVAL * 1000;
                if (this.webRequest.Headers == null)
                {
                    this.webRequest.Headers = new WebHeaderCollection();
                }
                foreach (string headerKey in this.Headers.AllKeys)
                {
                    if (headerKey.Equals("Content-Type"))
                    {
                        this.webRequest.ContentType = this.Headers[headerKey];
                    }
                    else
                    {
                        this.webRequest.Headers[headerKey] = this.Headers[headerKey];
                    }
                }
                this.webRequest.BeginGetRequestStream(new AsyncCallback(firePostDataRequest), webRequest);
                this.allDone.WaitOne();
            }
            catch (Exception ex)
            {
                if (CompletionHandler != null)
                {
                    CompletionHandler(ResponseInfo.networkError(ex.Message), "");
                }
            }
        }

        /// <summary>
        /// 发送异步请求，并获取处理回复
        /// </summary>
        /// <param name="asyncResult">异步状态</param>
        private void firePostDataRequest(IAsyncResult asyncResult)
        {
            try
            {
                this.startTime = DateTime.Now;
                HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
                Stream postDataStream = request.EndGetRequestStream(asyncResult);

                int totalBytes = this.PostArgs.Data.Length;
                int writeTimes = totalBytes / BUFFER_SIZE;
                int bytesWritten = 0;
                if (totalBytes % BUFFER_SIZE != 0)
                {
                    writeTimes += 1;
                }
                for (int i = 0; i < writeTimes; i++)
                {
                    //检查取消信号
                    if (CancellationSignal != null && CancellationSignal())
                    {
                        if (CompletionHandler != null)
                        {
                            CompletionHandler(ResponseInfo.cancelled(), "");
                        }
                        postDataStream.Close();
                        return;
                    }
                    int offset = i * BUFFER_SIZE;
                    int size = BUFFER_SIZE;
                    if (i == writeTimes - 1)
                    {
                        size = totalBytes - i * BUFFER_SIZE;
                    }
                    postDataStream.Write(this.PostArgs.Data, offset, size);
                    bytesWritten += size;
                    //请求进度处理
                    if (ProgressHandler != null)
                    {
                        ProgressHandler(bytesWritten, totalBytes);
                    }
                }
                postDataStream.Flush();
                postDataStream.Close();
                request.BeginGetResponse(new AsyncCallback(handleResponse), request);
            }
            catch (Exception ex)
            {
                if (CompletionHandler != null)
                {
                    CompletionHandler(ResponseInfo.networkError(ex.Message), "");
                    this.allDone.Set();
                }
            }
        }

        /// <summary>
        /// 发送格式为multipart/form-data的POST请求
        /// </summary>
        /// <param name="url">请求Url</param>
        public void multipartPost(string url)
        {
            try
            {
                this.webRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (Exception ex)
            {
                if (this.CompletionHandler != null)
                {
                    this.CompletionHandler(ResponseInfo.invalidRequest(ex.Message), "");
                    return;
                }
            }

            try
            {
                this.webRequest.UserAgent = this.getUserAgent();
                this.webRequest.AllowAutoRedirect = false;
                this.webRequest.Method = "POST";
                this.webRequest.ContentType = string.Format("{0}; boundary={1}", APPLICATION_MULTIPART_FORM, MULTIPART_BOUNDARY);
                //准备数据
                this.postDataMemoryStream = new MemoryStream();
                byte[] boundarySepTag = Encoding.UTF8.GetBytes(MULTIPART_BOUNDARY_SEP_TAG);
                byte[] boundaryData = Encoding.UTF8.GetBytes(MULTIPART_BOUNDARY);
                byte[] multiPartSepLineData = Encoding.UTF8.GetBytes(MULTIPART_SEP_LINE);
                //写入参数
                if (this.PostArgs != null && this.PostArgs.Params != null)
                {
                    foreach (KeyValuePair<string, string> kvp in this.PostArgs.Params)
                    {
                        //写入boundary起始标记
                        postDataMemoryStream.Write(boundarySepTag, 0, boundarySepTag.Length);
                        //写入boundary
                        postDataMemoryStream.Write(boundaryData, 0, boundaryData.Length);
                        //写入头部和数据
                        postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                        byte[] contentHeaderData = Encoding.UTF8.GetBytes(
                            string.Format("Content-Disposition: form-data; name=\"{0}\"", kvp.Key));
                        postDataMemoryStream.Write(contentHeaderData, 0, contentHeaderData.Length);
                        postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                        postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                        byte[] contentData = Encoding.UTF8.GetBytes(kvp.Value);
                        postDataMemoryStream.Write(contentData, 0, contentData.Length);
                        postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                    }
                }
                //写入文件名词和MimeType
                postDataMemoryStream.Write(boundarySepTag, 0, boundarySepTag.Length);
                postDataMemoryStream.Write(boundaryData, 0, boundaryData.Length);
                postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                string fileName = this.PostArgs.FileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = string.Format("RandomFileName_{0}", genId());
                }
                byte[] fileHeaderData = Encoding.UTF8.GetBytes(
                    string.Format("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", fileName));
                string fileContentType = "application/octet-stream";
                if (!string.IsNullOrEmpty(this.PostArgs.MimeType))
                {
                    fileContentType = this.PostArgs.MimeType;
                }
                byte[] fileContentTypeData = Encoding.UTF8.GetBytes(string.Format("Content-Type: {0}", fileContentType));
                postDataMemoryStream.Write(fileHeaderData, 0, fileHeaderData.Length);
                postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                postDataMemoryStream.Write(fileContentTypeData, 0, fileContentTypeData.Length);
                postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                //写入文件数据
                if (FileContentType == PostContentType.BYTES)
                {
                    postDataMemoryStream.Write(this.PostArgs.Data, 0, this.PostArgs.Data.Length);
                }
                else if (FileContentType == PostContentType.FILE)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(this.PostArgs.File, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[BUFFER_SIZE];
                            int numRead = -1;
                            while ((numRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                postDataMemoryStream.Write(buffer, 0, numRead);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.CompletionHandler != null)
                        {
                            this.CompletionHandler(ResponseInfo.fileError(ex), "");
                        }
                        return;
                    }
                }
                else if (FileContentType == PostContentType.STREAM)
                {
                    try
                    {
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int numRead = -1;
                        while ((numRead = this.PostArgs.Stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            postDataMemoryStream.Write(buffer, 0, numRead);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.CompletionHandler != null)
                        {
                            this.CompletionHandler(ResponseInfo.fileError(ex), "");
                        }
                        return;
                    }
                }
                postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                //写入boundary结束标记
                postDataMemoryStream.Write(boundarySepTag, 0, boundarySepTag.Length);
                postDataMemoryStream.Write(boundaryData, 0, boundaryData.Length);
                postDataMemoryStream.Write(boundarySepTag, 0, boundarySepTag.Length);
                postDataMemoryStream.Write(multiPartSepLineData, 0, multiPartSepLineData.Length);
                postDataMemoryStream.Flush();
                //设置ContentLength
                this.webRequest.ContentLength = postDataMemoryStream.Length;
                this.webRequest.Timeout = Config.TIMEOUT_INTERVAL * 1000;
                if (this.webRequest.Headers == null)
                {
                    this.webRequest.Headers = new WebHeaderCollection();
                }
                foreach (string headerKey in this.Headers.AllKeys)
                {
                    this.webRequest.Headers[headerKey] = this.Headers[headerKey];
                }
                this.webRequest.BeginGetRequestStream(new AsyncCallback(fireMultipartPostRequest), webRequest);
                allDone.WaitOne();
            }
            catch (Exception ex)
            {
                if (CompletionHandler != null)
                {
                    CompletionHandler(ResponseInfo.networkError(ex.Message), "");
                    this.allDone.Set();
                }
            }
        }

        /// <summary>
        /// 发送异步请求，并获取处理回复
        /// </summary>
        /// <param name="asyncResult">异步状态</param>
        private void fireMultipartPostRequest(IAsyncResult asyncResult)
        {
            try
            {
                this.startTime = DateTime.Now;
                HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
                Stream postDataStream = request.EndGetRequestStream(asyncResult);

                int bytesWritten = 0;
                int totalBytes = (int)postDataMemoryStream.Length;
                int writeTimes = totalBytes / BUFFER_SIZE;
                if (totalBytes % BUFFER_SIZE != 0)
                {
                    writeTimes += 1;
                }
                postDataMemoryStream.Seek(0, SeekOrigin.Begin);
                int memNumRead = 0;
                byte[] memBuffer = new byte[BUFFER_SIZE];
                while ((memNumRead = postDataMemoryStream.Read(memBuffer, 0, memBuffer.Length)) != 0)
                {
                    //检查取消信号
                    if (this.CancellationSignal != null && this.CancellationSignal())
                    {
                        if (this.CompletionHandler != null)
                        {
                            this.CompletionHandler(ResponseInfo.cancelled(), "");
                        }
                        postDataStream.Close();
                        return;
                    }
                    postDataStream.Write(memBuffer, 0, memNumRead);
                    bytesWritten += memNumRead;
                    //处理进度
                    if (ProgressHandler != null)
                    {
                        ProgressHandler(bytesWritten, totalBytes);
                    }
                }
                postDataMemoryStream.Close();
                postDataStream.Flush();
                postDataStream.Close();
                request.BeginGetResponse(new AsyncCallback(handleResponse), request);
            }
            catch (Exception ex)
            {
                if (CompletionHandler != null)
                {
                    CompletionHandler(ResponseInfo.networkError(ex.Message), "");
                    this.allDone.Set();
                }
            }
        }

        /// <summary>
        /// 处理Http请求结果
        /// </summary>
        /// <param name="asyncResult">异步状态</param>
        private void handleResponse(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            //check for exception
            int statusCode = ResponseInfo.NetworkError;
            string reqId = null;
            string xlog = null;
            string ip = null;
            string xvia = null;
            string error = null;
            string host = null;
            string respData = null;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asyncResult);
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    response = (HttpWebResponse)wex.Response;
                }
                else
                {
                    error = wex.ToString();
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (response != null)
            {
                statusCode = (int)response.StatusCode;
                if (response.Headers != null)
                {
                    WebHeaderCollection respHeaders = response.Headers;
                    foreach (string headerName in respHeaders.AllKeys)
                    {
                        if (headerName.Equals("X-Reqid"))
                        {
                            reqId = respHeaders[headerName].ToString();
                        }
                        else if (headerName.Equals("X-Log"))
                        {
                            xlog = respHeaders[headerName].ToString();
                        }
                        else if (headerName.Equals("X-Via"))
                        {
                            xvia = respHeaders[headerName].ToString();
                        }
                        else if (headerName.Equals("X-Px"))
                        {
                            xvia = respHeaders[headerName].ToString();
                        }
                        else if (headerName.Equals("Fw-Via"))
                        {
                            xvia = respHeaders[headerName].ToString();
                        }
                        else if (headerName.Equals("Host"))
                        {
                            host = respHeaders[headerName].ToString();
                        }
                    }
                    using (StreamReader respStream = new StreamReader(response.GetResponseStream()))
                    {
                        respData = respStream.ReadToEnd();
                    }
                    ip = webRequest.RequestUri.Authority;
                }
                response.Close();
            }

            double duration = DateTime.Now.Subtract(this.startTime).TotalSeconds;
            ResponseInfo respInfo = new ResponseInfo(statusCode, reqId, xlog, xvia, host, ip, duration, error);
            if (this.CompletionHandler != null)
            {
                this.CompletionHandler(respInfo, respData);
            }
            allDone.Set();
        }
    }
}
