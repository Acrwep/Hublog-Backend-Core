using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Productivity
{
    public class MappingModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
    }
}
