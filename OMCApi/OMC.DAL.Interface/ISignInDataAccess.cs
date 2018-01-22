using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMC.Models;

namespace OMC.DAL.Interface
{
    public interface ISignInDataAccess
    {

        //void connection();
        bool InitiateSignInProcess(UserLogin user);
    }
}
