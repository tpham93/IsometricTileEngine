using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Isometric.TEngine
{
    class TileEngine
    {
        /*
         * attributes
         */

        /// <summary>
        /// the tiles managed by the engine
        /// </summary>
        private Tile[,] tiles;


        /// <summary>
        /// list of all types of tiles and their textures
        /// </summary>
        private List<List<Texture2D>> tileTypeTextures;

        /// <summary>
        /// the size of the tiles
        /// </summary>
        private Vector2 tileSize;

        /// <summary>
        /// the origin of the textures
        /// </summary>
        private Vector2 textureOrigin;

        /// <summary>
        /// the offset for each x
        /// </summary>
        private Vector2 tileOffset_X;

        /// <summary>
        /// the offset for each y
        /// </summary>
        private Vector2 tileOffset_Y;

        /// <summary>
        /// the offset of adjacent tiles used to draw
        /// </summary>
        private float stackingTileOffset;

        /*
         * properties
         */

        /// <summary>
        /// gets the tilematrix
        /// </summary>
        public Tile[,] Tiles
        {
            get { return tiles; }
        }

        /// <summary>
        /// gets the number of types
        /// </summary>
        public int TypeCount
        {
            get { return tileTypeTextures.Count; }
        }

        /*
         * methods
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dimension">worldsize</param>
        /// <param name="defaultTypeIndex">the index used for initialization of the tiles</param>
        public TileEngine(Point dimension)
        {
            tiles = new Tile[dimension.X, dimension.Y];
            for (int x = 0; x < dimension.X; ++x)
            {
                for (int y = 0; y < dimension.Y; ++y)
                {
                    tiles[x, y] = new Tile();
                }
            }
        }

        /// <summary>
        /// initializes the tileengine
        /// </summary>
        /// <param name="tileSize">the size of a normal tile texture</param>
        /// <param name="textureOrigin">the origin of the tiles</param>
        /// <param name="tileOffset_X">the offset for a tile relative to a neighbor on the x-axis</param>
        /// <param name="tileOffset_Y">the offset for a tile relative to a neighbor on the y-axis</param>
        /// <param name="stackingTileOffset">the offset used to stack tiles onto each other</param>
        public void initialize(Vector2 tileSize, Vector2 textureOrigin, Vector2 tileOffset_X, Vector2 tileOffset_Y, float stackingTileOffset)
        {
            this.tileSize = tileSize;
            this.textureOrigin = textureOrigin;
            this.tileOffset_X = tileOffset_X;
            this.tileOffset_Y = tileOffset_Y;
            this.stackingTileOffset = stackingTileOffset;
            this.tileTypeTextures = new List<List<Texture2D>>();
        }

        /// <summary>
        /// adds a new list for a new type of tiles
        /// </summary>
        public void addType()
        {
            this.tileTypeTextures.Add(new List<Texture2D>());
        }

        /// <summary>
        /// adds a new texture to the given type of tile
        /// </summary>
        /// <param name="texture">the texture which should be added to the type of tile</param>
        /// <param name="typeIndex">the index of tile</param>
        public void addTexture(Texture2D texture, int typeIndex)
        {
            this.tileTypeTextures[typeIndex].Add(texture);
        }

        /// <summary>
        /// add multiple textures in the same order to the given type of tile
        /// </summary>
        /// <param name="textures">textures which should be added to the type of tile</param>
        /// <param name="typeIndex">the index of tile</param>
        public void addTexture(Texture2D[] textures, int typeIndex)
        {
            foreach (Texture2D texture in textures)
            {
                addTexture(texture, typeIndex);
            }
        }

        /// <summary>
        /// drawing the tiles
        /// </summary>
        /// <param name="spriteBatch">spriteBatch used to draw the tiles onto the rendertarget</param>
        /// <param name="rotation">rotation of the world</param>
        public void draw(SpriteBatch spriteBatch, Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation._0_DEGREE:
                    drawTiles_0_Degree(spriteBatch);
                    break;
                case Rotation._90_DEGREE:
                    drawTiles_90_Degree(spriteBatch);
                    break;
                case Rotation._180_DEGREE:
                    drawTiles_180_Degree(spriteBatch);
                    break;
                case Rotation._270_DEGREE:
                    drawTiles_270_Degree(spriteBatch);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// drawing the tiles rotated by 0 degree
        /// </summary>
        /// <param name="spriteBatch">spriteBatch used to draw the tiles onto the rendertarget</param>
        private void drawTiles_0_Degree(SpriteBatch spriteBatch)
        {
            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    int t_x = x;
                    int t_y = y;
                        
                    drawTile(spriteBatch, tiles[t_x, t_y], new Point(x, y));
                }
            }
        }

        /// <summary>
        /// drawing the tiles rotated by 90 degree
        /// </summary>
        /// <param name="spriteBatch">spriteBatch used to draw the tiles onto the rendertarget</param>
        private void drawTiles_90_Degree(SpriteBatch spriteBatch)
        {
            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    int t_x = tiles.GetUpperBound(1) - y;
                    int t_y = tiles.GetUpperBound(0) - x;

                    drawTile(spriteBatch, tiles[t_x, t_y], new Point(x, y));
                }
            }
        }

        /// <summary>
        /// drawing the tiles rotated by 180 degree
        /// </summary>
        /// <param name="spriteBatch">spriteBatch used to draw the tiles onto the rendertarget</param>
        private void drawTiles_180_Degree(SpriteBatch spriteBatch)
        {
            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    int t_x = tiles.GetUpperBound(0) - x;
                    int t_y = tiles.GetUpperBound(1) - y;

                    drawTile(spriteBatch, tiles[t_x, t_y], new Point(x, y));
                }
            }
        }

        /// <summary>
        /// drawing the tiles rotated by 270 degree
        /// </summary>
        /// <param name="spriteBatch">spriteBatch used to draw the tiles onto the rendertarget</param>
        private void drawTiles_270_Degree(SpriteBatch spriteBatch)
        {
            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    int t_x = y;
                    int t_y = x;

                    drawTile(spriteBatch, tiles[t_x, t_y], new Point(x, y));
                }
            }
        }


        /// <summary>
        /// drawing the tile onto the screen
        /// </summary>
        /// <param name="spriteBatch">the spritebatch used to drawing this tile onto the rendertarget</param>
        /// <param name="indices">the world coordinates of the tile</param>
        public void drawTile(SpriteBatch spriteBatch, Tile tile, Point coordinates)
        {
            Vector2 pos = coordinates.X * tileOffset_X + coordinates.Y * tileOffset_Y;

            foreach (int index in tile.indices)
            {
                spriteBatch.Draw(tileTypeTextures[tile.typeIndex][index], pos, null, Color.White, 0f, textureOrigin, 1, SpriteEffects.None, 0f);
                pos.Y -= stackingTileOffset;
            }
        }
    }
}
