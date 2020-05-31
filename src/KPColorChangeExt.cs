using System;
using System.Drawing;
using System.Windows.Forms;

using KeePass;
using KeePass.Plugins;
using KeePass.UI;

using KeePassLib;

using PluginTranslation;
using PluginTools;
using System.Collections.Generic;
using KeePassLib.Utility;
using KeePassLib.Collections;
using KeePassLib.Delegates;

namespace KPColorChange
{
	public enum ExpiryStatus
	{
		NotExpiring,
		ExpiringSoon,
		Expired
	}

	public sealed class KPColorChangeExt : Plugin
	{
		#region members
		private IPluginHost m_host = null;

		private ToolStripMenuItem m_MenuOptions = null;
		private ToolStripMenuItem m_MenuExpiring = null;
		private ToolStripMenuItem m_MenuExpiringSingleDB = null;
		private ToolStripMenuItem m_MenuExpiringMultiDB = null;
		private ToolStrip m_MainToolBar = null;
		private ToolStripButton m_tsbToggle = null;

		//Timers of other plugins that will change the entry list view but do not call UpdateUI
		//We add our eventhandler to ensure we can color entries based ln their expiry 
		List<string> m_lTimers = new List<string>();
		private Dictionary<string, object> m_OtherTimers = new Dictionary<string, object>();
		#endregion

		public override bool Initialize(IPluginHost host)
		{
			Terminate();
			if (host == null) return false;
			m_host = host;

			PluginTranslate.Init(this, Program.Translation.Properties.Iso6391Code);
			Tools.DefaultCaption = PluginTranslate.PluginName;
			Tools.PluginURL = "https://github.com/rookiestyle/kpcolorchange/";

			PluginDebug.AddInfo("Display of expired entries: " + (Config.HideExpired ? "Hide" : "Show"), 1);
			m_lTimers = Config.GetOtherTimers();

			AddMenu();
			m_host.MainWindow.UIStateUpdated += OnUIStateUpdated;
			m_host.MainWindow.FormLoadPost += MainWindow_FormLoadPost;

			Tools.OptionsFormShown += OptionsFormShown;
			Tools.OptionsFormClosed += OptionsFormClosed;
			m_host.MainWindow.KeyDown += ToggleHideExpiredKey;
			m_host.MainWindow.KeyPreview = true;
			if (Config.ShowToolbarButton)
			{
				m_MainToolBar = (ToolStrip)Tools.GetControl("m_toolMain");
				if (m_MainToolBar != null)
				{
					m_tsbToggle = new ToolStripButton();
					m_tsbToggle.Name = "m_tbToggleExpired";
					m_tsbToggle.ToolTipText = PluginTranslate.HideExpiredToolBar;
					m_tsbToggle.Image = SmallIcon;
					m_tsbToggle.Click += (o, e) => ToggleHideExpired(null, null);
					m_tsbToggle.CheckState = Config.HideExpired ? CheckState.Checked : CheckState.Unchecked;
					m_tsbToggle.Checked = Config.HideExpired;

					m_MainToolBar.Items.Add(new ToolStripSeparator());
					m_MainToolBar.Items.Add(m_tsbToggle);
				}
				else
					PluginDebug.AddError("Could not locate m_toolMain", 0);
			}
			if (m_tsbToggle != null) m_tsbToggle.Enabled = Config.AlreadyExpiredActive;

			m_host.MainWindow.DocumentManager.ActiveDocumentSelected += HidingAllowedReset;
			PwGroup.GroupTouched += HidingAllowedReset;

			return true;
		}

