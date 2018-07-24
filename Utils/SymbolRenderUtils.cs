using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Utils
{
    public class SymbolRenderUtils
    {
        public static ISimpleMarkerSymbol CreateSimpleMarkSysbol(IColor color, float size, esriSimpleMarkerStyle markerstyle, IRgbColor outlineColor = null, float outlineSize = 0)
        {
            ISimpleMarkerSymbol markerSymbol = new SimpleMarkerSymbol();
            markerSymbol.Style = markerstyle;
            markerSymbol.Color = color;
            markerSymbol.Size = size;
            if (outlineColor != null)
            {
                markerSymbol.OutlineColor = outlineColor;
                markerSymbol.OutlineSize = outlineSize;
            }
            return markerSymbol;
        }

        public static ISimpleLineSymbol CreateSimpleLineSymbol(IColor color, float width, esriSimpleLineStyle linestyle)
        {
            ISimpleLineSymbol lineSymbol = new SimpleLineSymbol();
            lineSymbol.Color = color;
            lineSymbol.Style = linestyle;
            lineSymbol.Width = width;
            return lineSymbol;
        }

        public static ISimpleFillSymbol CreateSimpleBoundaryFillSymbol(IColor BouderyColor, float width, esriSimpleLineStyle linestyle)
        {
            ISimpleFillSymbol fillSymbol = new SimpleFillSymbol();
            fillSymbol.Style = esriSimpleFillStyle.esriSFSNull;
            fillSymbol.Outline = CreateSimpleLineSymbol(BouderyColor,width,linestyle);
            return fillSymbol;
        }

        public static IColor ConvertColor(Color color)
        { 
            RgbColor result = new RgbColor();
            result.Blue = color.B;
            result.Red = color.R;
            result.Green = color.G;
            result.Transparency = color.A;
            return result;
        }
    }
}
