using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum.UIKit
{
	public class ScrollBar : UIElement
	{
		Button[] btns = new Button[3];
		Button buttonUp { get { return btns[0]; } }
		Button buttonDown { get { return btns[1]; } }
		Button buttonBar { get { return btns[2]; } }

		public bool isDragging = false;

		public ScrollField field;
		int scrollRange { get { return field.scrollBottom - field.bounds.Height; } }

		public override bool InterceptsMouse { get { return true; } }

		public ScrollBar()
		{
			btns[0] = new Button();
			btns[1] = new Button();
			btns[2] = new Button();

			buttonUp.parent = this;
			buttonDown.parent = this;
			buttonBar.parent = this;

			buttonUp.actionDown = () => ApplyButtonScroll(-1);
			buttonDown.actionDown = () => ApplyButtonScroll(1);

			buttonBar.draggable = true;
			buttonBar.actionDown = () => StartBarScroll();
			buttonBar.actionHeld = () => ApplyBarScroll();
		}

		void ApplyButtonScroll(int clicks)
		{
			field.MouseScroll(clicks);
		}

		Point startPoint;
		void StartBarScroll()
		{
			startPoint = new Point(buttonBar.bounds.X, buttonBar.bounds.Y);
		}
		void ApplyBarScroll()
		{
			int range = bounds.Height - (bounds.Width * 5);

			buttonBar.bounds.Y = ((Input.MousePosition - buttonBar.dragPointScreen) + startPoint).Y;

			if (buttonBar.bounds.Y < bounds.Width) buttonBar.bounds.Y = bounds.Width;
			if (buttonBar.bounds.Y > range + bounds.Width) buttonBar.bounds.Y = range + bounds.Width;

			int pos = buttonBar.bounds.Y - bounds.Width;

			field.scroll.Y = (pos * scrollRange) / range;
			isDragging = true;
		}

		public override void MouseOver(Point point)
		{
			foreach (Button b in btns) if (b.drawBounds.Contains(point)) b.MouseOver(b.drawBounds.PointWithin(point));
		}

		public override void Update()
		{
			buttonUp.bounds = innerBounds.MarginTop(bounds.Width);
			buttonDown.bounds = innerBounds.MarginBottom(bounds.Width);

			if (scrollRange <= 0)
			{
				buttonBar.bounds = new Rectangle();
			}
			else if (!isDragging)
			{
				buttonBar.bounds = innerBounds.MarginTop(bounds.Width * 3);
				buttonBar.bounds.Y += bounds.Width;
				int range = bounds.Height - (bounds.Width * 5);
				buttonBar.bounds.Y += (field.scroll.Y * range) / scrollRange;
			}

			isDragging = false;

			foreach (Button b in btns) b.Update();
		}

		public RenderTarget2D target;
		public override void Draw(SpriteBatch sb)
		{
			if (target == null || target.Width != bounds.Width || target.Height != bounds.Height)
			{
				PresentationParameters pp = sb.GraphicsDevice.PresentationParameters;
				target = new RenderTarget2D(sb.GraphicsDevice, bounds.Width, bounds.Height, false, pp.BackBufferFormat, pp.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
			}

			UI.TargetPush(target);
			sb.GraphicsDevice.Clear(UI.colorBacking);

			foreach (Button b in btns) b.Draw(sb);

			for (int i = 0; i < 5; i++)
			{
				int width = (i + 1) * 2;

				sb.DrawRect(new Rectangle((bounds.Width / 2) - (i + 1), 5 + i, width, 1), UI.colorText);
				sb.DrawRect(new Rectangle((bounds.Width / 2) - (i + 1), bounds.Height - (6 + i), width, 1), UI.colorText);
			}

			UI.TargetPop();
			sb.Draw(target, bounds, Color.White);
		}
	}
}
