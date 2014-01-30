using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Isometric.TEngine
{
    public struct Tile
    {
        /***************************************************************************************************
         *  attributes
         ***************************************************************************************************/

        /// <summary>
        /// the index for the tileTypeTextures
        /// </summary>
        public int typeIndex;

        /// <summary>
        /// the indices of textures which are supposed to be stacked on each other
        /// </summary>
        public List<int> indices;




        /***************************************************************************************************
         *  methods
         ***************************************************************************************************/

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeIndex">the tiles typeIndex</param>
        /// <param name="coordinates">the tiles coordinates on the map</param>
        public Tile(int typeIndex)
            : this(typeIndex, new List<int>())
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">the tiles worldposition</param>
        /// <param name="typeIndex">the tiles typeIndex</param>
        /// <param name="indices">the texture indices</param>
        public Tile(int typeIndex, List<int> indices)
        {
            this.typeIndex = typeIndex;
            this.indices = indices;
        }
    }
}
