using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platinum
{
	public static class UI
	{
		static List<RenderTarget2D> targetStack = new List<RenderTarget2D>();
		public static SpriteFont Font { get { return Core.fontDebug; } }

		public static List<UIElement> elements = new List<UIElement>();

		// colors
		public static Color colorBG { get { return new Color(0.25f, 0.25f, 0.25f); } }
		public static Color colorBorder { get { return new Color(0.05f, 0.05f, 0.05f); } }
		public static Color colorHighlight { get { return new Color(0.75f, 0.75f, 0.75f); } }
		public static Color colorShadow { get { return new Color(0.125f, 0.125f, 0.125f); } }
		public static Color colorBacking { get { return new Color(0.5f, 0.5f, 0.5f); } }
		public static Color colorText { get { return Color.White; } }

		public static Vector4 colorMultMouseOver { get { return new Vector4(1.25f, 1.25f, 1.25f, 1f); } }
		//public static Vector4 colorMultMousePressed { get { return new Vector4(0.75f, 0.75f, 0.75f, 1f); } }

		// input focus
		public static UIElement focusMouse, focusScroll;
		public static UIElement focusDrag;

		public static void TargetPush(RenderTarget2D target)
		{
			targetStack.Add(target);
			try { Core.spriteBatch.End(); }
			catch (Exception e) { }
			Core.spriteBatch.GraphicsDevice.SetRenderTarget(target);
			Core.spriteBatch.CameraOff(false);
		}

		public static void TargetPop()
		{
			if (targetStack.Count == 0) return; // nope
			targetStack.RemoveAt(targetStack.Count - 1);
			RenderTarget2D newTarget;
			if (targetStack.Count == 0) newTarget = Core.screenTarget; // change
			else newTarget = targetStack[targetStack.Count - 1];

			try { Core.spriteBatch.End(); }
			catch (Exception e) { }
			Core.spriteBatch.GraphicsDevice.SetRenderTarget(newTarget);
			Core.spriteBatch.CameraOff(false);
		}

		public static void Update()
		{
			foreach (UIElement e in elements)
			{
				Rectangle db = e.drawBounds;
				if (db.Contains(Input.MousePosition)) e.MouseOver(db.PointWithin(Input.MousePosition));

				e.Update();
				
			}

			if (Input.mouseStateNow.LeftButton != ButtonState.Pressed) focusDrag = null;
			if (focusDrag != null)
			{
				TakeFocus(focusDrag);
			}

			if (Core.instance.IsActive)
			{
				if (focusMouse != null)
				{
					MouseState n = Input.mouseStateNow;
					MouseState p = Input.mouseStateLast;
					bool nl = n.LeftButton == ButtonState.Pressed;
					bool pl = p.LeftButton == ButtonState.Pressed;
					bool nr = n.RightButton == ButtonState.Pressed;
					bool pr = p.RightButton == ButtonState.Pressed;

					focusMouse.MouseAction(nl, nl && !pl, pl && !nl, nr, nr && !pr, pr && !nr);
				}
				if (focusScroll != null) focusScroll.MouseScroll(Input.MouseScroll);
			}

			focusMouse = focusScroll = null;
		}

		public static void Draw(SpriteBatch sb)
		{
			foreach (UIElement e in elements) e.Draw(sb);
		}

		public static void TakeFocus(UIElement element)
		{
			if (element.InterceptsMouse) focusMouse = element;
			if (element.InterceptsScrollwheel) focusScroll = element;
		}
	}
}
