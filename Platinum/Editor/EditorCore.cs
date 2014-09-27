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

		public static void Init()
		{
			// TEMP
			GameState.EnterScene(GameDef.defaultScene);

			// scene display!
			sceneDisplay = new SceneDisplay();
			UI.AddElement(sceneDisplay);

			// menu bar
			menuBar = new ScrollField();
			menuBar.hasScrollbar = false;
			menuBar.border[1] = true;
			UI.AddElement(menuBar);

			// set up sidebar
			sidebar = new SwitchField();
			UI.AddElement(sidebar);

			entityList = new ScrollField();
			entityList.hasScrollbar = true;
			entityList.border[2] = true;
			sidebar.AddPage(entityList);

			propertyPane = new ScrollField();
			propertyPane.hasScrollbar = true;
			propertyPane.border[2] = true;
			sidebar.AddPage(propertyPane);

			sidebar.currentPage = 1;

			// test
			ListLayout ll = new ListLayout();
			propertyPane.AddElement(ll);
			for (int i = 0; i < 320; i++)
			{
				UIButton b = new UIButton();
				b.bounds = new Rectangle(0, 0, 32, 32);
				b.text = "Testing button " + (i + 1);
				ll.AddElement(b);
			}

			// set up sizing
			RefreshLayout();

			// set up an event because why not
			Window.ClientSizeChanged += (s, ea) => { RefreshLayout(); };
		}

		public static SceneDisplay sceneDisplay;

		public static ScrollField menuBar;

		public static SwitchField sidebar;
		public static ScrollField entityList;
		public static ScrollField propertyPane;

		public static void RefreshLayout()
		{
			Rectangle screen = Window.ClientBounds;

			int MenuHeight = 32;
			int SidebarWidth = 200;

			menuBar.bounds = screen.MarginTop(MenuHeight);

			Rectangle subScreen = screen.MarginBottom(screen.Height - MenuHeight);
			sidebar.bounds = subScreen.MarginRight(SidebarWidth);

			sceneDisplay.bounds = subScreen.MarginLeft(subScreen.Width - SidebarWidth);
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

			Core.instance.BakeToScreen();
		}
	}
}
