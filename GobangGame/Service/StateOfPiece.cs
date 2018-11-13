using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animal
{
    class StateOfPiece
    {
        private bool RedDen;
        private bool BlueDen;

        private int AnimalsOfRed;
        private int AnimalsOfBlue;

        private int[,] Piece;//棋子
        private bool Handle;//执手方，false为红方，true为蓝方
        private int[,] Checkerboard = new int[9, 11]
        {
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,5,5,5,0,0,0,0},
            {0,2,0,0,5,5,5,0,0,4,0},
            {0,1,2,0,0,0,0,0,4,3,0},
            {0,2,0,0,5,5,5,0,0,4,0},
            {0,0,0,0,5,5,5,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0}
        };//1为红方兽穴，2为红方陷阱，3为蓝方兽穴，4为蓝方陷阱，5是……是什么来着？
        public int VictoryJudge()
        {
            if (RedDen || AnimalsOfRed == 0) return 1;
            else if (BlueDen || AnimalsOfBlue == 0) return 2;
            else return 0;
        }//判断是否满足胜利条件

        public void Reset()
        {
            Handle = true;
            RedDen = false;
            BlueDen = false;
            AnimalsOfRed = 8;
            AnimalsOfBlue = 8;
            Piece = new int[9, 11]
            {
                {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
                {-1,6,-1,0,-1,-1,-1,15,-1,13,-1},
                {-1,-1,3,-1,-1,-1,-1,-1,9,-1,-1},
                {-1,-1,-1,4,-1,-1,-1,10,-1,-1,-1},      //0~7分别是红方的鼠，猫，狼，狗，豹，虎，狮，象
                {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
                {-1,-1,-1,2,-1,-1,-1,12,-1,-1,-1},      //8~15分别是蓝方的鼠，猫，狼，狗，豹，虎，狮，象
                {-1,-1,1,-1,-1,-1,-1,-1,11,-1,-1},
                {-1,5,-1,7,-1,-1,-1,8,-1,14,-1},
                {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1}
            };

        }
        public int GetPiece(int x, int y)
        {
            return Piece[x, y];
        }
        public void Select(int x, int y, ref selected sel)
        {
            if (Piece[x, y] != -1)
            {
                if (Handle == false && Piece[x, y] < 8)
                {
                    sel.select = true;
                    sel.x = x;
                    sel.y = y;
                }
                else if (Handle == true && Piece[x, y] >= 8)
                {
                    sel.select = true;
                    sel.x = x;
                    sel.y = y;
                }
            }
        }
        public bool PieceMove(int Sx, int Sy, int x, int y, ref selected sel)
        {
            if (MoveJudge(Sx, Sy, x, y, ref sel))
            {
                Handle = Handle ? false : true;//移动后更换执手方
                Move(Sx, Sy, x, y);
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ActionScope(int Sx, int Sy, int x, int y)
        {
            if ((Sx == x && (Sy == y + 1 || Sy == y - 1)) || (Sy == y && (Sx == x + 1 || Sx == x - 1)))
            {
                return true;
            }
            else if ((Checkerboard[Sx + 1, Sy] == 5 || Checkerboard[Sx, Sy + 1] == 5 || Checkerboard[Sx - 1, Sy] == 5 || Checkerboard[Sx, Sy - 1] == 5) && (Piece[Sx, Sy] % 8 == 5 || Piece[Sx, Sy] % 8 == 6))
            {
                if (Checkerboard[Sx + 1, Sy] == 5 && x > Sx)
                {
                    for (int i = 1; i <= 2; i++) if (Piece[Sx + i, Sy] != -1) return false;//判断是否有棋子挡在河上
                    if (Sx + 3 == x && Sy == y) return true;
                    else return false;
                }
                else if (Checkerboard[Sx, Sy + 1] == 5 && y > Sy)
                {
                    for (int i = 1; i <= 3; i++) if (Piece[Sx, Sy + i] != -1) return false;
                    if (Sy + 4 == y && Sx == x) return true;
                    else return false;
                }
                else if (Checkerboard[Sx, Sy - 1] == 5 && y < Sy)
                {
                    for (int i = 1; i <= 3; i++) if (Piece[Sx, Sy - i] != -1) return false;
                    if (Sy - 4 == y && Sx == x) return true;
                    else return false;
                }
                else if (Checkerboard[Sx - 1, Sy] == 5 && x < Sx)
                {
                    for (int i = 1; i <= 2; i++) if (Piece[Sx - i, Sy] != -1) return false;
                    if (Sx - 3 == x && Sy == y) return true;
                    else return false;
                }//狮虎两种棋子跃河
                else return false;
            }
            else return false;
        }//判断行动是否是棋子的行动范围
        private bool MoveJudge(int Sx, int Sy, int x, int y, ref selected sel)
        {
            if (ActionScope(Sx, Sy, x, y))
            {
                if (Checkerboard[x, y] == 5 && (Piece[Sx, Sy] != 0 && Piece[Sx, Sy] != 8) && Piece[x, y] == -1)
                {
                    return false;
                }
                else if (Piece[x, y] != -1 && Piece[Sx, Sy] / 8 == Piece[x, y] / 8)
                {
                    sel.x = x;
                    sel.y = y;
                    return false;
                }//下一步是己方棋子
                else if (Piece[x, y] != -1 && Piece[Sx, Sy] / 8 != Piece[x, y] / 8)
                {
                    if (Piece[Sx, Sy] % 8 != 7 || Piece[x, y] % 8 != 0)
                    {
                        if ((Checkerboard[Sx, Sy] == 5 && Checkerboard[x, y] == 5) || (Checkerboard[Sx, Sy] != 5 && Checkerboard[x, y] != 5))
                        {
                            if (Piece[Sx, Sy] % 8 == 0 && Piece[x, y] % 8 == 7) return true;
                            else if ((Checkerboard[x, y] == 2 && Piece[Sx, Sy] / 8 == 0) || (Checkerboard[x, y] == 4 && Piece[Sx, Sy] / 8 == 1)) return true;//在陷阱中的棋子随便吃
                            else if (Piece[Sx, Sy] % 8 >= Piece[x, y] % 8)
                            {
                                return true;
                            }
                            else if (Math.Abs(Sx - x) > 1 || Math.Abs(Sy - y) > 1) return true;
                            else return false;
                        }//判断是否同为水或陆
                        else return false;
                    }
                    else return false;
                }//下一步是敌方棋子
                else if ((Checkerboard[x, y] == 1 && Piece[Sx, Sy] / 8 == 0) || (Checkerboard[x, y] == 3 && Piece[Sx, Sy] / 8 == 1))
                {
                    return false;
                }//不能进己方兽穴
                else return true;
            }
            else if (Piece[x, y] != -1 && Piece[Sx, Sy] / 8 == Piece[x, y] / 8)
            {
                sel.x = x;
                sel.y = y;
                return false;
            }
            else return false;
        }
        private void Move(int Sx, int Sy, int x, int y)
        {
            if (Piece[x, y] != -1)
            {
                if (Piece[x, y] >= 8) AnimalsOfBlue--;
                else AnimalsOfRed--;
                Piece[x, y] = Piece[Sx, Sy];
            }
            else Piece[x, y] = Piece[Sx, Sy];
            if (Checkerboard[x, y] == 1 || Checkerboard[x, y] == 3)
            {
                if (Checkerboard[x, y] == 1) RedDen = true;
                else BlueDen = true;
            }
            Piece[Sx, Sy] = -1;
        }//移动棋子
    }
    struct selected
    {
        public bool select;
        public int x, y;
    };

}
