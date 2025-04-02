namespace Hublog.Repository.Entities.Model.Productivity
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int OrganizationId { get; set; }
        public int? ProductivityId { get; set; }  
        public string ProductivityName { get; set; }
    }
}
