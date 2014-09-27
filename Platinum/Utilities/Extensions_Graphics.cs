using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public static partial class Extensions
	{
		public static void Draw(this SpriteBatch sb, ExtTexture texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects)
		{
			sb.Draw(texture.texture, position, sourceRectangle, color, rotation, origin, scale * texture.baseScale, SpriteEffects.None, 0f);
		}

		public static void Draw(this SpriteBatch sb, ExtTexture texture, Vector2 position, int frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects)
		{
			Rectangle? sourceRectangle = null;
			if (texture.animFrames > 1)
			{
				int fw = texture.texture.Width / texture.animFramesX;
				int fh = texture.texture.Height / texture.animFramesY;

				int fx = frame % texture.animFramesY;
				int fy = (frame - fx) / texture.animFramesY;

				sourceRectangle = new Rectangle(fw * fx, fh * fy, fw, fh);
			}
			sb.Draw(texture.texture, position, sourceRectangle, color, rotation, origin, scale * texture.baseScale, SpriteEffects.None, 0f);
		}

		public static void CameraOn(this SpriteBatch sb, bool alreadyRunning = true)
		{
			if (alreadyRunning) sb.End();
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, GameState.cameraTransform);
		}

		public static void CameraOff(this SpriteBatch sb, bool alreadyRunning = true)
		{
			if (alreadyRunning) sb.End();
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
		}

		// ---------- //
		// primitives //
		// ---------- //
		public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, Color color, float width = 1f)
		{
			sb.DrawLine(new LineSegment(start, end), color, width);
		}
		public static void DrawLine(this SpriteBatch sb, LineSegment line, Color color, float width = 1f)
		{
			sb.Draw(Core.txPixel, line.start, null, color, (float)Math.Atan2(line.Direction.Y, line.Direction.X), new Vector2(0f, 0.5f), new Vector2(line.Length, width), SpriteEffects.None, 0f);
		}

		public static void DrawRect(this SpriteBatch sb, Rectangle rect, Color color)
		{
			sb.Draw(Core.txPixel, rect, color);
		}

		// ----- //
		// color //
		// ----- //
		public static Color MultiplyBy(this Color first, Color mult)
		{
			return new Color(first.ToVector4() * mult.ToVector4());
		}
		public static Color MultiplyBy(this Color color, Vector4 mult)
		{
			return new Color(color.ToVector4() * mult);
		}
		public static Color MultiplyBy(this Color color, float mult)
		{
			return new Color(color.ToVector4() * mult);
		}
	}
}
