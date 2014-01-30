using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Isometric.TEngine
{
    class Helper
    {
        public static Texture2D crop(SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRect)
        {
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            RenderTargetBinding[] renderTargets =  graphicsDevice.GetRenderTargets();

            RenderTarget2D output = new RenderTarget2D(graphicsDevice,sourceRect.Width,sourceRect.Height);

            graphicsDevice.SetRenderTarget(output);

            graphicsDevice.Clear(new Color(0,0,0,0));

            spriteBatch.Begin();
            spriteBatch.Draw(texture,Vector2.Zero,sourceRect,Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTargets(renderTargets);
            return (Texture2D)output;
        }
    }
}
