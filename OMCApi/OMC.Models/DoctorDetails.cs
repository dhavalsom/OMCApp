namespace OMC.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DoctorDetails
    {
        public int DoctorId { get; set; }
        public string Doctor { get; set; }
        public string Expertise { get; set; }
        public int ProblemId { get; set; }
    }
}
