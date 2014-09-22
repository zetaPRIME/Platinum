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
		public static T2 Operate<T, T2>(this T2 list, Action<T> operation) where T2 : IEnumerable<T> { foreach (T t in list) operation(t); return list; }
		// specific versions
		public static List<T> Operate<T>(this List<T> list, Action<T> operation) { foreach (T t in list) operation(t); return list; }
		public static IOrderedEnumerable<T> Operate<T>(this IOrderedEnumerable<T> list, Action<T> operation) { foreach (T t in list) operation(t); return list; }

		// uncomment for lots of stupid // public static T _<T>(this T t, Action action) { action(); return t; }
	}
}
