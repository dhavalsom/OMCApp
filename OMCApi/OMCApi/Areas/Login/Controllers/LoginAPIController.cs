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
        
        // POST: api/LoginAPI
        [HttpPost]
        [Route("PostUserLogin")]
        public SignInResponse PostUserLogin([FromBody]UserLogin user)
        {
            var SignInObj = _Kernel.Get<ISignIn>();
            var SignInResult = SignInObj.InitiateSignInProcess(user);
            return SignInResult;
        }

        // POST: api/LoginAPI
        [HttpPost]
        [Route("GetAccessCode")]
        public UserAccessCodeResponse GetAccessCode([FromBody]UserLogin user)
        {
            var SignInObj = _Kernel.Get<ISignIn>();
            var getUserAccessCodeResult = SignInObj.GetAccessCode(user);

            return getUserAccessCodeResult;
        }
    }
}
