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
    public class FeatureAttributeAdapter2 : FeatureAttributeAdapter
    {
        public int PageContains { get; protected set; }
        public int FeatureCount { get; protected set; }
        public int CurrentPage { get; protected set; }
        public int PageCount { get; protected set; }

        public void CalSplitPageParams(int PageContains)
        {
            FeatureCount = TargetFeatureClass.FeatureCount(null);
            PageCount = FeatureCount / PageContains + 1;
            CurrentPage = 1;

            CreateNullRecordTable();
        }

        public void RefreshCurrentPageDataTable()
        {
            using (ComReleaser comreleaser = new ComReleaser())
            {
                IFeatureCursor cursor = TargetFeatureClass.Search(null, false);
                comreleaser.ManageLifetime(cursor);
                IFeature tempFt = cursor.NextFeature();
                comreleaser.ManageLifetime(tempFt);
                ISet<int> IngoreFieldIndex = ConvertIngoreColName2FieldIndex();
                for (int index = 0; index < FeatureCount; index++)
                {
                    if (index < CurrentPage * PageContains)
                    {
                        tempFt = cursor.NextFeature();
                    }
                    else if (index > (CurrentPage + 1) * PageContains)
                        break;
                    else
                    {
                        int colIndex = 0;
                        DataRow tempRow = ResultTable.NewRow();
                        for (int i = 0; i < TargetFeatureClass.Fields.FieldCount; i++)
                        {
                            if (IngoreFieldIndex.Contains(i))
                                continue;
                            tempRow[colIndex++] = tempFt.get_Value(index);
                        }
                    }
                }
            }
        }

        public void PageAfter()
        {
            if (CurrentPage + 1 <= PageCount)
                CurrentPage++;
        }

        public void PagePreview()
        {
            if (CurrentPage - 1 >= 1)
                CurrentPage--;
        }
    }
}
