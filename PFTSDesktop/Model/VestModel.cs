using PFTSDesktop.Properties;
using PFTSModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSDesktop.Model
{
    public class VestModel : IDataErrorInfo
    {
        #region 变量
        private DevVestService service = new DevVestService();
        public string Name { get; set; }
        public string OldName { get; set; }
        public int Id { get; set; }
        public string NoLeft { get; set; }
        public string OldNoLeft { get; set; }
        public string NoRight { get; set; }
        public string OldNoRight { get; set; }
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
            "Name",
            "NoLeft",
            "NoRight",
        };

        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;
                switch (propertyName)
                {
                    case "Name":
                        error = this.ValidateName();
                        break;

                    case "NoLeft":
                        error = this.ValidateNoLeft();
                        break;
                    case "NoRight":
                        error = this.ValidateNoRight();
                        break;
                    default:
                        Debug.Fail("Unexpected property being validated on VestModel: " + propertyName);
                        break;
                }
            return error;
        }

        private string ValidateName()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                return Resources.Vest_Error_MissName;
            }
            else if (IsValidReName(this.Name)) {
                return Resources.Vest_Error_ReName;
            }
            return null;
        }

        private string ValidateNoLeft()
        {
            if (String.IsNullOrEmpty(this.NoLeft))
            {
                return Resources.Vest_Error_MissNo;
            }
            else if (IsValidReNo(this.NoLeft))
            {
                return Resources.Vest_Error_ReNo;
            }
            return null;
        }

        private string ValidateNoRight()
        {
            if (String.IsNullOrEmpty(this.NoRight))
            {
                return Resources.Vest_Error_MissNo;
            }
            else if (IsValidReNo(this.NoRight))
            {
                return Resources.Vest_Error_ReNo;
            }
            return null;
        }

        private bool IsValidReName(string name) {
            if (String.IsNullOrEmpty(name))
                return false;
            var vestModel = service.GetByName(name, OldName);
            return vestModel == null ? false : true;
        }

        private bool IsValidReNo(string no)
        {
            if (String.IsNullOrEmpty(no))
                return false;
            var vestModel = service.GetByNo(no, OldNoLeft,OldNoRight);
            return vestModel == null ? false : true;
        }
        #endregion
    }
}
