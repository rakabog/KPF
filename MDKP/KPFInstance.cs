using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Metrics;

namespace KPF
{
    internal class KPFInstance
    {

        private int mNumItems;   // Number of items
        private int mNumForfiets;   // Number of constraints

        private int[] mItemValues;   // Value of each item
        // [items][constraints]
        private int[] mItemWeights;   // Weight of each item
        private int mCapacity;   // Capacity of each constraint
        private int   mOptimal;
        private KPFForfiet[] mForfiets;
        

        public int NumItems
        {
            get { return mNumItems; }
        }

        public int NumForfiets
        {
            get { return mNumForfiets; }
        }




        public int Optimal
        {
            get { return mOptimal; }
        }

        public int[] ItemValues
        {
            get { return mItemValues; }
        }
        public KPFForfiet[] Forfiets
        {
            get { return mForfiets; }
        }


        public int Capacity
        {
            get { return mCapacity; }
        }
        public int[] ItemWeights
        {
            get { return mItemWeights; }
        }


        public KPFInstance()
        {

        }

        public void LoadOptimum(string FileName, int Index)
        {
            string[] Lines = File.ReadAllLines(FileName);
            string[] words = Lines[Index].Split(',');
            double result;

            if (double.TryParse(words[1], out result) ){
                mOptimal = (int)Math.Round(result);
            }
        }


        public KPFInstance(int iNumItems, int iNumForfiets) {

            mNumForfiets = iNumForfiets;
            mNumItems = iNumItems;
            Allocate();
        }

        public void Allocate() { 
        
            mItemValues = new int[mNumItems];
            mItemWeights = new int[mNumItems];
            mForfiets = new KPFForfiet[mNumForfiets];
            
        }


        public bool Load(string FileName) {


            String[] Lines = File.ReadAllLines(FileName);
            string[] words;
            
            List<int> AllValues = new List<int>();
            int line = 0;
            KPFForfiet temp;

            words = Lines[line].Split(' ');


            
            //StartInfo
            mNumItems = Convert.ToInt32((string)words[0]);
            mNumForfiets = Convert.ToInt32((string)words[1]);
            mCapacity = Convert.ToInt32((string)words[2]);
            Allocate();



            //Profits
            line++;
            words = Lines[line].Split(' ');

            for (int i = 0; i < mNumItems; i++) {

                mItemValues[i] = Convert.ToInt32((string)words[i]);
            }

            //Weights
            line++;
            words = Lines[line].Split(' ');
            for (int i = 0; i < mNumItems; i++)
            {

                mItemWeights[i] = Convert.ToInt32((string)words[i]);
            }

            for (int i = 0; i < mNumForfiets; i++) {

                temp = new KPFForfiet();
                line++;
                words = Lines[line].Split(' ');
                temp.mCost = Convert.ToInt32((string)words[1]);

                line++;
                words = Lines[line].Split(' ');
                temp.mItem1 = Convert.ToInt32((string)words[0]);
                temp.mItem2 = Convert.ToInt32((string)words[1]);

                mForfiets[i] = temp;

            }

            return true;
        }



        

    }
}
