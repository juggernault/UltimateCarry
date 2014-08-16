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
	class G
	{

		#region Chat

		public static void Chat_WriteLocal(string Message, string Color = "#33FFFF")
		{
			Game.PrintChat("<font color='{0}'>{1}</font>", Color, Message);
		}

		#endregion

		#region Draw

		public static void Draw_RangeBasic(Spell Q, Spell W, Spell E, Spell R)
		{
			if(Menu_IsMenuActive(t.MenuItem_bool_DrawingActive))
			{
				if(Q != null) if(Menu_IsColorActive("color_Q"))
						Draw_CircleonMe(Q.Range, "color_Q");
				if(W != null) if(Menu_IsColorActive("color_W"))
						Draw_CircleonMe(W.Range, "color_W");
				if(E != null) if(Menu_IsColorActive("color_E"))
						Draw_CircleonMe(E.Range, "color_E");
				if(R != null) if(Menu_IsColorActive("color_R"))
						Draw_CircleonMe(R.Range, "color_R");
			}
		}

		public static void Draw_CircleonMe(float Range, String Color)
		{
			Utility.DrawCircle(ObjectManager.Player.Position, Range, Menu_GetColor(Color));
		}
		#endregion

		#region Menu

		internal static void Menu_AddManamanager(string Menu, int basic)
		{
			Program.Menu.SubMenu(Menu).AddItem(new MenuItem(Menu + "manamanager", "Manamanager").SetValue(new Slider(basic, 100, 0)));
			Program.Menuitemlist.Add(Menu + "manamanager");
		}

		internal static void Menu_CreateCostumItem(string Menu, string identify, string text, bool standart = true)
		{
			Program.Menu.SubMenu(Menu).AddItem(new MenuItem(identify, text).SetValue(standart));
		}

		internal static void Menu_CreateBasicDrawMenu(bool Q, bool W, bool E, bool R)
		{
			if(Q)
				Program.Menu.SubMenu(t.Menu_Drawing).AddItem(new MenuItem("color_Q", "Color Q").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
			if(W)
				Program.Menu.SubMenu(t.Menu_Drawing).AddItem(new MenuItem("color_W", "Color W").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
			if(E)
				Program.Menu.SubMenu(t.Menu_Drawing).AddItem(new MenuItem("color_E", "Color E").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
			if(R)
				Program.Menu.SubMenu(t.Menu_Drawing).AddItem(new MenuItem("color_R", "Color R").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
		}

		public static System.Drawing.Color Menu_GetColor(string Menu)
		{
			return (Program.Menu.Item(Menu).GetValue<Circle>().Color);
		}

		public static bool Menu_IsColorActive(string Menu)
		{
			return (Program.Menu.Item(Menu).GetValue<Circle>().Active);
		}

		public static bool Menu_IsMenuActive(string Menu)
		{
			return (Program.Menu.Item(Menu).GetValue<bool>());
		}

		public static bool Menu_IsKeyActive(string Menu)
		{
			return (Program.Menu.Item(Menu).GetValue<KeyBind>().Active);
		}

		public static bool Menu_ManamanagerCheck(string Menu)
		{
			if(Program.Menuitemlist.Contains(Menu + "manamanager"))
			{
				var value = Program.Menu.Item(Menu + "manamanager").GetValue<Slider>().Value;
				if((ObjectManager.Player.Mana / ObjectManager.Player.MaxMana * 100) > value)
					return true;
				return false;
			}
			return true;
		}

		internal static void Menu_LoadBasicMenu()
		{

			Program.Menu = new Menu(Player_GetName(), Player_GetName(), true);

			var TargetSelectorMenu = new Menu(t.Menu_TargetSelector_text, t.Menu_TargetSelector);
			SimpleTs.AddToMenu(TargetSelectorMenu);
			Program.Menu.AddSubMenu(TargetSelectorMenu);

			Program.Menu.AddSubMenu(new Menu(t.Menu_Orbwalker_text, t.Menu_Orbwalker));
			Program.Orbwalker = new Orbwalking.Orbwalker(Program.Menu.SubMenu(t.Menu_Orbwalker));

			Program.Menu.AddSubMenu(new Menu(t.Menu_Packets_text, t.Menu_Packets));
			Program.Menu.SubMenu(t.Menu_Packets).AddItem(new MenuItem(t.MenuItem_bool_usePackets, t.MenuItem_bool_usePackets_text).SetValue(true));

			Program.Menu.AddSubMenu(new Menu(t.Menu_Teamfight_text, t.Menu_Teamfight));
			Program.Menu.SubMenu(t.Menu_Teamfight).AddItem(new MenuItem(t.MenuItem_key_TeamfightIsActive, t.MenuItem_key_TeamfightIsActive_text).SetValue(new KeyBind(32, KeyBindType.Press)));

			Program.Menu.AddSubMenu(new Menu(t.Menu_Farm_text, t.Menu_Farm));
			Program.Menu.SubMenu(t.Menu_Farm).AddItem(new MenuItem(t.MenuItem_key_FarmIsActive, t.MenuItem_key_FarmIsActive_text).SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

			Program.Menu.AddSubMenu(new Menu(t.Menu_Harass_text, t.Menu_Harass));
			Program.Menu.SubMenu(t.Menu_Harass).AddItem(new MenuItem(t.MenuItem_key_HarassIsActive, t.MenuItem_key_HarassIsActive_text).SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

			Program.Menu.AddSubMenu(new Menu(t.Menu_Laneclear_text, t.Menu_Laneclear));
			Program.Menu.SubMenu(t.Menu_Laneclear).AddItem(new MenuItem(t.MenuItem_key_LaneclearIsActive, t.MenuItem_key_LaneclearIsActive_text).SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

			Program.Menu.AddSubMenu(new Menu(t.Menu_Killsteal_text, t.Menu_Killsteal));
			Program.Menu.SubMenu(t.Menu_Killsteal).AddItem(new MenuItem(t.MenuItem_bool_KillstealIsActive, t.MenuItem_bool_KillstealIsActive_text).SetValue(true));


			Program.Menu.AddSubMenu(new Menu(t.Menu_Drawing_text, t.Menu_Drawing));
			Program.Menu.SubMenu(t.Menu_Drawing).AddItem(new MenuItem(t.MenuItem_bool_DrawingActive, t.MenuItem_bool_DrawingActive_text).SetValue(true));

		}

		#endregion

		#region Player

		public static string Player_GetName()
		{
			return ObjectManager.Player.BaseSkinName;
		}

		#endregion

		#region Spell

		public static Vector3 Spell_GetReversePosition(Vector3 Position)
		{
			var Tx = Position.X;
			var Ty = Position.Y;

			var Mx = ObjectManager.Player.Position.X;
			var My = ObjectManager.Player.Position.Y;

			var diffX = Mx - Tx;
			var diffY = My - Tx;

			Position.X = Mx + diffX;
			Position.Y = My + diffY;

			return Position;
		}

		public static bool Spell_CanKill_Percent(Obj_AI_Hero Enemy, Spell Spell, DamageLib.SpellType SpellType, int Percent = 100, bool Skillshot = false, bool Condition = true)
		{
			if(Skillshot)
			{
				if(Enemy.IsValidTarget(Spell.Range) && Spell.IsReady() && (DamageLib.getDmg(Enemy, SpellType) > Enemy.Health) && (Spell.GetPrediction(Enemy).HitChance >= Prediction.HitChance.HighHitchance))
					return true;
				return false;
			}
			if(Enemy.IsValidTarget(Spell.Range) && Spell.IsReady() && (DamageLib.getDmg(Enemy, SpellType) > Enemy.Health))
				return true;
			return false;
		}

		public static bool Spell_Cast_onMousePos(string MainMenu, string Menu, Spell spell, SimpleTs.DamageType DmgType, int Range,string Objekt = "Enemy", bool Condition = true, int EnemyNearCastRange = 1)
		{
		if(Menu_IsMenuActive(Menu) && Menu_ManamanagerCheck(MainMenu) && Condition)
			{
				if(Objekt == "Enemy")
				{
					var Target = SimpleTs.GetTarget(Range, DmgType);
					if(spell.IsReady() && Target.IsValidTarget(Range))
					{
						spell.Cast(Game.CursorPos, Menu_IsMenuActive(t.MenuItem_bool_usePackets));
						return true;
					}
				}
				if(Objekt == "Minion")
				{

					var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Range, MinionTypes.All, MinionTeam.NotAlly);
					foreach(var Target in allMinions)
					{
						if(Target != null)
						{
							if(Target.IsValidTarget(Range) && spell.IsReady())
							{
								spell.Cast(Game.CursorPos, Menu_IsMenuActive(t.MenuItem_bool_usePackets));
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public static bool Spell_Cast_LineSkillshot(string MainMenu, string Menu, Spell Spell, SimpleTs.DamageType DmgType, string Objekt = "Enemy", bool Condition = true, bool Lasthit = false, DamageLib.StageType Stage = DamageLib.StageType.Default  )
		{
			if(Menu_IsMenuActive(Menu) && Menu_ManamanagerCheck(MainMenu) && Condition)
			{
				if(Objekt == "Enemy")
				{
					var Target = SimpleTs.GetTarget(Spell.Range, DmgType);
					if(Target != null)
					{
						if(Target.IsValidTarget(Spell.Range) && Spell.IsReady())
						{
							if(Spell.GetPrediction(Target).HitChance >= Prediction.HitChance.HighHitchance)
							{
								Spell.Cast(Target, Menu_IsMenuActive(t.MenuItem_bool_usePackets));
								return true;
							}
						}
					}
				}
				if(Objekt == "Minion")
				{
					var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spell.Range, MinionTypes.All, MinionTeam.NotAlly );
					foreach(var Target in allMinions)
					{
						if(Target != null)
						{
							var spelltype = DamageLib.SpellType.AD;

							if(Spell.Slot.ToString() == "Q")
								spelltype = DamageLib.SpellType.Q;
							if(Spell.Slot.ToString() == "W")
								spelltype = DamageLib.SpellType.W;
							if(Spell.Slot.ToString() == "E")
								spelltype = DamageLib.SpellType.E;
							if(Spell.Slot.ToString() == "R")
								spelltype = DamageLib.SpellType.R;

							if(Target.IsValidTarget(Spell.Range) && Spell.IsReady())
							{
								if((Lasthit && (DamageLib.getDmg(Target, spelltype, Stage) > Target.Health) || (DamageLib.getDmg(Target, spelltype, Stage) + 100 < Target.Health) && !Lasthit))
								{
									Spell.Cast(Target.Position, Menu_IsMenuActive(t.MenuItem_bool_usePackets));
									return true;
								}
							}
						}
					}
				}
				if(Objekt == "KS")
				{

				}

			}
			return true;
		}

		#endregion
	}
}
