using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platinum
{
	public class MessagePassable : IMessagePassable
	{
		protected Dictionary<string, Action<MessageDetails>> messageDefs = new Dictionary<string, Action<MessageDetails>>();

		public void Call(string name) { Call(name, new MessageDetails()); }
		public void Call(string name, MessageDetails param) { if (messageDefs.ContainsKey(name)) messageDefs[name](param); }
	}
}
