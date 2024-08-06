namespace Hublog.Repository.Entities.Login
{
    public class JWTSetting
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
