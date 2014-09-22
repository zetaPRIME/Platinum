using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum.UIKit
{
	public class ScrollField : UIElement
	{
		// consts
		public const int ScrollStep = 32;
		public const int Margin = 16;
		public const int ScrollbarSize = 16;

		// everything else
		public List<UIElement> children = new List<UIElement>();
		public void AddElement(UIElement element) { children.Add(element); element.parent = this; RecalculateCapacity(); }

		public int scrollBottom = 0;

		public RenderTarget2D target;
		public BitField border = 0; // UDLR

		public bool hasScrollbar = false;
		public ScrollBar scrollbar;

		public override bool InterceptsMouse { get { return true; } }
		public override bool InterceptsScrollwheel { get { return true; } }

		public override Rectangle innerBounds
		{
			get
			{
				if (hasScrollbar) return new Rectangle(0, 0, bounds.Width - ScrollbarSize, bounds.Height);
				return base.innerBounds;
			}
		}

		public override void MouseOver(Point point)
		{
			UI.TakeFocus(this);

			foreach (UIElement e in children) if (e.drawBounds.Contains(point)) e.MouseOver(e.drawBounds.PointWithin(point));

			if (scrollbar != null && scrollbar.bounds.Contains(point)) scrollbar.MouseOver(scrollbar.bounds.PointWithin(point));
		}
		public override void Update()
		{
			foreach (UIElement e in children) e.Update();

			if (hasScrollbar && scrollbar == null)
			{
				scrollbar = new ScrollBar();
				scrollbar.parent = this;
				scrollbar.field = this;
			}
			if (scrollbar != null)
			{
				scrollbar.bounds = innerBounds.MarginRight(ScrollbarSize);
				scrollbar.bounds.X += ScrollbarSize;

				scrollbar.Update();
			}
		}
		public override void MouseScroll(int clicks)
		{
			if (clicks == 0) return;
			scroll.Y += clicks * ScrollStep;

			if (scroll.Y > scrollBottom - bounds.Height) scroll.Y = scrollBottom - bounds.Height;
			if (scroll.Y < 0) scroll.Y = 0;
		}

		public override void Draw(SpriteBatch sb)
		{
			if (target == null || target.Width != bounds.Width || target.Height != bounds.Height) {
				PresentationParameters pp = sb.GraphicsDevice.PresentationParameters;
				target = new RenderTarget2D(sb.GraphicsDevice, bounds.Width, bounds.Height, false, pp.BackBufferFormat, pp.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
			}

			UI.TargetPush(target);
			sb.GraphicsDevice.Clear(UI.colorBG);

			if (border != 0)
			{
				Rectangle borderBounds = new Rectangle(0, 0, bounds.Width, bounds.Height);
				Color cb = UI.colorBorder;
				Color cib = new Color(cb, 0.5f);
				if (border[0])
				{
					sb.DrawRect(borderBounds.MarginTop(3), cib);
					sb.DrawRect(borderBounds.MarginTop(2), cib);
					sb.DrawRect(borderBounds.MarginTop(1), cb);
				}
				if (border[1])
				{
					sb.DrawRect(borderBounds.MarginBottom(3), cib);
					sb.DrawRect(borderBounds.MarginBottom(2), cib);
					sb.DrawRect(borderBounds.MarginBottom(1), cb);
				}
				if (border[2])
				{
					sb.DrawRect(borderBounds.MarginLeft(3), cib);
					sb.DrawRect(borderBounds.MarginLeft(2), cib);
					sb.DrawRect(borderBounds.MarginLeft(1), cb);
				}
				if (border[3])
				{
					sb.DrawRect(borderBounds.MarginRight(3), cib);
					sb.DrawRect(borderBounds.MarginRight(2), cib);
					sb.DrawRect(borderBounds.MarginRight(1), cb);
				}
			}

			foreach (UIElement e in children) e.Draw(sb);

			if (scrollbar != null) scrollbar.Draw(sb);

			UI.TargetPop();
			sb.Draw(target, drawBounds, Color.White);
		}

		public override void Refresh()
		{
			RecalculateCapacity();
		}
		public void RecalculateCapacity()
		{
			scrollBottom = 0;
			foreach (UIElement e in children)
			{
				int bottom = e.bounds.Bottom;
				if (bottom > scrollBottom) scrollBottom = bottom;
			}
			scrollBottom += Margin;
		}
	}
}
