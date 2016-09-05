using PFTSDesktop.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFTSDesktop.Model
{
    public class SuspectModel : IDataErrorInfo
    {
        #region 变量
        public int Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Sex { get; set; }
        public int VestId { get; set; }
        public int? LockerId { get; set; }
        public System.Nullable<int> OfficerId { get; set; }
        public string Status { get; set; }
        public string Recover { get; set; }
        public string PirvateGoods { set; get; }
        public string remark { get; set; }
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
            "No",
            "Name",
        };
        private string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "No":
                    error = this.ValidateNo();
                    break;

                case "Name":
                    error = this.ValidateName();
                    break;
                //case "VestId":
                //    error = this.ValidateVest();
                //    break;
                //case "OfficerId":
                //    error = this.ValidateOfficer();
                //    break;
                default:
                    Debug.Fail("Unexpected property being validated on SuspectModel: " + propertyName);
                    break;
            }

            return error;
        }

        private string ValidateNo()
        {
            if (String.IsNullOrEmpty(this.No))
            {
                return Resources.Suspect_Error_MissNo;
            }
            return null;
        }

        private string ValidateName()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                return Resources.Suspect_Error_MissName;
            }
            return null;
        }

        //private string ValidateVest()
        //{
        //    if (String.IsNullOrEmpty(this.VestId))
        //    {
        //        return Resources.Suspect_Error_MissVest;
        //    }
        //    return null;
        //}

        //private string ValidateOfficer()
        //{
        //    if (String.IsNullOrEmpty(this.OfficerId))
        //    {
        //        return Resources.Suspect_Error_MissOfficer;
        //    }
        //    return null;
        //}

        #endregion
    }
}
