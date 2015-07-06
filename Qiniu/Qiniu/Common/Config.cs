﻿
namespace Qiniu.Common
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class Config
    {
        //SDK的版本号
        public const string VERSION = "1.0.0";

        public const string RS_HOST = "http://rs.qiniu.com";

        public const string RSF_HOST = "http://rsf.qiniu.com";

        public const string IOVIP_HOST = "http://iovip.qbox.me";

        //分片上传块的大小，固定为4M，不可修改
        public const int BLOCK_SIZE = 4 * 1024 * 1024;

        //上传失败重试次数
        public const int RETRY_MAX = 5;

        //回复超时时间，单位秒
        public const int TIMEOUT_INTERVAL = 30;

        //默认上传服务器地址
        public static string UPLOAD_HOST = "http://upload.qiniu.com";

        //上传重试服务器地址
        public static string UP_HOST = "http://up.qiniu.com";

        //分片上传切片大小
        public static int CHUNK_SIZE = 2 * 1024 * 1024;

        //分片上传的阈值，文件超过该大小采用分片上传
        public static int PUT_THRESHOLD = 512 * 1024;

    }
}
