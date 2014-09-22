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

namespace Platinum
{
	public partial class Core
	{
		protected void Init_Editor()
		{
			Rectangle screen = Window.ClientBounds;

			ScrollField sf = new ScrollField();
			sf.bounds = new Rectangle(600, 0, 200, 600);
			sf.border[2] = true;
			sf.hasScrollbar = true;
			UI.elements.Add(sf);

			UIKit.Button btn = new UIKit.Button();
			btn.bounds = new Rectangle(100, 100, 64, 832);
			btn.actionUp = () => { btn.bounds.Height = 32; };
			sf.AddElement(btn);
		}

		protected void Update_Editor(GameTime gameTime)
		{
			UI.Update();
		}

		protected void Draw_Editor(GameTime gameTime)
		{
			PrepareTarget();

			GraphicsDevice.Clear(UI.colorBG);

			spriteBatch.CameraOff(false);

			UI.Draw(spriteBatch);

			BakeToScreen();
		}
	}
}
