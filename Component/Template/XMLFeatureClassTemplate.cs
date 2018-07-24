using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PS.Plot.ArcgisFrameWork.Component.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Component.Template
{
    /// <summary>
    /// 通过RecordSet.xml文件构建要素类的构建模板
    /// </summary>
    public class XMLFeatureClassTemplate : SimpleFeatureClassTemplate
    {
        /// <summary>
        /// 通过RecordSet.xml文件构建要素类的构建模板
        /// </summary>
        /// <param name="RecordSetXML">RecordSet类型的xml模板文件</param>
        public XMLFeatureClassTemplate(string RecordSetXML)
        {
            XMLFilePath = RecordSetXML;
        }

        public string XMLFilePath { get; set; }

        protected override bool OnAddField(FieldsHelper flds, string FcName, string FcAlias = "")
        {
            bool result = false;
            try
            {
                flds.ResetFields(new GdbImporter().GetRecordSetFields(XMLFilePath));
                result = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return result;
        }
    }
}
