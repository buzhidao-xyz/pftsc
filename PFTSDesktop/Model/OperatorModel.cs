using PFTSDesktop.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PFTSDesktop.Model
{
    public class OperatorModel : IDataErrorInfo
    {
        #region 变量
        public string Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        #endregion

        #region IDataErrorInfo Members
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return this.GetValidationError(propertyName); }
        }

        #endregion

        #region 错误验证
        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                    if (GetValidationError(property) != null)
                        return false;

                return true;
            }
        }

        static readonly string[] ValidatedProperties = 
        {
            "Account",
            "Password",
        };


        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Account":
                    //error = this.ValidateAccount();
                    break;

                case "Password":
                    //error = this.ValidatePassword();
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on OperatorModel: " + propertyName);
                    break;
            }

            return error;
        }

        private string ValidateAccount()
        {
            if (String.IsNullOrEmpty(this.Account))
            {
                return Resources.Operator_Error_MissAccount;
            }
            return null;
        }

        private string ValidatePassword()
        {
            if (String.IsNullOrEmpty(this.Password))
            {
                return Resources.Operator_Error_MissPasswprd;
            }
            return null;
        }

        static bool IsValidEmailAddress(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            // This regex pattern came from: http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        #endregion
    }
}
