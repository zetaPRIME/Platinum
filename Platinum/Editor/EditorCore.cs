using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LitJson;
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
			propertyPage = new ListLayout();
			propertyPane.AddElement(propertyPage);

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
		public static ListLayout propertyPage;

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


		// API stuffs
		public static void SaveScene()
		{
			GameState.scene.Save();
			string file = JsonMapper.ToPrettyJson(GameState.scene.def);
			GameState.scene.package.SaveDef(file);
		}

		static EntityPlacement propertyTarget;

		public static void SetPropertiesFromContext()
		{
			if (sceneDisplay.selection.Count == 1) SetPropertyPage(sceneDisplay.selection[0]);
			else if (sceneDisplay.selection.Count == 0)
			{
				propertyTarget = null;
				propertyPage.children.Clear();
				sidebar.currentPage = 0;
			}
			else
			{
				propertyTarget = null;
				propertyPage.children.Clear();
				sidebar.currentPage = 1;
				Label l = new Label();
				l.text = "Multiple entities selected;\nproperty editing disabled";
				propertyPage.AddElement(l);
				propertyPage.Update();
			}
		}
		public static void SetPropertyPage(EntityPlacement ep)
		{
			if (ep == propertyTarget) return; // no need
			propertyTarget = ep;
			if (ep == null) return; // what are you even doing

			propertyPage.children.Clear();

			// common things
			propertyPage.AddElement(new Label("Editing " + ep.typeName));
			propertyPage.AddElement(new Separator());

			propertyPage.AddElement(new Label("Position:"));
			TextField t;
			{
				t = new TextField();
				t.actionUpdate = (tf) =>
				{
					tf.text = "" + ep.position.X;
				};
				t.actionEnter = (txt) =>
				{
					float res = 0;
					if (float.TryParse(txt, out res)) ep.position.X = res;
				};
				propertyPage.AddElement(t);

				t = new TextField();
				t.actionUpdate = (tf) =>
				{
					tf.text = "" + ep.position.Y;
				};
				t.actionEnter = (txt) =>
				{
					float res = 0;
					if (float.TryParse(txt, out res)) ep.position.Y = res;
				};
				propertyPage.AddElement(t);
			}

			propertyPage.AddElement(new Label("Rotation:"));
			{
				t = new TextField();
				t.actionUpdate = (tf) =>
				{
					tf.text = "" + Math.Round(ep.rotation * (180f / Math.PI), 3);
				};
				t.actionEnter = (txt) =>
				{
					float res = 0;
					if (float.TryParse(txt, out res)) ep.rotation = res * ((float)Math.PI / 180f);
				};
				propertyPage.AddElement(t);
			}

			propertyPage.AddElement(new Label("Draw layer:"));
			{
				t = new TextField();
				t.actionUpdate = (tf) =>
				{
					tf.text = "" + ep.drawLayer;
				};
				t.actionEnter = (txt) =>
				{
					int res = 0;
					if (int.TryParse(txt, out res)) ep.drawLayer = res;
				};
				propertyPage.AddElement(t);
			}

			// custom properties
			List<UIElement> custom = ep.type.editorEntity.BuildProperties(ep);
			if (custom != null && custom.Count > 0)
			{
				propertyPage.AddElement(new Separator());
				foreach (UIElement e in custom) propertyPage.AddElement(e);
			}

			// and finish
			propertyPane.scroll = Point.Zero;
			propertyPage.Update();
			sidebar.currentPage = 1;
		}
	}
}
