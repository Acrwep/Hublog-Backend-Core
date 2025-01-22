using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Activity
{
    public class Timeline
    {
        public string Full_Name { get; set; } 
        public string Team_Name { get; set; }  
        public DateTime? Punch_In { get; set; }  
        public DateTime? Punch_Out { get; set; }  
        public DateTime? AttendanceDate { get; set; } 

        public DateTime? start_MorningBreak { get; set; }  
        public DateTime? MorningBreak_End { get; set; } 

        public DateTime? start_Lunch { get; set; }  
        public DateTime? Lunch_End { get; set; } 

        public DateTime? start_EveningBreak { get; set; }  
        public DateTime? EveningBreak_End { get; set; }  
    }
}
