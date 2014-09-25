using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Platinum
{
	public class Map
	{
		public string name;
		public Vector2 size;
		public List<Entity> entities;
		public List<EntityPlacement> placements;
	}
}
