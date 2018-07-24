using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Component.AddIn
{
    public abstract class BaseButtonService
    {
        public string ErrorMessage { get; protected set; }

        public abstract bool onExcute();

        public bool Excute()
        {
            bool result = true;
            try
            {
                result = onExcute();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                result = false;
            }
            return result;
        }
    }
}
