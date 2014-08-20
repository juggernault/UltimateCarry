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
	class Lucian : Champ
	{
		public static Spell Q;
		public static Spell Q_2;
		public static Spell W;
		public static Spell E;
		public static Spell R;

		public static int E_useRange;
		public static bool R_Active;
		public static int SpellCastetTick;

		public static bool CanUseSpells = true;
		public static bool WaitingForBuff = false;
		public static bool GainBuff = false;

		public Lucian()
		{
			Id = "Lucian";

			Q = new Spell(SpellSlot.Q, 675);

			Q_2 = new Spell(SpellSlot.Q, 1100);
			Q_2.SetSkillshot(0.35f, 10f, float.MaxValue, true, Prediction.SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 1000);
			W.SetSkillshot(0.3f, 80f, 1600, true, Prediction.SkillshotType.SkillshotLine);

			E = new Spell(SpellSlot.E, 475);
			E.SetSkillshot(0.25f, 0.01f, float.MaxValue, false, Prediction.SkillshotType.SkillshotLine);

			R = new Spell(SpellSlot.R, 1400);
			R.SetSkillshot(0.01f, 110, 2800f, true, Prediction.SkillshotType.SkillshotLine);
		}

		public override void Menu()
		{
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_Q_Combo", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_W_Combo", "Use W");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_E_Combo", "Use E");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_R_Combo", "Use R", false);

			G.Menu_CreateCostumItem(t.Menu_Harass, "use_Q_Harass", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Harass, "use_W_Harass", "Use W");

			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_Q_LaneClear_Enemy", "Use Q on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_W_LaneClear_Enemy", "Use W on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_E_LaneClear_Enemy", "Use E on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_Q_LaneClear_Minion", "Use Q on Minion");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_W_LaneClear_Minion", "Use W on Minion");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_E_LaneClear_Minion", "Use E on Minion");

			G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_Q_KS", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_W_KS", "Use W");
			G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_R_KS", "Use R");

			Program.Menu.AddSubMenu(new Menu("Extra Setting", "additional"));
			Program.Menu.SubMenu("additional").AddItem(new MenuItem("E_Range", "Target must be in Range").SetValue(new Slider(1100, 2000, 500)));
			
			G.Menu_CreateBasicDrawMenu(true, true, true, true);

		}

		internal override void OnGameUpdate(EventArgs args)
		{
			E_useRange = Program.Menu.Item("E_Range").GetValue<Slider>().Value;

			BuffCheck();

			UltCheck();

			if(R_Active)
				return;

			if(G.Menu_IsKeyActive(t.MenuItem_key_TeamfightIsActive))
				Combo();

			if(G.Menu_IsKeyActive(t.MenuItem_key_FarmIsActive))
				Lasthit();

			if(G.Menu_IsKeyActive(t.MenuItem_key_HarassIsActive))
				Harass();

			if(G.Menu_IsKeyActive(t.MenuItem_key_LaneclearIsActive)) 
				LaneClear();

			//if(G.Menu_IsMenuActive(t.MenuItem_bool_KillstealIsActive))
			//	Killsteal();
		}

		internal override void OnGameDraw(EventArgs args)
		{
			G.Draw_RangeBasic(Q, W, E, R); 
		}

		private static void Killsteal() 
		{


			//var KillTarget = ObjectManager.Get<Obj_AI_Hero>().First(Hero => Hero.IsValidTarget(Q.Range) && Q.GetPrediction(Hero).HitChance >= Prediction.HitChance.HighHitchance && Q.IsReady() && Hero.Health < DamageLib.getDmg(Hero, DamageLib.SpellType.Q));
			//if(KillTarget != null)
			//{
			//	Chat.Write("MYDmgQ: " + DamageLib.getDmg(KillTarget, DamageLib.SpellType.Q).ToString() + " hislife: " + KillTarget.Health.ToString());
			//	Cast_Speazial_Q("Enemy", KillTarget); 
			//}
			//var KillTarget = ObjectManager.Get<Obj_AI_Hero>().First(Hero => Hero.IsValidTarget(W.Range) && Q.GetPrediction(Hero).HitChance >= Prediction.HitChance.HighHitchance && W.IsReady() && Hero.Health < DamageLib.getDmg(Hero, DamageLib.SpellType.W));
			//if(KillTarget != null)
			//{
			//	Chat.Write("MYDmgW: " + DamageLib.getDmg(KillTarget, DamageLib.SpellType.W).ToString() + " hislife: " + KillTarget.Health.ToString());

			//	W.Cast(KillTarget, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
			//	UsedSkill();
			//}
			//KillTarget = ObjectManager.Get<Obj_AI_Hero>().First(Hero => Hero.IsValidTarget(R.Range) && Q.GetPrediction(Hero).HitChance >= Prediction.HitChance.HighHitchance && R.IsReady() && Hero.Health < DamageLib.getDmg(Hero, DamageLib.SpellType.R));
			//if(KillTarget != null)
			//{
			//	R.Cast(KillTarget, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
			//	UsedSkill();
			//}
		}
		
		private static void Combo()
		{
			if(G.Spell_Cast_onMousePos(t.Menu_Teamfight, "use_E_Combo", E, SimpleTs.DamageType.Magical, E_useRange,"Enemy", CanUseSpells))
				UsedSkill();
			if(G.Menu_IsMenuActive("use_Q_Combo"))
				Cast_Speazial_Q("Enemy");
			if(G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_W_Combo", W, SimpleTs.DamageType.Magical, "Enemy", CanUseSpells))
				UsedSkill();
			if(G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_R_Combo", R, SimpleTs.DamageType.Physical, "Enemy", CanUseSpells))
				UsedSkill();
		}

		private static void Lasthit()
		{
		}

		private static void Harass()
		{
			if(G.Menu_IsMenuActive("use_Q_Harass"))
				Cast_Speazial_Q("Enemy");
			if(G.Spell_Cast_LineSkillshot(t.Menu_Harass, "use_W_Harass", W, SimpleTs.DamageType.Magical, "Enemy", CanUseSpells))
				UsedSkill();

		}

		private static void LaneClear() 
		{
			if(G.Spell_Cast_onMousePos(t.Menu_Laneclear, "use_E_LaneClear_Enemy", E, SimpleTs.DamageType.Magical, E_useRange, "Enemy", CanUseSpells))
				UsedSkill();
			if(G.Menu_IsMenuActive("use_Q_LaneClear_Enemy"))
				Cast_Speazial_Q("Enemy");
			if(G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_W_LaneClear_Enemy", W, SimpleTs.DamageType.Magical, "Enemy", CanUseSpells))
				//	UsedSkill();
			if(G.Spell_Cast_onMousePos(t.Menu_Laneclear, "use_E_LaneClear_Minion", E, SimpleTs.DamageType.Magical, E_useRange, "Minion", CanUseSpells))
				UsedSkill();	
			if(G.Menu_IsMenuActive("use_Q_LaneClear_Minion"))
				Cast_Speazial_Q("Minion");
			if(G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_W_LaneClear_Minion", W, SimpleTs.DamageType.Magical, "Minion", CanUseSpells))
				UsedSkill();
		}

		private static void Cast_Speazial_Q(string Objekt,Obj_AI_Hero ForcedTarget = null)
		{
			if(Objekt == "Enemy")
			{
				var Target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
				if(ForcedTarget != null)
				{
					Target = ForcedTarget;
				}

				if(Target != null)
				{
					if((Target.IsValidTarget(Q.Range)) && CanUseSpells && Q.IsReady())
					{
						Q.Cast(Target, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
						UsedSkill();
					}
				}
				Target = SimpleTs.GetTarget(Q_2.Range, SimpleTs.DamageType.Physical);
				if(Target != null)
				{
					if((Target.IsValidTarget(Q_2.Range)) && CanUseSpells && Q.IsReady())
					{
						var Q_Collision = Q_2.GetPrediction(Target).CollisionUnitsList;
						foreach(var Q_CollisionChar in Q_Collision)
						{
							if(Q_CollisionChar.IsValidTarget(Q.Range))
							{
								Q.Cast(Q_CollisionChar, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
								UsedSkill();
							}
						}
					}
				}
			}
			if(Objekt == "Minion")
			{
				var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly );
				foreach(var Target in allMinions)
				{
					if(Target != null)
					{
						if(Q.IsReady() && CanUseSpells  && G.Menu_IsMenuActive("use_Q_LaneClear_Minion"))
						{
							Q.Cast(Target , G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
							UsedSkill();
						}
					}
				}
			}
		}

		private static void UsedSkill()
		{
			if(CanUseSpells)
			{
				CanUseSpells = false;
				SpellCastetTick = Environment.TickCount;
			}
		}

		private void UltCheck()
		{
			var Tempultactive = false;
			foreach(var buff in ObjectManager.Player.Buffs)
				if(buff.Name == "LucianR")
					Tempultactive = true;

			if(Tempultactive)
			{
				Program.Orbwalker.SetAttacks(false);
				R_Active = true;
			}
			if(!Tempultactive)
			{
				Program.Orbwalker.SetAttacks(true);
				R_Active = false;
			}
		}



		private static void BuffCheck()
		{
			if(CanUseSpells == false && WaitingForBuff == false && GainBuff == false)
			{
				WaitingForBuff = true;
			}

			if(WaitingForBuff == true)
				foreach(var buff in ObjectManager.Player.Buffs)
				{
					if(buff.Name == "lucianpassivebuff")
					{
						GainBuff = true;
					}
				}

			if(GainBuff == true)
			{
				WaitingForBuff = false;
				var tempgotBuff = false;
				foreach(var buff in ObjectManager.Player.Buffs)
				{
					if(buff.Name == "lucianpassivebuff")
					{
						tempgotBuff = true;
					}
				}
				if(tempgotBuff == false)
				{
					GainBuff = false;
					CanUseSpells = true;
				}
			}

			if(SpellCastetTick < Environment.TickCount - 1000 && WaitingForBuff == true)
			{
				WaitingForBuff = false;
				GainBuff = false;
				CanUseSpells = true;
			}
		}

	}
}
