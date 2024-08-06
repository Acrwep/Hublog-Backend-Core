namespace Hublog.Repository.Entities.Model
{
    public class UserScreenShotModels
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime ScreenShotDate { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte[] ImageData { get; set; }
    }
}
