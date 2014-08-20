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


// G.Chat_WriteLocal(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.CastRange.ToString() );
namespace UltimateCarry
{
	class Program
	{
		public const int Localversion = 10;

		public static Menu Menu;
		public static Orbwalking.Orbwalker Orbwalker;
		public static Champ Champ;
		public static List<string> Menuitemlist;

		private static void Main(string[] args)
		{
			Menuitemlist = new List<string>();
			Menuitemlist.Add(" ");
			CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
		}

		private static void Game_OnGameLoad(EventArgs args)
		{

			AutoUpdater.InitializeUpdater();

			G.Menu_LoadBasicMenu();

			var ChampionName = G.Player_GetName();

			if(ChampionName == "Ezreal")
				Champ = new Ezreal();
			if(ChampionName == "Gnar")
				Champ = new Gnar();
			if(ChampionName == "Lucian")
				Champ = new Lucian();
			if(ChampionName == "Zed")
				Champ = new Zed();

			if(Champ == null)
				Champ = new Champ();

			Champ.Menu();

			Menu.AddToMainMenu();

			Drawing.OnDraw += Drawing_OnDraw;
			Game.OnGameUpdate += Game_OnGameUpdate;
		}

		private static void Game_OnGameUpdate(EventArgs args)
		{
			Activator.OnGameUpdate(args);
			Champ.OnGameUpdate(args);
		}

		private static void Drawing_OnDraw(EventArgs args)
		{
			Champ.OnGameDraw(args);
		}

		

	}
}
