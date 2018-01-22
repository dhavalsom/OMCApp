using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMC.Models;
using OMC.DAL.Interface;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Configuration;

namespace OMC.DAL.Library
{
    public class SignInDataAccess:ISignInDataAccess
    {
        #region Declaration
       // ISignInDataAccess _signObj;
        #endregion


        #region Methods

        public bool InitiateSignInProcess(UserLogin user)
        {
            try
            {
                SqlConnection con = new SqlConnection( WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
                SqlCommand com = new SqlCommand("select * from [dbo].[UserDetail] where firstname = '" + user.Username + "' and password = '" + user.Password + "'", con);
                com.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Close();
                return true;
            }
            catch(Exception ex)
            {
                //log
                return false;
            }
            
        }

        #endregion
    }
}