		#region Handle other plugins' timers
		private void MainWindow_FormLoadPost(object sender, EventArgs e)
		{
			HookOtherTimers(false);

			m_OtherTimers.Clear();
			if (m_lTimers.Contains("None"))
			{
				PluginDebug.AddInfo("Hooking other timers disabled", 0);
			}
			else
			{
				foreach (string sTimer in m_lTimers)
				{
					string[] timer = sTimer.Split(new char[] { '.' }, 2);
					if (timer.Length != 2)
					{
						PluginDebug.AddError("Invalid timer format", 0, "Expected: PluginNamespace.NameOfTimerVariable", "Found: " + sTimer);
						continue;
					}
					AddTimerIfExists(timer[0], timer[1]);
				}
			}

			HookOtherTimers(true);
		}

		private void AddTimerIfExists(string sPluginName, string sTimerName)
		{
			object p = Tools.GetPluginInstance(sPluginName);
			bool bPlugin = p != null;
			object o = null;
			List<string> fields = new List<string>(sTimerName.Split('.'));
			for (int i = 0; i < fields.Count - 1; i++)
			{
				if (p != null) p = Tools.GetField(fields[i], p);
			}
			if (p != null)
			{
				o = Tools.GetField(fields[fields.Count - 1], p);
				if ((o is System.Timers.Timer) || (o is Timer))
					m_OtherTimers[sPluginName + "." + sTimerName] = o;
			}
			PluginDebug.AddInfo(sPluginName + " integration", 0, bPlugin ? "Plugin found" : "Plugin not found",
				o != null ? "Timer object found" : "Timer object not found (" + sTimerName + ")");
		}

		private void HookOtherTimers(bool activate)
		{
			foreach (KeyValuePair<string, object> kvp in m_OtherTimers)
			{
				if (kvp.Value == null)
				{
					PluginDebug.AddError("Hook other timers", 2, "Object: " + kvp.Key, "Hook/Unhook: " + (activate ? "Hook" : "Unhook"), "Error: Object is null");
					return;
				}
				if (kvp.Value is System.Timers.Timer)
				{
					if (activate)
						(kvp.Value as System.Timers.Timer).Elapsed += OnUIStateUpdated;
					else
						(kvp.Value as System.Timers.Timer).Elapsed -= OnUIStateUpdated;
				}
				if (kvp.Value is Timer)
				{
					if (activate)
						(kvp.Value as Timer).Tick += OnUIStateUpdated;
					else
						(kvp.Value as Timer).Tick -= OnUIStateUpdated;
				}
				PluginDebug.AddSuccess("Hook other timers", 2, "Object: " + kvp.Key, "Hook/Unhook: " + (activate ? "Hook" : "Unhook"));
			}
		}
		#endregion

		#region Plugin configuration
		private void OptionsFormShown(object sender, Tools.OptionsFormsEventArgs e)
		{
			Options options = new Options();
			options.host = m_host;
			options.Days = Program.Config.Application.ExpirySoonDays;
			if (Config.AlreadyExpiredIcon >= m_host.MainWindow.ClientIcons.Images.Count)
				options.expiredIcon = (int)PwIcon.Expired;
			else
				options.expiredIcon = Config.AlreadyExpiredIcon;
			if (Config.SoonExpiredIcon >= m_host.MainWindow.ClientIcons.Images.Count)
				options.expiringIcon = (int)PwIcon.Warning;
			else
				options.expiringIcon = Config.SoonExpiredIcon;
			options.expiredColor = Config.AlreadyExpiredColor;
			options.expiringColor = Config.SoonExpiredColor;
			options.ignoreTimeFraction = Config.IgnoreTimeFraction;
			options.hideExpired = Config.HideExpired;
			options.hkcToggle.HotKey = Config.ToggleKey;
			options.cgExpired.Checked = Config.AlreadyExpiredActive;
			options.cgExpiring.Checked = Config.SoonExpiredActive;
			Tools.AddPluginToOptionsForm(this, options);
		}

