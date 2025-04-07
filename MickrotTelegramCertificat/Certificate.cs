using tik4net.Objects;

namespace MickrotTelegramCertificat
{
    [TikEntity("certificate")]
    public class Certificate
    {
        [TikProperty("name")]
        public string Name { get; set; }

        [TikProperty("common-name")]
        public string CommonName { get; set; }

        [TikProperty("invalid-after")]
        public string InvalidAfter { get; set; }
    }
}
