
using System.Text;
namespace Qiniu.Util
{
    public class Auth
    {
        public static string createUploadToken(PutPolicy putPolicy, Mac mac)
        {
            string jsonData = putPolicy.ToString();
            return mac.SignWithData(Encoding.UTF8.GetBytes(jsonData));
        }

        public static string createManageToken(string url, byte[] reqBody, Mac mac)
        {
            return mac.SignRequest(url, reqBody);
        }
    }
}