		private void OptionsFormClosed(object sender, Tools.OptionsFormsEventArgs e)
		{
			if (e.form.DialogResult != DialogResult.OK) return;
			bool shown = false;
			Options options = (Options)Tools.GetPluginFromOptions(this, out shown);
			if (!shown) return;
			Program.Config.Application.ExpirySoonDays = options.Days;
			Config.AlreadyExpiredIcon = options.expiredIcon;
			Config.SoonExpiredIcon = options.expiringIcon;
			Config.AlreadyExpiredColor = options.expiredColor;
			Config.SoonExpiredColor = options.expiringColor;
			Config.IgnoreTimeFraction = options.ignoreTimeFraction;
			Config.ToggleKey = options.hkcToggle.HotKey;
			Config.SoonExpiredActive = options.cgExpiring.Checked;
			Config.AlreadyExpiredActive = options.cgExpired.Checked;
			m_tsbToggle.Enabled = Config.AlreadyExpiredActive;
			if ((Config.HideExpired != options.hideExpired))
			{
				if (m_tsbToggle != null)
					m_tsbToggle.Checked = options.hideExpired;
				Config.HideExpired = options.hideExpired;
				PluginDebug.AddInfo("Display of expired entries: " + (Config.HideExpired ? "Hide" : "Show"), 1);
			}
			m_host.MainWindow.UpdateUI(false, null, false, null, true, null, false);
		}
		#endregion

		#region Color (and hide) entries
		private void ToggleHideExpiredKey(object sender, KeyEventArgs e)
		{
			if ((Config.ToggleKey != Keys.None) && (e.KeyData == Config.ToggleKey))
				ToggleHideExpired(null, null);
		}

		private void ToggleHideExpired(object sender, EventArgs e)
		{
			Config.HideExpired = !Config.HideExpired;
			if (m_tsbToggle != null)
				m_tsbToggle.Checked = Config.HideExpired;

			PluginDebug.AddInfo("Display of expired entries: " + (Config.HideExpired ? "Hide" : "Show"), 0);
			m_host.MainWindow.UpdateUI(false, null, false, null, true, null, false);
		}

		private void OnUIStateUpdated(object sender, EventArgs e)
		{
			try
			{
				if (!Config.AlreadyExpiredActive && !Config.SoonExpiredActive) return;

				if ((m_host == null) || (m_host.Database == null) || !m_host.Database.IsOpen)
				{
					PluginDebug.AddInfo("No active database or database is not opened, nothing to do", 0);
					return;
				}

				ListView lv = (ListView)Tools.GetControl("m_lvEntries");
				if (lv == null)
				{
					PluginDebug.AddError("Could not find m_lvEntries", 0);
					return;
				}
				DateTime dtSoon = DateTime.Now.AddDays(Program.Config.Application.ExpirySoonDays);

				bool canHideExpired = true;
				int hidden = 0;
				PwGroup recycle = null;
				if (Config.HideExpired && Config.AlreadyExpiredActive)
				{
					canHideExpired = HidingAllowed(sender);
					recycle = m_host.Database.RecycleBinEnabled ? m_host.Database.RootGroup.FindGroup(m_host.Database.RecycleBinUuid, true) : null;
				}
				else PluginDebug.AddInfo("Hiding of expired entries", 0, "Active: " + false.ToString());

				if (!canHideExpired && !Config.AlreadyExpiredActive && !Config.SoonExpiredActive) return;

				lv.BeginUpdate();

				//don't use foreach and change the number of items... (thanks to darkdragon)
				//foreach (ListViewItem lvi in lv.Items)
				for (int i = lv.Items.Count - 1; i >= 0; i--)
				{
					ListViewItem lvi = lv.Items[i];
					PwListItem li = (lvi.Tag as PwListItem);
					if (li == null) continue;

					if (li.Entry == null)
					{
						PluginDebug.AddError("List entry does not contain valid PwEntry", 0, lvi.Text);
						continue; //should never happen but on the other side... you never know
					}
					ExpiryStatus expiry = EntryExpiry(li.Entry, dtSoon);
					if ((expiry == ExpiryStatus.Expired) && Config.AlreadyExpiredActive)
					{
						if (Config.HideExpired && canHideExpired)
						{
							if (!li.Entry.IsContainedIn(recycle))
							{
								lv.Items.RemoveAt(i);
								hidden++;
								PluginDebug.AddInfo("Hidden entry: " + li.Entry.Uuid.ToHexString(), 0, "Removed from list");
							}
							else PluginDebug.AddInfo("Hidden entry: " + li.Entry.Uuid.ToHexString(), 0, "Not removed from list, contained in recycle bin");
						}
						else
						{
							lvi.ImageIndex = Config.AlreadyExpiredIcon;
							lvi.BackColor = Config.AlreadyExpiredColor;
						}
					}
					else if (Config.SoonExpiredActive && (expiry == ExpiryStatus.ExpiringSoon))
					{
						lvi.ImageIndex = Config.SoonExpiredIcon;
						lvi.BackColor = Config.SoonExpiredColor;
					}
				}
				if (hidden > 0)
				{
					UIUtil.SetAlternatingBgColors(lv, UIUtil.GetAlternateColor(lv.BackColor), Program.Config.MainWindow.EntryListAlternatingBgColors);
					m_host.MainWindow.SetStatusEx(string.Format(PluginTranslate.HiddenExpired, hidden));
				}
				lv.EndUpdate();
			}
			catch (Exception ex)
			{
				bool bDM = PluginDebug.DebugMode;
				PluginDebug.DebugMode = true;
				PluginDebug.AddError("Exception during OnUIStateUpdate", -1, ex.Message, ex.Source, ex.StackTrace);
				PluginDebug.DebugMode = bDM;
			}
		}

