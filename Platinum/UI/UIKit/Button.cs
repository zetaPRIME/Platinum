﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum.UIKit
{
	public class Button : UIElement
	{
		public Action actionDown, actionHeld, actionUp;
		public string text = "";

		int mstate = 0;
		bool allowHold = false;
		int framesOff = 3;

		public bool draggable = false;
		public Point dragPoint;
		public Point dragPointScreen;

		Point lastOver;

		public override bool InterceptsMouse { get { return true; } }
		public override void MouseOver(Point point)
		{
			lastOver = point;
			UI.TakeFocus(this);
		}
		public override void MouseAction(bool left, bool leftP, bool leftR, bool right, bool rightP, bool rightR)
		{
			// I guess this will do
			if (leftP) allowHold = true;
			if (!left || framesOff > 1) allowHold = false;

			mstate++;
			if (allowHold && left) mstate++;

			framesOff = 0;

			if (allowHold && left && draggable)
			{
				if (UI.focusDrag != this) { dragPoint = lastOver; dragPointScreen = Input.MousePosition; }
				UI.focusDrag = this;
			}

			if (leftP && actionDown != null) actionDown();
			if (leftR && allowHold && actionUp != null) actionUp();
			if (allowHold && left && actionHeld != null) actionHeld();
		}
		public override void Update()
		{
			mstate = 0;
			if (framesOff < 2) framesOff++;
		}

		public override void Draw(SpriteBatch sb)
		{
			Rectangle db = drawBounds;
			Rectangle db2 = db; db2.Inflate(-2, -2);
			Rectangle db3 = db; db3.Inflate(-1, -1);

			Color cH = UI.colorHighlight;
			Color cS = UI.colorShadow;

			Vector4 mult = Vector4.One;
			if (mstate == 1) mult = UI.colorMultMouseOver;
			else if (mstate == 2) { Color sw = cH; cH = cS; cS = sw; }

			sb.DrawRect(db, UI.colorBorder.MultiplyBy(mult));
			sb.DrawRect(db3, cS.MultiplyBy(mult));
			db3.Width--; db3.Height--;
			sb.DrawRect(db3, cH.MultiplyBy(mult));
			sb.DrawRect(db2, UI.colorBG.MultiplyBy(mult));

			if (text != "") {
				Vector2 size = UI.Font.MeasureString(text);
				Vector2 center = new Vector2(db.X + (db.Width / 2), db.Y + (db.Height / 2));

				string[] lines = text.Split('\n');
				//float lineHeight = size.Y / lines.Length;
				for (int i = 0; i < lines.Length; i++)
				{
					Vector2 linesize = UI.Font.MeasureString(lines[i]);
					Vector2 pos = new Vector2(center.X - linesize.X / 2, (center.Y - size.Y) + linesize.Y * i + linesize.Y * 0.5f).Pixelize();
					sb.DrawString(UI.Font, lines[i], pos, UI.colorText);
				}
			}

		}
	}
}
