using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public struct BitField
	{
		byte byteVal;

		public BitField(byte value) { byteVal = value; }
		public bool this[int index]
		{
			get
			{
				if (index >= 8 || index < 0) throw new IndexOutOfRangeException();
				byte mask = (byte)(Math.Pow(2, index));

				return ((byteVal & mask) != 0);
			}
			set
			{
				if (index >= 8 || index < 0) throw new IndexOutOfRangeException();
				byte mask = (byte)(Math.Pow(2, index));

				if (!value)
				{
					// false
					mask ^= (byte)255;
					byteVal &= mask;
				}
				else
				{
					byteVal |= mask;
				}
			}
		}
		public bool this[BitFlag index]
		{
			get { return this[(int)index.bitNumber]; }
			set { this[(int)index.bitNumber] = value; }
		}

		public static implicit operator byte(BitField b)
		{
			return b.byteVal;
		}
		public static implicit operator BitField(byte b)
		{
			return new BitField(b);
		}
	}
}
