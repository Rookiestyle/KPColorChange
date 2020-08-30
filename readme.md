[![Version](https://img.shields.io/github/release/rookiestyle/kpcolorchange)](https://github.com/rookiestyle/kpcolorchange/releases/latest)
[![Releasedate](https://img.shields.io/github/release-date/rookiestyle/kpcolorchange)](https://github.com/rookiestyle/kpcolorchange/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/rookiestyle/kpcolorchange/total?color=%2300cc00)](https://github.com/rookiestyle/kpcolorchange/releases/latest/download/KPColorChange.plgx)\
[![License: GPL v3](https://img.shields.io/github/license/rookiestyle/kpcolorchange)](https://www.gnu.org/licenses/gpl-3.0)

KPColorChange visually indicates whether an entry is expired or will expire soon by showing specific icons and background colors in the entry view.

Per default expired entries are drawn with red background color and the icon is changed to an "X" Symbol whereas entries expiring soon are drawn with orange background and the icon is changed to an exclamation mark.\
All settings including the warning threshold for entries expiring soon can be customized.

You can also decide to not show expired entries at all.\
In this case the statusbar will inform you how many entries are not shown because they are expired.


# Table of Contents
- [Configuration](#configuration)
- [Translations](#translations)
- [Download and Requirements](#download-and-requirements)

# Configuration
KPColorChange integrates into KeePass' options form.\
<img src="images/KPColorChange%20options.png" alt="Options" height="50%" width="50%" />

You can activate coloring of expired and soon expiring entries independently from each other.\
For both you can select a specific icon as well as a specific background color.\
<img src="images/KPColorChange%20active.png" alt="KPColorChange active" height="50%" width="50%" />

For expired entries you can also decide to not show them at all.\
This helps to have a *cleaner* display.\
You can toggle display of expired entries by clicking the toolbar button or by using the shortcut that can be set in the options.\
<img src="images/KPColorChange%20active%20and%20hiding%20expired.png" alt="KPColorChange active and hiding expired entries" height="50%" width="50%" />

An entry is considered to expire soon if it's expiry date is within the number of days you defined.\
Optionally, you can have KPColorChange compare the date only.\
In this case an entry expiring 5pm will be colored even before 5pm.

# Translations
KPColorChange is provided with English language built-in and allow usage of translation files.
These translation files need to be placed in a folder called *Translations* inside in your plugin folder.
If a text is missing in the translation file, it is backfilled with English text.
You're welcome to add additional translation files by creating a pull request as described in the [wiki](https://github.com/Rookiestyle/KPColorChange/wiki/Create-or-update-translations).

Naming convention for translation files: `KPColorChange.<language identifier>.language.xml`\
Example: `KPColorChange.de.language.xml`
  
The language identifier in the filename must match the language identifier inside the KeePass language that you can select using *View -> Change language...*\
If [EarlyUpdateCheck](https://github.com/rookiestyle/earlyupdatecheck) is installed, this identifier is shown there as well.

# Download and Requirements
## Download
Please follow these links to download the plugin file itself.
- [Download newest release](https://github.com/rookiestyle/kpcolorchange/releases/latest/download/KPColorChange.plgx)
- [Download history](https://github.com/rookiestyle/kpcolorchange/releases)

If you're interested in any of the available translations in addition, please download them from the [Translations](Translations) folder.
## Requirements
* KeePass: 2.41
