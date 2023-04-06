using KPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPF

{
    internal class MDKPCplexEXT
    {
        public KPFSolution.MDKPItemStatus[] mStates;
        public int                           mNumElements;
        public KPFSolution                  mHotStart;
        public List<int[]>                    mPairs;



        public void InitAll() {

            mNumElements = -1;
            mStates = null;
            mHotStart = null;
            mPairs = null;

        }
        public MDKPCplexEXT()
        {

            InitAll();
        }

        public MDKPCplexEXT(KPFSolution.MDKPItemStatus[] iStates)
        {
            InitAll();
            mStates = iStates;
        }
        public MDKPCplexEXT(int iNumElements)
        {
            InitAll();
            mNumElements = iNumElements;
        }

        public MDKPCplexEXT(KPFSolution iHotStart)
        {
            mNumElements = -1;
            mStates = null;
            mHotStart = iHotStart;
        }
    }
}
