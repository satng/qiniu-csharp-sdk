using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qiniu.Common
{
    class Zone
    {
        public string UploadHost { set; get; }
        public string UploadBackupHost { set; get; }
        public string RsHost { set; get; }
        public string RsfHost { set; get; }
        public string ApiHost { set; get; }
        public string IovipHost { set; get; }

        public Zone(string uploadHost, string upHost, string rsHost, string rsfHost, string apiHost, string iovipHost)
        {
            this.UploadHost = uploadHost;
            this.UploadBackupHost = upHost;
            this.RsHost = rsHost;
            this.RsfHost = rsfHost;
            this.ApiHost = apiHost;
            this.IovipHost = iovipHost;
        }

        public static Zone ZONE_NB = new Zone("http://up.qiniu.com",
                                            "http://upload.qiniu.com",
                                            "http://rs.qiniu.com",
                                            "http://rsf.qiniu.com",
                                            "http://api.qiniu.com",
                                            "http://iovip.qbox.me"
                                            );

        public static Zone ZONE_BC = new Zone("http://up-z1.qiniu.com",
                                            "http://upload-z1.qiniu.com",
                                            "http://rs-z1.qiniu.com",
                                            "http://rsf-z1.qiniu.com",
                                            "http://api-z1.qiniu.com",
                                            "http://iovip-z1.qbox.me"
                                            );

        public static Zone ZONE_AWS = new Zone("http://up.gdipper.com",
                                             "http://up.gdipper.com",
                                             "http://rs.gdipper.com",
                                             "http://rsf.gdipper.com",
                                             "http://api.gdipper.com",
                                             ""
                                             );

        public static Zone ZONE_ABROAD_NB = new Zone("http://up.qiniug.com",
                                            "http://up.qiniu.com",
                                            "http://rs.qiniu.com",
                                            "http://rsf.qiniu.com",
                                            "http://api.qiniu.com",
                                            "http://iovip.qbox.me"
                                            );

    }
}
