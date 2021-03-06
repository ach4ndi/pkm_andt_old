﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Andi.Libs.HexBox;
using Andi.Toolkit.utils;
using Andi.Toolkit.utils.Internal;
using Andi.Utils.Database;
using Andi.Utils.Dialogs;
using Andi.Utils.Io;
using Andi.Utils.Nitro.Archive;
using SourceGrid;
using WeifenLuo.WinFormsUI.Docking;

namespace Andi.Toolkit.gen0
{
    public partial class vEvolutions : DockContent, ICommonFormLayout, IGridFormLayout
    {
        #region Variable and Object Var
        private AndiNarcReader[] Narc = new AndiNarcReader[2];

        private bool isBabyEvoOpened = false;
        private bool isGridFocus = true;
        private string _LastPath = "";
        private FileInfo flepath;

        Stream _EvolutionStream = new MemoryStream();
        Stream _BabyStream = new MemoryStream();
        private string[] pkmname = new string[1];
        private string[] itemname = new string[1];
        private string[] movename = new string[1];
        private string[] methodname = new string[1];
        private string[] pkmnamecom = new string[1];
        private bool isGridOrder = true;
        private int GridActiveRow = 0;
        vEnum.GameFormat gameformat = new vEnum.GameFormat();
        vEnum.GameVer gamever = new vEnum.GameVer();
        #endregion 

        public vEvolutions()
        {
            InitializeComponent();
            
            EventsFormLoad();
        }

        #region CommonFunction

        public void EventsFormLoad()
        {
            _LastPath = Program.GlobalPath;
            flepath = new FileInfo(_LastPath);

            this.GotFocus += GotFocusF;

            toolStripComboBox1.SelectedIndex = 0;
            andiCustomTabControl1.Enabled = false;
            grid1.SelectionMode = GridSelectionMode.Row;
            grid1.Selection.EnableMultiSelection = false;
            grid1.MouseClick += BaseGridSelection_SelectionChanged;
            grid1.KeyDown += BaseGridSelection_SelectionChanged;
            grid1.KeyUp += BaseGridSelection_SelectionChanged;
            grid1.Selection.FocusRowEntered += BaseGridSelection_FocusRowEntered;
        }

        public void OpenFile_Click(object sender, EventArgs e)
        {
            string path = "", path2 = "";

            path = AndiFileDialog.OpenDialog(path, Path.GetDirectoryName(_LastPath), "Evolution Narc File - File Open", "Any Files|*.*|Narc Files|*.*");

            if (path != "")
            {
                flepath = new FileInfo(path);
                Program.mForm.fname.Text = flepath.Name;
                Program.mForm.fsize.Text = flepath.Length + "";

                path2 = AndiFileDialog.OpenDialog(path2, Path.GetDirectoryName(_LastPath), "BabyEvolution Narc File - File Open", "Any Files|*.*|Narc Files|*.*");
            }

            if (path2 != "")
            {
                if (CheckMagicHeaderID.get(path2) != "NARC")
                {
                    MessageBox.Show(
                        "This Not NARC File, File Extension Signature is " + CheckMagicHeaderID.get(path2) + ", and is not NARC File!", "Error!");
                    isBabyEvoOpened = false;
                    andiImageComboBox4.Enabled = false;
                }
                else
                {
                    andiImageComboBox4.Enabled = true;
                    Narc[1] = new AndiNarcReader();
                    Narc[1].OpenData(path2);
                    isBabyEvoOpened = true;
                }
            }
            else
            {
                isBabyEvoOpened = false;
                andiImageComboBox4.Enabled = false;
            }

            if (path != "")
            {
                Program.GlobalPath = Path.GetDirectoryName(path);
                _LastPath = path;

                if (CheckMagicHeaderID.get(path) != "NARC")
                {
                    MessageBox.Show("This Not NARC File, File Extension Signature is " + CheckMagicHeaderID.get(path) + ", and is not NARC File!", "Error!");
                    return;
                }

                andiCustomTabControl1.Enabled = true;
                Narc[0] = new AndiNarcReader();
                Narc[0].OpenData(path);
                EventsAfterOpenFile();
            }
        }

