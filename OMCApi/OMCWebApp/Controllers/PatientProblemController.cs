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
        public async Task<ActionResult>  GetDoctorDetails(string problem)
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

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    IEnumerable<UserDetails> Doctors = JsonConvert.DeserializeObject<IEnumerable<UserDetails>>(DoctorsResponse);
                    if (Doctors.Count() > 0)
                        return View("Index");
                    else
                        return View("LoginFailure");
                }
                else
                {
                    var SignInResponse = Res.Content.ReadAsStringAsync().Result;
                    ViewData["ErrorMessage"] = SignInResponse.ToString();
                    return View("SignUpFailure");

                }
            }
        }

        // GET: PatientProblem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PatientProblem/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientProblem/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PatientProblem/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientProblem/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PatientProblem/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
