
namespace  Models.Services.Settings
{

    public class ServicesSettingsModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool UseMock { get; set; }
        public string Method { get; set; }
        public string BodyType { get; set; }
        public bool UseCert { get; set; }
        public string CertName { get; set; }
        public ConstantsModel[] Constants { get; set; }
        public bool UseFakeData { get; set; }
        public HeadersModel[] Headers { get; set; }
    }

    public class ConstantsModel
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
    }

    public class HeadersModel
    {
        public string HeaderName { get; set; }
        public string Value { get; set; }
    }
}

