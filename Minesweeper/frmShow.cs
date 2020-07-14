using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Minesweeper.DAL;

namespace Minesweeper
{
    public partial class frmShow : Form
    {
        GetDAL getData;

        enum chucNang : byte
        {
            charts = 1,
            rule = 2
        };
        chucNang chucnangs;
        public frmShow(byte cn)
        {
            InitializeComponent();
            getData = new GetDAL();
            chucnangs = (chucNang)cn;
        }

        void HienThiFormTheoChucNang(chucNang cn)
        {
            if (cn == chucNang.charts)
            {
                this.lvwCharts.Visible = true;
                this.lblTitle.Text = "TOP 10";

                //Get top 10
                List<LuotChoi> lst = getData.GetLuotChoiCoKetQua();
                LoadListView(lst);
            }
            else // (cn == chucNang.rule)
            {
                this.lvwCharts.Visible = false;
                this.lblTitle.Text = "Quy tắc chơi";

                //Thêm richtextbox hiển thi quy tắc chơi
                RichTextBox rtxtRule = new RichTextBox();
                rtxtRule.Location = lvwCharts.Location;
                rtxtRule.Font = new Font("Segoe Print", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                rtxtRule.ReadOnly = true;
                rtxtRule.ScrollBars = RichTextBoxScrollBars.Vertical;
                rtxtRule.Size = lvwCharts.Size;
                pnlFill.Controls.Add(rtxtRule);

                string str = GetNoiDungQuyTacChoi();
                if (str != null)
                    rtxtRule.Text = str;
            }
        }

        string GetNoiDungQuyTacChoi()
        {
            string filePath = @"Rule.txt";
            string str;
            if (File.Exists(filePath))
            {
                str = File.ReadAllText(filePath);
                return str;
            }
            else
                return null;
        }

        void LoadListView(List<LuotChoi> lst)
        {
            lvwCharts.Items.Clear();
            foreach (var item in lst)
            {
                ListViewItem lvi = new ListViewItem((lvwCharts.Items.Count + 1).ToString());
                lvi.SubItems.Add(item.tenNguoiChoi);
                lvi.SubItems.Add(item.thoiGian.ToString());
                string tenCD = getData.GetLevelByMa(item.maCapDo).tenCapDo;
                lvi.SubItems.Add(tenCD);
                lvi.Tag = item.maLuotChoi;
                lvwCharts.Items.Add(lvi);
                
            }
        }

        private void frmShow_Load(object sender, EventArgs e)
        {
            HienThiFormTheoChucNang(chucnangs);
        }
    }
}