        public void EventsAfterOpenFile()
        {
            toolStripLabel1.Text = Narc[0].FileCount + "";
            andiListBox1.Items.Clear();
            andiImageComboBox3.Items.Clear();

            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    MVGList.Load(12);
                    pkmname = MVGList.GetPokemonNameMVList();
                    methodname = Database.GetCommonText(4);
                    gameformat = vEnum.GameFormat.gen4;
                    itemname = Database.GetItemName(21);
                    break;
                case 1:
                    MVGList.Load(14);
                    pkmname = MVGList.GetPokemonNameMVList();
                    methodname = Database.GetCommonText(4);
                    gameformat = vEnum.GameFormat.gen4;
                    itemname = Database.GetItemName(21);
                    break;
                case 2:
                    MVGList.Load(17);
                    pkmname = MVGList.GetPokemonNameMVList();
                    methodname = Database.GetCommonText(3);
                    gameformat = vEnum.GameFormat.gen5;
                    itemname = Database.GetItemName(21);
                    break;
                case 3:
                    MVGList.Load(21);
                    pkmname = MVGList.GetPokemonNameMVList();
                    methodname = Database.GetCommonText(3);
                    gameformat = vEnum.GameFormat.gen5;
                    itemname = Database.GetItemName(21);
                    break;
            }

            andiImageComboBox2.Items.Clear();
            andiImageComboBox4.Items.Clear();
            andiImageComboBox3.Items.AddRange(methodname);
            andiListBox1.Items.AddRange(pkmname);

            List<string> n = new List<string>();
            List<string> m = new List<string>();

            n.Add("");
            m.Add("");

            switch (gameformat)
            {
                case vEnum.GameFormat.gen4:
                    n.AddRange(Database.GetPokemonName(4));
                    m.AddRange(MVGList.GetMoveList(4));
                    break;
                case vEnum.GameFormat.gen5:
                    n.AddRange(Database.GetPokemonName(5));
                    m.AddRange(MVGList.GetMoveList(5));
                    break;
            }

            movename = m.ToArray();
            pkmnamecom = n.ToArray();
            andiImageComboBox2.Items.AddRange(pkmnamecom);
            andiImageComboBox4.Items.AddRange(pkmnamecom);
            andiListBox1.SelectedIndex = 0;
        }

        public void GotFocusF(object sender, EventArgs e)
        {
            try
            {
                Program.mForm.fname.Text = flepath.Name;
                Program.mForm.fsize.Text = flepath.Length + "";
            }
            catch
            {
                Program.mForm.fname.Text = "";
                Program.mForm.fsize.Text = "";
            }
        }

        public void LoadCurrentData()
        {
            int count1 = (int)_EvolutionStream.Length / 6;
            int count2 = 0;

            object[,] datatemp1 = new object[count1, 3];

            for (int i = 0; i < count1; i++)
            {
                byte[] data = new byte[2];
                _EvolutionStream.Position = i * 6 + 4;
                _EvolutionStream.Read(data, 0, 2);
                datatemp1[i, 0] = BitConverter.ToUInt16(data, 0);

                _EvolutionStream.Position = i * 6 + 0;
                _EvolutionStream.Read(data, 0, 2);
                datatemp1[i, 1] = BitConverter.ToUInt16(data, 0);

                if (BitConverter.ToUInt16(data, 0) == 0)
                {
                    count2++;
                }

                toolStripLabel2.Text = (count1 - count2) + "/" + count1;

                _EvolutionStream.Position = i * 6 + 2;
                _EvolutionStream.Read(data, 0, 2);
                datatemp1[i, 2] = BitConverter.ToUInt16(data, 0);
            }

            FillGrid.Build(grid1, count1, 3, "Evolve Into", "Method", "Requestment");
            Fill(grid1, datatemp1);
        }

        public void WriteCurrentBack_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void HexView()
        {
            DynamicFileByteProvider dynamicFileByteProvider = null;

            try
            {
                // try to open in write mode
                dynamicFileByteProvider = new DynamicFileByteProvider(_EvolutionStream);
            }
            catch { }

            hexBox1.ByteProvider = dynamicFileByteProvider;
        }

