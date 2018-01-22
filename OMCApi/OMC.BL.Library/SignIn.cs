using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMC.BL.Interface;
using OMC.DAL.Interface;
using OMC.Models;

namespace OMC.BL.Library
{
    public class SignIn:ISignIn
    {

        #region Declarations
        ISignInDataAccess _signInDA;
        #endregion

        public SignIn(ISignInDataAccess SignInDA)
        {
            this._signInDA = SignInDA;
        }

        public bool InitiateSignInProcess(UserLogin user)
        {
            try
            {
                bool isSignin = this._signInDA.InitiateSignInProcess(user);
                return isSignin;
            }
            catch(Exception ex)
            {
                //Log
                return false;
            }
            finally
            {
                //Log
            }
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
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
