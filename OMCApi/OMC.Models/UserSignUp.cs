using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage ="FirstName required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName required")]
        public string LastName { get; set; }
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        [Required]
        [EmailAddress(ErrorMessage = "Bad email")]
        public string EmailAddress { get; set; }
        //[Required(ErrorMessage = "Address required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "PhoneNumber required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Gender")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "DOB")]
        public string DOB { get; set; }
        [Required(ErrorMessage = "Password")]
        public string Password { get; set; }
        public string AlternateNo { get; set; }
        public string EmergencyContactNo { get; set; }
        public string EmergencyContactPerson { get; set; }
        public string DLNumber { get; set; }
        public string SSN { get; set; }
        public int UserType { get; set; }
        public int Active { get; set; }
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
