using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Maps.Controls;

namespace Posmotrim.TootFM.Common
{
    public class MapBinder
    {
        public static readonly DependencyProperty MapLayersProperty =
      DependencyProperty.RegisterAttached(
          "MapLayers",
          typeof(List<MapLayer>),
          typeof(MapBinder),
          new PropertyMetadata(new List<MapLayer>(), MapLayersPropertyChanged));



        public static void SetMapLayers(DependencyObject obj, Boolean value)
        {
            obj.SetValue(MapLayersProperty, value);
        }

        public static List<MapLayer> GetMapLayers(DependencyObject obj)
        {
            return (List<MapLayer>)obj.GetValue(MapLayersProperty);
        }

        public static void MapLayersPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var map = o as Map;
            if (map != null)
            {
                map.Layers.Clear();
                if (((List<MapLayer>)e.NewValue).Count > 0 && map.Layers.Count == 0)
                {
                     UpdateMap(map,(List<MapLayer>)e.NewValue);
                }
               
            }
        }

        public static readonly DependencyProperty MapLayersAddProperty =
      DependencyProperty.RegisterAttached(
          "MapLayersAdd",
          typeof(List<MapLayer>),
          typeof(MapBinder),
          new PropertyMetadata(new List<MapLayer>(), MapLayersAddPropertyChanged));



        public static void SetMapLayersAdd(DependencyObject obj, Boolean value)
        {
            obj.SetValue(MapLayersProperty, value);
        }

        public static List<MapLayer> GetMapLayersAdd(DependencyObject obj)
        {
            return (List<MapLayer>)obj.GetValue(MapLayersProperty);
        }

        public static void MapLayersAddPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var map = o as Map;
            if (map != null)
            {
                if (((List<MapLayer>)e.NewValue).Count > 0 )
                {
                   UpdateMap(map,(List<MapLayer>)e.NewValue);
                }

            }
        }

        private static readonly object safe = new object();

        private static void UpdateMap(Map map, IEnumerable<MapLayer> mapLayers)
        {
            lock (safe)
            {


                foreach (var mapLayer in mapLayers)
                
                {
                   
                        map.Layers.Add(mapLayer);
                   
                    
                }
            }
           
        }
    }
}