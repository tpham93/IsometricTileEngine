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
using Isometric.GameComponents;

namespace Isometric
{
    public class Test
    {
        Vector2 position;

        Tile[,] tiles;

        Input input;

        Rotation rotationCounter;

        public Test()
        {
        }

        public void Initialize()
        {
            position = new Vector2(400,300);
            input = new Input();
            
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
                    tiles[x, y] = new Tile(0,indices);
                }
            }
        }

        public void LoadContent(ContentManager content, SpriteBatch spriteBatch)
        {
            Texture2D spriteSheet = content.Load<Texture2D>(@"tileSet");
            for (int i = 0; i < spriteSheet.Width / 64; ++i)
            {
                Rectangle sourceRect = new Rectangle(i * 64, 0, 64, 42);
                Tile.addTexture(Helper.crop(spriteBatch,spriteSheet,sourceRect),0);
            }


        }

        public void Update(GameTime gameTime)
        {
            input.Update();

            Vector2 movement = new Vector2();
            KeyboardState keyboardState = Keyboard.GetState();

            if (input.isKeyDown(Keys.A))
            {
                movement += Vector2.UnitX;
            }
            if (input.isKeyDown(Keys.D))
            {
                movement -= Vector2.UnitX;
            }
            if (input.isKeyDown(Keys.W))
            {
                movement += Vector2.UnitY;
            }
            if (input.isKeyDown(Keys.S))
            {
                movement -= Vector2.UnitY;
            }

            movement *= 128 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            position += movement;


            if (input.keyClicked(Keys.Q))
            {
                rotationCounter += 1;
                rotationCounter = (Rotation)((int)rotationCounter % 4);
            }
            if (input.keyClicked(Keys.E))
            {
                rotationCounter += 3;
                rotationCounter = (Rotation)((int)rotationCounter % 4);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate,
                null,
                null, 
                null, 
                null, 
                null,
                Matrix.CreateTranslation(new Vector3(position, 0)));

            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = tiles.GetUpperBound(0); x >= 0; --x)
                {
                    int t_x = 0;
                    int t_y = 0;
                    switch (rotationCounter)
                    {
                        case Rotation._0_DEGREE:
                            t_x = x;
                            t_y = y;
                            break;
                        case Rotation._90_DEGREE:
                            t_x = tiles.GetUpperBound(1) - y;
                            t_y = tiles.GetUpperBound(0) - x;
                            break;
                        case Rotation._180_DEGREE:
                            t_x = tiles.GetUpperBound(0) - x;
                            t_y = tiles.GetUpperBound(1) - y;
                            break;
                        case Rotation._270_DEGREE:
                            t_x = y;
                            t_y = x;
                            break;
                        default:
                            t_x = x;
                            t_y = y;
                            break;
                    }

                    tiles[t_x, t_y].Draw(spriteBatch, new Point(x, y));
                }
            }


            spriteBatch.End();
        }
    }
}
