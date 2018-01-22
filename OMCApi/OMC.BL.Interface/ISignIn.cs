using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMC.Models;


namespace OMC.BL.Interface
{
    public interface ISignIn : IDisposable
    {
      bool InitiateSignInProcess(UserLogin user);
    }
}
