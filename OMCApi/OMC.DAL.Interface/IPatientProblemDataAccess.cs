namespace OMC.DAL.Interface
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPatientProblemDataAccess
    {
        IEnumerable<DoctorDetails> GetDoctersFromProblem(string problem, string ipAddress);

        bool ConsultDoctor(int problemId, int doctorId);
    }
}
