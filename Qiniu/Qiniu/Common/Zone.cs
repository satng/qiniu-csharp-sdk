using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qiniu.Common
{
    class Zone
    {
        public string UploadHost { set; get; }
        public string UpHost { set; get; }
        public string RsHost { set; get; }
        public string RsfHost { set; get; }
        public string IovipHost { set; get; }

        public Zone(string uploadHost, string upHost, string rsHost, string rsfHost, string iovipHost)
        {
            this.UploadHost = uploadHost;
            this.UpHost = upHost;
            this.RsHost = rsHost;
            this.RsfHost = rsfHost;
            this.IovipHost = iovipHost;
        }

        public static Zone ZONE_0 = new Zone("http://upload.qiniu.com",
                                            "http://up.qiniu.com",
                                            "http://rs.qiniu.com",
                                            "http://rsf.qiniu.com",
                                            "http://iovip.qbox.me");

        public static Zone ZONE_1 = new Zone("http://upload-z1.qiniu.com",
                                            "http://up-z1.qiniu.com",
                                            "http://rs-z1.qiniu.com",
                                            "http://rsf-z1.qiniu.com",
                                            "http://iovip-z1.qbox.me");
    }
}
