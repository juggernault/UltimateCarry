#region References

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Linq;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace UltimateCarry
{
	class Champ
	{
		public string Id = "";

		public virtual void Menu()
		{
		}

		internal virtual void OnGameUpdate(EventArgs args)
		{
		}

		internal virtual void OnGameDraw(EventArgs args)
		{
		}

		internal virtual void OnProcessPacket(EventArgs args)
		{
		}

		internal virtual void OnSendPacket(EventArgs args)
		{
		}

		internal virtual void OnGameInput(EventArgs args)
		{
		}

	}
}
