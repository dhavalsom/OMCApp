using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OMC.Models;
using Ninject;
using OMC.BL.Interface;

namespace OMCApi.Areas.Login.Controllers
{
    [RoutePrefix("api/LoginAPI")]
    public class LoginAPIController : ApiController
    {
        #region Declarations

        private readonly IKernel _Kernel;

        #endregion

        #region Constructor

        public LoginAPIController()
        {
            //_Kernel = new StandardKernel(new OMC.Modules.SignInModule());
            _Kernel = new StandardKernel();
            _Kernel.Load(new OMC.Modules.SignInModule());
        }

        #endregion


        // GET: api/LoginAPI
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/LoginAPI/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/LoginAPI
        [HttpPost]
        [Route("PostUserLogin")]
        public bool PostUserLogin([FromBody]UserLogin user)
        {
            var SignInObj = _Kernel.Get<ISignIn>();

            string username = user.Username;
            string password = user.Password;

            var SignInResult = SignInObj.InitiateSignInProcess(user);

            return SignInResult;
        }

        // PUT: api/LoginAPI/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE: api/LoginAPI/5
        //public void Delete(int id)
        //{
        //}
    }
}
