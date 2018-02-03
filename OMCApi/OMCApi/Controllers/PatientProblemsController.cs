namespace OMCApi.Controllers
{
    using OMC.BL.Interface;
    using OMC.Models;
    using System.Collections.Generic;
    using System.Web.Http;

    [RoutePrefix("api/PatientProblems")]
    public class PatientProblemsController : ApiController
    {
        private IPatientProblem patientProblem;

        public PatientProblemsController(IPatientProblem patientProblem)
        {
            this.patientProblem = patientProblem;
        }

        
        [Route("search")]
        // GET api/PatientProblems?
        public IHttpActionResult GetUserDetails(string problem = "")
        {
            if (!(problem.Length > 200))
            {
                return this.Ok<IEnumerable<UserDetails>>(patientProblem.GetDoctersFromProblem(problem));
            }

            return this.BadRequest("Maximum {200} characters exceeded.");
        }
    }
}
