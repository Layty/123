using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataNotification
{
    public class ValidateModelBase : ObservableObject, IDataErrorInfo
    {
        #region 属性 
        /// <summary>
        /// 表当验证错误集合
        /// </summary>
        private readonly Dictionary<string, string> _dataErrors = new Dictionary<string, string>();

        /// <summary>
        /// 是否验证通过
        /// </summary>
        public Boolean IsValidated
        {
            get
            {
                if (_dataErrors != null && _dataErrors.Count > 0)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        public string this[string columnName]
        {
            get
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = columnName;
                var res = new List<ValidationResult>();
                var result = Validator.TryValidateProperty(this.GetType().GetProperty(columnName)?.GetValue(this, null), vc, res);
                if (res.Count > 0)
                {
                    AddDic(_dataErrors, vc.MemberName);
                    return string.Join(Environment.NewLine, res.Select(r => r.ErrorMessage).ToArray());
                }
                RemoveDic(_dataErrors, vc.MemberName);
                return null;
            }
        }

        public string Error => null;


        #region 附属方法

        /// <summary>
        /// 移除字典
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="dicKey"></param>
        private void RemoveDic(Dictionary<string, string> dics, string dicKey)
        {
            dics.Remove(dicKey);
        }

        /// <summary>
        /// 添加字典
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="dicKey"></param>
        private void AddDic(Dictionary<string, string> dics, string dicKey)
        {
            if (!dics.ContainsKey(dicKey)) dics.Add(dicKey, "");
        }
        #endregion
    }
}
