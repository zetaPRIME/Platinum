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

			explorer = new ScrollField();
			explorer.hasScrollbar = true;
			explorer.border[2] = true;
			sidebar.AddPage(explorer);

			explorerSearch = new TextField();
			explorerSearch.actionValidate = ExplorerSearchDelegate;
			explorerSearch.actionEnter = (s) => explorerSearch.Clear();
			//explorerSearch.bounds = new Rectangle(8, 0, 200 - 32, 20);
			//explorer.AddElement(explorerSearch);

			explorerList = new ListLayout();
			explorerList.bounds = new Rectangle(0, 22, 22, 22);
			explorer.AddElement(explorerList);

			propertyPane = new ScrollField();
			propertyPane.hasScrollbar = true;
			propertyPane.border[2] = true;
			sidebar.AddPage(propertyPane);

			//sidebar.currentPage = 1;

			// test
			propertyPage = new ListLayout();
			propertyPane.AddElement(propertyPage);

			// set up sizing
			RefreshLayout();

			// set up explorer
			//FindEntities();
			ExplorerGo(ExplorerMode.Entity, "");

			// set up an event because why not
			Window.ClientSizeChanged += (s, ea) => { RefreshLayout(); };
		}

		public static SceneDisplay sceneDisplay;

		public static ScrollField menuBar;

		public static SwitchField sidebar;
		public static ScrollField explorer;
		public static TextField explorerSearch;
		public static ListLayout explorerList;
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

		#region Properties
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
			propertyPage.AddElement(new Label("Editing " + ep.type.displayName));
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

			propertyPage.AddElement(new Label("Direction: (+/- 1)"));
			{
				t = new TextField();
				t.actionUpdate = (tf) =>
				{
					tf.text = "" + ep.direction;
				};
				t.actionEnter = (txt) =>
				{
					int res = 0;
					if (int.TryParse(txt, out res)) if (res == 1 || res == -1) ep.direction = res;
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

			propertyPage.AddElement(new Label("Name:"));
			{
				t = new TextField();
				t.actionUpdate = (tf) =>
				{
					tf.text = ep.name;
				};
				t.actionEnter = (txt) =>
				{
					ep.name = txt;
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
		#endregion

		#region Explorer
		public static List<string> placeableEntities = new List<string>();
		public static void FindEntities()
		{
			placeableEntities.Clear();

			foreach (KeyValuePair<string, PackageInfo> kvp in PackageManager.availablePackages)
			{
				if (kvp.Value.type != PackageType.Entity) continue;
				if (kvp.Key.StartsWith("Entity/"))
				{
					string name = kvp.Key.Substring("Entity/".Length);
					placeableEntities.Add(name);
					EntityDef.LoadEntity(name);
				}
				else if (kvp.Key.StartsWith(GameState.scene.name + "/"))
				{
					string name = kvp.Key.Substring(GameState.scene.name.Length + 1);
					placeableEntities.Add(name);
					EntityDef.LoadEntity(name);
				}
			}
			placeableEntities.Sort();
		}

		public static ExplorerMode explorerMode = ExplorerMode.Entity;
		public static string explorerFolder = "-";
		public static string selectedEntity = "";
		public static void ExplorerGo(ExplorerMode mode, string folder)
		{
			const int folderHeight = 20;

			if (explorerMode == mode && explorerFolder == folder) return;
			ExplorerClear();
			explorer.scroll = Point.Zero;

			Predicate<string> isSelected = (s) => false;
			Action<string> onSelect = null;
			//Action<SpriteBatch, Rectangle, string> draw = null;

			explorerFolder = folder;
			if (folder != "") explorerList.AddElement(new ExplorerEntry("(up one level)", (s) => false, (s) => { ExplorerUp(); }, ExplorerDrawString).SetHeight(folderHeight));

			if (mode == ExplorerMode.Entity)
			{
				isSelected = (s) => s == selectedEntity;
				onSelect = (s) => selectedEntity = s;

				List<string> folders = new List<string>();
				foreach (string str in placeableEntities)
				{
					if (!str.Contains('/')) continue;
					if (!str.StartsWith(explorerFolder)) continue;
					string nstr = str.Substring(0, str.LastIndexOf('/'));
					if (nstr.LastIndexOf('/') != explorerFolder.LastIndexOf('/')) continue; // forget more than one folder in
					nstr += "/";
					if (nstr == explorerFolder) continue;
					if (!folders.Contains(nstr)) folders.Add(nstr);
				}
				foreach (string str in folders)
				{
					explorerList.AddElement(new ExplorerEntry(str, (s) => false, (s) => { ExplorerGo(explorerMode, s); }, ExplorerDrawFolder).SetHeight(folderHeight));
				}

				List<string> entities = new List<string>();// placeableEntities;
				foreach (string str in placeableEntities)
				{
					if (!str.StartsWith(explorerFolder)) continue;
					if (str.LastIndexOf('/') > explorerFolder.Length) continue;
					entities.Add(str);
				}

				foreach (string str in entities)
				{
					explorerList.AddElement(new ExplorerEntry(str, isSelected, onSelect, ExplorerDrawEntity));
				}
			}
		}
		public static void ExplorerSearchDelegate(ref string s, ref int c) { ExplorerSearch(s); }
		public static void ExplorerSearch(string query)
		{
			string qtolow = query.ToLower();
			if (query == "")
			{
				string f = explorerFolder;
				explorerFolder = "-";
				ExplorerGo(explorerMode, f);
			}

			else if (explorerMode == ExplorerMode.Entity)
			{
				ExplorerClear();

				Predicate<string> isSelected = (s) => s == selectedEntity;
				Action<string> onSelect = (s) => selectedEntity = s;
				
				List<string> search = placeableEntities.FindAll((s) => EntityDef.defs[s].displayName.ToLower().Contains(qtolow)).OrderBy((s) => EntityDef.defs[s].displayName).ToList();
				foreach (string str in search)
				{
					explorerList.AddElement(new ExplorerEntry(str, isSelected, onSelect, ExplorerDrawEntity));
				}
			}
		}
		public static void ExplorerUp()
		{
			string folder = explorerFolder;
			folder = folder.Substring(0, folder.Length - 1);
			if (!folder.Contains('/')) folder = "";
			else folder = folder.Substring(0, folder.LastIndexOf('/') - 1);
			ExplorerGo(explorerMode, folder);
		}
		public static void ExplorerClear()
		{
			explorerList.children.Clear();
			explorerList.AddElement(explorerSearch);
		}

		public static void ExplorerDrawString(SpriteBatch sb, Rectangle rect, string str)
		{
			Vector2 measure = UI.Font.MeasureString(str);
			sb.DrawString(UI.Font, str, (new Vector2(rect.Center.X, rect.Center.Y) - measure / 2f).Pixelize(), UI.colorText);
		}
		public static void ExplorerDrawFolder(SpriteBatch sb, Rectangle rect, string str)
		{
			if (str.EndsWith("/")) str = str.Substring(0, str.Length - 1);
			if (!str.Contains('/')) ExplorerDrawString(sb, rect, str);
			else
			{
				ExplorerDrawString(sb, rect, str.Substring(str.LastIndexOf('/') + 1));
			}
		}
		public static void ExplorerDrawEntity(SpriteBatch sb, Rectangle rect, string str)
		{
			Rectangle pr = rect; pr.Inflate(-1, -1); pr = pr.MarginLeft(pr.Height);

			EntityDef def = EntityDef.defs[str];
			def.editorEntity.DrawIcon(pr, sb);

			Vector2 measure = UI.Font.MeasureString(def.displayName);
			sb.DrawString(UI.Font, def.displayName, new Vector2(rect.X + rect.Height + 2, rect.Center.Y - measure.Y / 2f).Pixelize(), UI.colorText);
		}
		#endregion
	}

	public enum ExplorerMode { Entity, Texture }
}
