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
		public Exception caughtException;

		protected void Init_Exception()
		{
			ScrollField sf = new ScrollField();
			sf.bounds = Window.ClientBounds;
			UI.AddElement(sf);
			Window.ClientSizeChanged += (s, e) => { sf.bounds = Window.ClientBounds; };

			ListLayout ll = new ListLayout();
			sf.AddElement(ll);

			ll.AddElement(new Label("" + caughtException.GetType().Name + " in " + caughtException.Source));
			ll.AddElement(new Separator());
			ll.AddElement(new Label(caughtException.Message));
			ll.AddElement(new Separator());
			ll.AddElement(new Label(caughtException.StackTrace));
			

			
		}

		protected void Update_Exception(GameTime gameTime)
		{
			UI.Update();
		}

		protected void Draw_Exception(GameTime gameTime)
		{
			PrepareTarget();

			spriteBatch.GraphicsDevice.Clear(UI.colorBG);

			spriteBatch.CameraOff(false);

			UI.Draw(spriteBatch);

			BakeToScreen();
		}
	}
}
