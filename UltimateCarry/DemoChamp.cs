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
	class DemoChamp : Champ
	{
		public static Spell Q;
		public static Spell W;
		public static Spell E;
		public static Spell R;

		public DemoChamp()
		{
			Id = "DemoChamp";
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
		}

		internal override void OnGameUpdate(EventArgs args)
		{
		}

		internal override void OnGameDraw(EventArgs args)
		{
		}
	}
}