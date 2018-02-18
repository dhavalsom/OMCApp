using Newtonsoft.Json;
using OMC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OMCWebApp.Controllers
{
    public class PatientProblemController : Controller
    {
        // GET: PatientProblem
        public ActionResult Index()
        {
            return View();
        }

        // GET: PatientProblem/Details/5
        public async Task<ActionResult> GetDoctorDetails(string problem)
        {
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseUrl"]);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/PatientProblems/search?problem=" + problem);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DoctorsResponse = await Res.Content.ReadAsStringAsync();
                    if (DoctorsResponse != null)
                    {
                        //Deserializing the response recieved from web api and storing into the Employee list  
                        IEnumerable<DoctorDetails> Doctors = JsonConvert.DeserializeObject<IEnumerable<DoctorDetails>>(DoctorsResponse);
                        if (Doctors.Count() > 0)
                            return PartialView("DoctorsList", Doctors);
                    }
                }

                return Content("false");

            }
        }

        public async Task<ActionResult> ConsultDoctor(int ProblemId, int DoctorId)
        {
            using (var client = new HttpClient())
            {

                //Passing service base url  
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseUrl"]);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string url = "api/PatientProblems/ConsultDoctor?problemId=" + ProblemId + "&doctorId=" + DoctorId;
                HttpResponseMessage Res = await client.GetAsync(url);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var DoctorsResponse = await Res.Content.ReadAsStringAsync();
                    if (DoctorsResponse != null)
                    {
                        //Deserializing the response recieved from web api and storing into the Employee list  
                        return Content(DoctorsResponse);
                    }
                }

                return Content("false");

            }
        }

        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// IP Address.
        /// </returns>
        private string getIPAddress()
        {
            string ipaddress;
            try
            {
                string[] computer_name = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
                String ecname = System.Environment.MachineName;
                ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (ipaddress == "" || ipaddress == null)
                    ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ipaddress;
        }
    }
}
