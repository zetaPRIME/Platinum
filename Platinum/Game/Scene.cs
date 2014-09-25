using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using LitJson;

namespace Platinum
{
	public class Scene
	{
		public Package package;
		public SceneService sceneService;
		public SceneMode sceneMode;

		public JsonData def { get { return package.def; } }
	}
}
