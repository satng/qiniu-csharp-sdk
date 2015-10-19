
namespace Qiniu.Common
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class Config
    {
        //SDK的版本号
        public const string VERSION = "1.0.0";

        //默认上传服务器地址
        public static string UPLOAD_HOST = "http://up.qiniu.com";

        //上传重试服务器地址
        public static string UPLOAD_BACKUP_HOST = "http://upload.qiniu.com";

        public static string RS_HOST = "http://rs.qiniu.com";

        public static string RSF_HOST = "http://rsf.qbox.me";

        public static string API_HOST = "http://api.qiniu.com";

        public static string IOVIP_HOST = "http://iovip.qbox.me";

        //分片上传块的大小，固定为4M，不可修改
        public const int BLOCK_SIZE = 4 * 1024 * 1024;

        //上传失败重试次数
        public const int RETRY_MAX = 5;

        //回复超时时间，单位秒
        public const int TIMEOUT_INTERVAL = 30;

        //分片上传切片大小
        public static int CHUNK_SIZE = 2 * 1024 * 1024;

        //分片上传的阈值，文件超过该大小采用分片上传
        public static int PUT_THRESHOLD = 512 * 1024;

        //upload to nb
        public static void UseZoneNB()
        {
            Config.UPLOAD_HOST = Zone.ZONE_NB.UploadHost;
            Config.UPLOAD_BACKUP_HOST = Zone.ZONE_NB.UploadBackupHost;
            Config.RS_HOST = Zone.ZONE_NB.RsHost;
            Config.RSF_HOST = Zone.ZONE_NB.RsfHost;
            Config.API_HOST = Zone.ZONE_NB.ApiHost;
            Config.IOVIP_HOST = Zone.ZONE_NB.IovipHost;
        }

        //upload to bc
        public static void UseZoneBC()
        {
            Config.UPLOAD_HOST = Zone.ZONE_BC.UploadHost;
            Config.UPLOAD_BACKUP_HOST = Zone.ZONE_BC.UploadBackupHost;
            Config.RS_HOST = Zone.ZONE_BC.RsHost;
            Config.RSF_HOST = Zone.ZONE_BC.RsfHost;
            Config.API_HOST = Zone.ZONE_BC.ApiHost;
            Config.IOVIP_HOST = Zone.ZONE_BC.IovipHost;
        }

        //upload to aws
        public static void UseZoneAWS()
        {
            Config.UPLOAD_HOST = Zone.ZONE_AWS.UploadHost;
            Config.UPLOAD_BACKUP_HOST = Zone.ZONE_AWS.UploadBackupHost;
            Config.RS_HOST = Zone.ZONE_AWS.RsHost;
            Config.RSF_HOST = Zone.ZONE_AWS.RsfHost;
            Config.API_HOST = Zone.ZONE_AWS.ApiHost;
            Config.IOVIP_HOST = Zone.ZONE_AWS.IovipHost;
        }

        //upload to us->nb
        public static void UseZoneAbroadNB()
        {
            Config.UPLOAD_HOST = Zone.ZONE_ABROAD_NB.UploadHost;
            Config.UPLOAD_BACKUP_HOST = Zone.ZONE_ABROAD_NB.UploadBackupHost;
            Config.RS_HOST = Zone.ZONE_ABROAD_NB.RsHost;
            Config.RSF_HOST = Zone.ZONE_ABROAD_NB.RsfHost;
            Config.API_HOST = Zone.ZONE_ABROAD_NB.ApiHost;
            Config.IOVIP_HOST = Zone.ZONE_ABROAD_NB.IovipHost;
        }
    }
}