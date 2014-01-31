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
using Isometric.TEngine;
using Isometric.GameComponents;

namespace Isometric
{
    public class Test
    {
        Vector2 position;

        TileEngine tileEngine;

        Input input;

        Rotation rotationCounter;

        public Test()
        {
        }

        public void Initialize()
        {
            position = new Vector2(0, 0);
            //position = new Vector2(400, 150);
            input = new Input();

            tileEngine = new TileEngine(new Point(10, 10));
            tileEngine.initialize(new Vector2(64, 42), new Vector2(32, 16), new Vector2(32, 16), new Vector2(-32, 16), 10);
            tileEngine.addType();

            List<int> indices = new List<int>();
            Tile[,] tiles = tileEngine.Tiles;
            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    indices = new List<int>();
                    for (int i = 0; i < 10 - Math.Max(x,y); ++i)
                    {
                        indices.Add(i);
                    }
                    tiles[x, y] = new Tile(0,indices);
                }
            }

            position = tileEngine.getTileTopOffset(new Point(0,0));
        }

        public void LoadContent(ContentManager content, SpriteBatch spriteBatch)
        {
            Texture2D spriteSheet = content.Load<Texture2D>(@"tileSet");
            for (int i = 0; i < spriteSheet.Width / 64; ++i)
            {
                Rectangle sourceRect = new Rectangle(i * 64, 0, 64, 42);
                tileEngine.addTexture(Helper.crop(spriteBatch,spriteSheet,sourceRect),0);
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

            tileEngine.draw(spriteBatch, rotationCounter);

            spriteBatch.End();
        }
    }
}
