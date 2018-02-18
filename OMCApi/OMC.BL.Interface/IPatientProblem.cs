using OMC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMC.BL.Interface
{
    public interface IPatientProblem
    {
        IEnumerable<DoctorDetails> GetDoctersFromProblem(string problem, string ipAddress);

        bool ConsultDoctor(int problemId, int doctorId);
    }
}
