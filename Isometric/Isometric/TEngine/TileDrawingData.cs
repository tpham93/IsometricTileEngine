using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Isometric.TEngine
{
    struct TileDrawingData
    {
        /// <summary>
        /// the tile which should be drawn
        /// </summary>
        public Tile tile;

        /// <summary>
        /// the position where the tile should be drawn
        /// </summary>
        public Point coordinates;

        /// <summary>
        /// the overlay of the tiles
        /// </summary>
        public Queue<TileOverlay> overlays;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tile">the tile which should be drawn</param>
        /// <param name="coordinates">the position where the tile should be drawn</param>
        public TileDrawingData(Tile tile, Point coordinates, Queue<TileOverlay> overlays)
        {
            this.tile = tile;
            this.coordinates = coordinates;
            this.overlays = overlays;
        }
    }
}
