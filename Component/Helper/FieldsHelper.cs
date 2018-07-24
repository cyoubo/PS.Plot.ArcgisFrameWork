using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Component.Helper
{
    /// <summary>
    /// 字段组帮助类
    /// </summary>
    public class FieldsHelper
    {
        public string ErrorMessage { get; protected set; }
        /// <summary>
        /// 待操作的字段组
        /// </summary>
        private IFields m_Fields;
        /// <summary>
        /// 获得被操作的字段组
        /// </summary>
        public IFields Fields
        {
            get { return m_Fields; }
        }
        /// <summary>
        /// 字段组帮助类
        /// </summary>
        public FieldsHelper()
        {
            m_Fields = new Fields();
        }
        /// <summary>
        /// 向字段组中添加一个几何字段
        /// </summary>
        /// <param name="name">几何字段名</param>
        /// <param name="geoType">几何类型</param>
        /// <param name="spRef">空间参考</param>
        public void AddShapeFields(string name , esriGeometryType geoType, ISpatialReference spRef = null)
        {

            IField field_Shape = new Field();
            IFieldEdit field_Shape_Edit = field_Shape as IFieldEdit;

            field_Shape_Edit.Name_2 = name;
            field_Shape_Edit.Type_2 = esriFieldType.esriFieldTypeGeometry;

            IGeometryDef geometrydef = new GeometryDef();
            IGeometryDefEdit geometrydeiEdit = geometrydef as IGeometryDef as IGeometryDefEdit;
            geometrydeiEdit.GeometryType_2 = geoType;
            geometrydeiEdit.SpatialReference_2 = spRef;
            field_Shape_Edit.GeometryDef_2 = geometrydef;

            (m_Fields as IFieldsEdit).AddField(field_Shape);
        }
        /// <summary>
        /// 向字段中添加一个Text类型字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="allias">字段别名</param>
        /// <param name="length">字段长度</param>
        /// <param name="nullable">是否可为null</param>
        public void AddTextField(string name, string allias = "", int length = 50, bool nullable = true)
        {
            allias = string.IsNullOrEmpty(allias) ? name : allias;

            IField field_Shape = new Field();
            IFieldEdit field_Shape_Edit = field_Shape as IFieldEdit;

            field_Shape_Edit.Name_2 = name;
            field_Shape_Edit.AliasName_2 = allias;
            field_Shape_Edit.Length_2 = length;
            field_Shape_Edit.IsNullable_2 = nullable;
            field_Shape_Edit.Type_2 = esriFieldType.esriFieldTypeString;

            (m_Fields as IFieldsEdit).AddField(field_Shape);
        }
        /// <summary>
        /// 向字段中添加一个整数类型字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="allias">字段别名</param>
        /// <param name="shortable">是否是短整型</param>
        /// <param name="nullable">是否可为null</param>
        public void AddIntField(string name, string allias = "", bool shortable = false, bool nullable = true)
        {
            allias = string.IsNullOrEmpty(allias) ? name : allias;

            IField field_Shape = new Field();
            IFieldEdit field_Shape_Edit = field_Shape as IFieldEdit;

            field_Shape_Edit.Name_2 = name;
            field_Shape_Edit.AliasName_2 = allias;
            field_Shape_Edit.IsNullable_2 = nullable;
            field_Shape_Edit.Type_2 = shortable ? esriFieldType.esriFieldTypeSmallInteger : esriFieldType.esriFieldTypeInteger;

            (m_Fields as IFieldsEdit).AddField(field_Shape);
        }
        /// <summary>
        /// 向字段中添加一个小数类型字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="allias">字段别名</param>
        /// <param name="singleble">是否是单精度小数型</param>
        /// <param name="nullable">是否可为null</param>
        public void AddNumberField(string name, string allias = "", bool singleble = false, bool nullable = true)
        {
            allias = string.IsNullOrEmpty(allias) ? name : allias;

            IField field_Shape = new Field();
            IFieldEdit field_Shape_Edit = field_Shape as IFieldEdit;

            field_Shape_Edit.Name_2 = name;
            field_Shape_Edit.AliasName_2 = allias;
            field_Shape_Edit.IsNullable_2 = nullable;
            field_Shape_Edit.Type_2 = singleble ? esriFieldType.esriFieldTypeSingle : esriFieldType.esriFieldTypeDouble;

            (m_Fields as IFieldsEdit).AddField(field_Shape);
        }
        /// <summary>
        /// 向字段中添加一个二进制类型字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="allias">字段别名</param>
        /// <param name="length">字段长度</param>
        /// <param name="nullable">是否可为null</param>
        public void AddBlobField(string name, string allias = "", int length = 255, bool nullable = true)
        {
            allias = string.IsNullOrEmpty(allias) ? name : allias;

            IField field_Shape = new Field();
            IFieldEdit field_Shape_Edit = field_Shape as IFieldEdit;

            field_Shape_Edit.Name_2 = name;
            field_Shape_Edit.AliasName_2 = allias;
            field_Shape_Edit.IsNullable_2 = nullable;
            field_Shape_Edit.Type_2 = esriFieldType.esriFieldTypeBlob;
            field_Shape_Edit.Length_2 = length;

            (m_Fields as IFieldsEdit).AddField(field_Shape);
        }
        /// <summary>
        /// 向字段中添加一个日期类型字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="allias">字段别名</param>
        /// <param name="nullable">是否可为null</param>
        public void AddDateField(string name, string allias = "", bool nullable = true)
        {
            allias = string.IsNullOrEmpty(allias) ? name : allias;

            IField field_Shape = new Field();
            IFieldEdit field_Shape_Edit = field_Shape as IFieldEdit;

            field_Shape_Edit.Name_2 = name;
            field_Shape_Edit.AliasName_2 = allias;
            field_Shape_Edit.IsNullable_2 = nullable;
            field_Shape_Edit.Type_2 = esriFieldType.esriFieldTypeDate;

            (m_Fields as IFieldsEdit).AddField(field_Shape);
        }
        /// <summary>
        /// 重置字段组(该方法将清除原有字段组的所有字段信息)
        /// </summary>
        /// <param name="fields"></param>
        public void ResetFields(IFields fields)
        {
            if (m_Fields != null)
                (m_Fields as IFieldsEdit).DeleteAllFields();
            m_Fields = fields;
        }
        /// <summary>
        /// 获得字段组中的几何字段名
        /// </summary>
        /// <returns>若无几何字段则返回空字符串</returns>
        public string GetShapeFieldName()
        {
            for (int index = 0; index < m_Fields.FieldCount; index++)
            {
                if (m_Fields.get_Field(index).Type == esriFieldType.esriFieldTypeGeometry)
                    return  m_Fields.get_Field(index).Name;
            }
            return "";
        }

        /// <summary>
        /// 获得字段组中的OID段名
        /// </summary>
        /// <returns>若无几何字段则返回空字符串</returns>
        public string GetOIDFieldName()
        {
            for (int index = 0; index < m_Fields.FieldCount; index++)
            {
                if (m_Fields.get_Field(index).Type == esriFieldType.esriFieldTypeOID)
                    return m_Fields.get_Field(index).Name;
            }
            return "";
        }

        public ISpatialReference GetSpatialRef()
        {
            for (int index = 0; index < m_Fields.FieldCount; index++)
            {
                if (m_Fields.get_Field(index).Type == esriFieldType.esriFieldTypeGeometry)
                    return m_Fields.get_Field(index).GeometryDef.SpatialReference;
            }
            return null;
        }

        /// <summary>
        /// 检验字段合理性
        /// </summary>
        /// <param name="targetWorkspace">目标工作空间</param>
        /// <param name="isReset">是否使用检验结果重置目标字段</param>
        /// <returns>是否通过字段检验，若不通过则错误信息可通过ErrorMessage属性获取</returns>
        public bool ValidateFields(IWorkspace targetWorkspace,bool isReset)
        {
            IFieldChecker pFieldChecker = new FieldChecker();
            IEnumFieldError pEnumFieldError = null;
            IFields validateFilelds = null;
            pFieldChecker.ValidateWorkspace = targetWorkspace;
            pFieldChecker.Validate(m_Fields, out pEnumFieldError, out validateFilelds);

            if (pEnumFieldError != null)
            {
                IList<string> result = new List<string>();
                IFieldError error = pEnumFieldError.Next();
                while (error != null)
                {
                    result.Add(string.Format("字段 {0} 出错，原因为 :{1}", m_Fields.Field[error.FieldIndex].Name, error.FieldError.ToString()));
                    error = pEnumFieldError.Next();
                }
                ErrorMessage = string.Join(";", result);

                if (isReset)
                    ResetFields(validateFilelds);

                return false;
            }
            return true;
        }



    }
}
