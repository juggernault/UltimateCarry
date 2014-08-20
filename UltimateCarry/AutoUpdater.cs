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
	class AutoUpdater
	{
		public static int localversion = Program.Localversion;
		internal static bool isInitialized;

		internal static void InitializeUpdater()
		{
			isInitialized = true;
			UpdateCheck();
		}

		private static void UpdateCheck()
		{
			Chat.Write("UltimateCarry by Lexxes loading ...", "#33FFFF");
			var bgw = new BackgroundWorker();
			bgw.DoWork += bgw_DoWork;
			bgw.RunWorkerAsync();
		}

		private static void bgw_DoWork(object sender, DoWorkEventArgs e)
		{
			var myUpdater = new Updater("https://raw.githubusercontent.com/LXMedia1/Leage-Sharp/master/Versions/UltimateCarry.ver",
					"https://github.com/LXMedia1/Leage-Sharp/raw/master/UltimateCarry.exe", localversion);
			if(myUpdater.NeedUpdate)
			{
				Chat.Write("UltimateCarry is Updateing ...", "#33FFFF");
				Chat.Write("-- Using trellis Updater --", "#33FFFF");
				if(myUpdater.Update())
				{
					Chat.Write("UltimateCarry is Updateed, Reload Please.", "#33FFFF");
				}
			}
			else
			{
				Chat.Write(string.Format("UltimateCarry ( Version: {0} ) loaded!", localversion), "#33FFFF");
			}
		}
	}

	internal class Updater
	{
		private readonly string _updatelink;

		private readonly System.Net.WebClient _wc = new System.Net.WebClient
		{
			Proxy = null
		};
		public bool NeedUpdate = false;

		public Updater(string versionlink, string updatelink, int localversion)
		{
			_updatelink = updatelink;

			NeedUpdate = Convert.ToInt32(_wc.DownloadString(versionlink)) > localversion;
		}

		public bool Update()
		{
			try
			{
				if(
					System.IO.File.Exists(
						System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".bak"))
				{
					System.IO.File.Delete(
						System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".bak");
				}
				System.IO.File.Move(System.Reflection.Assembly.GetExecutingAssembly().Location,
					System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location) + ".bak");
				_wc.DownloadFile(_updatelink,
					System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location));
				return true;
			}
			catch(Exception ex)
			{
				Chat.Write("UltimateCarry-Updater Error: " + ex.Message, "#33FFFF");
				return false;
			}
		}
	}
}
