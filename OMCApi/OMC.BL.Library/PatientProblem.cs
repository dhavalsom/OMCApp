namespace OMC.BL.Library
{
    using DAL.Interface;
    using Models;
    using OMC.BL.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PatientProblem : IPatientProblem
    {
        #region Declarations
        private readonly IPatientProblemDataAccess _PatientProblemDAL;
        #endregion

        public PatientProblem(IPatientProblemDataAccess patientProblemDAL)
        {
            this._PatientProblemDAL = patientProblemDAL;
        }

        public IEnumerable<UserDetails> GetDoctersFromProblem(string problem)
        {
            try
            {
                return this._PatientProblemDAL.GetDoctersFromProblem(problem);
            }
            catch (Exception ex)
            {
                //Log
                return null;
            }
            finally
            {
                //Log
            }
        }
    }
}
