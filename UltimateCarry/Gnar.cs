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
	class Gnar : Champ
	{
		public static Spell Q_mini;
		public static Spell E_mini;

		public static Spell Q_mega;
		public static Spell W_mega;
		public static Spell E_mega;
		public static Spell R_mega;

		public static Spell Q;
		public static Spell W;
		public static Spell E;
		public static Spell R;

		public static int Delay = 0;

		public static string Transform_Soon = "gnartransformsoon";
		public static string Transformed = "gnartransform";

		public static int GnarState = 1;

		public Gnar()
		{
			Id = "Gnar";
			Q_mini = new Spell(SpellSlot.Q,1200);
			Q_mini.SetSkillshot(0.5f, 50f, 1200f, true, Prediction.SkillshotType.SkillshotLine);

			Q_mega = new Spell(SpellSlot.Q, 1200);
			Q_mega.SetSkillshot(0.5f, 70f, 1200f, true, Prediction.SkillshotType.SkillshotLine);

			W_mega = new Spell(SpellSlot.W, 525);
			W_mega.SetSkillshot(0.5f, 80f, float.MaxValue, false, Prediction.SkillshotType.SkillshotLine);

			E_mini = new Spell(SpellSlot.E, 550);
			E_mini.SetSkillshot(0.6f, 150f, float.MaxValue, false, Prediction.SkillshotType.SkillshotCircle);

			E_mega = new Spell(SpellSlot.E, 550);
			E_mega.SetSkillshot(0.6f, 350f, float.MaxValue, false, Prediction.SkillshotType.SkillshotCircle);

			R_mega = new Spell(SpellSlot.R, 590);
			R_mega.SetSkillshot(0.5f, 590f, float.MaxValue, false, Prediction.SkillshotType.SkillshotCircle);

		}

		public override void Menu()
		{
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_Q_Combo", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_W_Combo", "Use W");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_E_Combo", "Use E");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_R_Combo", "Use R");

			G.Menu_CreateCostumItem(t.Menu_Farm, "use_Q_Farm", "Use Q");
		//	G.Menu_AddManamanager(t.Menu_Farm, 50);

			G.Menu_CreateCostumItem(t.Menu_Harass, "use_Q_Harass", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Harass, "use_W_Harass", "Use W");
		//	G.Menu_AddManamanager(t.Menu_Harass, 50);

			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_Q_LaneClear_Enemy", "Use Q on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_Q_LaneClear_Minion", "Use Q on Minion");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_W_LaneClear_Enemy", "Use W on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_W_LaneClear_Minion", "Use W on Minion");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_E_LaneClear_Enemy", "Use E on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_E_LaneClear_Minion", "Use E on Minion");
		
			//	G.Menu_AddManamanager(t.Menu_Laneclear, 25);

			G.Menu_CreateBasicDrawMenu(true, true, true, true);

			G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_Q_KS", "Use Q");
		//	G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_W_KS", "Use W");
		//	G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_E_KS", "Use E", false);
		//	G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_R_KS", "Use R");

		//	Program.Menu.AddSubMenu(new Menu("Extra Setting", "additional"));
		//	Program.Menu.SubMenu("additional").AddItem(new MenuItem("minimum_R", "R Range minimum").SetValue(new Slider(1000, 1100, 0)));
		//	Program.Menu.SubMenu("additional").AddItem(new MenuItem("maximum_R", "R Range maximum").SetValue(new Slider(3000, 20000, 1000)));
		}

		internal override void OnGameUpdate(EventArgs args)
		{
			CheckState();

			if(GnarState == 3)
			{
				Q = Q_mega;
				E = E_mega;
			}
			else
			{
				Q = Q_mini;
				E = E_mini;
			}
			W = W_mega;
			R = R_mega;

			if(G.Menu_IsMenuActive(t.MenuItem_bool_KillstealIsActive))
				Killsteal();

			if(G.Menu_IsKeyActive(t.MenuItem_key_TeamfightIsActive))
				Combo();

			if(G.Menu_IsKeyActive(t.MenuItem_key_FarmIsActive))
				Lasthit();

			if(G.Menu_IsKeyActive(t.MenuItem_key_HarassIsActive))
				Harass();

			if(G.Menu_IsKeyActive(t.MenuItem_key_LaneclearIsActive))
				LaneClear();
		}

		private static void CheckState()
		{
			var TempState = 1;
			foreach(var buff in ObjectManager.Player.Buffs)
			{
				if(buff.Name == Transform_Soon)
					TempState = 2;
				if(buff.Name == Transformed)
					TempState = 3;
			}
			GnarState = TempState;
		}

		internal override void OnGameDraw(EventArgs args)
		{
			G.Draw_RangeBasic(Q_mini, W_mega, E_mini, R_mega );
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

		private static void Killsteal()
		{
			foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.IsValidTarget(20000))))
			{
				if(G.Spell_CanKill_Percent(hero, Q, DamageLib.SpellType.Q, 95, true) && G.Menu_IsMenuActive("use_Q_KS"))
					Q.Cast(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

			//	if(G.Spell_CanKill_Percent(hero, W, DamageLib.SpellType.W, 95, true) && G.Menu_IsMenuActive("use_W_KS"))
			//		W.Cast(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

			//	if(G.Spell_CanKill_Percent(hero, E, DamageLib.SpellType.E, 95, true) && G.Menu_IsMenuActive("use_E_KS"))
			//		E.Cast(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

			//	if(G.Spell_CanKill_Percent(hero, R, DamageLib.SpellType.R, 95, true, (hero.Distance(ObjectManager.Player) > R_min)) && G.Menu_IsMenuActive("use_R_KS"))
			//		R.Cast(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
			}
		}

		private static void Combo()
		{
			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_Q_Combo", Q, SimpleTs.DamageType.Physical, "Enemy");
			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_W_Combo", W, SimpleTs.DamageType.Physical, "Enemy",GnarState > 1);
			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_E_Combo", E, SimpleTs.DamageType.Physical, "Enemy", GnarState > 1);
			if(G.Menu_IsMenuActive("use_R_Combo") && G.Menu_ManamanagerCheck(t.Menu_Teamfight) && GnarState > 1 && R.IsReady() )
			{
				var Target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical);
				if(Target.IsValidTarget(R.Range))
				{
					var Position = G.Spell_GetReversePosition(Target.Position);
					R.Cast(Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
				}
			}

			//var Condition = (SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Physical).Distance(ObjectManager.Player) > R_min);
			//G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_R_Combo", R, SimpleTs.DamageType.Physical, "Enemy", Condition);

		}

		private static void Lasthit()
		{
			var DamageState = DamageLib.StageType.Default;

			if(GnarState != 1)
				DamageState = DamageLib.StageType.FirstDamage;

			G.Spell_Cast_LineSkillshot(t.Menu_Farm, "use_Q_Farm", Q, SimpleTs.DamageType.Physical, "Minion", true, true, DamageState);
		}

		private static void Harass()
		{
			G.Spell_Cast_LineSkillshot(t.Menu_Harass, "use_Q_Harass", Q, SimpleTs.DamageType.Physical, "Enemy");
			G.Spell_Cast_LineSkillshot(t.Menu_Harass, "use_W_Harass", W, SimpleTs.DamageType.Physical, "Enemy", GnarState > 1);
		}

		private static void LaneClear()
		{
			var DamageState = DamageLib.StageType.Default ;

			if(GnarState != 1)
				DamageState = DamageLib.StageType.FirstDamage;

			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_Q_LaneClear_Enemy", Q, SimpleTs.DamageType.Physical, "Enemy");
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_W_LaneClear_Enemy", W, SimpleTs.DamageType.Physical, "Enemy", GnarState > 1);
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_E_LaneClear_Enemy", E, SimpleTs.DamageType.Physical, "Enemy", GnarState > 1);
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_Q_LaneClear_Minion", Q, SimpleTs.DamageType.Physical, "Minion", true, true, DamageState);
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_Q_LaneClear_Minion", Q, SimpleTs.DamageType.Physical, "Minion", true, false, DamageState);
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_W_LaneClear_Minion", W, SimpleTs.DamageType.Physical, "Minion", GnarState > 1, true, DamageState);
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_W_LaneClear_Minion", W, SimpleTs.DamageType.Physical, "Minion", GnarState > 1, false, DamageState);
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_E_LaneClear_Minion", E, SimpleTs.DamageType.Physical, "Minion", GnarState > 1, true, DamageState);
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_E_LaneClear_Minion", E, SimpleTs.DamageType.Physical, "Minion", GnarState > 1, false, DamageState);
		
		}
	}
}
