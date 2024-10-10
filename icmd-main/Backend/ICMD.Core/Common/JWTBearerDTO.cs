namespace ICMD.Core.Common
{
    public class JWTBearerDTO
    {
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
    }
}
