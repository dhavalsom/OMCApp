﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMC.Models;

namespace OMC.DAL.Interface
{
    public interface ISignUpDataAccess
    {
        DataSet InitiateSignUpProcess(UserSignUp signupdetails);
    }
}