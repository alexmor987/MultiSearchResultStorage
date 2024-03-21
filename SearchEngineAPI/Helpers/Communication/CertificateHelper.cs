using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Communication
{
    public class CertificateHelper
    {
        public static X509Certificate2? GetCertificate(string certFriendlyName)
        {
            X509Store userCaStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            X509Certificate2? cert = null;

            try
            {
                userCaStore.Open(OpenFlags.ReadOnly);
                for (int i = 0; i < userCaStore.Certificates.Count; i++)
                {
                    if (userCaStore.Certificates[i].FriendlyName.ToLower().IndexOf(certFriendlyName.ToLower()) >= 0)
                    {
                        cert = userCaStore.Certificates[i];
                        break;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                userCaStore.Close();
            }

            return cert;
        }

    }
}
