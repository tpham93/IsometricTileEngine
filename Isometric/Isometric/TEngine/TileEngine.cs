﻿using System;
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
        /// the size of the map
        /// </summary>
        private Point mapSize;

        /// <summary>
        /// the maximum of the x axis and y axis length
        /// </summary>
        private int maxSize;

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
        private Vector2 stackingTileOffset;


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
            get { return mapSize; }
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
            mapSize = dimension;
            maxSize = Math.Max(mapSize.X,mapSize.Y);
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
        public void initialize(Vector2 tileSize, Vector2 textureOrigin, Vector2 tileOffset_X, Vector2 tileOffset_Y, Vector2 stackingTileOffset)
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
        /// adds multiple types
        /// </summary>
        /// <param name="num">the number of types to add</param>
        public void addType(int num)
        {
            for (int i = 0; i < num; ++i)
            {
                addType();
            }
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
        /// <param name="transformedScreen">the screen with its position and size</param>
        /// <param name="scale">the scale used to draw</param>
        /// <param name="rotation">rotation of the world</param>
        /// <param name="overlays">the overlays which should be drawn onto the tiles</param>
        public void draw(SpriteBatch spriteBatch, Vector2 screenOffset, Rectangle screen, float scale, Rotation rotation, List<TileOverlay> overlays = null)
        {
            //create a new comparer to sort the overlays
            DrawingOrderComparer drawingOrderComparer = new DrawingOrderComparer(MapSize);

            //a queue to save the sorted overlays
            Queue<TileOverlay> sortedOverlays = new Queue<TileOverlay>();

            //sort the overlays if there exists any
            if (overlays != null)
            {
                //iterate through all overlays to save and rotate them
                foreach (TileOverlay overlay in overlays)
                {
                    //rotate all overlays
                    sortedOverlays.Enqueue(new TileOverlay(getRotatedCoordinates(overlay.Position, rotation), overlay.Texture, overlay.Origin, overlay.Color));
                }
                //sort the queue with the drawingOrderComparer
                sortedOverlays = new Queue<TileOverlay>(sortedOverlays.OrderBy(overlay => overlay.Position, drawingOrderComparer));
            }

            // a queue to save the tiles in the right order with their information
            Queue<TileDrawingData> drawingTiles = new Queue<TileDrawingData>(maxSize * maxSize);

            // the rectangle representing the screen in the world
            Rectangle transformedScreen = new Rectangle((int)(screen.X * scale) - (int)screenOffset.X, (int)(screen.Y * scale) - (int)screenOffset.Y, (int)(screen.Width), (int)(screen.Height));

            //iterate through every tile to select the tiles needed to draw
            for (int y = 0; y <= maxSize; ++y)
            {
                for (int x = maxSize; x >= 0; --x)
                {
                    //the rotated coordinates from the world
                    Point coordinates = new Point(x, y);
                    //the coordinates from the tile which should be drawn at the rotated coordinates
                    Point tileCoordinates = getRotatedCoordinates(coordinates, inverseRotation(rotation));

                    //see if the tile is within the world
                    if (tileCoordinates.X >= 0 && tileCoordinates.Y >= 0 && tileCoordinates.X < mapSize.X && tileCoordinates.Y < mapSize.Y)
                    {
                        //the offset between the lowest and he highest stacked tile
                        Vector2 tileTopOffset = new Vector2(0, -getTileTopOffset(tiles[tileCoordinates.X, tileCoordinates.Y]).Y);
                        //the worldposition
                        Vector2 position = (getTilePosition(coordinates) - textureOrigin - tileTopOffset) * scale;
                        //the rectangle of the tile
                        Rectangle tileRect = new Rectangle((int)position.X, (int)position.Y, (int)(tileSize.X * scale), (int)((tileSize.Y + tileTopOffset.Y) * scale));
                        //test if the 
                        if (transformedScreen.Intersects(tileRect))
                        {
                            //create a queue to save the overlays for the specific tile
                            Queue<TileOverlay> tileOverlays = new Queue<TileOverlay>();

                            //look for overlays which could be the overlay for the coordinate
                            while (sortedOverlays.Count > 0 && drawingOrderComparer.Compare(sortedOverlays.Peek().Position, coordinates) <= 0)
                            {
                                //take the first overlay
                                TileOverlay overlay = sortedOverlays.Dequeue();
                                //discard the overlay if the position isn't the same
                                if (overlay.Position == coordinates)
                                {
                                    //add it to the queue otherwise
                                    tileOverlays.Enqueue(overlay);
                                }
                            }
                            //add the tile to the drawing queue with its overlays and coordinates
                            drawingTiles.Enqueue(new TileDrawingData(tiles[tileCoordinates.X, tileCoordinates.Y], coordinates, tileOverlays));
                        }
                    }
                }
            }

            // draw all tiles in the drawing queue
            while (drawingTiles.Count > 0)
            {
                // take the first tile
                TileDrawingData tilePosition = drawingTiles.Dequeue();

                //draw the tile
                drawTile(spriteBatch, tilePosition.tile, tilePosition.coordinates);

                // draw all overlays of that tile
                while (tilePosition.overlays.Count > 0)
                {
                    // take the first overlay
                    TileOverlay overlay = tilePosition.overlays.Dequeue();
                    //draw the overlay
                    spriteBatch.Draw(overlay.Texture, getTilePosition(tilePosition.coordinates) + getTileTopOffset(tilePosition.tile), null, overlay.Color, 0, overlay.Origin, 1f, SpriteEffects.None, 0);
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
            // the current position of the tile
            Vector2 pos = getTilePosition(coordinates);
            List<int> indices = tile.Indices;


            // iterate through every stacked tiles
            for(int i=0;i<tile.Indices.Count;++i)
            {
                // draw the tile
                spriteBatch.Draw(tileTypeTextures[tile.TypeIndex][indices[i]], pos, null, Color.White, 0f, textureOrigin, 1, SpriteEffects.None, 0f);
                //set the position to the position of the next tile
                pos += stackingTileOffset;
            }
        }

        /// <summary>
        /// gets the world position according the given coordinates
        /// </summary>
        /// <param name="coordinates">the coordinate you want the position from</param>
        /// <returns>the world position of the coordinates</returns>
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
            return stackingTileOffset * (tile.Indices.Count - 1);
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
            switch (rotation)
            {
                case Rotation._0_DEGREE:
                    return coordinates;

                case Rotation._90_DEGREE:
                    return new Point(coordinates.Y, maxSize - coordinates.X);

                case Rotation._180_DEGREE:
                    return new Point(maxSize - coordinates.X, maxSize - coordinates.Y);

                case Rotation._270_DEGREE:
                    return new Point(maxSize - coordinates.Y, coordinates.X);

                default:
                    return Point.Zero;
            }
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
