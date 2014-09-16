using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public struct BitField32
	{
		UInt32 intVal;

		public BitField32(UInt32 value) { intVal = value; }
		public bool this[int index]
		{
			get
			{
				if (index >= 32 || index < 0) throw new IndexOutOfRangeException();
				UInt32 mask = (UInt32)(Math.Pow(2, index));

				return ((intVal & mask) != 0);
			}
			set
			{
				if (index >= 32 || index < 0) throw new IndexOutOfRangeException();
				UInt32 mask = (UInt32)(Math.Pow(2, index));

				if (!value)
				{
					// false
					mask ^= (byte)255;
					intVal &= mask;
				}
				else
				{
					intVal |= mask;
				}
			}
		}

		public static implicit operator UInt32(BitField32 b)
		{
			return b.intVal;
		}
		public static implicit operator BitField32(UInt32 b)
		{
			return new BitField32(b);
		}
	}
}
