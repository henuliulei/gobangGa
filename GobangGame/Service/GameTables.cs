using Animal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    /// <summary>处理游戏大厅中每个房间的玩家</summary>
    public class GameTables
    {
        private const int max = 4; //棋盘网格最大的行列数
        public const int None = -1; //无棋子
        public const int Black = 0; //黑棋
        public const int White = 1; //白棋
        public int[] card = {1,2,3,4,5,6,7,8,-1,-2,-3,-4,-5,-6,-7,-8};//正：黑，负：白
        public int write_count = 8;
        public int black_count = 8;
        /// <summary>保存同一房间的两个玩家</summary>
        public User[] players { get; set; }

        /// <summary>
        /// 棋盘 
        /// 黑棋(1,2,3,4,5,6,7,8)
        ///白棋(-1,-2,-3,-4,-5,-6,-7,-8)
        ///</summary>
        private int[,] grid = new int[max, max];

        /// <summary>
        /// 棋盘
        /// 1：翻牌； 
        /// 0 ：无牌；
        /// -1 ：未翻牌
        ///</summary>
        private int[,] grid_flag = new int[max, max];

        /// <summary>下一步棋子颜色号（0：黑棋,1：白棋）</summary>
        private int nextColor = 0;

        public GameTables()
        {
            players = new User[2];
            ResetGrid();
        }

        /// <summary>打乱集合顺序</summary>
        public class MyCollections
        {
            public static void shuffle<T>(ref List<T> list)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                List<T> newList = new List<T>();//儲存結果的集合  
                foreach (T item in list)
                {
                    newList.Insert(rand.Next(0, newList.Count), item);
                }
                newList.Remove(list[0]);//移除list[0]的值  
                newList.Insert(rand.Next(0, newList.Count), list[0]);//再重新隨機插入第一筆  

                list = newList;

            }   
        }

        /// <summary>重置棋盘</summary>
        public void ResetGrid()
        {
            List<int> list = new List<int>();
            //Step 1.加入字串Apple、Banana、Blueberry、Cherry、Grape  
            foreach (var i in card) {
                list.Add(i);
            }

            //Step 2.打亂順序  
            MyCollections.shuffle(ref list);


            //放入数组
            int tmp = 0;
            for (int i = 0; i < max; i++) {
                for (int j = 0; j < max; j++) {
                    grid[i, j] = list[tmp];
                    grid_flag[i, j] = -1;
                    tmp++;
                }
            }
        }

        /// <summary>
        /// 获取棋子落下后是否获胜
        /// </summary>
        public bool IsWin()
        {
            if (nextColor == 0&&black_count==0)
            {
                return true;
            }
            if (nextColor == 1 && write_count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>放置棋子。参数：行，列</summary>
        public void SetGridDot(int i, int j,int k,int i1 = -1,int i2 = -1)
        {
            grid[i, j] = card[k];
            grid_flag[i, j] = 1;
            players[0].callback.ShowSetDot(i, j, k);
            players[1].callback.ShowSetDot(i, j, k);
            if (IsWin())
            {
                players[0].IsStarted = false;
                players[1].IsStarted = false;
                string message = nextColor == 0 ? "黑方胜" : "白方胜";
                players[0].callback.GameWin(message);
                players[1].callback.GameWin(message);
                this.ResetGrid();
            }
            else
            {
                nextColor = (nextColor + 1) % 2;
            }
        }

        public void SetGridDot_1(int i, int j, int k, int i1, int j1)
        {
            grid[i, j] = card[k];
            grid_flag[i, j] = 1;
            grid_flag[i1, j1] = 0;
            players[0].callback.ShowSetDot_1(i, j, k,i1,j1);
            players[1].callback.ShowSetDot_1(i, j, k,i1,j1);
            if (IsWin())
            {
                players[0].IsStarted = false;
                players[1].IsStarted = false;
                string message = nextColor == 0 ? "黑方胜" : "白方胜";
                players[0].callback.GameWin(message);
                players[1].callback.GameWin(message);
                this.ResetGrid();
            }
            else
            {
                nextColor = (nextColor + 1) % 2;
            }
        }

    }
}
