using Minesweeper.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Minesweeper.DAL;
using Microsoft.VisualBasic;

namespace Minesweeper
{
    public partial class frmPlay : Form
    {
        DB_MinesweeperDataContext db;
        GetDAL getData;
        SetDAL setData;
        Board board;
        CapDo c;
        private enum menuChucNang : int
        {
            setup = 0,
            play1 = 1,
            play2 = 2
        };
        menuChucNang chucNang;
        public frmPlay(int setupState)
        {
            InitializeComponent();
            this.chucNang = (menuChucNang) setupState;

            getData = new GetDAL();
            setData = new SetDAL();
        }
        

        private void frmPlay_Load(object sender, EventArgs e)
        {
                //Thêm dữ liệu cho combobox Level & thêm menu con của mnuLevel
            LoadComboBoxLevel();
            //Load game theo điều kiện, nếu cấp độ có game đã lưu => Load lại game lưu
            LuotChoi lcdaluu=null;
            if ((int)this.chucNang == 1)
            {
                lcdaluu = getData.GetLuotChoiDaLuu(1);
                if (lcdaluu != null)
                {
                    List<Cell> lstCell = new List<Cell>();
                    lstCell = lcdaluu.Cells.ToList();

                    setData.DeleteLuotChoi(lcdaluu.maLuotChoi, lstCell);
                    LoadGameByGameSaved(1, lstCell, (int)lcdaluu.thoiGian);
                }
                else
                    LoadGame(1);
            }
            else if ((int)this.chucNang == 2)
                LoadGame(1);
            else // ((int)this.chucNang == 0)
                LoadGameBySetup();
            
            btnStart.Enabled = true;
            this.pnlPlay.Enabled = false;
            btnIdea.Enabled = false;
            btnResetGame.Enabled = false;
            //
            switch (chucNang)
            {
                case menuChucNang.play1:
                    rdo1Player.Checked = true;
                    break;
                case menuChucNang.play2:
                    rdo2Player.Checked = true;
                    break;
                default:
                    rdo1Player.Checked = rdo2Player.Checked = false;
                    break;
            }
            HienThiFormTheoChucNang(chucNang);
        }

        

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (chucNang == menuChucNang.setup)
            {
                if (this.board.btnCells[0, 0].CellState == CellState.Closed)
                {
                    board.TimeRun.Start();
                    btnIdea.Enabled = true;
                    btnResetGame.Enabled = false;
                }
                else
                {
                    btnResetGame.Enabled = false;
                    btnIdea.Enabled = false;
                }

            }
            else
            {
                board.TimeRun.Start();
                btnResetGame.Enabled = true;
                btnIdea.Enabled = true;
            }
            btnStart.Enabled = false;
            this.pnlPlay.Enabled = true;


        }

        void LoadComboBoxLevel()
        {
            //gán dữ liệu cho combobox
            db = new DB_MinesweeperDataContext();
            var q = db.CapDos.ToList();

            cboLevel.DataSource = q;
            cboLevel.ValueMember = "maCapDo";
            cboLevel.DisplayMember = "tenCapDo";
            cboLevel.SelectedIndex = 0;

            //thêm item cho menu level
            foreach (var item in q)
            {
                mnuLevel.DropDownItems.Add(item.tenCapDo);
            }
            
        }
        void LoadGame(int ma)
        {
            getData = new GetDAL();
            c = getData.GetLevelByMa(ma);
            lblSoMinFlag.Text = c.soMin.ToString();
            board = new Board(pnlLayOut, pnlPlay, (int)c.soCot, (int)c.soDong, (int)c.soMin, pnlPlayState, (int)chucNang);

            board.SetupBoard(false);
            board.PlaceMines();
        }
        void LoadGameByGameSaved(int ma,List<Cell> lstCell,int tg)
        {
            getData = new GetDAL();
            c = getData.GetLevelByMa(ma);
            lblSoMinFlag.Text = c.soMin.ToString();
            board = new Board(pnlLayOut, pnlPlay, (int)c.soCot, (int)c.soDong, (int)c.soMin, pnlPlayState, (int)chucNang);
            board.SetupBoardCoSan(lstCell, tg);
        }

        void LoadGameBySetup()
        {
            btnStart.Enabled = true;
            getData = new GetDAL();
            CapDo c = getData.GetLevelByMa((int)cboLevel.SelectedValue);
            lblSoMinFlag.Text = c.soMin.ToString();
            board = new Board(pnlLayOut, pnlPlay, (int)c.soCot, (int)c.soDong, (int)c.soMin, pnlPlayState, (int)chucNang);
            board.pnlPlay.Controls.Clear();
            board.SetupBoard(true);
        }
        
        private void btnPlay_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            pnlPlay.Enabled = false;
            btnIdea.Enabled = false;
            btnResetGame.Enabled = false;

            if (rdo1Player.Checked)
                chucNang = menuChucNang.play1;
            else if (rdo2Player.Checked)
                chucNang = menuChucNang.play2;
            else
            {
                MessageBox.Show("Bạn hãy chọn số Players!!");
                return;
            }
                
