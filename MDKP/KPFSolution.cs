using KPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KPF.KPFSolution;

namespace KPF
{
    internal class KPFSolution
    {
        public enum KPFItemStatus { Using, NotUsing, Unknown};
        KPFInstance     mInstanace;
        KPFItemStatus[] mItemStatuses;
        int              mObjective;
        public int       mNumCreatedDuplicate;

        double[]         mContributions;
        double         mAvaillableCapacity;   

        List<int>           mAvaillableItems;
        List<int>           mSelectedItems;
        
        Random mGenerator;


        public List<int> AvaillableItems { 
        
            get { return mAvaillableItems; }
        }

        public double[] Contributions
        {

            get { return mContributions; }
        }


        public void SetGenerator(Random iGenerator) { 
        
            mGenerator = iGenerator;
        }

        public bool CanAdd(int iItem) {

            return mInstanace.ItemWeights[iItem] <= mAvaillableCapacity;
        }
        public void InitGreedy() {

            mContributions = new double[mInstanace.NumItems];
            mAvaillableItems = new List<int>();
            mSelectedItems = new List<int>();
            for (int i = 0; i < mInstanace.NumItems; i++) {

                mContributions[i] = mInstanace.ItemValues[i] / (double)mInstanace.ItemWeights[i];
                mAvaillableItems.Add(i);
                mItemStatuses[i] = KPFItemStatus.Unknown;
            }

            mAvaillableCapacity = mInstanace.Capacity;    

        }

        public bool AddItem(int iIndex) {

            if (mItemStatuses[iIndex] == KPFItemStatus.Using)
                return false;

            if (mInstanace.ItemWeights[iIndex] > mAvaillableCapacity)
                return false;

            mItemStatuses[iIndex] = KPFItemStatus.Using;
            mAvaillableCapacity -= mInstanace.ItemWeights[iIndex];

            for (int i = 0; i < mInstanace.Forfiets.Length; i++) {

                if ((mInstanace.Forfiets[i].mItem1 == iIndex) && (mItemStatuses[mInstanace.Forfiets[i].mItem2] == KPFItemStatus.Unknown)){
                    mContributions[mInstanace.Forfiets[i].mItem2] -= mInstanace.Forfiets[i].mCost / (double)mInstanace.ItemWeights[mInstanace.Forfiets[i].mItem2];
                }

                if ((mInstanace.Forfiets[i].mItem2 == iIndex) && (mItemStatuses[mInstanace.Forfiets[i].mItem1] == KPFItemStatus.Unknown))
                {
                    mContributions[mInstanace.Forfiets[i].mItem1] -= mInstanace.Forfiets[i].mCost / (double)mInstanace.ItemWeights[mInstanace.Forfiets[i].mItem1];
                }

            }

            mAvaillableItems.Remove(iIndex);
            mSelectedItems.Add(iIndex);
            return true;
        }


       


        public KPFItemStatus[] ItemStatuses
        {
            get { return mItemStatuses; }
        }

        public int Objective { 
            get { return mObjective; }
        }
        public void CalculateAvaillableCapacity() {



                mAvaillableCapacity = mInstanace.Capacity;

            for (int i = 0; i < mInstanace.NumItems; i++)
            {
                if (mItemStatuses[i] == KPFItemStatus.Using)
                {

                        mAvaillableCapacity -= mInstanace.ItemWeights[i];
                }
            }

        }
        public bool CanSwap(int GoingOut, int GoingIn) {


                if (mAvaillableCapacity + mInstanace.ItemWeights[GoingOut] < mInstanace.ItemWeights[GoingIn])
                    return false;

            return true;
        }

