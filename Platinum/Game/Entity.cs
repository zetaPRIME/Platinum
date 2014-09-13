﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class Entity : EventPassable
	{
		public Vector2 position;

		public virtual void Update() { }
		public virtual void Draw(SpriteBatch sb) { }
	}
}
