using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Component
{
    public class FeatureAttributeAdapter
    {
        protected IFeatureClass TargetFeatureClass;

        public IList<string> IngoreColNames {get;set;}

        public DataTable ResultTable { protected set; get; }


        public void Initial(IFeatureClass targetFeatureClass)
        {
            this.TargetFeatureClass = targetFeatureClass;
            if (IngoreColNames != null)
                IngoreColNames.Clear();
            IngoreColNames = new List<string>();
        }

        public void CreateAllRecordTable(bool ColNameUseAlias = false)
        {
            NotifyDestoryResultTable();
            CreateNullRecordTable(ColNameUseAlias);
            ISet<int> IngoreFieldIndex = ConvertIngoreColName2FieldIndex();
            using (ComReleaser comreleaser = new ComReleaser())
            {
                IFeatureCursor cursor = TargetFeatureClass.Search(null, false);
                comreleaser.ManageLifetime(cursor);
                IFeature tempFt = cursor.NextFeature();
                comreleaser.ManageLifetime(tempFt);
                while (tempFt != null)
                {
                    int colIndex = 0;
                    DataRow tempRow = ResultTable.NewRow();
                    for (int index = 0; index < TargetFeatureClass.Fields.FieldCount; index++)
                    {
                        if (IngoreFieldIndex.Contains(index))
                            continue;
                        tempRow[colIndex++] = tempFt.get_Value(index);
                    }
                    ResultTable.Rows.Add(tempRow);
                    tempFt = cursor.NextFeature();
                }
            }
            IngoreFieldIndex.Clear();
        }

        public void  CreateNullRecordTable(bool ColNameUseAlias = false)
        {
            ResultTable = new DataTable();
            for (int index = 0; index < TargetFeatureClass.Fields.FieldCount; index++)
            {
                IField temp = TargetFeatureClass.Fields.Field[index];
                if (IngoreColNames.Contains(temp.Name) == false || IngoreColNames.Contains(temp.AliasName) == false)
                {
                    if (ColNameUseAlias)
                        ResultTable.Columns.Add(new DataColumn(temp.AliasName, typeof(object)));
                    else
                        ResultTable.Columns.Add(new DataColumn(temp.Name, typeof(object)));
                }
            }
        }

        public ISet<int> ConvertIngoreColName2FieldIndex()
        {
            ISet<int> IngoreFieldIndex = new HashSet<int>();
            for (int index = 0; index < TargetFeatureClass.Fields.FieldCount; index++)
            {
                IField temp = TargetFeatureClass.Fields.Field[index];
                if (IngoreColNames.Contains(temp.Name) || IngoreColNames.Contains(temp.AliasName))
                    IngoreFieldIndex.Add(index);
            }
            return IngoreFieldIndex;
        }

        public void NotifyClearResultTable()
        {
            if(ResultTable!=null)
                ResultTable.Rows.Clear();
        }

        public void NotifyDestoryResultTable()
        {
            if (ResultTable != null)
            {
                NotifyClearResultTable();
                ResultTable.Columns.Clear();
            }
        }
    }
}