        private void reloadalldata()
        {
            andiImageComboBox1.Enabled = true;
            numericUpDown1.Enabled = true;

            switch (gameformat)
            {
                case vEnum.GameFormat.gen4:
                    switch (andiImageComboBox3.SelectedIndex)
                    {
                        case 4:
                        case 22:
                        case 23:
                            numericUpDown1.Maximum = 100;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Text = "none, level value";
                            andiImageComboBox1.Items.Add("none, level value");
                            andiImageComboBox1.Enabled = false;
                            break;
                        case 15:
                            numericUpDown1.Maximum = 255;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Text = "none, Hapinness value";
                            andiImageComboBox1.Items.Add("none, Hapinness value");
                            andiImageComboBox1.Enabled = false;
                            break;
                        case 6:
                        case 7:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                            numericUpDown1.Maximum = itemname.Length;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Items.AddRange(itemname);
                            numericUpDown1.Enabled = false;
                            break;
                        case 20:
                            numericUpDown1.Maximum = movename.Length;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Items.AddRange(movename);
                            numericUpDown1.Enabled = false;
                            break;
                        case 21:
                            numericUpDown1.Maximum = pkmname.Length;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Items.AddRange(pkmname);
                            numericUpDown1.Enabled = false;
                            break;
                        default:
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Text = "None";
                            andiImageComboBox1.Items.Add("none");
                            andiImageComboBox1.Enabled = false;
                            numericUpDown1.Value = 0;
                            numericUpDown1.Enabled = false;
                            break;
                    }
                    break;
                case vEnum.GameFormat.gen5:
                    switch (andiImageComboBox3.SelectedIndex)
                    {
                        case 4:
                        case 23:
                        case 24:
                            numericUpDown1.Maximum = 100;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Text = "none, level value";
                            andiImageComboBox1.Items.Add("none, level value");
                            andiImageComboBox1.Enabled = false;
                            break;
                        case 16:
                            numericUpDown1.Maximum = 255;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Text = "none, Hapinness value";
                            andiImageComboBox1.Items.Add("none, Hapinness value");
                            andiImageComboBox1.Enabled = false;
                            break;
                        case 6:
                        case 8:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                            numericUpDown1.Maximum = itemname.Length;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Items.AddRange(itemname);
                            numericUpDown1.Enabled = false;
                            break;
                        case 21:
                            numericUpDown1.Maximum = movename.Length;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Items.AddRange(movename);
                            numericUpDown1.Enabled = false;
                            break;
                        case 22:
                            numericUpDown1.Maximum = pkmname.Length;
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Items.AddRange(pkmname);
                            numericUpDown1.Enabled = false;
                            break;
                        default:
                            andiImageComboBox1.Items.Clear();
                            andiImageComboBox1.Text = "None";
                            andiImageComboBox1.Items.Add("none");
                            andiImageComboBox1.Enabled = false;
                            numericUpDown1.Value = 0;
                            numericUpDown1.Enabled = false;
                            break;
                    }
                    break;
            }
            if (isGridOrder)
            {
                andiImageComboBox1.SelectedIndex = 0;
            }
            //
        }

        public void WriteNarcBack()
        {
            byte[] temp = new byte[_EvolutionStream.Length];

            _EvolutionStream.Position = 0;
            _EvolutionStream.Read(temp, 0, (int)_EvolutionStream.Length);

            Narc[0].ReplaceEntry(andiListBox1.SelectedIndex, temp.Length, temp);
        }

        public void SaveFile_Click(object sender, EventArgs e)
        {
            AndiFileDialog.NarcSaveDialog(Narc[0], Path.GetDirectoryName(_LastPath), "Save Evolution Data", "narc file|*.narc");

            if (isBabyEvoOpened)
            {
                AndiFileDialog.NarcSaveDialog(Narc[1], Path.GetDirectoryName(_LastPath), "Save Base Evolution Data", "narc file|*.narc");
            }
        }

