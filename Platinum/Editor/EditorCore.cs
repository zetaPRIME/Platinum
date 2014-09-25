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
using UIButton = Platinum.UIKit.Button;

namespace Platinum.Editor
{
	public static class EditorCore
	{
		public static GameWindow Window { get { return Core.instance.Window; } }

		static string blah = "blah";

		public static void Init()
		{
			Rectangle screen = Window.ClientBounds;

			ScrollField sf = new ScrollField();
			sf.bounds = new Rectangle(600, 0, 200, 600);
			sf.border[2] = true;
			sf.hasScrollbar = true;
			UI.elements.Add(sf);

			UIButton btn = new UIButton();
			btn.bounds = new Rectangle(100, 100, 64, 832);
			btn.actionUp = () => { btn.bounds.Height = 32; };
			btn.text = "Pineapple\nsundae";
			sf.AddElement(btn);

			TextField tf = new TextField();
			tf.bounds = new Rectangle(8, 8, 100, 18);
			sf.AddElement(tf);
		}

		public static void Update()
		{
			UI.Update();
		}

		public static void Draw(SpriteBatch sb)
		{
			Core.instance.PrepareTarget();

			sb.GraphicsDevice.Clear(UI.colorBG);

			sb.CameraOff(false);

			UI.Draw(sb);

			//Input.OperateText(ref blah);
			//sb.DrawString(Core.fontDebug, blah, new Vector2(32, 32), Color.White);

			Core.instance.BakeToScreen();
		}
	}
}
