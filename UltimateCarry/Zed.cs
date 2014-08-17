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
	class Zed : Champ
	{
		public static Obj_AI_Minion Clone_W = null;
		public static bool Clone_W_Created = false;
		public static bool Clone_W_Found = false;
		public static int Clone_W_Tick = 0;
		public static int W_Cast_Tick = 0;

		public static Obj_AI_Minion Clone_R = null;
		public static bool Clone_R_Created = false;
		public static bool Clone_R_Found = false;
		public static int Clone_R_Tick = 0;
		public static int R_Cast_Tick = 0;

		public static Vector3 Clone_R_nearPosition;

		public static int Delay = 100;
		public static int DelayTick = 0;

		public static Spell Q;
		public static Spell W;
		public static Spell E;
		public static Spell R;
		public static Obj_AI_Hero Target;
		public static bool Clonecasted = false;

		public Zed()
		{
			Id = "Zed";
			Obj_SpellMissile.OnCreate += OnSpellCast;

			Q = new Spell(SpellSlot.Q, 900);
			Q.SetSkillshot(0.235f, 50f, 1700, false, Prediction.SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 550);

			E = new Spell(SpellSlot.E, 290);

			R = new Spell(SpellSlot.R, 600);
		}

		private void OnSpellCast(GameObject sender, EventArgs args)
		{
			var Spell = (Obj_SpellMissile)sender;
			var Unit = Spell.SpellCaster.Name;
			var Name = Spell.SData.Name;

			if(Unit == ObjectManager.Player.Name && Name == "ZedShadowDashMissile")
				Clone_W_Created = true;
			if(Unit == ObjectManager.Player.Name && Name == "ZedUltMissile")
			{
				Clone_R_Created = true;
				Clone_R_nearPosition = ObjectManager.Player.ServerPosition;
			}
		}


		public override void Menu()
		{
			G.Menu_CreateBasicDrawMenu(true, false, true, true);

			G.Menu_CreateCostumItem(t.Menu_Farm, "use_Q_Farm", "Use Q", false);

			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_Q_LaneClear_Enemy", "Use Q on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_Q_LaneClear_Minion", "Use Q on Minion");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_E_LaneClear_Enemy", "Use E on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_E_LaneClear_Minion", "Use E on Minion");

			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_Q_Combo", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_W_Combo", "Use W Follow");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_E_Combo", "Use E");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_R_Combo", "Use R");

			G.Menu_CreateCostumItem(t.Menu_Harass, "use_Q_Harass", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Harass, "use_W_Harass", "Use W no Follow");
			G.Menu_CreateCostumItem(t.Menu_Harass, "use_E_Harass", "Use E");

			//G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_R_KS", "Use R");

		}

		internal override void OnGameUpdate(EventArgs args)
		{

			if(Clone_W_Created && !Clone_W_Found)
				SearchForClone("W");
			if(Clone_R_Created && !Clone_R_Found)
				SearchForClone("R");

			if(Clone_W != null && (Clone_W_Tick < Environment.TickCount - 4000))
			{
				Clone_W = null;
				Clone_W_Created = false;
				Clone_W_Found = false;
			}

			if(Clone_R != null && (Clone_R_Tick < Environment.TickCount - 6000))
			{
				Clone_R = null;
				Clone_R_Created = false;
				Clone_R_Found = false;
			}

			if(G.Menu_IsKeyActive(t.MenuItem_key_TeamfightIsActive))
				Combo();

			if(G.Menu_IsKeyActive(t.MenuItem_key_FarmIsActive))
				Lasthit();

			if(G.Menu_IsKeyActive(t.MenuItem_key_HarassIsActive) )
				Harass();

			if(G.Menu_IsKeyActive(t.MenuItem_key_LaneclearIsActive))
				LaneClear();
		}

		private static bool IsTeleportToClone(string Spell)
		{

			if(Spell == "W")
			{
				if(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "zedw2")
					return true;
			}
			if(Spell == "R") 
			{
				if(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "ZedR2")
					return true;
			}
			return false;
		}

		private static void SearchForClone(string p)
		{
			if(p == "W")
			{
				var Shadow = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(hero => (hero.Name == "Shadow" && hero.IsAlly && (hero != Clone_R)));
				if(Shadow != null)
				{
					Clone_W = Shadow;
					Clone_W_Found = true;
					Clone_W_Tick = Environment.TickCount;
				}
			}
			if(p == "R")
			{
				var Shadow = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(hero => ((hero.ServerPosition.Distance(Clone_R_nearPosition)) < 50) && hero.Name == "Shadow" && hero.IsAlly && hero != Clone_W );
				if(Shadow != null)
				{
					Clone_R = Shadow;
					Clone_R_Found = true;
					Clone_R_Tick = Environment.TickCount;
				}
			}
		}

		internal override void OnGameDraw(EventArgs args)
		{
			if(G.Menu_IsMenuActive(t.MenuItem_bool_DrawingActive))
			{
				if(Clone_W != null)
				{
					Utility.DrawCircle(Clone_W.ServerPosition, Q.Range, System.Drawing.Color.Aquamarine);
					Utility.DrawCircle(Clone_W.ServerPosition, E.Range, System.Drawing.Color.Aquamarine);
				}
				if(Clone_R != null)
				{
					Utility.DrawCircle(Clone_R.ServerPosition, Q.Range, System.Drawing.Color.Aquamarine);
					Utility.DrawCircle(Clone_R.ServerPosition, E.Range, System.Drawing.Color.Aquamarine);
				}
			}
			G.Draw_RangeBasic(Q, null, E, R);
		}

		internal override void OnProcessPacket(EventArgs args)
		{
		}

		internal override void OnSendPacket(EventArgs args)
		{
		}

		internal override void OnGameInput(EventArgs args)
		{
		}

		//private static void Killsteal()
		//{
		//	foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone.Position) < R.Range) && hero.IsValidTarget(30000) && hero.Health < DamageLib.getDmg(hero, DamageLib.SpellType.R) && G.Menu_IsMenuActive("use_R_KS")))
		//		R.CastOnUnit(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
		//}

		private static bool IsEnoughEnergy(float energy)
		{
			if(energy <= ObjectManager.Player.Mana)
				return true;
			return false;
		}

		private static float GetCost(SpellSlot Spell)
		{
			return ObjectManager.Player.Spellbook.GetSpell(Spell).ManaCost;
		}

		private static void Combo()
		{
			if(G.Menu_IsMenuActive("use_R_Combo") && R.IsReady() && !IsTeleportToClone("R") && (IsEnoughEnergy(GetCost(SpellSlot.Q) + GetCost(SpellSlot.W)) ))
			{
				Target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
				if(Target != null) 
					R.CastOnUnit(Target, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
			}

			if (G.Menu_IsMenuActive("use_R_Combo") && R.IsReady() && !IsTeleportToClone("R") && (DamageLib.getDmg(Target, DamageLib.SpellType.R) > Target.Health))
				R.CastOnUnit(Target, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_Q_Combo", Q, SimpleTs.DamageType.Physical, "Enemy", !R.IsReady());
			
			if(G.Menu_IsMenuActive("use_W_Combo")  && !IsTeleportToClone("W"))
			{
				Target = SimpleTs.GetTarget(W.Range + Q.Range, SimpleTs.DamageType.Physical);
				if(Target != null)
				{
					if((W.IsReady() && Q.IsReady() && Target.IsValidTarget(Q.Range + W.Range) && IsEnoughEnergy(GetCost(SpellSlot.Q) + GetCost(SpellSlot.W)))
						|| (W.IsReady() && E.IsReady() && Target.IsValidTarget(W.Range + E.Range) && IsEnoughEnergy(GetCost(SpellSlot.W) + GetCost(SpellSlot.E)))
						|| (W.IsReady() && Target.IsValidTarget(E.Range + Orbwalking.GetRealAutoAttackRange(Target))))
					{
						DelayTick = Environment.TickCount;
						W.Cast(Target.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
					}
				}
			}

			if(G.Menu_IsMenuActive("use_E_Combo") && E.IsReady())
			{
				Target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
				{
					if(Target != null)
						E.Cast();
				}
			}


			if(Clone_W != null && G.Menu_IsMenuActive("use_E_Combo") && E.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_W.Position) < E.Range) && hero.IsValidTarget() && hero.IsVisible))
					E.Cast();

			if(Clone_R != null && G.Menu_IsMenuActive("use_E_Combo") && E.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_R.Position) < E.Range) && hero.IsValidTarget() && hero.IsVisible))
					E.Cast();

			if ( G.Menu_IsMenuActive("use_Q_Combo") & (Clone_W != null ) && Q.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_W.Position) < Q.Range) && hero.IsValidTarget() && hero.IsVisible ))
					Q.Cast(hero.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

			if ( G.Menu_IsMenuActive("use_Q_Combo") & (Clone_R != null ) && Q.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_R.Position) < Q.Range) && hero.IsValidTarget() && hero.IsVisible ))
					Q.Cast(hero.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
			
			if (G.Menu_IsMenuActive("use_W_Combo"))
			{
				Target = SimpleTs.GetTarget(E.Range , SimpleTs.DamageType.Physical); 
				if (Target == null)
				{
					if(Clone_W != null)
						foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_W.Position) < E.Range) && hero.IsValidTarget() && hero.IsVisible))
							W.Cast();
					if(Clone_R != null)
						foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_R.Position) < E.Range) && hero.IsValidTarget() && hero.IsVisible))
							W.Cast();
				}
			}

		}

		private static void Lasthit()
		{
			G.Spell_Cast_LineSkillshot(t.Menu_Farm, "use_Q_Farm", Q, SimpleTs.DamageType.Physical, "Minion", true, true);
		}

		private static void Harass()
		{

			if(G.Menu_IsMenuActive("use_W_Harass") && !IsTeleportToClone("W") && Delay < Environment.TickCount - DelayTick)
			{
				DelayTick = Environment.TickCount;
				Target = SimpleTs.GetTarget(W.Range + Q.Range, SimpleTs.DamageType.Physical);
				if(Target != null)
				{
					if((W.IsReady() && Q.IsReady() && Target.IsValidTarget(Q.Range + W.Range) && IsEnoughEnergy(GetCost(SpellSlot.Q) + GetCost(SpellSlot.W)))
						|| (W.IsReady() && E.IsReady() && Target.IsValidTarget(W.Range + E.Range) && IsEnoughEnergy(GetCost(SpellSlot.W) + GetCost(SpellSlot.E)))
						|| (W.IsReady() && Target.IsValidTarget(E.Range + Orbwalking.GetRealAutoAttackRange(Target))))
					{
						W.Cast(Target.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets)); 
					}
				}
			}

			if(G.Menu_IsMenuActive("use_E_Harass") && E.IsReady())
			{
				Target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
				{
					if(Target != null)
						E.Cast();
				}
			}

			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_Q_Harass", Q, SimpleTs.DamageType.Physical, "Enemy", !R.IsReady());

			if(Clone_W != null && G.Menu_IsMenuActive("use_E_Harass") && E.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_W.Position) < E.Range) && hero.IsValidTarget() && hero.IsVisible))
					E.Cast();

			if(Clone_R != null && G.Menu_IsMenuActive("use_E_Harass") && E.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_R.Position) < E.Range) && hero.IsValidTarget() && hero.IsVisible))
					E.Cast();

			if(G.Menu_IsMenuActive("use_Q_Harass") & (Clone_W != null) && Q.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_W.Position) < Q.Range) && hero.IsValidTarget() && hero.IsVisible))
					Q.Cast(hero.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

			if(G.Menu_IsMenuActive("use_Q_Harass") & (Clone_R != null) && Q.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone_R.Position) < Q.Range) && hero.IsValidTarget() && hero.IsVisible))
					Q.Cast(hero.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

		}

		private static void LaneClear()
		{
			G.Spell_Cast_LineSkillshot(t.Menu_Farm, "use_Q_LaneClear_Enemy", Q, SimpleTs.DamageType.Physical, "Enemy");
			G.Spell_Cast_LineSkillshot(t.Menu_Farm, "use_Q_LaneClear_Minion", Q, SimpleTs.DamageType.Physical, "Minion", true, true);
			G.Spell_Cast_LineSkillshot(t.Menu_Farm, "use_Q_LaneClear_Minion", Q, SimpleTs.DamageType.Physical, "Minion", true);
			
			if(G.Menu_IsMenuActive("use_E_LaneClear_Enemy") && E.IsReady())
			{
				Target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
				{
					if(Target != null)
						E.Cast();
				}
			}

			if(G.Menu_IsMenuActive("use_E_LaneClear_Minion") && E.IsReady())
			{
				var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
				foreach(var Minion in allMinions)
				{
					if(Minion != null)
						if(Minion.IsValidTarget(E.Range))
							if((DamageLib.getDmg(Minion, DamageLib.SpellType.E) > Minion.Health) || (DamageLib.getDmg(Minion, DamageLib.SpellType.E) + 100 < Minion.Health))
								E.Cast();
				}
			}
		}


	}
}
