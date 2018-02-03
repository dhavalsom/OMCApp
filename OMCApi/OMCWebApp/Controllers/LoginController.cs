﻿using Newtonsoft.Json;
using Ninject;
using OMC.Models;
using OMCWebApp.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseUrl"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await client.PostAsync("api/LoginAPI/PostUserLogin", content);
                
                if (Res.IsSuccessStatusCode)
                {
                    var SignInResponse = JsonConvert.DeserializeObject<SignInResponse>( Res.Content.ReadAsStringAsync().Result);
                    if (SignInResponse.IsPasswordVerified)
                    {
                        if (SignInResponse.TwoFactorAuthDone)
                        {
                            return View(SignInResponse);
                        }
                        else
                        {
                            var model = new GetAccessCodeModel
                            {
                                ObjSignInResponse = SignInResponse,
                                IPAddress = user.IPAddress,
                                UserName = user.Username,
                                Method = "Email"
                            };
                            return View("GetAccessCode", model);
                        }
                    }
                    else
                        return View("LoginFailure");
                }
                else
                    return View("LoginFailure");

            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetAccessCode(GetAccessCodeModel getAccessCodeModel)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseUrl"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(new UserLogin
                                                        {
                                                            UserId = getAccessCodeModel.ObjSignInResponse.UserId,
                                                            IPAddress = getAccessCodeModel.IPAddress,
                                                            GetCodeMethod = getAccessCodeModel.Method
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await client.PostAsync("api/LoginAPI/GetAccessCode", content);

                if (Res.IsSuccessStatusCode)
                {
                    var UserAccessCodeResponse = JsonConvert.DeserializeObject<UserAccessCodeResponse>(Res.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrEmpty(UserAccessCodeResponse.AccessCode))
                    {
                        UserAccessCodeResponse.AccessCode = string.Empty;
                        return View("ValidateAccessCode", UserAccessCodeResponse);
                    }
                }
                return View("LoginFailure");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ValidateAccessCode(UserAccessCodeResponse userAccessCode)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseUrl"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonConvert.SerializeObject(userAccessCode);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await client.PostAsync("api/LoginAPI/ValidateAccessCode", content);

                if (Res.IsSuccessStatusCode)
                {
                    userAccessCode = JsonConvert.DeserializeObject<UserAccessCodeResponse>(Res.Content.ReadAsStringAsync().Result);
                    if (userAccessCode.objValidateAccessCodeResponse != null)
                    {
                        return View("ValidateAccessCode", userAccessCode);
                    }
                }
                return View("LoginFailure");
            }
        }

        #endregion
    }
}
