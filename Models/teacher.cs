using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScoolProject.Models 
{
    public class teacher
    {
        public int TeacherId { get; set; }
        public string TeacherfName {  get; set; }
        public string TeacherlName {  get; set; }
        public string EmployeeNum { get; set; }
        public DateTime HireDate { get; set; }  
        public double Salary { get; set; }
        public string className { get; set; }
        public int classId { get; set; }
    }
}