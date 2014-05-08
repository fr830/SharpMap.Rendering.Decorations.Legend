// Copyright 2014 - Felix Obermaier (www.ivv-aachen.de)
//
// This file is part of SharpMap.Rendering.Decoration.Legend.
// SharpMap.Rendering.Decoration.Legend is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap.Rendering.Decoration.Legend is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Drawing;
using SharpMap.Layers;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    /// <summary>
    /// Factory class to create a legend
    /// </summary>
    public class LegendFactory : ILegendFactory
    {
        private static readonly Dictionary<Type, ILegendItemFactory> LegendItemFactories =
            new Dictionary<Type, ILegendItemFactory>();
        
        /// <summary>
        /// The forecolor of the header fonts
        /// </summary>
        public Brush ForeColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default header font
        /// </summary>
        public Font HeaderFont { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default item font
        /// </summary>
        public Font ItemFont { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default indentation value
        /// </summary>
        public int Indentation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default padding between items
        /// </summary>
        public Size Padding { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating the default symbol size
        /// </summary>
        public Size SymbolSize { get; set; }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        public LegendFactory()
        {
            ForeColor = Brushes.DarkSlateBlue;
            HeaderFont = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Pixel);
            ItemFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
            SymbolSize = new Size(16, 16);
            Indentation = SymbolSize.Width;
            Padding = new Size(3, 3);
            
            Register(new DefaultLayerGroupLegendItemFactory());
            Register(new DefaultVectorLayerLegendItemFactory());
            Register(new DefaultVectorStyleLegendItemFactory());
            Register(new DefaultThemeLegendItemFactory());
            Register(new DefaultSymbolizerLegendItemFactory());
            Register(new DefaultSymbolizerLayerLegendItemFactory());
        }

        /// <summary>
        /// Method to create a legend <see cref="IMapDecoration"/> for the provided map
        /// </summary>
        /// <param name="map">The map</param>
        /// <returns>A legend map decoration</returns>
        public virtual ILegend Create(Map map)
        {
            var res = new Legend(map, this) {Root = {Label = "Map", LabelFont = HeaderFont, LabelBrush = ForeColor}};

            if (map.VariableLayers.Count > 0)
            {
                res.Root.SubItems.Add(Create(res, "Variable", map.VariableLayers));
            }

            if (map.Layers.Count > 0)
            {
                res.Root.SubItems.Add(Create(res, "Static", map.Layers));
            }

            if (map.BackgroundLayer.Count > 0)
            {
                res.Root.SubItems.Add(Create(res, "Background", map.BackgroundLayer));
            }

            res.BorderColor = Color.Tomato;
            res.BorderMargin = new Size(5, 5);
            res.BorderWidth = 2;
            res.RoundedEdges = true;
            res.BackgroundColor = Color.LightBlue;
            
            return res;
        }

        /// <summary>
        /// Indexer for legend item factories
        /// </summary>
        /// <param name="item">The item to get a factory for</param>
        /// <returns>A factory to create a legend item</returns>
        ILegendItemFactory ILegendFactory.this[object item]
        {
            get
            {
                var type = item.GetType();
                return LookUpFactory(type);
            }
        }

        /// <summary>
        /// Method to register a legend item factory
        /// </summary>
        /// <param name="itemFactory"></param>
        public void Register(ILegendItemFactory itemFactory)
        {
            foreach (var forType in itemFactory.ForType    )
            {
                LegendItemFactories[forType] = itemFactory;
            }
        }

        /// <summary>
        /// Method to create a legend item for a layer collection
        /// </summary>
        /// <param name="legend">The legend</param>
        /// <param name="title">The title</param>
        /// <param name="layerCollection">The layer collection</param>
        /// <returns>The created legend item</returns>
        protected virtual ILegendItem Create(ILegend legend, string title, LayerCollection layerCollection)
        {
            var res = new LegendItem
            {
                Indentation = Indentation,
                Label = title,
                LabelFont = HeaderFont,
                LabelBrush = ForeColor,
                Expanded = true,
                Padding = Padding,
            };

            for (var i = layerCollection.Count-1; i >= 0; i--)
            {
            	var layer = layerCollection[i];
            
                var item = Create(legend, layer);
            	res.SubItems.Add(item);
            }
            return res;
        }

        /// <summary>
        /// Method to create a legend item for a layer
        /// </summary>
        /// <param name="legend">The legend </param>
        /// <param name="layer">The layer to create an item for</param>
        /// <returns>A legend item</returns>
        protected virtual ILegendItem Create(ILegend legend, ILayer layer)
        {
            var factory = LookUpFactory(layer.GetType());
            if (factory != null)
                return factory.Create(legend, layer);

            return new LegendItem
            {
                Label = layer.LayerName, LabelFont = ItemFont, LabelBrush = ForeColor,
                Symbol = CreateGenericSymbol(layer),
                Exclude = !layer.Enabled,
                Indentation = Indentation,
                Padding = Padding,
                Expanded = true
            };
        }

        /// <summary>
        /// Method to create a generic symbol for the layer
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        private Image CreateGenericSymbol(ILayer layer)
        {
            if (SymbolSize == Size.Empty)
                return null;

            Image tmp;
            using (var m = new Map(new Size(10*SymbolSize.Width, 10*SymbolSize.Height)))
            {
                m.Layers.Add(layer);
                m.ZoomToExtents();
                tmp = m.GetMap();
                //tmp.Save("map.png");
                //System.Diagnostics.Process.Start("map.png");
                m.Layers.Remove(layer);
            }
            
           var res = new Bitmap(SymbolSize.Width, SymbolSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
           using (var g = Graphics.FromImage(res))
           {
           		g.DrawImage(tmp, new Rectangle(0, 0, SymbolSize.Width, SymbolSize.Height),
           	                 	 new Rectangle(0, 0, tmp.Width, tmp.Height), GraphicsUnit.Pixel);
           }
                   
            return res;
        }

        private static ILegendItemFactory LookUpFactory(Type t)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            ILegendItemFactory res = null;
            while (true)
            {
                if (t == null) break;

                if (LegendItemFactories.TryGetValue(t, out res))
                    break;

                foreach (var i in t.GetInterfaces())
                {
                    if (LegendItemFactories.TryGetValue(i, out res))
                        return res;
                }

                if (t == typeof (object))
                    break;

                var baseType = t.BaseType;
                t = baseType;
            }
            return res;
        }
    }
}