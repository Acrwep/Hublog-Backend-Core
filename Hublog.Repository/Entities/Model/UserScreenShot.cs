namespace Hublog.Repository.Entities.Model
{
    public class UserScreenShot       
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime ScreenShotDate { get; set; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }
    }
}
