using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Utils
{
        /// <summary>
        /// 几何操作的工具类
        /// </summary>
        public class GeometryUtils
        {
            public String ErrorMessage { get; protected set; }
            /// <summary>
            /// 获得几何图形的重心坐标
            /// </summary>
            /// <param name="geometry">IEnvelope、IMultiPatch、IPolygon和IRing几何类型</param>
            /// <returns>若参数类型异常则返回null</returns>
            public IPoint GetGrivateCenter(IGeometry geometry)
            {
                if (geometry is IEnvelope || geometry is IMultiPatch || geometry is IPolygon || geometry is IRing)
                    return (geometry as IArea).Centroid;
                return null;
            }

            /// <summary>
            /// 获得两条多段线的交点集合，若不相交则返回null
            /// </summary>
            /// <param name="line1">多段线1</param>
            /// <param name="line2">多段线2</param>
            /// <returns>角点的集合</returns>
            public IPointCollection GetPolyLineIntersection(IPolyline line1, IPolyline line2)
            {
                ITopologicalOperator topoOperator = line1 as ITopologicalOperator;
                IGeometry geo = topoOperator.Intersect(line2, esriGeometryDimension.esriGeometry0Dimension);
                return geo.IsEmpty ? null : geo as IPointCollection;
            }
            /// <summary>
            /// 将几何图形转换为点集合
            /// </summary>
            /// <param name="geometry">待转换的几何图形</param>
            /// <returns>若计算转换失败则返回长度为0的列表</returns>
            public IList<IPoint> ConvertGeometryToPointSets(IGeometry geometry)
            {
                IList<IPoint> result = new List<IPoint>();
                IPointCollection pointCollections = geometry as IPointCollection;
                if (geometry is IPoint)
                {
                    result.Add(geometry as IPoint);
                }
                else
                {
                    for (int i = 0; i < pointCollections.PointCount; i++)
                    {
                        result.Add(pointCollections.get_Point(i));
                    }
                }
                return result;
            }
            /// <summary>
            /// 将要素集合和转换为点集合
            /// </summary>
            /// <param name="features">待转换的要素集合</param>
            /// <returns>若计算转换失败则返回长度为0的列表</returns>
            public IList<IPoint> ConvertFeaturesToPointSets(IList<IFeature> features)
            {
                IList<IPoint> result = new List<IPoint>();
                foreach (var tempFeature in features)
                {
                    if (tempFeature.Shape is IPoint)
                    {
                        result.Add(tempFeature.Shape as IPoint);
                    }
                    else
                    {
                        IPointCollection pointCollections = tempFeature.Shape as IPointCollection;
                        for (int i = 0; i < pointCollections.PointCount; i++)
                        {
                            result.Add(pointCollections.get_Point(i));
                        }
                    }
                }
                return result;
            }
            /// <summary>
            /// 判断点是否在多段线上
            /// </summary>
            /// <param name="pt">目标点</param>
            /// <param name="pl">目标线</param>
            /// <returns>如果包含则返回true</returns>
            public bool IsPointInPolyLine(IPoint pt, IPolyline pl)
            {
                IRelationalOperator relaOperator = pl as IRelationalOperator;
                return relaOperator.Contains(pt);
            }
            /// <summary>
            /// 判断点是否在多段面中
            /// </summary>
            /// <param name="pt">目标点</param>
            /// <param name="pl">目标面</param>
            /// <returns>如果包含则返回true</returns>
            public bool IsPointInPolyGon(IPoint pt, IPolygon pg)
            {
                IRelationalOperator relaOperator = pg as IRelationalOperator;
                return relaOperator.Contains(pt);
            }

            public string FormatSpatialDefinition(ISpatialReference spatialRef)
            {
                string result = "";
                try
                {
                    IESRISpatialReferenceGEN2 gen = spatialRef as IESRISpatialReferenceGEN2;
                    int Bytes;
                    gen.ExportToESRISpatialReference2(out result, out Bytes);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
                return result;
            }

            public ISpatialReference CreateSpatialReferenceFromDefinitionStr(string str)
            {
                ISpatialReference spatialRef = new UnknownCoordinateSystem() as ISpatialReference;
                try
                {
                    string tempFile = System.IO.Path.GetTempFileName();
                    string prjFile = System.IO.Path.ChangeExtension(tempFile, ".prj");
                    File.Move(tempFile, prjFile);//修改文件扩展名

                    using (FileStream fs = new FileStream(prjFile, FileMode.Open))
                    {
                        StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
                        sw.WriteLine(str);
                        sw.Flush();
                        sw.Close();

                        spatialRef = new SpatialReferenceEnvironment().CreateESRISpatialReferenceFromPRJFile(prjFile);

                        File.Delete(prjFile);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
                return spatialRef;
            }
        }

}
