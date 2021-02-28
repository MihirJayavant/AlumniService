using System;

namespace Core.Entities
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public int YearOfJoining { get; set; }
        public long AnnualSalary { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

    }
}
