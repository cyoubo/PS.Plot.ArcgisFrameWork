using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PS.Plot.ArcgisFrameWork.Component.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Component.Template
{
    /// <summary>
    /// 简单要素类构建模板
    /// </summary>
    public abstract class SimpleFeatureClassTemplate
    {
        /// <summary>
        /// 自定义添加要素字段
        /// </summary>
        /// <param name="FcInfo">提供要素类名等基础信息，方法返回前需要确保ShapeFieldName参数被正确赋值</param>
        /// <param name="FieldsHelper">提供字段操作帮助</param>
        /// <returns>若字段添加成功则返回ture</returns>
        protected abstract bool OnAddField(FieldsHelper FieldsHelper, string FcName, string FcAlias = "");

        /// <summary>
        /// 字段组帮助类
        /// </summary>
        public FieldsHelper FieldsHelper { get; protected set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// 构建的结果要素类
        /// </summary>
        public IFeatureClass ResultFeatureClass { get; protected set; }

        public bool CreateOrOpenFeatureClass(IFeatureWorkspace targetWorkSpace, string FcName, string FcAlias = "")
        {
            if ((targetWorkSpace as IWorkspace2).get_NameExists(esriDatasetType.esriDTFeatureClass, FcName))
            {
                ResultFeatureClass = targetWorkSpace.OpenFeatureClass(FcName);
                return true;
            }
            else
                return CreateFeatureClass(targetWorkSpace, FcName, FcAlias);
        }
        /// <summary>
        /// 创建一个独立要素类
        /// </summary>
        /// <param name="targetWorkSpace">目标工作空间</param>
        /// <param name="FcName">要素类名</param>
        /// <param name="FcAlias">要素类别名</param>
        /// <returns>若创建成功则返回true</returns>
        public bool CreateFeatureClass(IFeatureWorkspace targetWorkSpace, string FcName,string FcAlias = "")
        {
            if ((targetWorkSpace as IWorkspace2).get_NameExists(esriDatasetType.esriDTFeatureClass, FcName))
            {
                ErrorMessage = "当前要素类已经存在";
                return false;
            }

            bool result = true;
            try
            {
                UID CLSID = new UID();
                CLSID.Value = "esriGeoDatabase.Feature";

                FieldsHelper = new FieldsHelper();
                if (OnAddField(FieldsHelper, FcName, FcAlias) == false)
                {
                    ErrorMessage = "添加字段失败 : "+ErrorMessage;
                    return false;
                }

                ResultFeatureClass = targetWorkSpace.CreateFeatureClass(FcName, FieldsHelper.Fields, CLSID, null, esriFeatureType.esriFTSimple, FieldsHelper.GetShapeFieldName(), null);

                if (string.IsNullOrEmpty(FcAlias) == false)
                {
                    IClassSchemaEdit pClassSchemaEdit = ResultFeatureClass as IClassSchemaEdit;
                    pClassSchemaEdit.AlterAliasName(FcAlias);
                }
            }
            catch (Exception ex)
            {
                result = false;
                ErrorMessage = ex.Message;
            }
            return result;
        }

        public bool CreateOrOpenDatasetFeatureClass(IFeatureWorkspace targetWorkSpace, string DatasetName, string FcName, string FcAlias = "")
        {
            IWorkspace2 space2 = (targetWorkSpace as IWorkspace2);
            if (space2.get_NameExists(esriDatasetType.esriDTFeatureDataset, DatasetName) && space2.get_NameExists(esriDatasetType.esriDTFeatureClass, FcName))
            {
                ResultFeatureClass = (targetWorkSpace.OpenFeatureDataset(DatasetName) as IFeatureClassContainer).get_ClassByName(FcName);
                return true;
            }
            else
                return CreateDatasetFeatureClass(targetWorkSpace, DatasetName, FcName, FcAlias);
        
        }
        /// <summary>
        /// 构建一个在要素集中的要素类
        /// </summary>
        /// <param name="targetWorkSpace">目标工作空间</param>
        /// <param name="DatasetName">要素集名</param>
        /// <param name="FcName">要素类名</param>
        /// <param name="FcAlias">要素类别名</param>
        /// <returns></returns>
        public bool CreateDatasetFeatureClass(IFeatureWorkspace targetWorkSpace, string DatasetName ,string FcName, string FcAlias = "")
        {
            bool result = true;
            try
            {

                FieldsHelper = new FieldsHelper();
                OnAddField(FieldsHelper, FcName, FcAlias);

                IFeatureDataset targetDataset = null;
                if ((targetWorkSpace as IWorkspace2).get_NameExists(esriDatasetType.esriDTFeatureDataset, DatasetName) == false)
                    targetDataset = targetWorkSpace.CreateFeatureDataset(DatasetName, FieldsHelper.GetSpatialRef());
                else
                    targetDataset = targetWorkSpace.OpenFeatureDataset(DatasetName);


                if ((targetDataset as IFeatureClassContainer).get_ClassByName(FcName) != null)
                {
                    ErrorMessage = "当前要素类已经存在";
                    return false;
                }

                UID CLSID = new UID();
                CLSID.Value = "esriGeoDatabase.Feature";

                ResultFeatureClass = targetDataset.CreateFeatureClass(FcName, FieldsHelper.Fields, CLSID, null, esriFeatureType.esriFTSimple, FieldsHelper.GetShapeFieldName(), null);

                if (string.IsNullOrEmpty(FcAlias) == false)
                {
                    IClassSchemaEdit pClassSchemaEdit = ResultFeatureClass as IClassSchemaEdit;
                    pClassSchemaEdit.AlterAliasName(FcAlias);
                }
            }
            catch (Exception ex)
            {
                result = false;
                ErrorMessage = ex.Message;
            }
            return result;
        }
    }
}