		private bool HidingAllowed(object sender)
		{
			System.Diagnostics.StackTrace callStack = new System.Diagnostics.StackTrace();
			bool HidingAllowed = true;
			string m = string.Empty;
			foreach (var timer in m_OtherTimers)
			{
				if ((timer.Value != null) && (timer.Value == sender))
				{
					HidingAllowed = false;
					m = timer.Key;
					break;
				}
			}
			if (HidingAllowed)
			{
				for (int i = 0; i < callStack.FrameCount; i++)
				{
					string methodname = callStack.GetFrame(i).GetMethod().Name;
					HidingAllowed &= methodname != "ShowExpiredEntries";
					if (!Config.ProgressiveHidingAllowedCheck) HidingAllowed &= !methodname.StartsWith("OnPwList");
					HidingAllowed &= !methodname.StartsWith("OnFind");
					HidingAllowed &= methodname != "PerformQuickFind";
					HidingAllowed &= methodname != "OpenDatabase";
					HidingAllowed &= methodname != "OnFileLock";
					if ((methodname == "UpdateUIState") && (i < callStack.FrameCount - 1))
					{
						methodname = callStack.GetFrame(i + 1).GetMethod().Name;
						if (methodname == "WndProc")
						{
							HidingAllowed = false;
							methodname = "WndProc - message: m_nTaskbarButtonMessage";
						}
					}
					if (!HidingAllowed)
					{
						m = methodname;
						break;
					}
				}
			}
			List<string> lMsg = new List<string>();
			lMsg.Add("Active:" + Config.HideExpired.ToString());
			lMsg.Add("Progressive hiding check:" + Config.ProgressiveHidingAllowedCheck.ToString());

			//Progressive = Remember setting
			//Reset only if current callstack does not allow hiding expired entries
			if (Config.ProgressiveHidingAllowedCheck)
			{
				if (!HidingAllowed) Config.ProgressiveHidingAllowed = Config.HidingStatus.NotAllowed;
				//Set hiding status if not yet defined
				if (Config.ProgressiveHidingAllowed == Config.HidingStatus.Unknown)
				{
					if (HidingAllowed) Config.ProgressiveHidingAllowed = Config.HidingStatus.Allowed;
					else
					{
						Config.ProgressiveHidingAllowed = Config.HidingStatus.NotAllowed;
						lMsg.Add("Found method: " + m);
					}
				}
				lMsg.Add("Hiding possible: " + (Config.ProgressiveHidingAllowed == Config.HidingStatus.Allowed).ToString() + " - " + HidingAllowed.ToString());
				PluginDebug.AddInfo("Hiding of expired entries", 0, lMsg.ToArray());
				return Config.ProgressiveHidingAllowed == Config.HidingStatus.Allowed;
			}
			if (!HidingAllowed) lMsg.Add("Found method: " + m);
			lMsg.Add("Hiding possible:" + HidingAllowed.ToString());
			PluginDebug.AddInfo("Hiding of expired entries", 0, lMsg.ToArray());
			return HidingAllowed;
		}

