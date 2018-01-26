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

                //SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=HealthCare;Integrated Security=True;");
                //SqlCommand com = new SqlCommand("select * from [dbo].[UserDetail] where firstname = '" + user.Username + "' and password = '" + user.Password + "'", con);
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

                SqlCommand com = new SqlCommand("[SP_GET_LOGIN_DETAILS]",con);
                com.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(com);
                da.SelectCommand.Parameters.Add(new SqlParameter("@USERNAME", SqlDbType.NVarChar, 100));
                da.SelectCommand.Parameters["@USERNAME"].Value = user.Username;
                da.SelectCommand.Parameters.Add(new SqlParameter("@PASSWORD", SqlDbType.NVarChar, 100));
                da.SelectCommand.Parameters["@PASSWORD"].Value = user.Password;
              
                con.Open();

                DataSet ds = new DataSet();
                da.Fill(ds);

                int count = ds.Tables[0].Rows.Count;
                con.Close();

                if (count == 1)
                    return true;
                else
                    return false;
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
