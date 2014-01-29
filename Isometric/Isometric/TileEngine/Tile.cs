using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Isometric.TileEngine
{
    class Tile
    {
        /***************************************************************************************************
         *  static attributes
         ***************************************************************************************************/

        /// <summary>
        /// list of all types of tiles and their textures
        /// </summary>
        private static List<List<Texture2D>> tileTypeTextures;

        /// <summary>
        /// the size of the used window
        /// </summary>
        private static Vector2 windowSize;

        /// <summary>
        /// the size of the tiles
        /// </summary>
        private static Vector2 tileSize;

        /// <summary>
        /// the origin of the textures
        /// </summary>
        private static Vector2 textureOrigin;

        /// <summary>
        /// the offset of adjacent tiles used to draw
        /// </summary>
        private static Vector2 tileOffset;

        /// <summary>
        /// the offset of adjacent tiles used to draw
        /// </summary>
        private static float stackingTileOffset;



        /***************************************************************************************************
         *  static methods
         ***************************************************************************************************/

        public static void initialize(Vector2 windowSize, Vector2 tileSize, Vector2 textureOrigin, Vector2 tileOffset, float stackingTileOffset)
        {
            Tile.windowSize = windowSize;
            Tile.tileSize = tileSize;
            Tile.textureOrigin = textureOrigin;
            Tile.tileOffset = tileOffset;
            Tile.stackingTileOffset = stackingTileOffset;
            tileTypeTextures = new List<List<Texture2D>>();
        }

        /// <summary>
        /// adds a new list for a new type of tiles
        /// </summary>
        public static void addType()
        {
            tileTypeTextures.Add(new List<Texture2D>());
        }

        /// <summary>
        /// adds a new texture to the given type of tile
        /// </summary>
        /// <param name="texture">the texture which is to added to the type of tile</param>
        /// <param name="typeIndex">the index of tile</param>
        public static void addTexture(Texture2D texture, int typeIndex)
        {
            tileTypeTextures[typeIndex].Add(texture);
        }




        /***************************************************************************************************
         *  attributes
         ***************************************************************************************************/

        /// <summary>
        /// the index for the tileTypeTextures
        /// </summary>
        private int typeIndex;

        /// <summary>
        /// the indices of textures which are supposed to be stacked on each other
        /// </summary>
        private List<int> indices;

        /// <summary>
        /// the coordinates on the map of the tile
        /// </summary>
        private Point coordinates;




        /***************************************************************************************************
         *  methods
         ***************************************************************************************************/

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeIndex">the tiles typeIndex</param>
        /// <param name="coordinates">the tiles coordinates on the map</param>
        public Tile(int typeIndex, Point coordinates)
            : this(typeIndex, new List<int>(), coordinates)
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">the tiles worldposition</param>
        /// <param name="typeIndex">the tiles typeIndex</param>
        /// <param name="indices">the texture indices</param>
        public Tile(int typeIndex, List<int> indices, Point coordinates)
        {
            this.typeIndex = typeIndex;
            this.indices = indices;
            this.coordinates = coordinates;
        }


        /// <summary>
        /// drawing the tile onto the screen
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = new Vector2(tileSize.X / 2f,0) + new Vector2(coordinates.X * tileOffset.X, coordinates.Y * tileOffset.Y);
            if (coordinates.Y % 2 == 1)
            {
                pos += new Vector2(tileSize.X / 2f, 0);
            }
            foreach(int index in indices)
            {
                spriteBatch.Draw(tileTypeTextures[typeIndex][index],pos,null,Color.White,0f,textureOrigin,1,SpriteEffects.None,0f);
                pos.Y -= stackingTileOffset;
            }
        }
    }
}
