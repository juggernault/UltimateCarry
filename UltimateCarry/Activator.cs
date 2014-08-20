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
	class Activator
	{

		internal static int Delaytick = 0;

		internal static void OnGameUpdate(EventArgs args)
		{
			if(Delaytick <= Environment.TickCount - 250)
			{
				Check_Summoners();
				Check_Potions();
				Check_Active_Items();
				Check_AntiStun_Me();
				Check_AntiStun_Friend();
				Delaytick = Environment.TickCount;
			}
		}

		private static void Check_Summoners()
		{
			var SumBarrier = Utility.GetSpellSlot(ObjectManager.Player, "SummonerBarrier");
			if(SumBarrier != SpellSlot.Unknown && ObjectManager.Player.SummonerSpellbook.CanUseSpell(SumBarrier) == SpellState.Ready)
			{
				if(ObjectManager.Player.Health < ObjectManager.Player.MaxHealth / 100 * 10)
					ObjectManager.Player.SummonerSpellbook.CastSpell(SumBarrier);
			}

			var SumHeal = Utility.GetSpellSlot(ObjectManager.Player, "SummonerHeal");
			if(SumHeal != SpellSlot.Unknown && ObjectManager.Player.SummonerSpellbook.CanUseSpell(SumHeal) == SpellState.Ready)
			{
				if(ObjectManager.Player.Health < ObjectManager.Player.MaxHealth / 100 * 10)
					ObjectManager.Player.SummonerSpellbook.CastSpell(SumHeal);
			}
			//var SumExhoust = Utility.GetSpellSlot(ObjectManager.Player, "SummonerExhaust");
			//if(SumExhoust != SpellSlot.Unknown && ObjectManager.Player.SummonerSpellbook.CanUseSpell(SumExhoust) == SpellState.Ready)
			//{
			//	if (G.Menu_IsKeyActive(t.MenuItem_key_TeamfightIsActive))
			//	{
			//		var MostdmgHerodmg = 0;
			//		var Obj_AI_Hero MostdmgHero;
			//		foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where( hero => hero.IsValidTarget(550 + 100)
			//		{
			//			if (MostdmgHerodmg < hero.BaseAttackDamage + hero.BaseAbilityDamage)
			//				MostdmgHerodmg = hero.
			//		}
			//	}
			//}
		}

		private static void Check_Active_Items()
		{
			if(G.Menu_IsKeyActive(t.MenuItem_key_TeamfightIsActive))
			{
				Check_HEXGUN();
				Check_HYDRA();
				Check_TIAMANT();
				Check_DFG();
				Check_BILGEWATER();
				Check_BOTRK();
			}
		}
		private static void Check_Potions()
		{
			if(Need_Health())
			{
				try
				{
					var ItemList = new List<Item>();
					ItemList.Add(new Item(2003, "Health Potion", "1,2,3,4", "Neutral"));
					ItemList.Add(new Item(2010, "Biscuit", "1,2,3,4", "Neutral"));
					ItemList.Add(new Item(2041, "Crystalline Flask", "1,2,3", "Neutral"));
					
					foreach(Item item in ItemList)
					{
						if(item.isMap())
							if(Items.CanUseItem(item.ID))
								if(item.isEnabled())
								{
									Items.UseItem(item.ID);
									return;
								}
					}
				}
				catch
				{
				}
			}
			if(Need_Mana())
			{
				var ItemList = new List<Item>();
				ItemList.Add(new Item(2004, "Mana Potion", "1,2,3,4", "Neutral"));
				ItemList.Add(new Item(2041, "Crystalline Flask", "1,2,3", "Neutral"));

				foreach(Item item in ItemList) 
				{
					if(item.isMap())
						if(Items.CanUseItem(item.ID))
							if(item.isEnabled())
							{
								Items.UseItem(item.ID);
								return;
							}
				}
			}
		}

		private static bool Need_Health()
		{
			if(!ObjectManager.Player.IsDead && ObjectManager.Player.Health < ObjectManager.Player.MaxHealth / 100 * 30)
				return (!ObjectManager.Player.Buffs.Any(buff => buff.Name == "RegenerationPotion" || buff.Name == "ItemCrystalFlask" || buff.Name == "ItemMiniRegenPotion"));
			return false;
		}

		private static bool Need_Mana()
		{
			if(!ObjectManager.Player.IsDead && ObjectManager.Player.Mana < ObjectManager.Player.MaxMana  / 100 * 30)
				return (!ObjectManager.Player.Buffs.Any(buff => buff.Name == "ItemCrystalFlask" || buff.Name == "FlaskOfCrystalWater"));
			return false; 
		}

		private static void Check_TIAMANT()
		{
			try
			{
				var Item = new Item(3077, "Tiamat", "1,2,3,4", "Active", 400);
				if(Item.isMap())
					if(Items.CanUseItem(Item.ID))
						if(Item.isEnabled())
						{
							var Targ = ObjectManager.Get<Obj_AI_Hero>().First(Hero => Hero.IsValidTarget(Item.Range));
							if(Targ != null)
								Items.UseItem(Item.ID);
						}
			}
			catch
			{
			}
		}

		private static void Check_HYDRA()
		{
			try
			{
				var Item = new Item(3074, "Ravenous Hydra", "1,2,3,4", "Active", 400);
				if(Item.isMap())
					if(Items.CanUseItem(Item.ID))
						if(Item.isEnabled())
						{
							var Targ = ObjectManager.Get<Obj_AI_Hero>().First(Hero => Hero.IsValidTarget(Item.Range));
							if(Targ != null)
								Items.UseItem(Item.ID);
						}
			}
			catch
			{
			}
		}

		private static void Check_HEXGUN()
		{
			try
			{
				var Item = new Item(3146, "Bilgewater Cutlass", "1,2,3,4", "Active", 700);
				var targ = SimpleTs.GetTarget(Item.Range, SimpleTs.DamageType.Magical );

				if(Item.isMap())
					if(Items.CanUseItem(Item.ID))
						if(Item.isEnabled())
							if(targ != null)
								Items.UseItem(Item.ID,targ);
			}
			catch
			{
			}
		}

		private static void Check_BILGEWATER()
		{
			try
			{
				var Item = new Item(3144, "Bilgewater Cutlass", "1,2,3,4", "Active", 450);
				var targ = SimpleTs.GetTarget(Item.Range, SimpleTs.DamageType.Physical);

				if(Item.isMap())
					if(Items.CanUseItem(Item.ID))
						if(Item.isEnabled())
							if(targ != null)
								Items.UseItem(Item.ID, targ);
			}
			catch
			{
			}
		}

		private static void Check_DFG()
		{
			try
			{
				var Item = new Item(3128, "Deathfire Grasp", "1,4", "Active", 750);
				var targ = SimpleTs.GetTarget(Item.Range, SimpleTs.DamageType.Magical );

				if(Item.isMap())
					if(Items.CanUseItem(Item.ID))
						if(Item.isEnabled())
							if(targ != null)
								Items.UseItem(Item.ID, targ);
			}
			catch
			{
			}
		}

		private static void Check_BOTRK()
		{
			try
			{
				var Item = new Item(3153, "Blade of the Ruined King", "1,2,3,4", "Active", 450);
				var targ = SimpleTs.GetTarget(Item.Range, SimpleTs.DamageType.Magical);

				if(Item.isMap())
					if(Items.CanUseItem(Item.ID))
						if(Item.isEnabled())
							if(targ != null)
								Items.UseItem(Item.ID, targ);
			}
			catch
			{
			}
		}

		private static void Check_AntiStun_Me()
		{
			try
			{
				var ItemList = new List<Item>();
				ItemList.Add(new Item(3139, "Mercurial Scimitar", "1,4", "Defensive"));
				ItemList.Add(new Item(3137, "Dervish Blade", "2,3", "Defensive"));
				ItemList.Add(new Item(3140, "Quicksilver Sash", "1,2,3,4", "Defensive"));

				foreach(Item item in ItemList)
				{
					if(item.isMap())
						if(Items.CanUseItem(item.ID))
							if(ObjectManager.Player.HasBuffOfType(BuffType.Snare) || ObjectManager.Player.HasBuffOfType(BuffType.Stun))
								if(item.isEnabled())
								{
									Items.UseItem(item.ID);
									return;
								}
				}
			}
			catch
			{
			}
		}

		private static void Check_AntiStun_Friend()
		{
			try
			{
				var Item = new Item(3222, "Mikael's Crucible", "1,2,3,4", "Defensive", 750); // 
				var Friend = ObjectManager.Get<Obj_AI_Hero>().First(Hero => Hero.IsAlly && !Hero.IsDead && (Hero.HasBuffOfType(BuffType.Snare) || Hero.HasBuffOfType(BuffType.Stun)) && Hero.Distance(ObjectManager.Player) <= Item.Range && Items.CanUseItem(Item.ID) && Item.isMap() && Item.isEnabled());
				if(Friend != null)
					Items.UseItem(Item.ID, Friend);
			}
			catch
			{
			}


		}
	}
}
