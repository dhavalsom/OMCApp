using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OMC.Models
{
    //[Serializable]
    public class UserSignUp
    {
        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string Password { get; set; }
        //public string AlternateNo { get; set; }
        //public string EmergencyContactNo { get; set; }
        //public string EmergencyContactPerson { get; set; }
        //public string DLNumber { get; set; }
        //public string SSN { get; set; }
        //public string UserType { get; set; }
        #endregion

        #region Serialization
        public string Serialize()
        {
            return Helper.Serialize<UserSignUp>(this);
        }

        public bool ShouldSerializeEmailAddress()
        {
            return !string.IsNullOrEmpty(EmailAddress);
        }
        #endregion
    }

    public static class Helper
    {
        public static string Serialize<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }
}
