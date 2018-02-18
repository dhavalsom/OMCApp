namespace OMCApi.Controllers
{
    using OMC.BL.Interface;
    using OMC.Models;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Http;
    using System.Net.Http;
    using System.Web;
    using System.Linq;
    using System.ServiceModel.Channels;

    [RoutePrefix("api/PatientProblems")]
    public class PatientProblemsController : ApiController
    {
        private IPatientProblem patientProblem;

        public PatientProblemsController(IPatientProblem patientProblem)
        {
            this.patientProblem = patientProblem;
        }

        [HttpGet]
        [Route("search")]
        // GET api/PatientProblems?
        public IHttpActionResult GetDoctors(string problem = "")
        {
            if (!(problem.Length > 200))
            {
                try
                {
                    return this.Ok<IEnumerable<DoctorDetails>>(patientProblem.GetDoctersFromProblem(problem, this.getIPAddress(this.Request)));
                }
                catch (System.Exception ex)
                {
                    return this.InternalServerError(ex);
                }
            }

            return this.BadRequest("Maximum {200} characters exceeded.");
        }

        [HttpGet]
        [Route("ConsultDoctor")]
        public IHttpActionResult ConsultDoctor(int problemId, int doctorId)
        {
            try
            {
                return this.Ok<bool>(patientProblem.ConsultDoctor(problemId, doctorId));
            }
            catch (System.Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        private string getIPAddress(HttpRequestMessage request)
        {
            string ipAddress = GetHeader(request, "X-Forwarded-For");

            if (!string.IsNullOrWhiteSpace(ipAddress))
            {
                return ipAddress;
            }

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                if (prop != null)
                {
                    return prop.Address;
                }
            }

            return null;
        }

        public static string GetHeader(HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys = null;
            if (!request.Headers.TryGetValues(key, out keys))
            {
                return null;
            }

            return keys.First();
        }
    }
}