            try
            {
                board.TimeRun.Stop();
                pnlPlay.Controls.Clear();
                int ma = (int)cboLevel.SelectedValue;
                LuotChoi lcdaluu=null;
                if ((int)this.chucNang == 1)
                {
                    lcdaluu = getData.GetLuotChoiDaLuu(ma);
                    if (lcdaluu != null)
                    {
                        List<Cell> lstCell = new List<Cell>();
                        lstCell = lcdaluu.Cells.ToList();

                        setData.DeleteLuotChoi(lcdaluu.maLuotChoi, lstCell);
                        LoadGameByGameSaved(ma, lstCell, (int)lcdaluu.thoiGian);
                    }
                    else
                    {
                        LoadGame(ma);
                    }
                }
                else //if ((int)this.chucNang == 2)
                    LoadGame(ma);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        void HienThiFormTheoChucNang(menuChucNang cn)
        {
            if (cn == menuChucNang.play1)
            {

            }
            else if (cn == menuChucNang.play2)
            {

            }
            else // (cn == menuChucNang.setup)
            {

            }
        }
        private void frmPlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            board.TimeRun.Stop();
            if (chucNang==menuChucNang.play1 && !board.checkThua && !board.checkLuuGameThang)
            {
                SaveGame(null);
            }
        }
        private void btnSetup_Click(object sender, EventArgs e)
        {
            rdo1Player.Checked = rdo2Player.Checked = false;
            chucNang = menuChucNang.setup;
            pnlPlay.Enabled = false;
            if (!btnStart.Enabled)
                this.board.TimeRun.Stop();
            LoadGameBySetup();
            
        }
        

        private void LuuGame()
        {
            if (chucNang==menuChucNang.play1)
            {
                List<LuotChoi> lcLuu = getData.GetLuotChoiCoKetQua();
                if (lcLuu.Count < 10)
                {
                    object ten = Interaction.InputBox("Bạn đã lọt vào Top 10, nếu bạn muốn lưu kết quả => hãy nhập tên của bạn!!", "Nhập tên");
                    if ((string)ten != "")
                        SaveGame((string)ten);
                }
                else
                {
                    float xepHangCuoi = (float)lcLuu[0].thoiGian / (float)lcLuu[0].CapDo.soMin;
                    int malc = lcLuu[0].maLuotChoi;
                    for (int i = 1; i < lcLuu.Count; i++)
                    {
                        float xepHang = (float)lcLuu[i].thoiGian / (float)lcLuu[i].CapDo.soMin;
                        if (xepHang > xepHangCuoi)
                        {
                            xepHangCuoi = xepHang;
                            malc = lcLuu[i].maLuotChoi;
                        }
                    }
                    float xepHangBoard = (float)board.SoGiay / (float)board.NumMines;
                    if (xepHangBoard < xepHangCuoi)
                    {
                        object ten = Interaction.InputBox("Bạn đã lọt vào Top 10, nếu bạn muốn lưu kết quả => hãy nhập tên của bạn!!", "Nhập tên");
                        if ((string)ten != "")
                        {
                            SaveGame((string)ten);
                            setData.DeleteLuotChoi(malc, getData.GetDSCellByMaLuotChoi(malc));
                        }
                    }
                }
            }
        }
        public void SaveGame(string tenNguoiChoi)
        {
            try
            {
                int maCapDo = this.c.maCapDo;
                LuotChoi luot = new LuotChoi();
                var temp = getData.GetMaLuotChoiCuoi();
                luot.maLuotChoi = temp + 1;
                if (tenNguoiChoi != null)
                    luot.tenNguoiChoi = tenNguoiChoi;
                else
                    luot.tenNguoiChoi = null;
                luot.thoiGian = int.Parse(lblTime.Text);
                luot.maCapDo = maCapDo;

                List<Cell> lstCell = this.board.GetDSCell();
                foreach (Cell c in lstCell)
                {
                    int maC = getData.GetMaCellCuoi();
                    c.maCell = maC + 1+lstCell.IndexOf(c);
                    c.maLuotChoi = luot.maLuotChoi;
                }
                if (setData.SaveLuotChoi(luot, lstCell))
                {
                    MessageBox.Show("Thành công!!");
                }
                else
                    MessageBox.Show("Thất bại!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void pbxState_BackgroundImageChanged(object sender, EventArgs e)
        {
            if (!board.checkThua && board.checkLuuGameThang)
            {
                if (this.chucNang == menuChucNang.play1)
                    LuuGame();
                else
                    MessageBox.Show("Chúc mừng bạn đã thắng!!");
            }
            if (chucNang == menuChucNang.setup && board.btnCells[0,0].CellState == CellState.Closed)
            {
                btnStart.Enabled = true;
                pnlPlay.Enabled = false;
            }
        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShow frm = new frmShow(2);
            frm.ShowDialog();
        }

        private void xemBXHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShow frm = new frmShow(1);
            frm.ShowDialog();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Random ran = new Random();
            int x = ran.Next(0, (int)this.c.soCot);
            int y = ran.Next(0, (int)this.c.soDong);
            if (!board.btnCells[x, y].IsMine() && board.btnCells[x, y].CellState == CellState.Closed)
            {
                board.btnCells[x, y].CellState = CellState.Opened;
                board.btnCells[x, y].UpdateDisplay();
            }
            else
                if(!HienThiCellGoiY())
                    MessageBox.Show("Tất cả đều là mìn");
        }
        private bool HienThiCellGoiY()
        {
            for(int y=0;y<this.c.soDong;y++)
                for(int x =0;x<this.c.soCot;x++)
                    if(!board.btnCells[x, y].IsMine() && board.btnCells[x, y].CellState == CellState.Closed)
                    {
                        board.btnCells[x, y].CellState = CellState.Opened;
                        board.btnCells[x, y].UpdateDisplay();
                        return true;
                    }
            return false;
        }

        private void btnResetGame_Click(object sender, EventArgs e)
        {
            board.RestartGame();
            board.pnlPlay.Enabled = false;
            btnStart.Enabled = true;
            btnResetGame.Enabled = false;
            btnIdea.Enabled = false;
        }
        
    }
}
