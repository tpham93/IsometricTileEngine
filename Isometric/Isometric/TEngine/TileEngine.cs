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
        /***************************************************************************************************
         *  attributes
         ***************************************************************************************************/

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




        /***************************************************************************************************
         *  properties
         ***************************************************************************************************/

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

        /// <summary>
        /// gets the size of the map
        /// </summary>
        public Point MapSize
        {
            get { return new Point(tiles.GetUpperBound(0) + 1, tiles.GetUpperBound(1) + 1); }
        }




        /***************************************************************************************************
         *  public methods
         ***************************************************************************************************/

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dimension">worldsize</param>
        /// <param name="defaultTypeIndex">the index used for initialization of the tiles</param>
        public TileEngine(Point dimension)
            : this(dimension, -1)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dimension">worldsize</param>
        /// <param name="defaultTypeIndex">the index used for initialization of the tiles</param>
        public TileEngine(Point dimension, int standartType)
        {
            tiles = new Tile[dimension.X, dimension.Y];
            for (int x = 0; x < dimension.X; ++x)
            {
                for (int y = 0; y < dimension.Y; ++y)
                {
                    tiles[x, y] = new Tile(standartType);
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
        /// removes a tile type
        /// </summary>
        /// <param name="index">the index of the type</param>
        public void removeType(int index)
        {
            this.tileTypeTextures.RemoveAt(index);
        }

        /// <summary>
        /// adds a new texture to the given type of tile
        /// </summary>
        /// <param name="textures">textures you want to add to the type of tile</param>
        /// <param name="typeIndex">the index of tile</param>
        public void addTexture(Texture2D texture, int typeIndex)
        {
            this.tileTypeTextures[typeIndex].Add(texture);
        }

        /// <summary>
        /// add multiple textures in the same order to the given type of tile
        /// </summary>
        /// <param name="textures">textures you want to add to the type of tile</param>
        /// <param name="typeIndex">the index of tile</param>
        public void addTexture(Texture2D[] textures, int typeIndex)
        {
            foreach (Texture2D texture in textures)
            {
                this.addTexture(texture, typeIndex);
            }
        }



        /// <summary>
        /// drawing the tiles
        /// </summary>
        /// <param name="spriteBatch">the spritebatch which you want to use to draw the tiles onto the rendertarget</param>
        /// <param name="screenOffset">the translation added to the spritebatch after scaling</param>
        /// <param name="screen">the screen with its position and size</param>
        /// <param name="scale">the scale used to draw</param>
        /// <param name="rotation">rotation of the world</param>
        /// <param name="overlays">the overlays which should be drawn onto the tiles</param>
        public void draw(SpriteBatch spriteBatch, Vector2 screenOffset, Rectangle screen, float scale, Rotation rotation, List<TileOverlay> overlays = null)
        {
            DrawingOrderComparer drawingOrderComparer = new DrawingOrderComparer(MapSize);

            Queue<TileOverlay> sortedOverlays = new Queue<TileOverlay>();

            if (overlays != null)
            {
                Point mapSize = MapSize;
                foreach (TileOverlay overlay in overlays)
                {
                    if (overlay.Position.X >= 0 && overlay.Position.Y >= 0 && overlay.Position.X < mapSize.X && overlay.Position.Y < mapSize.Y)
                    {
                        sortedOverlays.Enqueue(new TileOverlay(getRotatedCoordinates(overlay.Position, rotation), overlay.Texture, overlay.Origin, overlay.Color));
                    }
                }
                sortedOverlays = new Queue<TileOverlay>(sortedOverlays.OrderBy(overlay => overlay.Position, drawingOrderComparer));
            }

            int maxSize = Math.Max(tiles.GetUpperBound(0), tiles.GetUpperBound(1));

            Queue<TilePosition> drawingTiles = new Queue<TilePosition>(maxSize * maxSize);

            screen = new Rectangle((int)(screen.X * scale) - (int)screenOffset.X, (int)(screen.Y * scale) - (int)screenOffset.Y, (int)(screen.Width), (int)(screen.Height));

            for (int y = 0; y <= maxSize; ++y)
            {
                for (int x = maxSize; x >= 0; --x)
                {
                    Point coordinates = new Point(x, y);
                    Point tileCoordinates = getRotatedCoordinates(new Point(x, y), inverseRotation(rotation));
                    int t_x = tileCoordinates.X;
                    int t_y = tileCoordinates.Y;
                    if (t_x >= 0 && t_y >= 0 && t_x <= tiles.GetUpperBound(0) && t_y <= tiles.GetUpperBound(1))
                    {
                        Vector2 tileTopOffset = new Vector2(0,- getTileTopOffset(tiles[t_x, t_y]).Y);
                        Vector2 position = (getTilePosition(new Point(x, y)) - textureOrigin - tileTopOffset) * scale;
                        Rectangle tileRect = new Rectangle((int)position.X, (int)position.Y, (int)(tileSize.X * scale), (int)(( tileSize.Y + tileTopOffset.Y) * scale));
                        if (screen.Intersects(tileRect))
                        {
                            Queue<TileOverlay> tileOverlays = new Queue<TileOverlay>();

                            while (sortedOverlays.Count > 0 && drawingOrderComparer.Compare(sortedOverlays.Peek().Position, coordinates) <= 0)
                            {
                                TileOverlay overlay = sortedOverlays.Dequeue();
                                if (overlay.Position == coordinates)
                                    tileOverlays.Enqueue(overlay);
                            }
                            drawingTiles.Enqueue(new TilePosition(tiles[t_x, t_y], coordinates, tileOverlays));
                        }
                    }
                }
            }
            
            while (drawingTiles.Count > 0)
            {
                TilePosition tilePosition = drawingTiles.Dequeue();

                drawTile(spriteBatch, tilePosition.tile, tilePosition.position);

                while (tilePosition.overlays.Count > 0)
                {
                    TileOverlay overlay = tilePosition.overlays.Dequeue();
                    spriteBatch.Draw(overlay.Texture, getTilePosition(tilePosition.position) + getTileTopOffset(tilePosition.tile), null, overlay.Color, 0, overlay.Origin, 1f, SpriteEffects.None, 0);
                }
            }
        }

        /// <summary>
        /// drawing the tile onto the screen
        /// </summary>
        /// <param name="spriteBatch">the spritebatch which you want to use to draw the tiles onto the rendertarget</param>
        /// <param name="indices">the world coordinates of the tile</param>
        public void drawTile(SpriteBatch spriteBatch, Tile tile, Point coordinates)
        {
            Vector2 pos = getTilePosition(coordinates);

            foreach (int index in tile.Indices)
            {
                spriteBatch.Draw(tileTypeTextures[tile.TypeIndex][index], pos, null, Color.White, 0f, textureOrigin, 1, SpriteEffects.None, 0f);
                pos.Y -= stackingTileOffset;
            }
        }

        public Vector2 getTilePosition(Point coordinates)
        {
            return coordinates.X * tileOffset_X + coordinates.Y * tileOffset_Y;
        }

        /// <summary>
        /// calculates the offset of the tile's offset relatively to the origin
        /// </summary>
        /// <param name="tile">the tile calculating the offset from</param>
        /// <returns>the offset from the top relatively to the origin</returns>
        public Vector2 getTileTopOffset(Tile tile)
        {
            return -Vector2.UnitY * stackingTileOffset * (tile.Indices.Count - 1);
        }

        /// <summary>
        /// calculates the offset of the tile's top relatively to the origin
        /// </summary>
        /// <param name="tileCoordinate">the coordinates for the tile</param>
        /// <param name="currentRotation">the rotation of the world</param>
        /// <returns>the offset from the top relatively to the origin</returns>
        public Vector2 getTileTopOffset(Point tileCoordinate, Rotation currentRotation = Rotation._0_DEGREE)
        {
            Point worldCoordinates = getRotatedCoordinates(tileCoordinate, currentRotation);
            return getTileTopOffset(tiles[worldCoordinates.X, worldCoordinates.Y]);
        }

        /// <summary>
        /// rotate the coordinates according to the give rotation
        /// </summary>
        /// <param name="coordinates">the coordinates which have to be rotated</param>
        /// <param name="rotation">the rotation of the result</param>
        /// <returns>a rotated coordinate</returns>
        public Point getRotatedCoordinates(Point coordinates, Rotation rotation)
        {
            Point output = new Point();
            int maxSize = Math.Max(tiles.GetUpperBound(0), tiles.GetUpperBound(1));
            switch (rotation)
            {
                case Rotation._0_DEGREE:
                    output.X = coordinates.X;
                    output.Y = coordinates.Y;
                    break;
                case Rotation._90_DEGREE:
                    output.X = coordinates.Y;
                    output.Y = maxSize - coordinates.X;
                    break;
                case Rotation._180_DEGREE:
                    output.X = maxSize - coordinates.X;
                    output.Y = maxSize - coordinates.Y;
                    break;
                case Rotation._270_DEGREE:
                    output.X = maxSize - coordinates.Y;
                    output.Y = coordinates.X;
                    break;
                default:
                    break;
            }

            return output;
        }

        /// <summary>
        /// inverses the rotation
        /// </summary>
        /// <param name="rotation">the rotation which you want to get the inverse rotation from</param>
        /// <returns>the inverted rotation</returns>
        public static Rotation inverseRotation(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation._90_DEGREE:
                    return Rotation._270_DEGREE;
                case Rotation._270_DEGREE:
                    return Rotation._90_DEGREE;
                default:
                    return rotation;
            }
        }

        /// <summary>
        /// rotate a point according to the given rotation
        /// </summary>
        /// <param name="point">the point you want to rotate</param>
        /// <param name="rotation">the rotation according which you want to rotate the point</param>
        /// <returns>a rotated point</returns>
        public static Point rotatePoint(Point point, Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation._0_DEGREE:
                    return point;
                case Rotation._90_DEGREE:
                    return new Point(-point.Y, point.X);
                case Rotation._180_DEGREE:
                    return new Point(-point.X, -point.Y);
                case Rotation._270_DEGREE:
                    return new Point(point.Y, -point.X);
                default:
                    return Point.Zero;
            }
        }


    }
}
