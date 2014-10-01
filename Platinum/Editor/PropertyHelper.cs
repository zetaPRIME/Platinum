using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LitJson;

using Platinum.UIKit;

namespace Platinum.Editor
{
	public static class PropertyHelper
	{
		public static void JsonProperty(List<UIElement> list, string name, string label, JsonData j)
		{
			Label l = new Label(label);
			List<UIElement> add = new List<UIElement>();
			add.Add(l);

			switch (j[name].GetJsonType())
			{
				case JsonType.Double:
					TextField t = new TextField();
					t.actionUpdate = (tf) =>
					{
						tf.text = "" + (double)j[name];
					};
					t.actionEnter = (txt) =>
					{
						double res = 0;
						if (double.TryParse(txt, out res)) j[name] = res;
					};
					add.Add(t);
					break;

				default: return;
			}

			list.AddRange(add);
		}
	}
}
