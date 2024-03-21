using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Communication
{
    public class HttpClientInitializer
    {
        public static HttpClient GetHttpClient(bool useCert, string certFriendlyName)
        {
            if (useCert)
            {
                var cert = CertificateHelper.GetCertificate(certFriendlyName);

                if (cert == null)
                {
                    throw new ArgumentException($"Certificate with FriendlyName: {certFriendlyName} was not found in Local Machine storage");
                }

                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(cert);

                return new HttpClient(handler);
            }
            else
            {
                return new HttpClient();
            }
        }
    }
}
