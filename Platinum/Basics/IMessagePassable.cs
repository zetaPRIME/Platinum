using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public interface IMessagePassable
	{
		void Call(string name);
		void Call(string name, MessageDetails param);
	}
}
