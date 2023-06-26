using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.Resources;
using KeePass.UI;
using KeePassLib;
using PluginTools;
using PluginTranslation;

namespace KPColorChange
{
  /// <summary>
  /// Description of Options.
  /// </summary>
  /// 
  public partial class Options : UserControl
  {
    public IPluginHost host;
    private int days;
    public int Days
    {
      get { return days; }
      set
      {
        days = value;
        warningDays.Text = value.ToString();
      }
    }

    private int m_expiredIcon;
    public int expiredIcon
    {
      get { return m_expiredIcon; }
      set { updateIcon(bExpired, value); }
    }
    private int m_expiringIcon;
    public int expiringIcon
    {
      get { return m_expiringIcon; }
      set { updateIcon(bExpiring, value); }
    }

    public Color expiredColor
    {
      get { return Color.FromName(cbExpiredColors.GetItemText(cbExpiredColors.SelectedItem)); }
      set { cbExpiredColors.SelectedIndex = cbExpiredColors.FindStringExact(value.Name); }
    }
    public Color expiringColor
    {
      get { return Color.FromName(cbExpiringColors.GetItemText(cbExpiringColors.SelectedItem)); }
      set { cbExpiringColors.SelectedIndex = cbExpiringColors.FindStringExact(value.Name); }
    }

    public bool ignoreTimeFraction
    {
      get { return cbUseDateOnly.Checked; }
      set { cbUseDateOnly.Checked = value; }
    }

    public bool hideExpired
    {
      get { return cbHideExpired.Checked; }
      set { cbHideExpired.Checked = value; }
    }

    public Options()
    {
      //
      // The InitializeComponent() call is required for Windows Forms designer support.
      //
      InitializeComponent();
      //
      // TODO: Add constructor code after the InitializeComponent() call.
      //
      PropertyInfo[] piColorList = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
      foreach (PropertyInfo c in piColorList)
      {
        cbExpiredColors.Items.Add(c.Name);
        cbExpiringColors.Items.Add(c.Name);
      }

      Text = PluginTranslate.PluginName;
      cgExpired.Text = KPRes.ExpiredEntries;
      cgExpiring.Text = PluginTranslate.SoonExpiring;
      lExpiredColor.Text = lSoonExpiringColor.Text = KPRes.BackgroundColor + ":";
      lSoonExpiringThreshold.Text = PluginTranslate.SoonExpiringWarning;
      cbUseDateOnly.Text = PluginTranslate.UseDateOnly;
      cbHideExpired.Text = PluginTranslate.HideExpired;
      lExpired.Text = lSoonExpiring.Text = KPRes.Icon + ":";
    }

    private void updateIcon(Button myButton, int iconId)
    {
      if (myButton == null) return;
      UIUtil.SetButtonImage(myButton, KeePassLib.Utility.GfxUtil.ScaleImage(host.MainWindow.ClientIcons.Images[iconId], DpiUtil.ScaleIntX(16), DpiUtil.ScaleIntY(16)), true);
      if (myButton.Name == bExpired.Name)
        m_expiredIcon = iconId;
      else if (myButton.Name == bExpiring.Name)
        m_expiringIcon = iconId;
    }

    void IconButtonClick(object sender, EventArgs e)
    {
      KeePass.Forms.IconPickerForm ipf = new KeePass.Forms.IconPickerForm();
      PwUuid customIcon = PwUuid.Zero;
      int iconId = 0;
      if ((sender as Button).Name == bExpired.Name)
        iconId = m_expiredIcon;
      else if ((sender as Button).Name == bExpiring.Name)
        iconId = m_expiringIcon;
      if ((iconId >= (int)PwIcon.Count) && (iconId <= (int)(PwIcon.Count + host.Database.CustomIcons.Count)))
        customIcon = host.Database.CustomIcons[iconId - (int)PwIcon.Count].Uuid;
      ipf.InitEx(host.MainWindow.ClientIcons, (uint)PwIcon.Count, host.Database, (uint)iconId, customIcon);
      if (ipf.ShowDialog() != DialogResult.OK)
      {
        UIUtil.DestroyForm(ipf);
        return;
      }
      if (!ipf.ChosenCustomIconUuid.Equals(PwUuid.Zero))
        iconId = (int)PwIcon.Count + host.Database.GetCustomIconIndex(ipf.ChosenCustomIconUuid);
      else
        iconId = (int)ipf.ChosenIconId;
      UIUtil.DestroyForm(ipf);
      updateIcon(sender as Button, iconId);
    }


    void OptionsValidating(object sender, CancelEventArgs e)
    {
      int dummy;
      if (int.TryParse(warningDays.Text, out dummy))
        Days = Math.Max(dummy, 1);
    }

    void drawItem(object sender, DrawItemEventArgs e)
    {
      if (e.Index < 0)
        return;
      string n = ((ComboBox)sender).Items[e.Index].ToString();
      Color c = Color.FromName(n);
      Brush b = new SolidBrush(c);
      e.Graphics.FillRectangle(b, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
    }
  }
}





















