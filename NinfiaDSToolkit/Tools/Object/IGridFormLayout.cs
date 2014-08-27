﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SourceGrid;

namespace NinfiaDSToolkit.Tools.Object
{
    internal interface IGridFormLayout
    {
        void BaseGridSelectionChanged();

        void BaseGridSelection_FocusRowEntered(object sender, RowEventArgs e);

        void BaseGridSelection_SelectionChanged(object sender, KeyEventArgs e);

        void BaseGridSelection_SelectionChanged(object sender, MouseEventArgs e);
    }
}
