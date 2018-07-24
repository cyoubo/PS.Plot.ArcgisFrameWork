using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Utils
{
    public class QueryUtils
    {
        public string ErrorMessage { get; protected set; }
        /// <summary>
        /// 根据查询图形在目标要素集中查找图形相同的要素
        /// 返回查询到的第一个要素id
        /// </summary>
        /// <param name="queryGeo">查询图形</param>
        /// <param name="targetClass">目标要素集</param>
        /// <param name="resultOId">查询结果oid</param>
        /// <returns>查询到的</returns>
        public int QuerySameGeoFetaure(IGeometry queryGeo, IFeatureClass targetClass, string whereclause = null)
        {
            int result = -1;
            if (queryGeo == null || targetClass == null)
                return result;

            //为了提高效率，通过点图形来进行空间查询，缩小范围后进行关系对比
            IPoint queryPoint = null;
            try
            {
                ISpatialFilter spatialFilter = new SpatialFilter()
                {
                    GeometryField = targetClass.ShapeFieldName,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                    Geometry = queryPoint ?? queryGeo
                };
                if (!String.IsNullOrEmpty(whereclause))
                    spatialFilter.WhereClause = whereclause;

                if (queryGeo.GeometryType == esriGeometryType.esriGeometryPoint)
                    queryPoint = queryGeo as IPoint;
                else if (queryGeo.GeometryType == esriGeometryType.esriGeometryPolyline)
                    queryPoint = (queryGeo as IPolyline).ToPoint;
                else if (queryGeo.GeometryType == esriGeometryType.esriGeometryPolygon)
                    queryPoint = (queryGeo as IPolygon).FromPoint;

                using (ComReleaser comreleaser = new ComReleaser())
                {
                    //进行空间查询
                    IRelationalOperator relationalOperator = queryGeo as IRelationalOperator;
                    IFeatureCursor featureCursor = targetClass.Search(spatialFilter, true);
                    IFeature target = featureCursor.NextFeature();

                    comreleaser.ManageLifetime(featureCursor);
                    comreleaser.ManageLifetime(target);

                    while (target != null && target.Shape != null)
                    {
                        if (relationalOperator != null)
                        {
                            //既包含又在内，说明完全相同
                            if (relationalOperator.Equals(target.Shape))
                            {
                                result = target.OID; break;
                            }
                        }
                        target = featureCursor.NextFeature();
                    }
                    featureCursor = null;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 根据查询图形在目标要素集中查找图形相同的要素
        /// 返回查询到的第一个要素id
        /// </summary>
        /// <param name="queryGeo">查询图形</param>
        /// <param name="targetClass">目标要素集</param>
        /// <param name="resultOIds">查询结果oid列表</param>
        /// <returns>查询到的</returns>
        public IList<int> QuerySameGeoFetaures(IGeometry queryGeo, IFeatureClass targetClass, string whereclause = null)
        {
            IList<int> result = new List<int>();
            if (queryGeo == null || targetClass == null)
                return result;
            
            IPoint queryPoint = null;
            try
            {
                ISpatialFilter spatialFilter = new SpatialFilter()
                {
                    GeometryField = targetClass.ShapeFieldName,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                    Geometry = queryPoint ?? queryGeo
                };
                if (!String.IsNullOrEmpty(whereclause))
                {
                    spatialFilter.WhereClause = whereclause;
                }

                if (queryGeo.GeometryType == esriGeometryType.esriGeometryPoint)
                    queryPoint = queryGeo as IPoint;
                else if (queryGeo.GeometryType == esriGeometryType.esriGeometryPolyline)
                    queryPoint = (queryGeo as IPolyline).FromPoint;
                else if (queryGeo.GeometryType == esriGeometryType.esriGeometryPolygon)
                    queryPoint = (queryGeo as IPolygon).FromPoint;

                //进行空间查询
                using (ComReleaser comreleaser = new ComReleaser())
                {
                    IRelationalOperator relationalOperator = queryGeo as IRelationalOperator;
                    IFeatureCursor featureCursor = targetClass.Search(spatialFilter, true);
                    IFeature target = featureCursor.NextFeature();
                    comreleaser.ManageLifetime(featureCursor);
                    comreleaser.ManageLifetime(target);
                    while (target != null && target.Shape != null)
                    {
                        if (relationalOperator != null)
                        {
                            //既包含又在内，说明完全相同
                            if (relationalOperator.Contains(target.Shape) && relationalOperator.Within(target.Shape))
                            {
                                result.Add(target.OID);
                            }
                        }
                        target = featureCursor.NextFeature();
                    }
                    
                    featureCursor = null;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
            return result;
        }

        public IList<T> QuerySingleFieldValue<T>(ITable targetTable, string QueryFieldName,string whereclause = null)
        {
            IList<T> result = new List<T>();
            
            int colIndex = -1;
            if((colIndex = targetTable.FindField(QueryFieldName)) ==-1)
                return result;

            IQueryFilter filter = new QueryFilter();
            if (string.IsNullOrEmpty(whereclause))
                filter.WhereClause = whereclause;
            filter.SubFields = QueryFieldName;


            using (ComReleaser releaser = new ComReleaser())
            {
                ICursor cursor = targetTable.Search(filter, false);
                IRow row = cursor.NextRow();
                while (row != null)
                {
                    result.Add(row.get_Value(colIndex));
                    row = cursor.NextRow();
                }
            }
            return result.Cast<T>().ToList();
        }
    }
}
