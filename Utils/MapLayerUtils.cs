using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Plot.ArcgisFrameWork.Utils
{
    public class MapLayerUtils
    {
        public static void RemoveLayerByName(IMap map, string Name)
        {
            IEnumLayer layers = map.get_Layers();
            ILayer layer = layers.Next();
            while (layer != null)
            {
                if(layer.Name.Equals(Name))
                    map.DeleteLayer(layer);
                layer = layers.Next();
            }
        }

        public static void ClearEmptyGroupLayer(IMap map)
        {
            IEnumLayer layers = map.get_Layers();
            ILayer layer = layers.Next();
            while (layer != null)
            {
                if (layer is IGroupLayer)
                {
                    onClearEmptyGroupLayer(map, layer as IGroupLayer);
                }
                layer = layers.Next();
            }
        }

        private static void onClearEmptyGroupLayer(IMap map, IGroupLayer grouplayer)
        {
            if ((grouplayer as ICompositeLayer).Count == 0)
                map.DeleteLayer(grouplayer);
            else
            {
                for (int index = 0; index < (grouplayer as ICompositeLayer).Count; index++)
                {
                    ILayer temp = (grouplayer as ICompositeLayer).get_Layer(index);
                    if(temp is IGroupLayer)
                    {
                        onClearEmptyGroupLayer(map, temp as IGroupLayer);
                    }
                }
            }
        }

        public static void RemoveFeatureLayerByFeatureClassName(IMap map, string FcName)
        {
            IEnumLayer layers = map.get_Layers();
            ILayer layer = layers.Next();
            while (layer != null)
            {
                if (layer is IFeatureLayer && ((layer as IFeatureLayer).FeatureClass as IDataset).Name.Equals(FcName))
                {
                    map.DeleteLayer(layer);
                }
                layer = layers.Next();
            }
        }

        public static void RemoveFeatureLayerByFeatureClassAlias(IMap map, string FcAlias)
        {
            IEnumLayer layers = map.get_Layers();
            ILayer layer = layers.Next();
            while (layer != null)
            {
                if (layer is IFeatureLayer && (layer as IFeatureLayer).FeatureClass.AliasName.Equals(FcAlias))
                {
                    map.DeleteLayer(layer);
                }
                layer = layers.Next();
            }
        }
    }
}
