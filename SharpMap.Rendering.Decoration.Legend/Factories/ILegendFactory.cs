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
using System.Drawing;

namespace SharpMap.Rendering.Decoration.Legend.Factories
{
    /// <summary>
    /// An interface for classes that can create legend <see cref="IMapDecoration"/>s.
    /// </summary>
    public interface ILegendFactory
    {
        /// <summary>
        /// Method to create a legend <see cref="IMapDecoration"/> for the provided map
        /// </summary>
        /// <param name="map">The map</param>
        /// <param name="settings"></param>
        /// <returns>A legend map decoration</returns>
        ILegend Create(Map map, ILegendSettings settings);

        /// <summary>
        /// Method to register a legend item factory
        /// </summary>
        /// <param name="itemFactory">The item factory to register</param>
        void Register(ILegendItemFactory itemFactory);

        /// <summary>
        /// Indexer for legend item factories
        /// </summary>
        /// <param name="item">The item to get a factory for</param>
        /// <returns>A factory to create a legend item</returns>
        ILegendItemFactory this[object item] { get; }

        /// <summary>
        /// Get or sets the default legend settings
        /// </summary>
        [Obsolete]
        ILegendSettings LegendSettings { get; set; }
    }
}