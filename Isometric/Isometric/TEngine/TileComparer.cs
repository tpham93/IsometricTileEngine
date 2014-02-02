using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Isometric.TEngine
{
    class DrawingOrderComparer : IComparer<Point>
    {
        /// <summary>
        /// the size of the map
        /// </summary>
        Point mapSize;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapSize">the size of the map</param>
        public DrawingOrderComparer(Point mapSize)
        {
            this.mapSize = mapSize;
        }

        /// <summary>
        /// compare the two tileOverlay according to their drawing order
        /// </summary>
        /// <param name="coordinate1">the first tile you want to compare</param>
        /// <param name="tileOerlay2">the second tile you want to compare</param>
        /// <returns>
        /// a negative int if tileOverlay1 has to be drawn before tileOverlay2
        /// zero if the position are the same
        /// a positive int if tileOverlay2 ast do be drawn before tileOverlay2
        /// </returns>
        public int Compare(Point coordinate1, Point coordinate2)
        {
            int iX = (mapSize.X - 1 - coordinate1.X) + mapSize.X * coordinate1.Y;
            int iY = (mapSize.X - 1 - coordinate2.X) + mapSize.X * coordinate2.Y;
            return iX - iY;
        }
    }
}
