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
	class Item
	{
		public int ID;
		public string Name;
		public string Mapstring;
		public string Modestring;
		public int Range;

		public Item(int ID, string Name, string Mapstring, string Modestring, int Range = 0)
		{
			this.ID = ID;
			this.Name = Name;
			this.Modestring = Modestring;
			this.Range = Range;

			Mapstring = Mapstring.Replace("1", Utility.Map.MapType.SummonersRift.ToString());
			Mapstring = Mapstring.Replace("2", Utility.Map.MapType.TwistedTreeline.ToString());
			Mapstring = Mapstring.Replace("3", Utility.Map.MapType.CrystalScar.ToString());
			Mapstring = Mapstring.Replace("4", Utility.Map.MapType.HowlingAbyss.ToString());
			this.Mapstring = Mapstring;
		}

		internal bool isEnabled()
		{
			try
			{
				var ret = (Program.Menu.Item("Item" + ID.ToString() + Modestring).GetValue<bool>());
				return ret;
			}
			catch
			{
				return false;
			}
		}

		internal bool isMap()
		{
			return Mapstring.Contains(Utility.Map.GetMap().ToString());
		}
	}
}
