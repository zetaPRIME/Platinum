using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LitJson;

using Platinum;
using Platinum.Editor;

namespace ExampleBase
{
	public class RotoshredderEE : EditorEntity
	{
		public override VecRect DrawBounds
		{
			get
			{
				return VecRect.Radius * 256f; // okay, can't get the placement instance! herp of the derp.
			}
			set { }
		}

		public override void DrawInEditor(EntityPlacement ep, SpriteBatch sb, Color colorModifier)
		{
			Vector2 rad = new Vector2(0, (float)ep.def["radius"] * -1f);
			SpriteEffects fx = SpriteEffects.None;
			if ((double)ep.def["rotationSpeed"] < 0) fx = SpriteEffects.FlipHorizontally;
			sb.Draw(def.mainTexture, rad.Transform(ep.Transform), 0, colorModifier, ep.Rotation * 4f, def.mainTexture.center, 1f, fx);

			Rectangle r = (SelectBounds + ep.Position).AsRectangle; // r.Inflate(-8, -8);
			Rectangle r2 = r; r2.Inflate(-7, 0);
			sb.DrawRect(r2, colorModifier);
			r2 = r; r2.Inflate(0, -7);
			sb.DrawRect(r2, colorModifier);
		}

		public override void BuildDefaultJson(JsonData j)
		{
			j.WriteDefault("radius", JsonType.Double, 96.0);

			j.WriteDefault("rotationSpeed", JsonType.Double, 3.0);
		}
		public override List<UIElement> BuildProperties(EntityPlacement p)
		{
			List<UIElement> list = new List<UIElement>();
			JsonData j = p.def;

			PropertyHelper.JsonProperty(list, "radius", "Radius:", j);
			PropertyHelper.JsonProperty(list, "rotationSpeed", "Rotation speed:", j);

			return list;
		}
	}
}
