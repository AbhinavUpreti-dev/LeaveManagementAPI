namespace LeaveManagementAPI.Config
{
    public class JWTConfig
    {
        public string SecretKey { get; set; }
        public string ValidAudienceURL { get; set; }
        public string ValidIssuerURL { get; set; }
    }
}
