using Dapper;
using Newtonsoft.Json;
using OMC.DAL.Interface;
using OMC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OMC.DAL.Library
{
    public class PatientProblemDataAccess : DataAccessBase, IPatientProblemDataAccess
    {
        #region Methods

        public IEnumerable<UserDetails> GetDoctersFromProblem(string problem)
        {
            try
            {
                Log.Info("Started db call to Get Doctors From Problem");
                Log.Info("parameter values" + JsonConvert.SerializeObject(problem));

                IEnumerable<UserDetails> result = null;

                var param = new DynamicParameters();
                param.Add("@Problem", this.String(problem), dbType: DbType.String, direction: ParameterDirection.Input);

                using (SqlConnection con = new SqlConnection(Connection.ConnectionString))
                {
                    var task = con.QueryMultiple("SP_GetDoctorsFromProblem", param, commandType: CommandType.StoredProcedure);
                    result = task.Read<UserDetails>();
                }

                Log.Info("End db call to Get Doctors From Problem");
                return result;
            }
            catch (Exception ex)
            {
                //log
                return null;
            }

            finally
            {
                Connection.Close();
            }
        }

        #endregion

    }
}
