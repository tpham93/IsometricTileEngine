using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Isometric.TEngine
{
    struct TilePosition
    {
        /// <summary>
        /// the tile which should be drawn
        /// </summary>
        public Tile tile;

        /// <summary>
        /// the position where the tile should be drawn
        /// </summary>
        public Point position;

        /// <summary>
        /// the overlay of the tiles
        /// </summary>
        public Queue<TileOverlay> overlays;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tile">the tile which should be drawn</param>
        /// <param name="position">the position where the tile should be drawn</param>
        public TilePosition(Tile tile, Point position, Queue<TileOverlay> overlays)
        {
            this.tile = tile;
            this.position = position;
            this.overlays = overlays;
        }
    }
}
