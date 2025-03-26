using System.ComponentModel.DataAnnotations;

namespace Hublog.Repository.Entities.Model.ApplicationModel
{
    public class ApplicationUsage
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Application name is required.")]
        public string ApplicationName { get; set; }
        [Required(ErrorMessage = "Total usage is required.")]
        public string TotalUsage { get; set; }
        [Required(ErrorMessage = "Details are required.")]
        public string Details { get; set; }
        [Required(ErrorMessage = "Usage date is required.")]
        public string UsageDate { get; set; }
    }
}