        private void WriteMethod(int index, int off)
        {
            byte[] data = ByteConverter.ToByte(off, 2);
            _EvolutionStream.Position = (index * 6) + 0 * 2;
            _EvolutionStream.Write(data, 0, 2);
        }

        private void WriteRequestment(int index, int off)
        {
            byte[] data = ByteConverter.ToByte(off, 2);
            _EvolutionStream.Position = (index * 6) + 0 * 2 + 2;
            _EvolutionStream.Write(data, 0, 2);
        }

        private void WritePokemonEv(int index, int off)
        {
            byte[] data = ByteConverter.ToByte(off, 2);
            _EvolutionStream.Position = (index * 6) + 0 * 2 + 4;
            _EvolutionStream.Write(data, 0, 2);
        }
        #endregion

        #region Grid EventHandler Function
        public void BaseGridSelection_FocusRowEntered(object sender, RowEventArgs e)
        {
            BaseGridSelectionChanged();

            if (!isGridFocus)
            {
                andiListBox1.Focus();
                isGridFocus = true;
            }
        }

        public void BaseGridSelection_SelectionChanged(object sender, KeyEventArgs e)
        {
            BaseGridSelectionChanged();
            isGridFocus = true;
        }

        public void BaseGridSelection_SelectionChanged(object sender, MouseEventArgs e)
        {
            BaseGridSelectionChanged();
            isGridFocus = true;
        }

        public void BaseGridSelectionChanged()
        {
            try
            {
                isGridOrder = false;
                GridActiveRow = grid1.Selection.ActivePosition.Row;
                toolStripLabel3.Text = GridActiveRow + "";

                andiImageComboBox3.Text = grid1[grid1.Selection.ActivePosition.Row, 2] + "";
                andiImageComboBox2.Text = grid1[grid1.Selection.ActivePosition.Row, 1] + "";

                switch (gameformat)
                {
                    case vEnum.GameFormat.gen4:
                        switch (andiImageComboBox3.SelectedIndex)
                        {
                            case 4:
                            case 22:
                            case 23:
                            case 15:
                                andiImageComboBox1.Enabled = false;
                                numericUpDown1.Value = int.Parse(grid1[grid1.Selection.ActivePosition.Row, 3].Value.ToString());
                                break;
                            default:
                                andiImageComboBox1.Enabled = true;
                                andiImageComboBox1.Text = grid1[grid1.Selection.ActivePosition.Row, 3] + "";
                                numericUpDown1.Value = 0;
                                break;
                        }
                        break;
                    case vEnum.GameFormat.gen5:
                        switch (andiImageComboBox3.SelectedIndex)
                        {
                            case 4:
                            case 23:
                            case 24:
                            case 16:
                                numericUpDown1.Value = int.Parse(grid1[grid1.Selection.ActivePosition.Row, 3].Value.ToString());
                                break;
                            default:
                                andiImageComboBox1.Text = grid1[grid1.Selection.ActivePosition.Row, 3] + "";
                                break;
                        }
                        break;
                }

                isGridOrder = true;
            }
            catch { }
        }
        #endregion 

        #region SelectedIndex EventHandler
        private void andiListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            isGridFocus = false;
            int angka = andiListBox1.SelectedIndex;

            _EvolutionStream = new MemoryStream(Narc[0].getdataselected(angka));

            if (isBabyEvoOpened)
            {
                bool checkstate = false;

                if (gameformat == vEnum.GameFormat.gen4)
                {
                    if (angka < 494)
                    {
                        andiImageComboBox4.Enabled = true;
                        _BabyStream = new MemoryStream(Narc[1].getdataselected(angka));
                        checkstate = true;
                    }
                    else
                    {
                        andiImageComboBox4.Text = "None";
                        andiImageComboBox4.Enabled = false;
                    }
                }
                else
                {
                    if (angka < 650)
                    {
                        andiImageComboBox4.Enabled = true;
                        _BabyStream = new MemoryStream(Narc[1].getdataselected(angka));
                        checkstate = true;
                    }
                    else
                    {
                        andiImageComboBox4.Text = "None";
                        andiImageComboBox4.Enabled = false;
                    }
                }

                if (checkstate)
                {
                    byte[] data2 = new byte[2];
                    _BabyStream.Position = 0;
                    _BabyStream.Read(data2, 0, 2);

                    andiImageComboBox4.SelectedIndex = BitConverter.ToUInt16(data2, 0);
                }
            }

