using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMC.BL.Interface;
using OMC.DAL.Interface;
using OMC.Models;


namespace OMC.BL.Library
{
    public class SignUp:ISignUp
    {
        #region Declarations
        ISignUpDataAccess _signUpDA;
        #endregion

        public SignUp(ISignUpDataAccess SignUpDA)
        {
            this._signUpDA = SignUpDA;
        }

        public DataSet InitiateSignUpProcess(UserSignUp signupdetails)
        {
            try
            {
                DataSet isSignin = this._signUpDA.InitiateSignUpProcess(signupdetails);
                return isSignin;
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

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            { }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