        public KPFSolution(KPFInstance iInstanace) {


            mInstanace = iInstanace;
            Allocate();
            

        }
        public bool Improve(Random iGenerator) {

            List<int> Indexes = new List<int>();

            for (int i = 0; i < mInstanace.NumItems; i++)
                Indexes.Add(i);

            KPFProblem.shuffle<int>(Indexes, iGenerator);
            CalculateAvaillableCapacity();

            int GoingOut;
            int GoingIn;
            bool Improve = true ;

            while (Improve)
            {
                Improve = false;
                for (int i = 0; i < mInstanace.NumItems; i++)
                {
                    for (int j = i + 1; j < mInstanace.NumItems; j++)
                    {
                        if (ItemStatuses[Indexes[i]] != ItemStatuses[Indexes[j]])
                        {

                            if (ItemStatuses[Indexes[i]] == KPFItemStatus.Using)
                            {
                                GoingOut = Indexes[i];
                                GoingIn = Indexes[j];
                            }
                            else
                            {
                                GoingIn = Indexes[i];
                                GoingOut = Indexes[j];
                            }

                            if (mInstanace.ItemValues[GoingIn] <= mInstanace.ItemValues[GoingOut])
                                continue;

                            if (CanSwap(GoingOut, GoingIn))
                            {
                                Improve = true;
                                ItemStatuses[GoingOut] = KPFItemStatus.NotUsing;
                                ItemStatuses[GoingIn] = KPFItemStatus.Using;

                                    mAvaillableCapacity += mInstanace.ItemWeights[GoingIn] - mInstanace.ItemWeights[GoingOut];

                            }
                        }
                    }
                }
            }

            return false;
        }
        public KPFSolution(List<int> indexes, KPFInstance iInstanace)
        {


            mInstanace = iInstanace;
            Allocate();

            for (int i = 0; i < mInstanace.NumItems; i++)
            {
                mItemStatuses[i] = KPFItemStatus.NotUsing;
            }

            foreach (int index in indexes) {
                mItemStatuses[index] = KPFItemStatus.Using;
            }
            mSelectedItems = indexes;
            

        }




        public void Allocate() {

            mItemStatuses = new KPFItemStatus[mInstanace.NumItems];
            mSelectedItems = new List<int>();
        }
        public void ResetSolution() {

            for (int i = 0; i < mInstanace.NumItems; i++) {
                mItemStatuses[i] = KPFItemStatus.Unknown;
            }
            mSelectedItems.Clear();
        }
        public int GetNumberOfUsedItems() {

            int Result = 0;

            for (int i = 0; i < mInstanace.NumItems; i++) {
                if (mItemStatuses[i] == KPFItemStatus.Using)
                    Result++;
            }

            return Result;
        
        }

        public void SaveSolution(string FileName) {

            StreamWriter T = new StreamWriter(FileName);


            
                
            T.WriteLine(GetNumberOfUsedItems());

            for (int i = 0; i < mInstanace.NumItems; i++) {

                if (ItemStatuses[i] == KPFItemStatus.Using)
                    T.WriteLine(i);
            }
        
            T.Close();
        }

        public void LoadSolution(string FileName)
        {

            string[] Lines = File.ReadAllLines(FileName);
            int line = 0;
            int NumSelected = Convert.ToInt16(Lines[line++]);
            int Index;

            for (int i = 0; i < mInstanace.NumItems; i++) {
                ItemStatuses[i] = KPFSolution.KPFItemStatus.NotUsing;
            }

            for (int i = 0; i < NumSelected; i++)
            {
                Index = Convert.ToInt16(Lines[line++]);
                ItemStatuses[Index] = KPFSolution.KPFItemStatus.NotUsing;
                    
            }

            
        }



        public void SetStatusItem(int ItemIndex, KPFItemStatus iStatus) {
            mItemStatuses[ItemIndex] = iStatus;
        }
        public bool IsSame(KPFSolution iSolution) {

            for (int i = 0; i < mInstanace.NumItems; i++) {

                if (ItemStatuses[i] != iSolution.ItemStatuses[i])
                    return false;
            }

            return true;
        }
        public bool CheckConstraint() {

                if (GetTotalWeight() > mInstanace.Capacity)
                    return false;
            


                return true;
        }

        public int GetTotalWeight() {

            int Result = 0;

            for (int i = 0; i < mInstanace.NumItems; i++)
            {
                if (mItemStatuses[i] == KPFItemStatus.Using)
                    Result += mInstanace.ItemWeights[i];
            }

            return Result;
        
        }


        public int CalculateObjective() {


            int Sum = 0;

            for (int i = 0; i < mInstanace.NumItems; i++)
            {
                if (mItemStatuses[i] == KPFItemStatus.Using)
                    Sum += mInstanace.ItemValues[i];
            }

            for (int i = 0; i < mInstanace.NumForfiets; i++)
            {

                if ((mItemStatuses[mInstanace.Forfiets[i].mItem1] == KPFItemStatus.Using) &&
                   (mItemStatuses[mInstanace.Forfiets[i].mItem2] == KPFItemStatus.Using))
                            Sum -= mInstanace.Forfiets[i].mCost;
            }



            mObjective = Sum;

            return Sum;
        }

        public void AddInfoFromSolution(KPFSolution iSolution, List<int> Translate) {

            for (int i = 0; i < iSolution.ItemStatuses.Length; i++) {

                mItemStatuses[Translate[i]] = iSolution.ItemStatuses[i];
            }
        }



    }
}
