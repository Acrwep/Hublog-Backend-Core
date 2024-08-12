namespace Hublog.Repository.Entities.Model
{
    public class Role
    {
        public int Id { get; set; } //(int, not null)
        public string Name { get; set; } //(varchar(50), not null)
        public int AccessLevel { get; set; } //(int, not null)
        public string Description { get; set; } //(varchar(200), null)
        public bool Admin { get; set; } //(bit, not null)
        public bool URLS { get; set; } //(bit, not null)
        public bool ScreenShot { get; set; } //(bit, not null)
        public bool LiveStream { get; set; } //(bit, not null)
        public int OrganizationId { get; set; } //(int, not null)
    }
}
