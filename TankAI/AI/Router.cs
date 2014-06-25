using TankAI.Util;
using System;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TankAI.AI
{
    
    public struct PathFinderNode
    {
            #region Variables Declaration
            public int F;
            public int G;
            public int H;  // f = gone + heuristic
            public int X;
            public int Y;
            public int PX; // Parent
            public int PY;
            #endregion
    }
    public enum PathFinderNodeType
    {
        Start = 1,
        End = 2,
        Open = 4,
        Close = 8,
        Current = 16,
        Path = 32
    }

    public enum HeuristicFormula
    {
        Manhattan = 1,
        MaxDXDY = 2,
        DiagonalShortCut = 3,
        Euclidean = 4,
        EuclideanNoSQR = 5,
        Custom1 = 6
    }
    class ComparePFNode : IComparer<PathFinderNode>
        {
            #region IComparer Members
            public int Compare(PathFinderNode x, PathFinderNode y)
            {
                if (x.F > y.F)
                    return 1;
                else if (x.F < y.F)
                    return -1;
                return 0;
            }
            #endregion
        }

    public  class Router{


        
        #region Variables Declaration
                
        private static  byte[,]                         mGrid                   = null;
        private PriorityQueueB<PathFinderNode>  mOpen                   = new PriorityQueueB<PathFinderNode>(new ComparePFNode());
        private List<PathFinderNode>      mClose                  = new List<PathFinderNode>();
        private bool                            mStop                   = false;
        private bool                            mStopped                = true;
        private int                             mHoriz                  = 0;
        private HeuristicFormula                mFormula                = HeuristicFormula.Manhattan;
        //private bool                            mDiagonals              = true;
        private int                             mHEstimate              = 2;
        private bool                            mPunishChangeDirection  = true;
        private bool                            mReopenCloseNodes       = false;
        private bool                            mTieBreaker             = false;
        private bool                            mHeavyDiagonals         = false;
        private int                             mSearchLimit            = 2000;
        private double                          mCompletedTime          = 0;
        private bool                            mDebugProgress          = false;
        private bool                            mDebugFoundPath         = false;
        #endregion

        #region Constructors


        public Router(byte[,] grid)
        {
            if (grid == null)
                throw new Exception("Grid cannot be null");

            mGrid = grid;

            for (int i = 0; i < Constant.MAP_SIZE; i++)
            {
                for (int j = 0; j < Constant.MAP_SIZE; j++)
                {
                    mGrid[i, j] = 7;
                }
            }
        }
        #endregion

        #region Properties
        public bool Stopped
        {
            get { return mStopped; }
        }

       
       
        public bool HeavyDiagonals
        {
            get { return mHeavyDiagonals; }
            set { mHeavyDiagonals = value; }
        }

        public int HeuristicEstimate
        {
            get { return mHEstimate; }
            set { mHEstimate = value; }
        }

        public bool PunishChangeDirection
        {
            get { return mPunishChangeDirection; }
            set { mPunishChangeDirection = value; }
        }

        public bool ReopenCloseNodes
        {
            get { return mReopenCloseNodes; }
            set { mReopenCloseNodes = value; }
        }

        public bool TieBreaker
        {
            get { return mTieBreaker; }
            set { mTieBreaker = value; }
        }

        public int SearchLimit
        {
            get { return mSearchLimit; }
            set { mSearchLimit = value; }
        }

        public double CompletedTime
        {
            get { return mCompletedTime; }
            set { mCompletedTime = value; }
        }

        public bool DebugProgress
        {
            get { return mDebugProgress; }
            set { mDebugProgress = value; }
        }

        public bool DebugFoundPath
        {
            get { return mDebugFoundPath; }
            set { mDebugFoundPath = value; }
        }
        #endregion






        public byte getCost(int x, int y)
        {
            return mGrid[x, y];
        }

        public void FindPathStop()
        {
            mStop = true;
        }

        public List<PathFinderNode> FindPath(Point start, Point end)
        {
          ///////////////////////  HighResolutionTime.Start();
            int mycost = mGrid[start.X, start.Y];
            PathFinderNode parentNode;
            bool found  = false;
            int  gridX  = mGrid.GetUpperBound(0);//get gridwidth
            int  gridY  = mGrid.GetUpperBound(1);//get gridheight

            mStop       = false;    //setparameters
            mStopped    = false;
            mOpen.Clear();   //clear both close and open sets
            mClose.Clear();

            

            sbyte[,] direction;
                direction = new sbyte[4,2]{ {0,-1} , {1,0}, {0,1}, {-1,0}};//for the easy of calulating direction

            parentNode.G         = 0;       //length at intial node
            parentNode.H         = mHEstimate;  // Hueristic of that node
            parentNode.F         = parentNode.G + parentNode.H;
            parentNode.X         = start.X;
            parentNode.Y         = start.Y;

            parentNode.PX        = parentNode.X; //parent of intial node=initial
            parentNode.PY        = parentNode.Y;
            mOpen.Push(parentNode);    //add to initial node 

            while(mOpen.Count > 0 && !mStop)  //if open list is empty or exceed the search limmit loop will break
            {
                parentNode = mOpen.Pop();

               // Console.WriteLine("least cost subscriber" + parentNode.X + " " + parentNode.Y);
              

                if (parentNode.X == end.X && parentNode.Y == end.Y) //Goal state
                {
                    mClose.Add(parentNode); 
                    found = true;
                    break;
                }

                if (mClose.Count > mSearchLimit)    //if exceed the feasible distance =>no path
                {
                    mStopped = true;
                    return null;
                }

                if (mPunishChangeDirection)
                    mHoriz = (parentNode.X - parentNode.PX); 

                //for  each successors
                for (int i=0; i<4; i++)
                {
                    PathFinderNode newNode;
                    newNode.X = parentNode.X + direction[i,0];
                    newNode.Y = parentNode.Y + direction[i,1];

                    //if nodes are in out of the grid bound ignore
                    if (newNode.X < 0 || newNode.Y < 0 || newNode.X >= gridX || newNode.Y >= gridY)
                        continue;

                    int newG;
                    if (mHeavyDiagonals && i>3)
                        newG = parentNode.G + (int) (mGrid[newNode.X, newNode.Y] * 2.41);
                    else
                        newG = parentNode.G + mGrid[newNode.X, newNode.Y];


                    if (newG == parentNode.G)
                    {
                        //Unbrekeable
                        continue;
                    }

                    if (mPunishChangeDirection)
                    {
                        if ((newNode.X - parentNode.X) != 0)
                        {
                            if (mHoriz == 0)
                                //  newG += 20;
                                newG += 10;
                        }
                        if ((newNode.Y - parentNode.Y) != 0)
                        {
                            if (mHoriz != 0)
                               // newG += 20;
                            newG += 10;

                        }
                    }

                    int     foundInOpenIndex = -1;
                    for(int j=0; j<mOpen.Count; j++)
                    {
                        if (mOpen[j].X == newNode.X && mOpen[j].Y == newNode.Y)
                        {
                            foundInOpenIndex = j;
                            break;
                        }
                    }
                    if (foundInOpenIndex != -1 && mOpen[foundInOpenIndex].G <= newG)
                        continue;

                    int     foundInCloseIndex = -1;
                    for(int j=0; j<mClose.Count; j++)
                    {
                        if (mClose[j].X == newNode.X && mClose[j].Y == newNode.Y)
                        {
                            foundInCloseIndex = j;
                            break;
                        }
                    }
                    if (foundInCloseIndex != -1 && (mReopenCloseNodes || mClose[foundInCloseIndex].G <= newG))
                        continue;

                    newNode.PX      = parentNode.X;
                    newNode.PY      = parentNode.Y;
                    newNode.G       = newG;

                    switch(mFormula)
                    {
                        default:
                        case HeuristicFormula.Manhattan:
                            newNode.H       = mHEstimate * (Math.Abs(newNode.X - end.X) + Math.Abs(newNode.Y - end.Y));
                            break;
                        case HeuristicFormula.MaxDXDY:
                            newNode.H       = mHEstimate * (Math.Max(Math.Abs(newNode.X - end.X), Math.Abs(newNode.Y - end.Y)));
                            break;
                        case HeuristicFormula.DiagonalShortCut:
                            int h_diagonal  = Math.Min(Math.Abs(newNode.X - end.X), Math.Abs(newNode.Y - end.Y));
                            int h_straight  = (Math.Abs(newNode.X - end.X) + Math.Abs(newNode.Y - end.Y));
                            newNode.H       = (mHEstimate * 2) * h_diagonal + mHEstimate * (h_straight - 2 * h_diagonal);
                            break;
                        case HeuristicFormula.Euclidean:
                            newNode.H       = (int) (mHEstimate * Math.Sqrt(Math.Pow((newNode.X - end.X) , 2) + Math.Pow((newNode.Y - end.Y), 2)));
                            break;
                        case HeuristicFormula.EuclideanNoSQR:
                            newNode.H       = (int) (mHEstimate * (Math.Pow((newNode.X - end.X) , 2) + Math.Pow((newNode.Y - end.Y), 2)));
                            break;
                        case HeuristicFormula.Custom1:
                            Point dxy       = new Point(Math.Abs(end.X - newNode.X), Math.Abs(end.Y - newNode.Y));
                            int Orthogonal  = Math.Abs(dxy.X - dxy.Y);
                            int Diagonal    = Math.Abs(((dxy.X + dxy.Y) - Orthogonal) / 2);
                            newNode.H       = mHEstimate * (Diagonal + Orthogonal + dxy.X + dxy.Y);
                            break;
                    }
                    if (mTieBreaker)
                    {
                        int dx1 = parentNode.X - end.X;
                        int dy1 = parentNode.Y - end.Y;
                        int dx2 = start.X - end.X;
                        int dy2 = start.Y - end.Y;
                        int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
                        newNode.H = (int) (newNode.H + cross * 0.001);
                    }
                    newNode.F       = newNode.G + newNode.H;

                   
                    

                    //It is faster if we leave the open node in the priority queue
                    //When it is removed, all nodes around will be closed, it will be ignored automatically
                    //if (foundInOpenIndex != -1)
                    //    mOpen.RemoveAt(foundInOpenIndex);

                    //if (foundInOpenIndex == -1)
                        mOpen.Push(newNode);
                }

            //    Console.WriteLine("Closed "+parentNode.X + " " + parentNode.Y);
                mClose.Add(parentNode);

                
            }

           // mCompletedTime = HighResolutionTime.GetTime();
            if (found)
            {
                PathFinderNode fNode = mClose[mClose.Count - 1];
                for(int i=mClose.Count - 1; i>=0; i--)
                {
                    if (fNode.PX == mClose[i].X && fNode.PY == mClose[i].Y || i == mClose.Count - 1)
                    {
                       
                        fNode = mClose[i];
                    }
                    else
                        mClose.RemoveAt(i);
                }
                mStopped = true;
                return mClose;
            }
            mStopped = true;
            return null;
        }

        public void updateCost(int x,int y,byte value)
        {
            mGrid[y, x] = value;  
        }
        
    }


}
