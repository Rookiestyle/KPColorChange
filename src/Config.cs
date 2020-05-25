using KeePass.App.Configuration;
using KeePassLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KPColorChange
{
	internal static class Config
	{
		private const string ConfigPrefix = "KPColorChange.";
		private static string m_ConfigShowToolbarButton = ConfigPrefix + "showToolbarButton";
		private static string m_ConfigAlreadyExpiredIcon = ConfigPrefix + "alreadyExpiredIcon";
		private static string m_ConfigAlreadyExpiredColor = ConfigPrefix + "alreadyExpiredColor";
		private static string m_ConfigAlreadyExpiredActive = ConfigPrefix + "alreadyExpiredActive";

		private static string m_ConfigSoonExpiredIcon = ConfigPrefix + "soonExpiringIcon";
		private static string m_ConfigSoonExpiredColor = ConfigPrefix + "soonExpiredColor";
		private static string m_ConfigSoonExpiredActive = ConfigPrefix + "soonExpiredActive";
		private static string m_ConfigIgnoreTimeFraction = ConfigPrefix + "ignoreTimeFraction";

		private static string m_ConfigHideExpired = ConfigPrefix + "hideExpired";
		private static string m_ConfigToggleKey = ConfigPrefix + "ToggleExpiredHotkey";

		//Timers of other plugins that will change the entry list view but do not call UpdateUI
		//We add our eventhandler to ensure we can color entries based on their expiry 
		private static string m_ConfigOtherTimers = ConfigPrefix + "HookTimers";

		private static AceCustomConfig m_conf = null;
		static Config()
		{
			m_conf = KeePass.Program.Config.CustomConfig;
		}

		internal static bool ShowToolbarButton
		{
			get { return m_conf.GetBool(m_ConfigShowToolbarButton, true); }
			set { m_conf.SetBool(m_ConfigShowToolbarButton, value); }
		}

		internal static bool AlreadyExpiredActive
		{
			get { return m_conf.GetBool(m_ConfigAlreadyExpiredActive, true); }
			set { m_conf.SetBool(m_ConfigAlreadyExpiredActive, value); }
		}
		internal static int AlreadyExpiredIcon
		{
			get { return (int)m_conf.GetLong(m_ConfigAlreadyExpiredIcon, (int)PwIcon.Expired); }
			set { m_conf.SetLong(m_ConfigAlreadyExpiredIcon, (long)value); }
		}
		internal static Color AlreadyExpiredColor
		{
			get
			{
				try
				{
					return Color.FromName(m_conf.GetString(m_ConfigAlreadyExpiredColor, Color.Red.Name));
				}
				catch
				{
					return Color.Red;
				}
			}
			set { m_conf.SetString(m_ConfigAlreadyExpiredColor, value.Name); }
		}
		internal static bool HideExpired
		{
			get { return m_conf.GetBool(m_ConfigHideExpired, true); }
			set { m_conf.SetBool(m_ConfigHideExpired, value); }
		}

		internal static bool SoonExpiredActive
		{
			get { return m_conf.GetBool(m_ConfigSoonExpiredActive, true); }
			set { m_conf.SetBool(m_ConfigSoonExpiredActive, value); }
		}
		internal static int SoonExpiredIcon
		{
			get { return (int)m_conf.GetLong(m_ConfigSoonExpiredIcon, (int)PwIcon.Warning); }
			set { m_conf.SetLong(m_ConfigSoonExpiredIcon, (long)value); }
		}
		internal static Color SoonExpiredColor
		{
			get
			{
				try
				{
					return Color.FromName(m_conf.GetString(m_ConfigSoonExpiredColor, Color.Orange.Name));
				}
				catch
				{
					return Color.Red;
				}
			}
			set { m_conf.SetString(m_ConfigSoonExpiredColor, value.Name); }
		}
		internal static bool IgnoreTimeFraction
		{
			get { return m_conf.GetBool(m_ConfigIgnoreTimeFraction, true); }
			set { m_conf.SetBool(m_ConfigIgnoreTimeFraction, value); }
		}

		internal static Keys ToggleKey
		{
			get
			{
				string toggleKey = m_conf.GetString(m_ConfigToggleKey, Keys.None.ToString());
				try
				{
					return (Keys)Enum.Parse(typeof(Keys), toggleKey);
				}
				catch (Exception) { }
				return Keys.None;
			}
			set { m_conf.SetString(m_ConfigToggleKey, value.ToString()); }
		}

		internal static List<string> GetOtherTimers()
		{
			List<string> lTimer = new List<string>();
			string hookPluginTimers = m_conf.GetString(m_ConfigOtherTimers, "TrayTotpGT.liRefreshTimer,KeeTrayTOTP.liRefreshTimer,KeePassOTP.m_columnOTP.m_columnRefreshTimer");
			//Quick & Dirty fix as I changed the class holding the timer in my KeePassOTP plugin
			if (hookPluginTimers.Contains("KeePass") && !hookPluginTimers.Contains("KeePassOTP.m_columnOTP.m_columnRefreshTimer"))
				hookPluginTimers += ",KeePassOTP.m_columnOTP.m_columnRefreshTimer";
			m_conf.SetString(m_ConfigOtherTimers, hookPluginTimers);
			string[] sTimers = hookPluginTimers.Split(',');
			for (int i = 0; i < sTimers.Length; i++)
			{
				string sTimer = sTimers[i].Trim().Replace(" ", string.Empty);
				if (!string.IsNullOrEmpty(sTimer) && !lTimer.Contains(sTimer))
					lTimer.Add(sTimer);
			}
			return lTimer;
		}
	}
}
