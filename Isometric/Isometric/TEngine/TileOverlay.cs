using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Isometric.TEngine
{
    public class TileOverlay
    {
        /***************************************************************************************************
         *  attributes
         ***************************************************************************************************/

        /// <summary>
        /// the position of the tile
        /// </summary>
        private Point position;

        /// <summary>
        /// the texture of the overlay
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// the texture of the overlay
        /// </summary>
        private Vector2 origin;

        /// <summary>
        /// the color used to draw the overlay
        /// </summary>
        private Color color;




        /***************************************************************************************************
         *  properties
         ***************************************************************************************************/

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        


        /***************************************************************************************************
         *  methods
         ***************************************************************************************************/

        /// <summary>
        /// Constructor (color is set to Color.White)
        /// </summary>
        /// <param name="texture"></param>
        public TileOverlay(Point position, Texture2D texture)
            :this(position, texture, new Vector2(texture.Bounds.Width/2, texture.Bounds.Height/2), Color.White)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">the overlay's texture</param>
        /// <param name="origin">the texture's origin</param>
        /// <param name="color">the color you want to draw the overlay with</param>
        public TileOverlay(Point position, Texture2D texture, Vector2 origin, Color color)
        {
            this.position = position;
            this.texture = texture;
            this.origin = origin;
            this.color = color;
        }
    }
}
