using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.DAL
{
    public class GetDAL
    {
        DB_MinesweeperDataContext db;
        public GetDAL()
        {
            db = new DB_MinesweeperDataContext();
        }
        
        public CapDo GetLevelByMa(int ma)
        {
            CapDo cd = new CapDo();
            var q = db.CapDos.FirstOrDefault(c => c.maCapDo == ma);
            cd = q;
            return cd;
        }
        public LuotChoi GetLuotChoiDaLuu(int macd)
        {
            LuotChoi lc = null;
            var q = db.LuotChois.FirstOrDefault(l => l.maCapDo == macd && l.tenNguoiChoi == null);
            lc = q;
            return lc;
        }
        public Cell GetCellByMa(int macell)
        {
            Cell cell = new Cell();
            var q = db.Cells.FirstOrDefault(c => c.maCell == macell);
            cell = q;
            return cell;
        }

        public int GetMaLuotChoiCuoi()
        {
            var q = db.LuotChois.ToList().LastOrDefault();
            if (q != null)
                return q.maLuotChoi;
            return 0;
        }

        public int GetMaCellCuoi()
        {
            var q = db.Cells.ToList().LastOrDefault();
            if (q != null)
                return q.maCell;
            return 0;
        }

        public List<LuotChoi> GetLuotChoiCoKetQua()
        {
            List<LuotChoi> lst = new List<LuotChoi>();
            var q = db.LuotChois
                .Join(db.CapDos, l => l.maCapDo, c => c.maCapDo, (l, c) => new { l, c })
                .Where(lt=>lt.l.tenNguoiChoi != "")
                .OrderBy(lc => lc.c.soMin)
                .ThenBy(lc => lc.l.thoiGian);
            lst = q.Select(lc => lc.l).ToList();
            return lst;
        }
        public List<Cell> GetDSCellByMaLuotChoi(int malc)
        {
            List<Cell> lst = new List<Cell>();
            lst = db.Cells.Where(c => c.maLuotChoi == malc).ToList();
            return lst;
        }
    }
}
