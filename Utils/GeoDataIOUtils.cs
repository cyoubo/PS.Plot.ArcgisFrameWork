using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Utils
{
    public class GeoDataIOUtils
    {
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// 依据Workspace.xml文件创建要素类
        /// </summary>
        /// <param name="space">目标工作空间</param>
        /// <param name="xmlpath">workspace类型的xmlw文件</param>
        /// <param name="IsContainsData">创建时，是否导入xml文件中的要素数据</param>
        /// <returns></returns>
        public bool ImportWorkspaceXML(IWorkspace space, string xmlpath, bool IsContainsData = false)
        {
            bool result = false;
            try
            {
                IEnumNameMapping namemapping = null;
                GdbImporter importer = new GdbImporter();
                importer.GenerateNameMapping(xmlpath, space, out namemapping);
                importer.ImportWorkspace(xmlpath, namemapping, space, !IsContainsData);
                result = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return result;
        }
        /// <summary>
        /// 将指定要素类复制到目标数据库中（在目标数据库中创建要素类）
        /// </summary>
        /// <param name="pSrcFC">源要素类</param>
        /// <param name="pQueryFilter">查询条件</param>
        /// <param name="pTargetWks">目标数据库</param>
        /// <param name="sMessage">转换消息</param>
        /// <param name="sTargetName">目标要素类名</param>
        /// <param name="sAlias">目标要素类别名</param>
        /// <param name="sFDatasetName">目标要素数据集</param>
        /// <returns></returns>
        public bool TranslateFeatureClass(IFeatureClass pSrcFC, IQueryFilter pQueryFilter, IWorkspace pTargetWks, string sTargetName = null, string sAlias = null, string sFDatasetName = null)
        {
            GeoDataUtils geoDataUtils = new GeoDataUtils();
            
            IDataset pSrcDataset = pSrcFC as IDataset;
            IFeatureClassName pSrcName = pSrcDataset.FullName as IFeatureClassName;
            IFeatureDatasetName pFeatureDatasetName = null;
            if (pTargetWks.Type != esriWorkspaceType.esriFileSystemWorkspace)
            {
                string sFeatureDatasetName = sFDatasetName;
                if (string.IsNullOrEmpty(sFeatureDatasetName) && pSrcFC.FeatureDataset != null)
                {
                    sFeatureDatasetName = geoDataUtils.ExtractDatasetName(pSrcFC.FeatureDataset.Name);
                }
                if (!string.IsNullOrEmpty(sFeatureDatasetName))
                {
                    if (geoDataUtils.IsNameExist(pTargetWks, sFeatureDatasetName, esriDatasetType.esriDTFeatureDataset))
                    {
                        IFeatureDataset pTargetFeatureDataset = (pTargetWks as IFeatureWorkspace).OpenFeatureDataset(sFeatureDatasetName);
                        pFeatureDatasetName = pTargetFeatureDataset.FullName as IFeatureDatasetName;
                        Marshal.ReleaseComObject(pTargetFeatureDataset);
                    }
                    else
                    {
                        pFeatureDatasetName = new FeatureDatasetName() as IFeatureDatasetName;
                        IDatasetName pDatasetName = pFeatureDatasetName as IDatasetName;
                        pDatasetName.Name = sFeatureDatasetName;
                        pDatasetName.WorkspaceName = (pTargetWks as IDataset).FullName as IWorkspaceName;
                    }
                }
            }

            //目标要素类名
            FeatureClassName pTargetName = new FeatureClassName();
            IDatasetName pTargetDatasetName = pTargetName as IDatasetName;
            if (string.IsNullOrEmpty(sTargetName))
            {
                sTargetName = geoDataUtils.ExtractDatasetName(pSrcDataset.Name);
            }
            pTargetDatasetName.Name = sTargetName;
            pTargetDatasetName.WorkspaceName = (pTargetWks as IDataset).FullName as IWorkspaceName;
            geoDataUtils.DeleteDataset(pTargetWks, pTargetDatasetName);

            // 验证字段
            IFields pTargetFields;
            string sShapeFieldName;
            geoDataUtils.CheckFields(pSrcFC, pTargetWks, out pTargetFields, out sShapeFieldName);
            IEnumInvalidObject pEnumInvalidObject = null;
            IFeatureDataConverter pConverter = null;
            try
            {
                int iIndex = pTargetFields.FindField(sShapeFieldName);
                IGeometryDef pTargetGeometryDef = pTargetFields.get_Field(iIndex).GeometryDef;

                // 执行批量转换
                pConverter = new FeatureDataConverter();
                pEnumInvalidObject = pConverter.ConvertFeatureClass(pSrcName, pQueryFilter, pFeatureDatasetName as IFeatureDatasetName,
                    pTargetName as IFeatureClassName, pTargetGeometryDef, pTargetFields, "", 1000, 0);

                // 检查转换过程中是否产生错误
                if (pEnumInvalidObject != null)
                {
                    IInvalidObjectInfo pInvalidInfo = null;
                    pEnumInvalidObject.Reset();
                    while ((pInvalidInfo = pEnumInvalidObject.Next()) != null)
                    {
                        ErrorMessage += pSrcFC.AliasName + "图层要素：" + pInvalidInfo.InvalidObjectID + "执行失败！";
                    }
                }
            }
            finally //释放资源
            {
                if (pEnumInvalidObject != null)
                {
                    Marshal.ReleaseComObject(pEnumInvalidObject);
                }
                if (pConverter != null)
                {
                    Marshal.ReleaseComObject(pConverter);
                }
            }
            //修改要素类别名
            if (string.IsNullOrEmpty(sAlias))
            {
                sAlias = pSrcFC.AliasName;
            }
            IFeatureClass pTargetFC = (pTargetWks as IFeatureWorkspace).OpenFeatureClass(sTargetName);
            geoDataUtils.AlterDatasetAlias(pTargetFC, sAlias);
            Marshal.ReleaseComObject(pTargetFC);
            return true;
        }
    }
}
