using Dapper;
using Newtonsoft.Json;
using OMC.DAL.Interface;
using OMC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OMC.DAL.Library
{
    public class PatientProblemDataAccess : DataAccessBase, IPatientProblemDataAccess
    {
        #region Methods

        public IEnumerable<DoctorDetails> GetDoctersFromProblem(string problem, string ipAddress)
        {
            try
            {
                Log.Info("Started db call to Get Doctors From Problem");
                Log.Info("parameter values" + JsonConvert.SerializeObject(problem));

                IEnumerable<DoctorDetails> result = null;

                var param = new DynamicParameters();
                param.Add("@Problem", this.String(problem), dbType: DbType.String, direction: ParameterDirection.Input);
                param.Add("@IpAddress", this.String(ipAddress), dbType: DbType.String, direction: ParameterDirection.Input);

                using (SqlConnection con = new SqlConnection(Connection.ConnectionString))
                {
                    var task = con.QueryMultiple("SP_GetDoctorsFromProblem", param, commandType: CommandType.StoredProcedure);
                    result = task.Read<DoctorDetails>();
                }

                Log.Info("End db call to Get Doctors From Problem");
                return result;
            }
            catch (Exception ex)
            {
                //log
                return null;
            }
        }

        public bool ConsultDoctor(int problemId, int doctorId)
        {
            try
            {
                bool result;

                var param = new DynamicParameters();
                param.Add("@ProblemId", this.Integer(problemId), dbType: DbType.Int32, direction: ParameterDirection.Input);
                param.Add("@DoctorId", this.Integer(doctorId), dbType: DbType.Int32, direction: ParameterDirection.Input);

                using (SqlConnection con = new SqlConnection(Connection.ConnectionString))
                {
                    var task = con.Query<bool>("SP_ConsultDoctor", param, commandType: CommandType.StoredProcedure);
                    result = task.SingleOrDefault<bool>();
                }

                Log.Info("End db call to Get Doctors From Problem");
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion

    }
}
