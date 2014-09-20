using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LitJson;

namespace Platinum
{
	public static partial class Extensions
	{
		// read operations for basic types
		public static void Read(this JsonData j, string name, ref bool target) { if (j.IsObject && j.Has(name) && j.IsBoolean) target = (bool)j; }
		public static void Read(this JsonData j, string name, ref int target) { if (j.IsObject && j.Has(name) && j.IsInt) target = (int)j; }
		public static void Read(this JsonData j, string name, ref long target) { if (j.IsObject && j.Has(name) && j.IsLong) target = (long)j; }
		public static void Read(this JsonData j, string name, ref float target) { if (j.IsObject && j.Has(name) && j.IsDouble) target = (float)j; }
		public static void Read(this JsonData j, string name, ref double target) { if (j.IsObject && j.Has(name) && j.IsDouble) target = (double)j; }
		public static void Read(this JsonData j, string name, ref string target) { if (j.IsObject && j.Has(name) && j.IsString) target = (string)j; }
		
		// nullable versions
		public static void Read(this JsonData j, string name, ref bool? target) { if (j.IsObject && j.Has(name) && j.IsBoolean) target = (bool)j; }
		public static void Read(this JsonData j, string name, ref int? target) { if (j.IsObject && j.Has(name) && j.IsInt) target = (int)j; }
		public static void Read(this JsonData j, string name, ref long? target) { if (j.IsObject && j.Has(name) && j.IsLong) target = (long)j; }
		public static void Read(this JsonData j, string name, ref float? target) { if (j.IsObject && j.Has(name) && j.IsDouble) target = (float)j; }
		public static void Read(this JsonData j, string name, ref double? target) { if (j.IsObject && j.Has(name) && j.IsDouble) target = (double)j; }

		// less basic types!
		public static void Read(this JsonData j, string name, ref Vector2 target)
		{
			if (!j.IsObject || !j.Has(name)) return;
			JsonData sub = j[name];
			if (!sub.IsArray || sub.Count != 2) return;
			if (!(sub[0].IsLong || sub[0].IsInt) || !(sub[1].IsLong || sub[1].IsInt)) return;
			target = new Vector2((float)sub[0], (float)sub[1]);
		}

		public static void Read(this JsonData j, string name, ref VecRect target)
		{
			if (!j.IsObject || !j.Has(name) || !j[name].IsArray) return;
			JsonData sub = j[name];
			if (sub.Count == 2) // two pairs
			{
				if (!sub[0].IsArray || sub[1].Count != 2 || sub[1].IsArray || sub[1].Count != 2) return;
				if (!(sub[0][0].IsLong || sub[0][0].IsInt) || !(sub[0][1].IsLong || sub[0][1].IsInt)) return;
				if (!(sub[1][0].IsLong || sub[1][0].IsInt) || !(sub[1][1].IsLong || sub[1][1].IsInt)) return;
				target = new VecRect(new Vector2((float)sub[0][0], (float)sub[0][1]), new Vector2((float)sub[1][0], (float)sub[1][1]));
				return;
			}
			if (sub.Count == 4) // plain values
			{
				if (!(sub[0].IsLong || sub[0].IsInt) || !(sub[1].IsLong || sub[1].IsInt)) return;
				if (!(sub[2].IsLong || sub[2].IsInt) || !(sub[3].IsLong || sub[3].IsInt)) return;
				target = new VecRect(new Vector2((float)sub[0], (float)sub[1]), new Vector2((float)sub[2], (float)sub[3]));
			}
		}
	}
}
