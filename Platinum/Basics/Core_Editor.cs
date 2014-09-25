using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fluent.IO;
using Ionic.Zip;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Platinum.UIKit;
using Platinum.Editor;

namespace Platinum
{
	public partial class Core
	{
		protected void Init_Editor()
		{
			EditorCore.Init();
		}

		protected void Update_Editor(GameTime gameTime)
		{
			EditorCore.Update();
		}

		protected void Draw_Editor(GameTime gameTime)
		{
			EditorCore.Draw(spriteBatch);
		}
	}
}
