using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public partial class Extensions
	{
		public void Draw(this SpriteBatch sb, ExtTexture texture, Vector2 position, float scale = 1f, float rotation = 1f)
		{
			sb.Draw(texture.texture, position, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}
	}
}
