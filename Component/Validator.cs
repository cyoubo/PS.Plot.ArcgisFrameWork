using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Component
{
    public class Validator
    {
        private IList<string> errorMessage;

        public Validator()
        {
            errorMessage = new List<string>();
        }

        public void ClearErrorMessage()
        {
            errorMessage.Clear();
        }

        public bool IsValidate
        {
            get
            {
                return errorMessage.Count == 0;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return string.Join("\r\n", errorMessage);
            }
        }

        public void ValidateNullString(string content, string caution)
        {
            if (string.IsNullOrEmpty(content))
                errorMessage.Add(string.Format("请输入 {0} ", caution));
        }



        public void ValidateInteger(string p1, string p2)
        {
            if (string.IsNullOrEmpty(p1))
                errorMessage.Add(string.Format("请输入 {0} ", p2));
            else
            {
                try
                {
                    int.Parse(p1);
                }
                catch (Exception)
                {
                    errorMessage.Add(string.Format("{0} 要求为整数，请输入正确格式的数据"));
                }
            }
        }

        public void ValidateDouble_Positive(string p1, string p2)
        {
            if (string.IsNullOrEmpty(p1))
                errorMessage.Add(string.Format("请输入 {0} ", p2));
            else
            {
                try
                {
                    if (double.Parse(p1) < 0)
                        errorMessage.Add(string.Format("{0} 要求为正小数，请输入正确格式的数据"));
                }
                catch (Exception)
                {
                    errorMessage.Add(string.Format("{0} 要求为正小数，请输入正确格式的数据"));
                }
            }
        }
    }
}