		//Reset hiding allowed status if another document is selected or if another group is selected
		private void HidingAllowedReset(object sender, EventArgs e)
		{
			Config.ProgressiveHidingAllowed = Config.HidingStatus.Unknown;
		}

		private ExpiryStatus EntryExpiry(PwEntry entry, DateTime dtSoon)
		{
			if ((entry == null) || !entry.Expires) return 0;
			DateTime dtEntry = entry.ExpiryTime.ToLocalTime();
			DateTime dtNow = DateTime.Now;
			if (dtEntry <= dtNow) return ExpiryStatus.Expired;
			if (Config.IgnoreTimeFraction)
			{
				dtEntry = dtEntry.Date;
				dtSoon = dtSoon.Date;
				dtNow = dtNow.Date.AddSeconds(-1);
			}
			if (dtEntry <= dtSoon) return ExpiryStatus.ExpiringSoon;
			return ExpiryStatus.NotExpiring;
		}
		#endregion

		private void ShowExpiringEntriesSingleDB()
		{
			if ((m_host.Database == null) || !m_host.Database.IsOpen) return;
			DateTime dtSoon = DateTime.Now.AddDays(Program.Config.Application.ExpirySoonDays);

			PwGroup pg = GetExpiringEntries(m_host.Database, dtSoon);

			if (pg.Entries.UCount > 0)
				m_host.MainWindow.UpdateUI(false, null, true, pg, true, pg, false);
			else
				Tools.ShowInfo(PluginTranslate.NoEntries);
		}

