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
	class Ezreal : Champ
	{
		public static Spell Q;
		public static Spell W;
		public static Spell E;
		public static Spell R;
		public static int R_min;
		public Ezreal()
		{
			Id = "Ezreal";
			Q = new Spell(SpellSlot.Q, 1200);
			Q.SetSkillshot(0.25f, 60f, 2000f, true, Prediction.SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 1050);
			W.SetSkillshot(0.25f, 80f, 2000f, false, Prediction.SkillshotType.SkillshotLine);

			E = new Spell(SpellSlot.E, 475);
			E.SetSkillshot(0.25f, 80f, 1600f, false, Prediction.SkillshotType.SkillshotCircle);

			R = new Spell(SpellSlot.R, 3000);
			R.SetSkillshot(1f, 160f, 2000f, false, Prediction.SkillshotType.SkillshotLine);
		}

		public override void Menu()
		{
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_Q_Combo", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_W_Combo", "Use W");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_R_Combo", "Use R", false);

			G.Menu_CreateCostumItem(t.Menu_Farm, "use_Q_Farm", "Use Q");
			G.Menu_AddManamanager(t.Menu_Farm, 50);

			G.Menu_CreateCostumItem(t.Menu_Harass, "use_Q_Harass", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Harass, "use_W_Harass", "Use W");
			G.Menu_AddManamanager(t.Menu_Harass, 50);

			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_Q_LaneClear_Enemy", "Use Q on Enemy");
			G.Menu_CreateCostumItem(t.Menu_Laneclear, "use_Q_LaneClear_Minion", "Use Q on Minion");
			G.Menu_AddManamanager(t.Menu_Laneclear, 25);

			G.Menu_CreateBasicDrawMenu(true, true, true, false);

			G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_Q_KS", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_W_KS", "Use W");
			G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_E_KS", "Use E", false);
			G.Menu_CreateCostumItem(t.Menu_Killsteal, "use_R_KS", "Use R");

			Program.Menu.AddSubMenu(new Menu("Extra Setting", "additional"));
			Program.Menu.SubMenu("additional").AddItem(new MenuItem("minimum_R", "R Range minimum").SetValue(new Slider(1000, 1100, 0)));
			Program.Menu.SubMenu("additional").AddItem(new MenuItem("maximum_R", "R Range maximum").SetValue(new Slider(3000, 20000, 1000)));
		}

		internal override void OnGameUpdate(EventArgs args)
		{
			R.Range  = Program.Menu.Item("maximum_R").GetValue<Slider>().Value;
			R_min = Program.Menu.Item("minimum_R").GetValue<Slider>().Value;

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

		internal override void OnGameDraw(EventArgs args)
		{
			G.Draw_RangeBasic(Q, W, E, null);
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
				if (G.Spell_CanKill_Percent(hero,Q,DamageLib.SpellType.Q,95,true) && G.Menu_IsMenuActive("use_Q_KS"))
					Q.Cast(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

				if(G.Spell_CanKill_Percent(hero, W, DamageLib.SpellType.W, 95, true) && G.Menu_IsMenuActive("use_W_KS"))
					W.Cast(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

				if(G.Spell_CanKill_Percent(hero, E, DamageLib.SpellType.E, 95, true) && G.Menu_IsMenuActive("use_E_KS"))
					E.Cast(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

				if(G.Spell_CanKill_Percent(hero, R, DamageLib.SpellType.R, 95, true, (hero.Distance(ObjectManager.Player) > R_min)) && G.Menu_IsMenuActive("use_R_KS"))
					R.Cast(hero, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
			}
		}

		private static void Combo()
		{
			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_Q_Combo", Q,  SimpleTs.DamageType.Physical, "Enemy");
			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_W_Combo", W,  SimpleTs.DamageType.Physical, "Enemy");

			var Condition = (SimpleTs.GetTarget(R.Range , SimpleTs.DamageType.Physical).Distance(ObjectManager.Player) > R_min);
			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_R_Combo", R,  SimpleTs.DamageType.Physical, "Enemy", Condition);

		}

		private static void Lasthit()
		{
			G.Spell_Cast_LineSkillshot(t.Menu_Farm, "use_Q_Farm", Q,  SimpleTs.DamageType.Physical, "Minion", true, true);
		}

		private static void Harass()
		{
			G.Spell_Cast_LineSkillshot(t.Menu_Harass, "use_Q_Harass", Q,  SimpleTs.DamageType.Physical, "Enemy");
			G.Spell_Cast_LineSkillshot(t.Menu_Harass, "use_W_Harass", W,  SimpleTs.DamageType.Physical, "Enemy");
		}

		private static void LaneClear()
		{
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_Q_LaneClear_Enemy", Q,  SimpleTs.DamageType.Physical, "Enemy");
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_Q_LaneClear_Minion", Q,  SimpleTs.DamageType.Physical, "Minion", true, true);
			G.Spell_Cast_LineSkillshot(t.Menu_Laneclear, "use_Q_LaneClear_Minion", Q, SimpleTs.DamageType.Physical, "Minion", true, false);
		}
	}
}
