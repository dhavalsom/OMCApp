using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OMC.Models;
using Ninject;
using OMC.BL.Interface;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace OMCApi.Areas.Login.Controllers
{
    public class LoginController : Controller
    {

        #region Declarations

        private readonly IKernel _Kernel;

        #endregion

        #region Constructor

        public LoginController()
        {
            _Kernel = new StandardKernel();
            _Kernel.Load(new OMC.Modules.SignInModule());
        }

        #endregion

        #region Methods

        // GET: Login/Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signin(UserLogin user)
        {
            var SignInObj = _Kernel.Get<ISignIn>();

            string username = user.Username;
            string password = user.Password;

            using (var client = new HttpClient())
            {
                //var SignInResult = SignInObj.InitiateSignInProcess(user);

                //Passing service base url  
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseUrl"]);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage Res = await client.GetAsync("/api/LoginAPI/");
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await client.PostAsync("api/LoginAPI/PostUserLogin", content);
                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var SignInResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    //UserInfo = JsonConvert.DeserializeObject<List<User>>(SignInResponse);

                }

            }
            return View();
        }
        // GET: Login/Login/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Login/Login/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Login/Login/Create
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

        // GET: Login/Login/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Login/Login/Edit/5
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

        // GET: Login/Login/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Login/Login/Delete/5
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

        #endregion
    }
}
