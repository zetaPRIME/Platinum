using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class SceneService
	{
		public virtual void PreUpdate() { }
		public virtual void PostUpdate() { }

		public virtual void PreDraw(SpriteBatch sb) { }
		public virtual void PostDraw(SpriteBatch sb) { }
	}
}
