using Microsoft.Extensions.Configuration;
using Models.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Helpers
{
    public class AppSettingsHelper
    {
        public static ServicesSettingsModel GetServiceSettings(IConfiguration config, string serviceName)
        {
            List<ServicesSettingsModel> settings = config.GetSection("ServicesSettings").Get<ServicesSettingsModel[]>().ToList();
            var concreteServiceSettings = settings.FirstOrDefault(i => i.Name == serviceName);

            return concreteServiceSettings;
        }
    }
}