            toolStripLabel1.Text = Narc[0].FileCount + "/" + (andiListBox1.SelectedIndex + 1);
            HexView();
            LoadCurrentData();
        }

        private void andiImageComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = GridActiveRow - 1;

            reloadalldata();

            if (isGridOrder)
            {
                WriteMethod(index, andiImageComboBox3.SelectedIndex);

                try
                {
                    grid1[index + 1, 2] = new SourceGrid.Cells.Cell(andiImageComboBox3.Text);
                    SourceGrid.Cells.Views.Cell view = new SourceGrid.Cells.Views.Cell();
                    grid1.AutoSizeCells();
                    grid1[index + 1, 2].View = view;
                    grid1.Refresh();
                }
                catch { }

                andiImageComboBox1.SelectedIndex = 0;

                if (andiImageComboBox3.SelectedIndex == 0)
                {
                    andiImageComboBox1.SelectedIndex = 0;
                    WriteRequestment(index, 0);
                    andiImageComboBox2.SelectedIndex = 0;
                    WritePokemonEv(index, 0);
                }

                WriteNarcBack();
            }
        }

        private void andiImageComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = GridActiveRow - 1;

            if (isGridOrder)
            {
                switch (andiImageComboBox3.SelectedIndex)
                {
                    case 4:
                    case 23:
                    case 24:
                    case 16:
                        break;
                    case 6:
                    case 8:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                        WriteRequestment(index, andiImageComboBox1.SelectedIndex);
                        numericUpDown1.Value = andiImageComboBox1.SelectedIndex;
                        break;
                    default:
                        WriteRequestment(index, 0);
                        break;
                }

                try
                {
                    switch (andiImageComboBox3.SelectedIndex)
                    {
                        case 4:
                        case 23:
                        case 24:
                        case 16:
                            break;
                        default:
                            grid1[index + 1, 3] = new SourceGrid.Cells.Cell(andiImageComboBox1.Text);
                            SourceGrid.Cells.Views.Cell view = new SourceGrid.Cells.Views.Cell();
                            grid1.AutoSizeCells();
                            grid1[index + 1, 3].View = view;
                            grid1.Refresh();
                            break;
                    }
                }
                catch { }
                WriteNarcBack();
            }
        }

        private void andiImageComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = GridActiveRow - 1;

            andiImageBox1.Image = ImageIconHandler.setImagePictureBox(andiImageComboBox2.SelectedIndex);

            if (isGridOrder)
            {
                WritePokemonEv(index, andiImageComboBox2.SelectedIndex);
                try
                {
                    grid1[index + 1, 1] = new SourceGrid.Cells.Cell(andiImageComboBox2.Text);
                    SourceGrid.Cells.Views.Cell view = new SourceGrid.Cells.Views.Cell();
                    grid1.AutoSizeCells();
                    grid1[index + 1, 1].View = view;
                    grid1.Refresh();
                }
                catch { }

                WriteNarcBack();
            }
        }

        private void andiImageComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isBabyEvoOpened)
            {
                bool checktrue = false;

                if (gameformat == vEnum.GameFormat.gen4)
                {
                    if (andiListBox1.SelectedIndex < 493)
                    {
                        checktrue = true;
                    }
                }
                else
                {
                    if (andiListBox1.SelectedIndex < 649)
                    {
                        checktrue = true;
                    }
                }

                if (checktrue)
                {
                    byte[] bya = ByteConverter.ToByte(andiImageComboBox4.SelectedIndex, 2);
                    _BabyStream.Position = 0;
                    _BabyStream.Write(bya, 0, 2);

                    byte[] temp = new byte[_BabyStream.Length];

                    _BabyStream.Position = 0;
                    _BabyStream.Read(temp, 0, (int)_BabyStream.Length);

                    Narc[1].ReplaceEntry(andiListBox1.SelectedIndex, temp.Length, temp);

                    pkm_1.Image = ImageIconHandler.setImagePictureBox(andiImageComboBox4.SelectedIndex);
                }
            }
        }
        #endregion

        #region Filling Grid Function
        private void Fill(Grid a, object[,] data, bool multiline = true)
        {
            SourceGrid.Cells.Views.Cell view = new SourceGrid.Cells.Views.Cell();
            SourceGrid.Cells.Editors.TextBox editor = new SourceGrid.Cells.Editors.TextBox(typeof(string));
            view.BackColor = Color.White;
            editor.Control.Multiline = multiline;

            for (int r = a.FixedRows; r < a.RowsCount; r++)
            {
                a[r, 1] = new SourceGrid.Cells.Cell(pkmnamecom[int.Parse(data[r - 1,0].ToString())]);
                a[r, 2] = new SourceGrid.Cells.Cell(methodname[int.Parse(data[r - 1, 1].ToString())]);
                switch (gameformat)
                {
                    case vEnum.GameFormat.gen4:
                        switch (int.Parse(data[r - 1, 1].ToString()))
                        {
                            case 4:
                            case 22:
                            case 23:
                            case 15:
                                a[r, 3] = new SourceGrid.Cells.Cell(data[r - 1,2]);
                                break;
                            case 6:
                            case 7:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                                a[r, 3] = new SourceGrid.Cells.Cell(itemname[int.Parse(data[r - 1, 2].ToString())]);
                                break;
                            case 20:
                                a[r, 3] = new SourceGrid.Cells.Cell(movename[int.Parse(data[r - 1, 2].ToString())]);
                                break;
                            case 21:
                                a[r, 3] = new SourceGrid.Cells.Cell(pkmname[int.Parse(data[r - 1, 2].ToString())]);
                                break;
                            default:
                                a[r, 3] = new SourceGrid.Cells.Cell("None");
                                break;
                        }
                        break;
                    case vEnum.GameFormat.gen5:
                        switch (int.Parse(data[r - 1, 1].ToString()))
                        {
                            case 4:
                            case 23:
                            case 24:
                            case 16:
                                a[r, 3] = new SourceGrid.Cells.Cell(data[r - 1, 2]);
                                break;
                            case 6:
                            case 8:
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                                a[r, 3] = new SourceGrid.Cells.Cell(itemname[int.Parse(data[r - 1, 2].ToString())]);
                                break;
                            case 21:
                                a[r, 3] = new SourceGrid.Cells.Cell(movename[int.Parse(data[r - 1, 2].ToString())]);
                                break;
                            case 22:
                                a[r, 3] = new SourceGrid.Cells.Cell(pkmname[int.Parse(data[r - 1, 2].ToString())]);
                                break;
                            default:
                                a[r, 3] = new SourceGrid.Cells.Cell("None");
                                break;
                        }
                        break;
                }
                
                a[r, 1].View = view;
                a[r, 2].View = view;
                a[r, 3].View = view;
            }

            a.Update();
            a.Selection.Focus(new SourceGrid.Position(1, 1), true);
            a.AutoSizeCells();
        }
        #endregion

        #region Other EventHandler
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int index = GridActiveRow - 1;
            if (isGridOrder)
            {
                switch (andiImageComboBox3.SelectedIndex)
                {
                    case 4:
                    case 23:
                    case 24:
                    case 16:
                        WriteRequestment(index, (int) numericUpDown1.Value);
                        break;
                    case 6:
                    case 8:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                        break;
                    default:
                        WriteRequestment(index, 0);
                        break;
                }
                try
                {
                    grid1[index + 1, 3] = new SourceGrid.Cells.Cell(numericUpDown1.Value);
                    SourceGrid.Cells.Views.Cell view = new SourceGrid.Cells.Views.Cell();
                    grid1.AutoSizeCells();
                    grid1[index + 1, 3].View = view;
                    grid1.Refresh();
                }
                catch { }

                WriteNarcBack();
            }
        }

        private void textupdate1(object sender, EventArgs e)
        {
            reloadalldata();
        }
        #endregion
    }
}
