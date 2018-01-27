using OMC.DAL.Interface;
using OMC.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace OMC.DAL.Library
{
    public class SignInDataAccess : DataAccessBase, ISignInDataAccess
    {
        #region Declaration
       // ISignInDataAccess _signObj;
        #endregion

        #region Methods

        public bool InitiateSignInProcess(UserLogin user)
        {
            try
            {
                Command.CommandText = "SP_GET_LOGIN_DETAILS";
                Command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(Command);
                da.SelectCommand.Parameters.Add(new SqlParameter("@USERNAME", SqlDbType.NVarChar, 100));
                da.SelectCommand.Parameters["@USERNAME"].Value = user.Username;
                da.SelectCommand.Parameters.Add(new SqlParameter("@PASSWORD", SqlDbType.NVarChar, 100));
                da.SelectCommand.Parameters["@PASSWORD"].Value = user.Password;
                
                Connection.Open();

                DataSet ds = new DataSet();
                da.Fill(ds);

                int count = ds.Tables[0].Rows.Count;
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

            finally
            {
                Connection.Close();
            }
        }

        #endregion
    }
}
