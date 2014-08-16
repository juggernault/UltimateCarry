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


		public static GameObject Clone = null;
		public static int CloneTimer = 0;
		public static int W_Casttime = 0;
		public static int W_CheckTick = 0;
		public static bool W_Casted = false;
		public static Spell Q;
		public static Spell W;
		public static Spell E;

		public static bool Clonecasted = false;

		public Zed()
		{
			Id = "Zed";
			GameObject.OnCreate += OnCreateObject;
			GameObject.OnDelete += OnDeleteObject;
			Obj_SpellMissile.OnCreate += OnSpellCast;

			Q = new Spell(SpellSlot.Q, 925);
			Q.SetSkillshot(0.235f, 50f, 1700, false, Prediction.SkillshotType.SkillshotLine);

			W = new Spell(SpellSlot.W, 1300);

			E = new Spell(SpellSlot.E, 125);
		}

		private void OnSpellCast(GameObject sender, EventArgs args)
		{
			var Spell = (Obj_SpellMissile)sender;
			var Unit = Spell.SpellCaster.Name;
			var Name = Spell.SData.Name;

			if(Unit == ObjectManager.Player.Name && Name == "ZedShadowDashMissile")
				W_Casted = true;
			W_Casttime = Environment.TickCount;
		}





		private static void OnCreateObject(GameObject sender, EventArgs args)
		{
			if(sender.IsValid && sender.Name == "Shadow")
				if(Clone == null)
				{
					Clone = sender;
					CloneTimer = Environment.TickCount;
				}
			if(sender.IsValid && sender.Name == "Zed_Base_CloneDeath.troy")
			{
				Clone = null;
				W_Casted = false;
			}

		}

		private static void OnDeleteObject(GameObject sender, EventArgs args)
		{
		}

		public override void Menu()
		{
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_Q_Combo", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_W_Combo", "Use W Follow");
			G.Menu_CreateCostumItem(t.Menu_Teamfight, "use_E_Combo", "Use E");

			G.Menu_CreateCostumItem(t.Menu_Harass, "use_Q_Harass", "Use Q");
			G.Menu_CreateCostumItem(t.Menu_Harass, "use_W_Combo", "Use W no Follow");
			G.Menu_CreateCostumItem(t.Menu_Harass, "use_E_Harass", "Use E");

			Program.Menu.SubMenu(t.Menu_Drawing).AddItem(new MenuItem("color_Clone", "Color Clone").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
		}

		internal override void OnGameUpdate(EventArgs args)
		{
			//if( Clone != null && CloneTimer <= Environment.TickCount - 4000)
			//	Clone = null;  

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

			if(Clone != null && CloneTimer > Environment.TickCount - 4000)
				Utility.DrawCircle(Clone.Position, 100, G.Menu_GetColor("color_Clone"));
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

		}

		private static void Combo()
		{
			
			
			if(W_CheckTick < Environment.TickCount - 200 && G.Menu_IsMenuActive("use_E_Combo"))
			{
				W_CheckTick = Environment.TickCount; 
				var CastClone = (W_Casttime < Environment.TickCount - 4000);
				if( W.IsReady() && (Q.IsReady() || E.IsReady()))
					foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(ObjectManager.Player) < 1200) && hero.IsEnemy && !hero.IsDead))
							W.Cast(hero.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
			}


			G.Spell_Cast_LineSkillshot(t.Menu_Teamfight, "use_Q_Combo", Q, SimpleTs.DamageType.Physical, "Enemy");


			if(Clone != null && G.Menu_IsMenuActive("use_Q_Combo") && Q.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone.Position) < Q.Range) && hero.IsValidTarget(30000)))
				{
					Q.Cast(hero.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
				}
			if(G.Menu_IsMenuActive("use_E_Combo") && E.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(E.Range)))
					E.CastOnUnit(ObjectManager.Player);

			if(Clone != null && G.Menu_IsMenuActive("use_E_Combo") && E.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone.Position) < E.Range) && hero.IsValidTarget(30000)))
					E.CastOnUnit(ObjectManager.Player);
		}

		private static void Lasthit()
		{
			
		}

		private static void Harass()
		{

			if(W_CheckTick < Environment.TickCount - 200 && G.Menu_IsMenuActive("use_E_Harass"))
			{
				W_CheckTick = Environment.TickCount;
				var CastClone = (W_Casttime < Environment.TickCount - 4000);
				if(!W_Casted && W.IsReady() && (Q.IsReady() || E.IsReady()))
					foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(ObjectManager.Player) < 1200) && hero.IsEnemy && !hero.IsDead))
						if(!W_Casted)
							W.Cast(hero.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));
			}

			G.Spell_Cast_LineSkillshot(t.Menu_Harass, "use_Q_Harass", Q, SimpleTs.DamageType.Physical, "Enemy");


			if(Clone != null && G.Menu_IsMenuActive("use_Q_Harass") && Q.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone.Position) < Q.Range) && hero.IsEnemy))
					Q.Cast(hero.Position, G.Menu_IsMenuActive(t.MenuItem_bool_usePackets));

			if(G.Menu_IsMenuActive("use_E_Harass") && E.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(ObjectManager.Player) < E.Range) && hero.IsEnemy))
					E.CastOnUnit(ObjectManager.Player);

			if(Clone != null && G.Menu_IsMenuActive("use_E_Harass") && E.IsReady())
				foreach(var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => (hero.Distance(Clone.Position) < E.Range) && hero.IsEnemy))
					E.CastOnUnit(ObjectManager.Player);

		}

		private static void LaneClear()
		{

		}

		// Jump NOT to Clone
		//if(W_CheckTick < Environment.TickCount - 200)
		//	{
		//		W_CheckTick = Environment.TickCount; 
		//		var CastClone = (W_Casttime < Environment.TickCount - 4000);
		//		if(!W_Casted && W.IsReady() && (Q.IsReady() || E.IsReady()))
		//			W.Cast(Game.CursorPos); 
		//	}

		// Jump to Clone
		//if(W_CheckTick < Environment.TickCount - 200)
		//	{
		//		W_CheckTick = Environment.TickCount; 
		//		var CastClone = (W_Casttime < Environment.TickCount - 4000);
		//		if(W.IsReady() && (Q.IsReady() || E.IsReady()))
		//			W.Cast(Game.CursorPos); 
		//	}

	}
}
