﻿using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using OMC.Models;
using System;
using log4net;

namespace OMC.DAL.Library
{
    public class DataAccessBase
    {
        #region Properties
        public SqlConnection Connection { get; set; }
        public SqlCommand Command { get; set; }
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Constructors 
        public DataAccessBase()
        {
            //Connection = new SqlConnection(@"Data Source=localhost;Initial Catalog=HealthCare;Integrated Security=True;");
            Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            Command = new SqlCommand();
            Command.Connection = Connection;
        }
        #endregion

        #region Methods

        public void LogError(ErrorLog error)
        {
            try
            {
                Command.CommandText = "SP_LOG_ERRORS";
                Command.CommandType = CommandType.StoredProcedure;

                Command.Parameters.AddWithValue("@MESSAGE", error.Message);
                Command.Parameters.AddWithValue("@STACK_TRACE", error.StackTrace);
                Command.Parameters.AddWithValue("@EXCEPTION_TYPE", error.ExceptionType);
                Command.Parameters.AddWithValue("@SOURCE", error.Source);
                Command.Parameters.AddWithValue("@USER_ID", 1);
                Connection.Open();
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }

            finally
            {
                Connection.Close();
            }
        }

        #endregion
    }
}
