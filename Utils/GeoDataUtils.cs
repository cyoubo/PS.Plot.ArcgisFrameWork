using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Utils
{
    public enum EnumLoaclGeoDatabaseType
    {
        MDB, GDB, SHP
    }

    public class GeoDataUtils
    {
        public string ErrorMessage { get; protected set; }
        /// <summary>
        /// 判断数据库中是否存在与输入名相同的数据集
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="sName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsNameExist(IWorkspace pWorkspace, string sName, esriDatasetType type = esriDatasetType.esriDTFeatureClass)
        {
            bool IsNameExist = false;
            if ((pWorkspace as IWorkspace2) != null)
            {
                IsNameExist = (pWorkspace as IWorkspace2).get_NameExists(type, sName);
            }
            return IsNameExist;
        }

        public string ExtractDatasetName(string sSrcName)
        {
            string sTargetName = sSrcName;
            string[] pTempStrs = sTargetName.Split('.');
            if (pTempStrs != null && pTempStrs.Length == 2)
            {
                sTargetName = pTempStrs[1];
            }
            return sTargetName;
        }
        /// <summary>
        /// 改变数据集的别名
        /// </summary>
        /// <param name="objectClass"></param>
        /// <param name="sAlias"></param>
        public bool AlterDatasetAlias(IObjectClass objectClass, string sAlias)
        {
            bool result = true;
            try
            {
                (objectClass as IClassSchemaEdit).AlterAliasName(sAlias);
            }
            catch(Exception ex)
            {
                result = false;
                ErrorMessage = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 删除已存在的指定类型的数据集
        /// </summary>
        /// <param name="targetWorkspace"></param>
        /// <param name="datasetName"></param>
        /// <param name="type"></param>
        public void DeleteDataset(IWorkspace targetWorkspace, IDatasetName datasetName, esriDatasetType type = esriDatasetType.esriDTFeatureClass)
        {
            if (IsNameExist(targetWorkspace, datasetName.Name, type))
            {
                (targetWorkspace as IFeatureWorkspaceManage).DeleteByName(datasetName);
            }
        }

        public IWorkspace CreateInMemoryWorkspace()
        {
            Type factoryType = Type.GetTypeFromProgID( "esriDataSourcesGDB.InMemoryWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
            IWorkspaceName workspaceName = workspaceFactory.Create("", "MyWorkspace",null, 0);
            IName name = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)name.Open();
            return workspace;
        }

        public IWorkspace CreateOrOpenLoaclGeoDataBase(string path, EnumLoaclGeoDatabaseType type)
        {
            try
            {
                IWorkspaceFactory pTargetWsf = null;
                switch (type)
                {
                    case EnumLoaclGeoDatabaseType.MDB:
                        pTargetWsf = new AccessWorkspaceFactory();
                        break;
                    case EnumLoaclGeoDatabaseType.GDB:
                        pTargetWsf = new FileGDBWorkspaceFactory();
                        break;
                    case EnumLoaclGeoDatabaseType.SHP:
                        pTargetWsf = new ShapefileWorkspaceFactory();
                        break;
                }
                if (System.IO.Directory.Exists(path) || System.IO.File.Exists(path))
                {
                    if (pTargetWsf.IsWorkspace(path))
                        return pTargetWsf.OpenFromFile(path, 0);
                    else
                    {
                        ErrorMessage = "当前工作空间已经损坏";
                        return null;
                    }
                }
                else
                {
                    string sPath = System.IO.Path.GetDirectoryName(path);
                    string sName = System.IO.Path.GetFileNameWithoutExtension(path);
                    IWorkspaceName pWorkspaceName = pTargetWsf.Create(sPath, sName, null, 0);
                    return pTargetWsf.Open(pWorkspaceName.ConnectionProperties, 0);
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }

        public IWorkspace OpenLoaclGeoDataBase(string path, EnumLoaclGeoDatabaseType type)
        {
            try
            {
                IWorkspaceFactory pTargetWsf = null;
                switch (type)
                {
                    case EnumLoaclGeoDatabaseType.MDB:
                        pTargetWsf = new AccessWorkspaceFactory();
                        break;
                    case EnumLoaclGeoDatabaseType.GDB:
                        pTargetWsf = new FileGDBWorkspaceFactory();
                        break;
                    case EnumLoaclGeoDatabaseType.SHP:
                        pTargetWsf = new ShapefileWorkspaceFactory();
                        break;
                }
                return pTargetWsf.OpenFromFile(path, 0);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 设置空间索引
        /// </summary>
        /// <param name="pTargetFields"></param>
        /// <returns>SHAPE字段名</returns>
        public string AddSpatialIndex(IFields pTargetFields , Double gridOneSize = 0, Double gridTwoSize = 0, Double gridThreeSize = 0)
        {
            string sShapeFieldName = "";
            IGeometryDef pTargetGeometryDef = null;
            for (int i = 0; i < pTargetFields.FieldCount; i++)
            {
                IField pField = pTargetFields.get_Field(i);
                if (pField.Type == esriFieldType.esriFieldTypeGeometry)
                {
                    sShapeFieldName = pField.Name;
                    pTargetGeometryDef = pField.GeometryDef;
                    //Allow ArcGIS to determine a valid grid size for the data loaded
                    IGeometryDefEdit pGeometryDefEdit = pTargetGeometryDef as IGeometryDefEdit;
                    pGeometryDefEdit.GridCount_2 = 3;
                    pGeometryDefEdit.set_GridSize(0, gridOneSize);
                    pGeometryDefEdit.set_GridSize(1, gridTwoSize);
                    pGeometryDefEdit.set_GridSize(2, gridThreeSize);
                    pGeometryDefEdit.SpatialReference_2 = pField.GeometryDef.SpatialReference;
                    break;
                }
            }
            return sShapeFieldName;
        }

        /// <summary>
        /// 重建要素类空间索引
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="gridOneSize"></param>
        /// <param name="gridTwoSize"></param>
        /// <param name="gridThreeSize"></param>
        /// <returns></returns>
        public bool RebuildSpatialIndex(IFeatureClass pFeatureClass, Double gridOneSize = 0, Double gridTwoSize = 0, Double gridThreeSize = 0)
        {
            ISchemaLock pSchemaLock = pFeatureClass as ISchemaLock;
            try
            {
                pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                // Get an enumerator for indexes based on the shape field.
                IIndexes indexes = pFeatureClass.Indexes;
                String shapeFieldName = pFeatureClass.ShapeFieldName;
                IEnumIndex enumIndex = indexes.FindIndexesByFieldName(shapeFieldName);
                enumIndex.Reset();

                // Get the index based on the shape field (should only be one) and delete it.
                IIndex index = enumIndex.Next();
                if (index != null)
                {
                    pFeatureClass.DeleteIndex(index);
                }

                // Clone the shape field from the feature class.
                int shapeFieldIndex = pFeatureClass.FindField(shapeFieldName);
                IFields fields = pFeatureClass.Fields;
                IField sourceField = fields.get_Field(shapeFieldIndex);
                IClone sourceFieldClone = (IClone)sourceField;
                IClone targetFieldClone = sourceFieldClone.Clone();
                IField targetField = (IField)targetFieldClone;

                // Open the geometry definition from the cloned field and modify it.
                IGeometryDef geometryDef = targetField.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.GridCount_2 = 3;
                geometryDefEdit.set_GridSize(0, gridOneSize);
                geometryDefEdit.set_GridSize(1, gridTwoSize);
                geometryDefEdit.set_GridSize(2, gridThreeSize);

                // Create a spatial index and set the required attributes.
                IIndex newIndex = new Index();
                IIndexEdit newIndexEdit = (IIndexEdit)newIndex;
                newIndexEdit.Name_2 = String.Concat(shapeFieldName, "_Index");
                newIndexEdit.IsAscending_2 = true;
                newIndexEdit.IsUnique_2 = false;

                // Create a fields collection and assign it to the new index.
                IFields newIndexFields = new Fields();
                IFieldsEdit newIndexFieldsEdit = (IFieldsEdit)newIndexFields;
                newIndexFieldsEdit.AddField(targetField);
                newIndexEdit.Fields_2 = newIndexFields;

                // Add the spatial index back into the feature class.
                pFeatureClass.AddIndex(newIndex);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
        }

        public bool DeleteSpatialIndex(IFeatureClass featureClass)
        {
            bool result = true;
            ISchemaLock pSchemaLock = featureClass as ISchemaLock;
            try
            {
                pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                IIndexes indexes = featureClass.Indexes;
                String shapeFieldName = featureClass.ShapeFieldName;
                IEnumIndex enumIndex = indexes.FindIndexesByFieldName(shapeFieldName);
                enumIndex.Reset();
                IIndex index = enumIndex.Next();
                if (index != null)
                {
                    featureClass.DeleteIndex(index);
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
                result = false;
            }
            finally
            {
                pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
            return result;
        }

        /// <summary>
        /// 根据目标数据库特点，检查可以与目标库匹配的字段
        /// </summary>
        /// <param name="pSrcFC"></param>
        /// <param name="pTargetWks"></param>
        /// <param name="pTargetFields"></param>
        /// <param name="sShapeFieldName"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public bool CheckFields(IFeatureClass pSrcFC,IWorkspace pTargetWks,out IFields pTargetFields,out string sShapeFieldName)
        {
            bool result = CheckFields(pSrcFC, pTargetWks, out pTargetFields);
            sShapeFieldName = AddSpatialIndex(pTargetFields);
            return result;
        }

        /// <summary>
        /// 将源表的字段放在目标数据库环境中进行检查，生成新的字段
        /// </summary>
        /// <param name="pClass"></param>
        /// <param name="pTargetWks"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public bool CheckFields(IClass pClass, IWorkspace pTargetWks, out IFields ResultFields)
        {
            ResultFields = null;
            IFields sourceFields = pClass.Fields;
            string sTableName = (pClass as IDataset).Name;
            if (sTableName.IndexOf('.') > 0)
            {
                //面积字段
                string sAreaFieldName = sTableName + ".AREA";
                int iAreaIndex = sourceFields.FindField(sAreaFieldName);
                if (iAreaIndex >= 0)
                {
                    IFieldEdit fieldEdit = sourceFields.get_Field(iAreaIndex) as IFieldEdit;
                    fieldEdit.Name_2 = "AREA";
                    fieldEdit.AliasName_2 = "面积";
                }
                //长度字段
                string sLenFieldName = sTableName + ".LEN";
                int iLenIndex = sourceFields.FindField(sLenFieldName);
                if (iLenIndex >= 0)
                {
                    IFieldEdit fieldEdit = sourceFields.get_Field(iLenIndex) as IFieldEdit;
                    fieldEdit.Name_2 = "LEN";
                    fieldEdit.AliasName_2 = "长度";
                }
            }
            // 创建字段检查对象
            IFieldChecker pFieldChecker = new FieldChecker();
            pFieldChecker.InputWorkspace = (pClass as IDataset).Workspace;
            pFieldChecker.ValidateWorkspace = pTargetWks;
            // 验证字段
            IEnumFieldError pEnumFieldError = null;
            try
            {
                ErrorMessage = "";
                IList<string> tempError = new List<string>();
                pFieldChecker.Validate(sourceFields, out pEnumFieldError, out ResultFields);
                if (pEnumFieldError != null)
                {
                    IFieldError pFieldError = null;
                    while ((pFieldError = pEnumFieldError.Next()) != null)
                    {
                       tempError.Add(sourceFields.get_Field(pFieldError.FieldIndex).Name + GetFieldNameError(pFieldError.FieldError));
                    }
                }
                ErrorMessage = string.Join(",", tempError);
            }
            finally
            {
                if (pEnumFieldError != null)
                {
                    Marshal.ReleaseComObject(pEnumFieldError);
                }
                Marshal.ReleaseComObject(pFieldChecker);
            }
            return string.IsNullOrEmpty(ErrorMessage);
        }

        /// <summary>
        /// 字段命名错误转换为中文字符串
        /// </summary>
        /// <param name="errorType"></param>
        /// <returns></returns>
        public string GetFieldNameError(esriFieldNameErrorType errorType)
        {
            switch (errorType)
            {
                case esriFieldNameErrorType.esriSQLReservedWord: return "字段名为数据库保留关键字";
                case esriFieldNameErrorType.esriInvalidCharacter:return "字段名包含无效字符";
                case esriFieldNameErrorType.esriInvalidFieldNameLength: return "字段名超出长度限制";
                case esriFieldNameErrorType.esriDuplicatedFieldName: return "字段重名";
                default : return "未知异常";
            }
        }

        public IList<IFeatureClass> TravelWorkSpace4FeatureClass(IWorkspace space)
        {
            IList<IFeatureClass> result = new List<IFeatureClass>();
            using (ComReleaser releaser = new ComReleaser())
            {
                //遍历要素集中的要素类
                IEnumDataset dts = space.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                releaser.ManageLifetime(dts);
                IDataset dt = dts.Next();
                releaser.ManageLifetime(dt);
                while (dt != null)
                {
                    IEnumDataset fcs = dt.Subsets;
                    releaser.ManageLifetime(fcs);
                    IDataset fc = fcs.Next();
                    releaser.ManageLifetime(fc);
                    while (fc != null)
                    {
                        result.Add((fc as IFeatureClass));
                        fc = fcs.Next();
                    }
                    dt = dts.Next();
                }
                //遍历独立要素类
                dts = space.get_Datasets(esriDatasetType.esriDTFeatureClass);
                dt = dts.Next();
                while (dt != null)
                {
                    result.Add((dt as IFeatureClass));
                    dt = dts.Next();
                }
            }
            return result;
        }

        public IList<IFeatureClass> TravelWorkSpace4SingleFeatureClass(IWorkspace workspace)
        {
            IList<IFeatureClass> result = new List<IFeatureClass>();
            using (ComReleaser releaser = new ComReleaser())
            {
                IEnumDataset dts = workspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
                //releaser.ManageLifetime(dts);
                IDataset dt = dts.Next();
                //releaser.ManageLifetime(dt);
                while (dt != null)
                {
                    result.Add((dt as IFeatureClass));
                    dt = dts.Next();
                }
            }
            return result;
        }

        public IList<IFeatureClass> TravelWorkSpace4FeatureClass(IWorkspace workspace, string datasetName)
        {
             IList<IFeatureClass> result = new List<IFeatureClass>();
             using (ComReleaser releaser = new ComReleaser())
             {
                 //遍历要素集中的要素类
                 IEnumDataset dts = workspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                 //releaser.ManageLifetime(dts);
                 IDataset dt = dts.Next();
                 //releaser.ManageLifetime(dt);
                 while (dt != null)
                 {
                     if(dt.Name.Equals(datasetName))
                     {
                         IEnumDataset fcs = dt.Subsets;
                         //releaser.ManageLifetime(fcs);
                         IDataset fc = fcs.Next();
                         //releaser.ManageLifetime(fc);
                         while (fc != null)
                         {
                             result.Add((fc as IFeatureClass));
                             fc = fcs.Next();
                         }
                     }
                     dt = dts.Next();
                 }
             }
             return result;
        }

        public IList<IDataset> TravelWorkSpace4Dataset(IWorkspace space)
        {
            IList<IDataset> result = new List<IDataset>();
            using (ComReleaser releaser = new ComReleaser())
            {
                //遍历要素集中的要素类
                IEnumDataset dts = space.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                //releaser.ManageLifetime(dts);
                IDataset dt = dts.Next();
                //releaser.ManageLifetime(dt);
                while (dt != null)
                {
                    result.Add(dt);
                    dt = dts.Next();
                }
            }
            return result;
        }

        public IFeatureClass OpenFeatureClass(IWorkspace space, string FcName, string DatasetName)
        {
            if (string.IsNullOrEmpty(DatasetName))
            {
                //遍历要素集中的要素类
                IEnumDataset dts = space.get_Datasets(esriDatasetType.esriDTFeatureClass);
                //releaser.ManageLifetime(dts);
                IDataset dt = dts.Next();
                //releaser.ManageLifetime(dt);
                while (dt != null)
                {
                    if (dt.Name.Equals(FcName))
                        return dt as IFeatureClass;
                    dt = dts.Next();
                }
            }
            else
            {
                //遍历要素集中的要素类
                IEnumDataset dts = space.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                //releaser.ManageLifetime(dts);
                IDataset dt = dts.Next();
                //releaser.ManageLifetime(dt);
                while (dt != null)
                {
                    if (dt.Name.Equals(DatasetName))
                    {
                        IEnumDataset Fcs = dt.Subsets;
                        IDataset fc = Fcs.Next();
                        if (fc.Name.Equals(FcName))
                            return fc as IFeatureClass;
                        fc = Fcs.Next();
                    }
                    dt = dts.Next();
                }
            }
            return null;
        }

        public IFeatureClass OpenShapeFile(string shapefileFullPath)
        {
            IFeatureClass result = null;
            try
            {
                IWorkspaceFactory pTargetWsf = new ShapefileWorkspaceFactory();
                string sPath = System.IO.Path.GetDirectoryName(shapefileFullPath);
                string sName = System.IO.Path.GetFileNameWithoutExtension(shapefileFullPath);
                IFeatureWorkspace space = pTargetWsf.OpenFromFile(sPath, 0) as IFeatureWorkspace;
                result = space.OpenFeatureClass(sName);
            }
            catch (Exception ex)
            {
                ErrorMessage = "工作空间损坏: " + ex.Message;
            }
            return result;
        }
    }
}
