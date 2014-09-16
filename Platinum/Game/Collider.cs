using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;

namespace Platinum
{
	public class Collider
	{
		public Entity parent;

		Body _physBody;
		public Body physBody
		{
			get { return _physBody; }
			set
			{
				_physBody = value;

				value.IsSensor = true;
				value.IsStatic = true;
				value.CollidesWith = Category.None;
				value.Enabled = false;
			}
		}

		public Vector2 offset = Vector2.Zero;
		public float rotOffset = 0f;

		public BitField layers = 255;
		public bool solid = false;

		public void Update()
		{
			/*Vector2 newPos = parent.Position + offset;
			physBody.SetTransformIgnoreContacts(ref newPos, parent.Rotation + rotOffset);*/
			physBody.SetTransformOnly(parent.Position + offset, parent.Rotation + rotOffset);
		}
		
		public bool CollidesWith(Collider other)
		{
			if (physBody == null || other.physBody == null) return false;

			//physBody.SetTransform(parent.Position + offset, parent.Rotation + rotOffset);

			if ((layers & other.layers) == 0) return false;

			foreach (Fixture f in physBody.FixtureList)
			{
				foreach (Fixture of in other.physBody.FixtureList)
				{
					if (Collision.TestFixture(f, of)) return true;
				}
			}

			return false;
		}
	}
}
