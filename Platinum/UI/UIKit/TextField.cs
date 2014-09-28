using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Platinum.UIKit
{
	public class TextField : UIElement
	{
		public string text = "";
		public Action<string> actionEnter;
		public Action<string> actionUnfocus;
		public Action<TextField> actionUpdate;
		public InputValidationDelegate actionValidate;
		public int cursorIndex = 0;

		public override bool InterceptsMouse { get { return true; } }
		public override bool InterceptsScrollwheel { get { return UI.focusText == this; } }

		bool focusLastFrame;
		Point lastMouse;
		public override void MouseOver(Point point) { UI.TakeFocus(this); lastMouse = point; }
		public override void MouseAction(bool left, bool leftP, bool leftR, bool right, bool rightP, bool rightR)
		{
			if (leftP)
			{
				UI.focusText = this;
				FindCursorPos();
			}
		}
		public override void TextInput()
		{
			int oci = cursorIndex;
			cursorIndex += Input.OperateText(ref text, cursorIndex);
			cursorIndex = Math.Max(0, Math.Min(cursorIndex, text.Length));
			if (Input.KeyHeld(Keys.LeftControl))
			{
				if (Input.KeyPressed(Keys.C))
				{
					Input.SetClipboard(text);
				}
				if (Input.KeyPressed(Keys.X))
				{
					Input.SetClipboard(text);
					text = "";
					cursorIndex = 0;
				}
				if (Input.KeyPressed(Keys.V))
				{
					string clip = Input.GetClipboard();
					text = text.Insert(cursorIndex, clip);
					cursorIndex += clip.Length;
				}
			}
			if (actionValidate != null)
			{
				actionValidate(ref text, ref cursorIndex);
				cursorIndex = Math.Max(0, Math.Min(cursorIndex, text.Length));
			}
			if (oci != cursorIndex) AdjustScrolling();
			// enter as confirmation
			if (Input.KeyPressed(Keys.Enter))
			{
				if (actionEnter != null) actionEnter(text);
				if (actionUnfocus != null) actionUnfocus(text);
				UI.focusText = null;
				focusLastFrame = false;
			}
		}

		void FindCursorPos()
		{
			float cx = 0;
			float px = scroll.X + lastMouse.X - 3;

			for (int i = 0; i <= text.Length; i++)
			{
				Vector2 size = UI.Font.MeasureString(text.Substring(0, i));
				float ncx = size.X;
				if (Math.Abs(ncx - px) > Math.Abs(cx - px)) break;
				cx = ncx;
				cursorIndex = i;
			}
			AdjustScrolling();
		}
		void AdjustScrolling()
		{
			Vector2 size = UI.Font.MeasureString(text);
			Vector2 csize = UI.Font.MeasureString(text.Substring(0, cursorIndex));

			float cpos = 1 + csize.X;
			const int margin = 5;
			float left = scroll.X + margin;
			float right = ((bounds.Width - 4) + scroll.X) - margin;
			
			if (cpos < left) scroll.X += (int)(cpos - left);
			if (cpos > right) scroll.X += (int)(cpos - right);

			scroll.X = Math.Max(0, Math.Min(scroll.X, (int)size.X - (bounds.Width - 4)));
		}
		public override void MouseScroll(int clicks)
		{
			scroll.X += clicks * 15;
			Vector2 size = UI.Font.MeasureString(text);
			scroll.X = Math.Max(0, Math.Min(scroll.X, (int)size.X - (bounds.Width - 4)));
		}

		public override void Update()
		{
			if (UI.focusText == this) focusLastFrame = true;
			else
			{
				if (focusLastFrame && actionUnfocus != null) actionUnfocus(text);
				focusLastFrame = false;
			}
		}

		RenderTarget2D target;
		public override void Draw(SpriteBatch sb)
		{
			Rectangle db = drawBounds;
			Rectangle db2 = db; db2.Inflate(-2, -2);
			Rectangle db3 = db; db3.Inflate(-1, -1);

			Rectangle tbounds = db; tbounds.Inflate(-2, -2);
			if (target == null || target.Width != tbounds.Width || target.Height != tbounds.Height)
			{
				PresentationParameters pp = sb.GraphicsDevice.PresentationParameters;
				target = new RenderTarget2D(sb.GraphicsDevice, tbounds.Width, tbounds.Height, false, pp.BackBufferFormat, pp.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
			}
			Vector2 stringSize = UI.Font.MeasureString(text);
			Vector2 selectPoint = UI.Font.MeasureString(text.Substring(0, cursorIndex));

			sb.DrawRect(db, UI.colorBorder);
			sb.DrawRect(db3, UI.colorShadow);
			db3.Width--; db3.Height--;
			sb.DrawRect(db3, UI.colorHighlight);

			UI.TargetPush(target);
			sb.GraphicsDevice.Clear(UI.colorEntryField);

			if (UI.focusText == this)
			{
				LineSegment selLine = new LineSegment(new Vector2(selectPoint.X + 1 - scroll.X, 0), new Vector2(selectPoint.X + 1 - scroll.X, tbounds.Height));
				sb.DrawLine(selLine, UI.colorBacking.MultiplyBy(new Vector4(0.25f, 0.25f, 0.25f, 0.25f)), 3f);
				sb.DrawLine(selLine, UI.colorBacking);
			}

			Vector4 mult = Vector4.One;
			if (UI.focusText != this) mult = UI.colorMultTextInactive;
			sb.DrawString(UI.Font, text, new Vector2(1 - scroll.X, tbounds.Height / 2 - stringSize.Y / 2).Pixelize(), UI.colorText.MultiplyBy(mult));
			UI.TargetPop();

			sb.Draw(target, tbounds, Color.White);
		}
	}
}
