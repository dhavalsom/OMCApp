using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OMC.Models;
using Ninject;
using OMC.BL.Interface;
using System.Data;

namespace OMCApi.Areas.Login.Controllers
{
    [RoutePrefix("api/SignUpAPI")]
    public class SignUpAPIController : ApiController
    {
        #region Declarations

        private readonly IKernel _Kernel;

        #endregion

        #region Constructor

        public SignUpAPIController()
        {
            //_Kernel = new StandardKernel(new OMC.Modules.SignInModule());
            _Kernel = new StandardKernel();
            _Kernel.Load(new OMC.Modules.SignUpModule());
        }

        #endregion

        // GET: api/SignUpAPI
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        [HttpPost]
        [Route("PostUserSignUp")]
        public DataSet PostUserSignUp([FromBody]UserSignUp userdetails)
        {
            var SignUpObj = _Kernel.Get<ISignUp>();

            var SignUpResult = SignUpObj.InitiateSignUpProcess(userdetails);

            return SignUpResult;
        }

        // PUT: api/SignUpAPI/5
        public void Put(int id, [FromBody]string value)
        {
        }

       
    }
}
