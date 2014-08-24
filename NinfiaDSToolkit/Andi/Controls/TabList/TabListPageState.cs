﻿using System;

namespace NinfiaDSToolkit.Andi.Controls.TabList
{
  // Cyotek TabList
  // Copyright (c) 2012-2013 Cyotek.
  // http://cyotek.com
  // http://cyotek.com/blog/tag/tablist

  // Licensed under the MIT License. See tablist-license.txt for the full text.

  // If you use this control in your applications, attribution, donations or contributions are welcome.

  [Flags]
  public enum TabListPageState
  {
    Normal = 0,

    Hot = 1,

    Selected = 2,

    Focused = 4
  }
}
