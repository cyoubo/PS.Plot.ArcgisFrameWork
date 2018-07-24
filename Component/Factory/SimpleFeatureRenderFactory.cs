using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using PS.Plot.ArcgisFrameWork.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Component.Factory
{
    /// <summary>
    /// 常用简单要素样式
    /// </summary>
   public enum EnumSimpleSymbol
    {
        Point_Red_Circle,
        Ployline_Red_Solid,
        Ploygon_Red_Boundary,
        Point_Green_Circle,
        Ployline_Green_Solid,
        Ploygon_Green_Boundaryd,
        Point_Blue_Circle,
        Ployline_Blue_Solid,
        Ploygon_Blue_Boundary,
        Point_Yellow_Circle,
        Ployline_Yellow_Solid,
        Ploygon_Yellow_Boundary
    }
    /// <summary>
    /// 简单要素渲染器工厂
    /// </summary>
    public class SimpleFeatureRenderFactory
    {
        /// <summary>
        /// 利用常用简单要素样式枚举常量，创建简单要素渲染器
        /// </summary>
        /// <param name="symbol">常用简单要素样式</param>
        /// <param name="width">渲染样式大小(点大小、线宽度、面外框宽度)</param>
        /// <returns>简单要素渲染器</returns>
        public IFeatureRenderer CreateSimpleFeatureRenderer(EnumSimpleSymbol symbol, int width = 5)
        {
            ISimpleRenderer renderer = new SimpleRenderer();
            renderer.Symbol = new SimpleSymbolFactory().CreateSimpleSymbol(symbol, width);
            return renderer as IFeatureRenderer;
        }
        /// <summary>
        /// 利用自定义样式样式枚举常量，创建简单要素渲染器
        /// </summary>
        /// <param name="symbol">自定义要素样式</param>
        /// <param name="Label">渲染器标签</param>
        /// <returns>简单要素渲染器</returns>
        public IFeatureRenderer CreateSimpleFeatureRenderer(ISymbol symbol, String Label = null)
        {
            ISimpleRenderer renderer = new SimpleRenderer();
            renderer.Symbol = symbol;
            if (Label != null)
                renderer.Label = Label;
            return renderer as IFeatureRenderer;
        }
    }

    /// <summary>
    /// 常用简单要素样式工厂
    /// </summary>
    public class SimpleSymbolFactory
    {
        /// <summary>
        /// 创建常用简单要素样式
        /// </summary>
        /// <param name="symbol">常用简单要素样式枚举</param>
        /// <param name="width">渲染样式大小(点大小、线宽度、面外框宽度)</param>
        /// <returns></returns>
        public ISymbol CreateSimpleSymbol(EnumSimpleSymbol symbol,int width = 5)
        {
            switch (symbol)
            {
                case EnumSimpleSymbol.Point_Red_Circle:
                    return SymbolRenderUtils.CreateSimpleMarkSysbol(SymbolRenderUtils.ConvertColor(Color.Red), width, esriSimpleMarkerStyle.esriSMSCircle) as ISymbol;
                case EnumSimpleSymbol.Ployline_Red_Solid:
                    return SymbolRenderUtils.CreateSimpleLineSymbol(SymbolRenderUtils.ConvertColor(Color.Red), width, esriSimpleLineStyle.esriSLSSolid) as ISymbol;
                case EnumSimpleSymbol.Ploygon_Red_Boundary:
                    return SymbolRenderUtils.CreateSimpleBoundaryFillSymbol(SymbolRenderUtils.ConvertColor(Color.Red), width, esriSimpleLineStyle.esriSLSSolid) as ISymbol;
                case EnumSimpleSymbol.Point_Green_Circle:
                    return SymbolRenderUtils.CreateSimpleMarkSysbol(SymbolRenderUtils.ConvertColor(Color.Green), width ,esriSimpleMarkerStyle.esriSMSCircle) as ISymbol;;
                case EnumSimpleSymbol.Ployline_Green_Solid:
                    return SymbolRenderUtils.CreateSimpleLineSymbol(SymbolRenderUtils.ConvertColor(Color.Green), width, esriSimpleLineStyle.esriSLSSolid) as ISymbol;
                case EnumSimpleSymbol.Ploygon_Green_Boundaryd:
                    return SymbolRenderUtils.CreateSimpleBoundaryFillSymbol(SymbolRenderUtils.ConvertColor(Color.Green),width,esriSimpleLineStyle.esriSLSSolid) as ISymbol;;
                case EnumSimpleSymbol.Point_Blue_Circle:
                    return SymbolRenderUtils.CreateSimpleMarkSysbol(SymbolRenderUtils.ConvertColor(Color.Blue), width ,esriSimpleMarkerStyle.esriSMSCircle) as ISymbol;;
                case EnumSimpleSymbol.Ployline_Blue_Solid:
                    return SymbolRenderUtils.CreateSimpleLineSymbol(SymbolRenderUtils.ConvertColor(Color.Blue), width, esriSimpleLineStyle.esriSLSSolid) as ISymbol;
                case EnumSimpleSymbol.Ploygon_Blue_Boundary:
                    return SymbolRenderUtils.CreateSimpleBoundaryFillSymbol(SymbolRenderUtils.ConvertColor(Color.Blue),width,esriSimpleLineStyle.esriSLSSolid) as ISymbol;;
                case EnumSimpleSymbol.Point_Yellow_Circle:
                    return SymbolRenderUtils.CreateSimpleMarkSysbol(SymbolRenderUtils.ConvertColor(Color.Yellow), width ,esriSimpleMarkerStyle.esriSMSCircle) as ISymbol;;
                case EnumSimpleSymbol.Ployline_Yellow_Solid:
                    return SymbolRenderUtils.CreateSimpleLineSymbol(SymbolRenderUtils.ConvertColor(Color.Yellow), width, esriSimpleLineStyle.esriSLSSolid) as ISymbol;
                case EnumSimpleSymbol.Ploygon_Yellow_Boundary:
                    return SymbolRenderUtils.CreateSimpleBoundaryFillSymbol(SymbolRenderUtils.ConvertColor(Color.Yellow),width,esriSimpleLineStyle.esriSLSSolid) as ISymbol;;
                default:
                    return null;
            }
        }

        
    }
}