		class DBExpiringEntries
		{
			public PwDatabase db;
			public PwGroup pg;
			public DBExpiringEntries()
			{
				db = null;
				pg = null;
			}
			public DBExpiringEntries(PwDatabase db, PwGroup pg)
			{
				this.db = db;
				this.pg = pg;
			}
		}
		private void ShowExpiringEntriesMultiDB()
		{
			if ((m_host.Database == null) || !m_host.Database.IsOpen) return;
			DateTime dtSoon = DateTime.Now.AddDays(Program.Config.Application.ExpirySoonDays);

			List<DBExpiringEntries> lExpiring = new List<DBExpiringEntries>();
			foreach (PwDatabase db in m_host.MainWindow.DocumentManager.GetOpenDatabases())
				lExpiring.Add(new DBExpiringEntries(db, GetExpiringEntries(db, dtSoon)));
			if (lExpiring.Count == 0)
			{
				Tools.ShowInfo(PluginTranslate.NoEntries);
				return;
			}

			Action<ListView> fInit = delegate (ListView lv)
			{
				int w = lv.ClientSize.Width - UIUtil.GetVScrollBarWidth();
				int wf = w / 4;
				int di = Math.Min(UIUtil.GetSmallIconSize().Width, wf);

				lv.Columns.Add(KeePass.Resources.KPRes.Database, wf + di);
				lv.Columns.Add(KeePass.Resources.KPRes.Title, wf);
				lv.Columns.Add(KeePass.Resources.KPRes.UserName, wf);
				lv.Columns.Add(KeePass.Resources.KPRes.ExpiryTime, wf - di);

				UIUtil.SetDisplayIndices(lv, new int[] { 0, 1, 2, 3 });
			};

			List<object> lEntries = new List<object>();

			//Prepare ImageList (CustomIcons can be different per database)
			ImageList il = new ImageList();
			ImageList il2 = (ImageList)Tools.GetField("m_ilCurrentIcons", m_host.MainWindow);
			foreach (Image img in il2.Images) il.Images.Add(img);

			foreach (DBExpiringEntries dbe in lExpiring)
			{
				foreach (PwEntry pe in dbe.pg.GetEntries(true))
				{
					PwGroup pge = pe.ParentGroup;
					if (pge != null)
					{
						if (lEntries.Find(x => (x is ListViewGroup) && ((x as ListViewGroup).Tag == pge)) == null)
						{
							ListViewGroup lvg = new ListViewGroup(pge.GetFullPath(" - ", pge.ParentGroup == null));
							lvg.Tag = pge;
							lEntries.Add(lvg);
						}
					}

					ListViewItem lvi = new ListViewItem(UrlUtil.GetFileName(dbe.db.IOConnectionInfo.Path));
					lvi.SubItems.Add(pe.Strings.ReadSafe(PwDefs.UserNameField));
					lvi.SubItems.Add(pe.Strings.ReadSafe(PwDefs.TitleField));
					lvi.SubItems.Add(pe.ExpiryTime.ToLocalTime().ToString());
					lvi.ImageIndex = (int)pe.IconId;
					if (!pe.CustomIconUuid.Equals(PwUuid.Zero))
					{
						il.Images.Add(dbe.db.GetCustomIcon(pe.CustomIconUuid, DpiUtil.ScaleIntX(16), DpiUtil.ScaleIntY(16)));
						lvi.ImageIndex = il.Images.Count - 1;
					}
					lvi.Tag = pe;
					lEntries.Add(lvi);
				}
			}

			KeePass.Forms.ListViewForm lvf = new KeePass.Forms.ListViewForm();
			lvf.InitEx(PluginTranslate.PluginName, PluginTranslate.SoonExpiring, null, SmallIcon, lEntries, il, fInit);

			UIUtil.ShowDialogAndDestroy(lvf);
			il.Dispose();
			if (lvf.DialogResult != DialogResult.OK) return;

			PwEntry peSelected = lvf.ResultItem as PwEntry;
			if (peSelected == null) return;
			DBExpiringEntries dbeSelected = lExpiring.Find(x => x.pg.FindEntry(peSelected.Uuid, true) != null);
			if (dbeSelected == null) return;

			m_host.MainWindow.UpdateUI(false, m_host.MainWindow.DocumentManager.FindDocument(dbeSelected.db), true, dbeSelected.pg, true, dbeSelected.pg, false);
			m_host.MainWindow.SelectEntries(new PwObjectList<PwEntry>() { peSelected }, true, true);
		}

		private PwGroup GetExpiringEntries(PwDatabase db, DateTime dtSoon)
		{
			PwGroup pg = new PwGroup(true, true, string.Empty, (PwIcon)Config.AlreadyExpiredIcon);
			pg.IsVirtual = true;
			if (db == null) return pg;
			if (!db.IsOpen) return pg;
			PwGroup recycle = db.RootGroup.FindGroup(db.RecycleBinUuid, true);
			EntryHandler eh = delegate (PwEntry pe)
			{
				if ((recycle != null) && pe.IsContainedIn(recycle))
					return true;
				if (EntryExpiry(pe, dtSoon) == ExpiryStatus.ExpiringSoon)
					pg.AddEntry(pe, false, false);
				return true;
			};

			db.RootGroup.TraverseTree(TraversalMethod.PreOrder, null, eh);
			return pg;
		}

