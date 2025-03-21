﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hublog.Repository.Entities.Model.Manual_Time
{
    public class Manual_Time
    {
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Summary { get; set; }
        public IFormFile? Attachment { get; set; } // For receiving the file
        public byte[]? AttachmentData { get; set; } // For storing the file in binary format
        public string? FileName { get; set; } // For storing the file name
    }
}
