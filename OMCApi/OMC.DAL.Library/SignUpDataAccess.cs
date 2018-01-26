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
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace OMC.DAL.Library
{
    public class SignUpDataAccess:ISignUpDataAccess
    {
        #region Declaration
        // ISignInDataAccess _signObj;
        #endregion


        #region Methods

        public DataSet InitiateSignUpProcess(UserSignUp signupdetails)
        {
            try
            {

                //SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=HealthCare;Integrated Security=True;");
                //SqlCommand com = new SqlCommand("select * from [dbo].[UserDetail] where firstname = '" + user.Username + "' and password = '" + user.Password + "'", con);
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

                SqlCommand com = new SqlCommand("[SP_USER_DETAIL_MANAGER]", con);
                com.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(com);
                da.SelectCommand.Parameters.Add(new SqlParameter("@USER_DETAIL_XML", SqlDbType.Xml, 100));
                da.SelectCommand.Parameters["@USER_DETAIL_XML"].Value = GetXMLFromObject(signupdetails);
                da.SelectCommand.Parameters.Add(new SqlParameter("@OPERATION", SqlDbType.NVarChar, 100));
                da.SelectCommand.Parameters["@OPERATION"].Value = DBNull.Value;
                da.SelectCommand.Parameters.Add(new SqlParameter("@USER_ID", SqlDbType.BigInt, 100));
                da.SelectCommand.Parameters["@USER_ID"].Value = 1;
                con.Open();

                int result = (int)com.ExecuteScalar();

                DataSet ds = new DataSet();
                //da.Fill(ds);

                int count = ds.Tables[0].Rows.Count;
                con.Close();

                if (count == 1)
                    return ds;
                else
                    return null;
            }
            catch (Exception ex)
            {
                //log
                return null;
            }

        }

        #endregion

        #region XML Serialize/De-Serialize

        public static string GetXMLFromObject(object o)
        {
            // removes version
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            XmlSerializer xsSubmit = new XmlSerializer(o.GetType());
            //StringWriter sw = new StringWriter();
            try
            {
                using (StringWriter sw = new StringWriter())
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    // removes namespace
                    var xmlns = new XmlSerializerNamespaces();
                    xmlns.Add(string.Empty, string.Empty);

                    xsSubmit.Serialize(writer, o, xmlns);
                    return sw.ToString(); // Your XML
                }
            }

            //StringWriter sw = new StringWriter();
            //XmlTextWriter tw = null;
            //try
            //{
            //    XmlSerializer serializer = new XmlSerializer(o.GetType());
            //    tw = new XmlTextWriter(sw);
            //    serializer.Serialize(tw, o);
            //}
            catch (Exception ex)
            {
                //Handle Exception Code
                return null;
            }
            finally
            {
                //sw.Close();
                //if (tw != null)
                //{
                //    tw.Close();
                //}
            }
            //return sw.ToString();
        }


        public static Object ObjectToXML(string xml, Type objectType)
        {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            Object obj = null;
            try
            {
                strReader = new StringReader(xml);
                serializer = new XmlSerializer(objectType);
                xmlReader = new XmlTextReader(strReader);
                obj = serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
                //Handle Exception Code
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                if (strReader != null)
                {
                    strReader.Close();
                }
            }
            return obj;
        }


        #endregion
    }
}
