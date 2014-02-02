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

        Rotation rotation;

        Texture2D cursor;

        Point selectedTile;

        public Test()
        {
        }

        public void Initialize()
        {
            position = Vector2.Zero;
            selectedTile = new Point();
            input = new Input();

            tileEngine = new TileEngine(new Point(10, 5));
            tileEngine.initialize(new Vector2(64, 42), new Vector2(32, 16), new Vector2(32, -16), new Vector2(32, 16), 10);
            tileEngine.addType();

            List<int> indices = new List<int>();
            Tile[,] tiles = tileEngine.Tiles;
            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    indices = new List<int>();
                    for (int i = 0; i <= Math.Max(x, y); ++i)
                    {
                        indices.Add(i);
                    }
                    tiles[x, y] = new Tile(0, indices);
                }
            }
        }

        public void LoadContent(ContentManager content, SpriteBatch spriteBatch)
        {
            Texture2D spriteSheet = content.Load<Texture2D>(@"tileSet");
            for (int i = 0; i < spriteSheet.Width / 64; ++i)
            {
                Rectangle sourceRect = new Rectangle(i * 64, 0, 64, 42);
                tileEngine.addTexture(Helper.crop(spriteBatch, spriteSheet, sourceRect), 0);
            }

            cursor = content.Load<Texture2D>(@"cursor");

            Texture2D overlay = content.Load<Texture2D>(@"tileOverlay");

            tileEngine.addTileOverlay(new TileOverlay(overlay, new Vector2(overlay.Bounds.Width, overlay.Bounds.Height) / 2f, Color.Blue * 0.75f));
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
                rotation += 1;
                rotation = (Rotation)((int)rotation % 4);
            }
            if (input.keyClicked(Keys.E))
            {
                rotation += 3;
                rotation = (Rotation)((int)rotation % 4);
            }

            Point mapSize = tileEngine.MapSize;
            Point cursorMovement = Point.Zero;

            if (input.keyClicked(Keys.Left))
            {
                --cursorMovement.X;
            }
            if (input.keyClicked(Keys.Right))
            {
                ++cursorMovement.X;
            }
            if (input.keyClicked(Keys.Up))
            {
                --cursorMovement.Y;
            }
            if (input.keyClicked(Keys.Down))
            {
                ++cursorMovement.Y;
            }

            cursorMovement = TileEngine.rotatePoint(cursorMovement, rotation);



            selectedTile.X = Math.Min(Math.Max(selectedTile.X + cursorMovement.X, 0), mapSize.X);
            selectedTile.Y = Math.Min(Math.Max(selectedTile.Y + cursorMovement.Y, 0), mapSize.Y);


            if (input.isKeyDown(Keys.R) || input.scrolledUp())
            {
                scale = Math.Min(scale + 0.01f, 1.5f);
            }
            if (input.isKeyDown(Keys.F) || input.scrolledDown())
            {
                scale = Math.Max(scale - 0.01f, 0.5f);

            }

            Tile[,] tiles = tileEngine.Tiles;

            for (int y = 0; y <= tiles.GetUpperBound(1); ++y)
            {
                for (int x = 0; x <= tiles.GetUpperBound(0); ++x)
                {
                    if (selectedTile == new Point(x, y) || selectedTile == new Point(x + 1, y) || selectedTile == new Point(x, y + 1) || selectedTile == new Point(x - 1, y) || selectedTile == new Point(x, y - 1))
                    {
                        tiles[x, y].OverlayIndex = 0;
                    }
                    else
                    {
                        tiles[x, y].OverlayIndex = -1;
                    }
                }
            }

        }
        float scale = 1.0f;
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 cursorOffset = -Vector2.UnitY * (16 + 10 * (float)Math.Sin(5 * gameTime.TotalGameTime.TotalSeconds)) + tileEngine.getTileTopOffset(selectedTile);

            Vector2 cursorPosition = tileEngine.getTilePosition(tileEngine.getRotatedCoordinates(selectedTile, rotation)) + cursorOffset;
            spriteBatch.Begin(SpriteSortMode.Immediate,
                null,
                null,
                null,
                null,
                null,
                Matrix.CreateTranslation(new Vector3(position, 0)) * Matrix.CreateScale(scale));

            tileEngine.draw(spriteBatch, rotation);

            spriteBatch.Draw(cursor, cursorPosition, null, Color.White, 0f, new Vector2(cursor.Bounds.Width / 2f, cursor.Bounds.Height), 1f, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
