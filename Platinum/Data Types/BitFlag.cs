using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public struct BitFlag
	{
		public byte bitNumber { get; private set; }
		private BitFlag(int bit) : this() { bitNumber = (byte)bit; }

		public static implicit operator BitFlag(int inp) { return new BitFlag(inp); }

		public static UInt32 operator +(BitFlag a, BitFlag b)
		{
			return ((UInt32)Math.Pow(2, a.bitNumber) | (UInt32)Math.Pow(2, b.bitNumber));
		}
		public static UInt32 operator +(UInt32 i, BitFlag b)
		{
			return (i | (UInt32)Math.Pow(2, b.bitNumber));
		}
		public static UInt32 operator +(BitFlag b, UInt32 i)
		{
			return (i | (UInt32)Math.Pow(2, b.bitNumber));
		}
	}
}