		private void AddMenu()
		{
			//Options
			m_MenuOptions = new ToolStripMenuItem();
			m_MenuOptions.Text = PluginTranslate.PluginName + "...";
			m_MenuOptions.Image = SmallIcon;
			m_MenuOptions.Click += (o, e) => Tools.ShowOptions();
			m_host.MainWindow.ToolsMenu.DropDownItems.Add(m_MenuOptions);

			ToolStripItem[] targets = Tools.FindToolStripMenuItems(m_host.MainWindow.MainMenu.Items, "m_menuEditShowExp", true);
			if (targets.Length == 0)
				targets = Tools.FindToolStripMenuItems(m_host.MainWindow.MainMenu.Items, "m_menuFindExp", true);
			if (targets.Length == 0) return;
			ToolStripMenuItem target = (ToolStripMenuItem)targets[0];
			int index = target.Owner.Items.IndexOf(target);
			m_MenuExpiring = new ToolStripMenuItem();
			m_MenuExpiring.Text = PluginTranslate.SoonExpiring;
			m_MenuExpiring.Image = SmallIcon;
			m_MenuExpiring.Name = "m_menuEditShowExpKPCC";

			m_MenuExpiringSingleDB = new ToolStripMenuItem(PluginTranslate.MenuShowSoonExpiringSingleDB);
			m_MenuExpiringSingleDB.Name = "m_menuEditShowExpKPCC_SingleDB";
			m_MenuExpiringSingleDB.Click += (o, e) => ShowExpiringEntriesSingleDB();
			m_MenuExpiring.DropDownItems.Add(m_MenuExpiringSingleDB);

			m_MenuExpiringMultiDB = new ToolStripMenuItem(PluginTranslate.MenuShowSoonExpiringMultiDB);
			m_MenuExpiringMultiDB.Name = "m_menuEditShowExpKPCC_MultiDB";
			m_MenuExpiringMultiDB.Click += (o, e) => ShowExpiringEntriesMultiDB();
			m_MenuExpiring.DropDownItems.Add(m_MenuExpiringMultiDB);

			target.Owner.Items.Insert(index, m_MenuExpiring);
			(target.OwnerItem as ToolStripMenuItem).DropDownOpening += OnDropDownOpening;
		}

		private void OnDropDownOpening(object sender, EventArgs e)
		{
			m_MenuExpiringSingleDB.Enabled = (m_host.Database != null) && m_host.Database.IsOpen;
			m_MenuExpiringMultiDB.Enabled = m_host.MainWindow.DocumentManager.GetOpenDatabases().Count > 0;
			m_MenuExpiring.Enabled = m_MenuExpiringSingleDB.Enabled | m_MenuExpiringMultiDB.Enabled;
		}

		public override void Terminate()
		{
			if (m_host == null)
				return;
			m_host.MainWindow.DocumentManager.ActiveDocumentSelected -= HidingAllowedReset;
			PwGroup.GroupTouched -= HidingAllowedReset;
			m_host.MainWindow.KeyDown -= ToggleHideExpiredKey;
			if (m_MainToolBar != null)
			{
				int i = m_MainToolBar.Items.IndexOf(m_tsbToggle);
				if (i >= 0) m_MainToolBar.Items.RemoveAt(i - 1);
				m_MainToolBar.Items.Remove(m_tsbToggle);
			}
			m_host.MainWindow.UIStateUpdated -= OnUIStateUpdated;
			HookOtherTimers(false);
			m_host.MainWindow.ToolsMenu.DropDownItems.Remove(m_MenuOptions);
			m_MenuOptions.Dispose();

			if (m_MenuExpiring != null)
			{
				if (m_MenuExpiring.Owner != null)
				{
					(m_MenuExpiring.OwnerItem as ToolStripMenuItem).DropDownOpening -= OnDropDownOpening;
					m_MenuExpiring.Owner.Items.Remove(m_MenuExpiring);
				}
				m_MenuExpiring.Dispose();
			}
			PluginDebug.SaveOrShow();
			m_host = null;
		}

		public override string UpdateUrl
		{
			get { return "https://raw.githubusercontent.com/rookiestyle/kpcolorchange/master/version.info"; }
		}

		public override Image SmallIcon
		{
			get
			{
				return GfxUtil.ScaleImage(Resources.colorchange, 16, 16);
			}
		}
	}
}