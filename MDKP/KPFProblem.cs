using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPF
{

    
    internal class KPFProblem
    {

        KPFInstance mInstance;
        Random mGenerator;
        KPFSolution mSolution;
        KPFSolution mBestSolution;
        KPFSolutionTracker mSolutionTracker;


        List<int> mIntermediateSolutions;
        List<long> mIntermediateSolutionsTimes;
        List<long> mIntermediateSolutionsIterations;
        Stopwatch mStopWatch;
        int mNumberOfSolutionsGenerated;
        long mTimeLimit;
        int mRCLSize;
        RCL<int> mRCL;

        public long TimeLimit{
            get { return mTimeLimit;}
            set { mTimeLimit = value; }
        }

        public int RCLSize
        {
            get { return mRCLSize; }
            set { mRCLSize = value; mRCL = new RCL<int>(mRCLSize); }
        }
        public KPFSolution Solution
        {
            get { return mSolution; }
        }
        public KPFSolution BestSolution
        {
            get { return mBestSolution; }
        }


        public int GetNumBinForFixedSetSize(int iSize) {


            double NumBinItems = mInstance.NumItems - iSize;
            double NumCombTotal = mInstance.NumItems * (mInstance.NumItems - 1) / 2;
            double NumRemove = (mInstance.NumItems - iSize) * iSize + (iSize * (iSize - 1))/2;
               

            return (int)Math.Round(NumBinItems + mInstance.NumForfiets*(1- NumRemove / NumCombTotal));
        
        }
        public int GetFixedSetSizeForNumBinVar(int MaxBinVar) {

            int MinFix = mInstance.NumItems;
            int cSize;

            for (int i = 0; i < mInstance.NumItems; i++) {

                cSize = GetNumBinForFixedSetSize(i);

                if ((cSize<MaxBinVar) && (i < MinFix)){
                    MinFix = i;
                }
            }

            return MinFix;
        }



        int GetHeuristic()
        {
            int cValue;
            int Select;
            if (mSolution.AvaillableItems.Count == 0)
                return -1;

            mRCL = new RCL<int>(mRCLSize);


            int counter = 0;
            foreach (int iItem in mSolution.AvaillableItems)
            {
                if((mSolution.Contributions[iItem]>0) && mSolution.CanAdd(iItem))
                
                    mRCL.Add(iItem, mSolution.Contributions[iItem]);
            }

            if (mRCL.CurrentSize == 0)
                return -1;
            return mRCL.GetCandidate(mGenerator.Next() % mRCL.CurrentSize);
        }


        public void InitTracking()
        {


            mIntermediateSolutions = new List<int>();
            mIntermediateSolutionsTimes = new List<long>();
            mIntermediateSolutionsIterations = new List<long>();


        }

        public void SaveIntermediate(string Filename) {

            StreamWriter File = new StreamWriter(Filename);

            for (int i = 0; i < mIntermediateSolutions.Count; i++) {

                File.WriteLine(mIntermediateSolutions[i] + " " + mIntermediateSolutionsIterations[i]+" " + mIntermediateSolutionsTimes[i]);
            }
            File.Close();
        }

        public bool CheckBest() {

            if (mBestSolution == null) {
                Solution.CalculateObjective();
                mBestSolution = Solution;
                

                mIntermediateSolutions.Add(mBestSolution.CalculateObjective());
                mIntermediateSolutionsIterations.Add(mNumberOfSolutionsGenerated);
                mIntermediateSolutionsTimes.Add(mStopWatch.ElapsedMilliseconds);

                return true;
            }
            if (mBestSolution.CalculateObjective() < mSolution.CalculateObjective()) {
                mBestSolution = Solution;

                mIntermediateSolutions.Add(mBestSolution.CalculateObjective());
                mIntermediateSolutionsIterations.Add(mNumberOfSolutionsGenerated);
                mIntermediateSolutionsTimes.Add(mStopWatch.ElapsedMilliseconds);

                return true; 
            }

            return false;
        } 

        public KPFProblem() {


        }
        public void InitRandom(int seed) {

            mGenerator = new Random(seed);
        }
        public KPFProblem(KPFInstance iInstance)
        {
            mInstance = iInstance;
            InitRandom(0);

        }


        public void SolveGreedy() {


            mSolution = new KPFSolution(mInstance);
            mSolution.InitGreedy();
            int tIndex;
                       

            while (true) {

                if (mSolution.AvaillableItems.Count == 0)
                    break;

                tIndex = GetHeuristic();
                if (tIndex == -1)
                    break;
                if (!mSolution.AddItem(tIndex))
                    break;
            }
                
        }

        public void SolveRandom() {

            int TotalWeight;
            List<int> Selected = new List<int>();
            
            TotalWeight = 0;
            

            List<int> Indexes = new List<int>();


            for (int i = 0; i < mInstance.NumItems; i++) {

                Indexes.Add(i);
            }

            shuffle<int>(Indexes, mGenerator);

            bool Satisfied = false;
            foreach (int index in Indexes) {

                     Satisfied = false;

                    if (TotalWeight + mInstance.ItemWeights[index] > mInstance.Capacity)
                        continue;

                    TotalWeight += mInstance.ItemWeights[index];

                    Selected.Add(index);
            }

            

            mSolution = new KPFSolution(Selected, mInstance);
        
        }
        
        

        public void SolveFixSet(int PopulationSize, int K, int MaxIterations, int iMaxBinVar, int MaxStag, double MaxCalcTime) {

            List<int> SolutionIndexes= new List<int>();
            List<int> SelIndexes = new List<int>();
            int BaseIndex;
            int cK ;
            KPFSolution BaseSolution;
            KPFSolution.KPFItemStatus[] Fix;
            MDKPCplexEXT cFix = new MDKPCplexEXT();
            KPFCplex Solver;
            int Stag =0;
            double cMaxCalcTime = MaxCalcTime;
            double BestMaxCalcTime = MaxCalcTime;

            int AddIndex;

            mStopWatch = new Stopwatch();
            mStopWatch.Start();

            InitTracking();
            mSolutionTracker = new KPFSolutionTracker(PopulationSize, mInstance);

            for (int i = 0; i < PopulationSize; i++) {

                Console.WriteLine("Solution  " +i);
                SolveRandom();
//                SolveGreedy();
                CheckBest();
                mSolutionTracker.AddSolution(mSolution);
                
            }



            int cFixSize = GetFixedSetSizeForNumBinVar(iMaxBinVar);
            while (mNumberOfSolutionsGenerated < MaxIterations) {


                SolutionIndexes.Clear();
                for (int j = 0; j < mSolutionTracker.GetNumSolutions(); j++) {
                    SolutionIndexes.Add(j);
                }

                BaseIndex = mGenerator.Next() % mSolutionTracker.GetNumSolutions();
                BaseSolution = mSolutionTracker.GetSolution(BaseIndex);
                BaseSolution.CalculateObjective();

                shuffle<int>(SolutionIndexes, mGenerator);
                SelIndexes.Clear();


                cK = 5 + mGenerator.Next() % K;

                if (mSolutionTracker.NumberOfSolutions < cK)
                    cK = mSolutionTracker.NumberOfSolutions;
                else
                    cK = K;
/*                */
                for (int s = 0; s < cK; s++) {
                    SelIndexes.Add(SolutionIndexes[s]);
                
                }


                 Fix = mSolutionTracker.GetFix(BaseIndex, SelIndexes,  cFixSize, mGenerator);


                Solver = new KPFCplex(mInstance);
                Solver.TimeLimit = cMaxCalcTime;

                cFix.mStates = Fix;
                cFix.mHotStart = mSolutionTracker.GetBestFit(Fix);
                Solver.Solve(cFix);


                mNumberOfSolutionsGenerated++;
                Stag++;


                if (Solver.Solution == null)
                    continue;

                mSolution = Solver.Solution;
                mSolution.CalculateObjective();
                AddIndex = mSolutionTracker.AddSolution(mSolution);

                if (CheckBest()) {

                    BestMaxCalcTime = cMaxCalcTime;
                }


                if (AddIndex >= 0 )
                {
                    Stag=0;
                }

                if (Stag >=  MaxStag)
                {
                       cMaxCalcTime = cMaxCalcTime * 2;

         //           if (BestMaxCalcTime / cMaxCalcTime > 4)
//                        break;
                       Stag = 0;
                }


                if (mBestSolution.Objective == mInstance.Optimal)
                    break;
                if (mStopWatch.ElapsedMilliseconds > mTimeLimit)
                    break;
                if (cMaxCalcTime > 1.6)
                    break;
            }
        
        }
      
        static public void shuffle<T>(List<T> list, Random nGenerator)
        {
            //            Random rng = new Random();   // i.e., java.util.Random.
            int n = list.Count;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = nGenerator.Next(n);  // 0 <= k < n.
                n--;                     // n is now the last pertinent index;
                T temp = list[n];     // swap array[n] with array[k] (does nothing if k == n).
                list[n] = list[k];
                list[k] = temp;
            }
        }

    }

}
