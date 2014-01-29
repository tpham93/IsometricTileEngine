using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Isometric.TileEngine;

namespace Isometric
{
    public class Test
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Vector2 position;

        Tile[,] tiles;



        public Test()
        {

        }

        public void Initialize()
        {
            Tile.addType();

            tiles = new Tile[10, 10];
            List<int> indices = new List<int>();
            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    indices = new List<int>();
                    for (int i = 0; i < 10 - x; ++i)
                    {
                        indices.Add(i);
                    }
                    tiles[x, y] = new Tile(0,indices,new Point(x,y));
                }
            }
        }

        public void LoadContent(ContentManager content, SpriteBatch spriteBatch)
        {
            Texture2D spriteSheet = content.Load<Texture2D>(@"tileSet");
            for (int i = 0; i < spriteSheet.Width / 64; ++i)
            {
                Rectangle sourceRect = new Rectangle(i * 64, 0, 64, 64);
                Tile.addTexture(Helper.crop(spriteBatch,spriteSheet,sourceRect),0);
            }


        }

        public void Update(GameTime gameTime)
        {
            Vector2 movement = new Vector2();
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                movement += Vector2.UnitX;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                movement -= Vector2.UnitX;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                movement += Vector2.UnitY;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                movement -= Vector2.UnitY;
            }

            movement *= 64 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            position += movement;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Matrix.CreateTranslation(new Vector3(position, 0)));

            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    tiles[x, y].Draw(spriteBatch);
                }
            }


            spriteBatch.End();
        }
    }
}
