using Newtonsoft.Json;


namespace Helpers
{
    public class JsonHelper
    {
        public static string ConverObjToJson(object obj)
        {
            string ser = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                Culture = new System.Globalization.CultureInfo("he-IL"),
                Formatting = Formatting.Indented
            });
            return ser;
        }
    }
}