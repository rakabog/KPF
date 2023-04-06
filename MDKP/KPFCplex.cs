using ILOG.Concert;
using ILOG.CPLEX;

namespace KPF
{
    class KPFCplex
    {

        KPFInstance mInstance;

        private IIntVar[] x;   // Decision variables
        private IIntVar[] v;   // Decision variables
        Cplex cplex;
        bool mSolved;
        double mTimeLimit;
        KPFSolution mSolution;


        public KPFSolution Solution
        {
            get { return mSolution; }
        }

        public double TimeLimit
        {
            get { return mTimeLimit; }
            set { mTimeLimit = value; }
        }


        public KPFCplex(KPFInstance iInstance)
        {
            mInstance = iInstance;
            mTimeLimit = 0;
        }

        public void GenerateModel(MDKPCplexEXT Fix = null)
        {

            cplex = new Cplex();
            GenerateVariables();
            GenerateConstraints();
            GenerateObjective();
            if (Fix != null)
            {
                GenerateFixConstraints(Fix);
            }

        }

        public void GenerateFixConstraints(MDKPCplexEXT Fix)
        {

            ILinearIntExpr expr;

            if (Fix.mStates != null)
            {
                for (int i = 0; i < Fix.mStates.Length; i++)
                {

                    expr = cplex.LinearIntExpr();

                    switch (Fix.mStates[i])
                    {
                        case KPFSolution.MDKPItemStatus.Unknown:
                            break;
                        case KPFSolution.MDKPItemStatus.NotUsing:
                            expr.AddTerm(1, x[i]);
                            cplex.AddEq(0, expr);
                            break;
                        case KPFSolution.MDKPItemStatus.Using:
                            expr.AddTerm(1, x[i]);
                            cplex.AddEq(1, expr);
                            break;
                    }
                }
            }

            if (Fix.mNumElements != -1)
            {

                expr = cplex.LinearIntExpr();
                for (int i = 0; i < mInstance.NumItems; i++)
                {

                    expr.AddTerm(x[i], 1);
                }

                cplex.AddEq(Fix.mNumElements, expr);

            }

            if (Fix.mPairs != null)
            {

                foreach (int[] cpair in Fix.mPairs)
                {

                    expr = cplex.LinearIntExpr();
                    expr.AddTerm(x[cpair[0]], 1);
                    expr.AddTerm(x[cpair[1]], -1);
                    cplex.AddEq(expr, 0);

                }
            }
        }

        private void GenerateVariables()
        {

            x = new IIntVar[mInstance.NumItems];
            v = new IIntVar[mInstance.NumForfiets];


            int[] xlb = new int[mInstance.NumItems];
            int[] xub = new int[mInstance.NumItems];

            int[] vlb = new int[mInstance.NumForfiets];
            int[] vub = new int[mInstance.NumForfiets];





            for (int i = 0; i < mInstance.NumItems; i++)
            {

                xlb[i] = 0;
                xub[i] = 1;
            }

            for (int i = 0; i < mInstance.NumForfiets; i++)
            {

                vlb[i] = 0;
                vub[i] = 1;
            }


            // Define the variables
            x = cplex.IntVarArray(mInstance.NumItems, xlb, xub);
            v = cplex.IntVarArray(mInstance.NumForfiets, vlb, vub);


            for (int i = 0; i < mInstance.NumItems; i++)
            {
                x[i].Name = "x" + (i);
            }


            for (int i = 0; i < mInstance.NumForfiets; i++)
            {
                v[i].Name = "v" + mInstance.Forfiets[i].mItem1 + "_"+ mInstance.Forfiets[i].mItem2;
            }




        }


        void GenerateObjective()
        {

            IObjective objective1;
            ILinearIntExpr expr = cplex.LinearIntExpr();

            expr = cplex.LinearIntExpr();

            for (int i = 0; i < mInstance.NumItems; i++)
            {
                expr.AddTerm(mInstance.ItemValues[i], x[i]);
            }

            for (int i = 0; i < mInstance.NumForfiets; i++)
            {
                expr.AddTerm(-mInstance.Forfiets[i].mCost, v[i]);
            }



            objective1 = cplex.Maximize(expr);
            cplex.Add(objective1);
        }

        private void GenerateConstraints()
        {

            ILinearIntExpr expr = cplex.LinearIntExpr();

            // Define the constraints

                expr = cplex.LinearIntExpr();
                for (int j = 0; j < mInstance.NumItems; j++)
                {
                    expr.AddTerm(mInstance.ItemWeights[j], x[j]);
                }
                cplex.AddLe(expr, mInstance.Capacity);



            for (int j = 0; j < mInstance.NumForfiets; j++)
            {
                expr = cplex.LinearIntExpr();
                expr.AddTerm(1, x[mInstance.Forfiets[j].mItem1]);
                expr.AddTerm(1, x[mInstance.Forfiets[j].mItem2]);
                expr.AddTerm(-1, v[j]);
                cplex.AddLe(expr, 1);
            }
            

        }

        public void AddFixedConstraints(int[] fix)
        {
            for (int i = 0; i < fix.Length; i++)
            {
                if (fix[i] == 1 || fix[i] == 0)
                {
                    cplex.AddEq(x[i], fix[i]);
                }
            }
        }

        public void Solve(MDKPCplexEXT Fix = null)
        {

            GenerateModel(Fix);

            if (Fix != null)
            {

                if (Fix.mHotStart != null)
                {

                    IIntVar[] startvar = new IIntVar[mInstance.NumItems];
                    double[] startval = new double[mInstance.NumItems];

                    for (int i = 0; i < mInstance.NumItems; i++)
                    {

                        startvar[i] = x[i];
                        if (Fix.mHotStart.ItemStatuses[i] == KPFSolution.MDKPItemStatus.Using)
                            startval[i] = 1;
                        else
                            startval[i] = 0;
                    }

                    cplex.AddMIPStart(startvar, startval, Cplex.MIPStartEffort.SolveMIP);
                }
            }

              cplex.ExportModel("lpex1.lp");

            if (mTimeLimit > 0)
                cplex.SetParam(Cplex.Param.TimeLimit, mTimeLimit);
            cplex.SetOut(null);



            if (cplex.Solve())
            {
                mSolution = new KPFSolution(mInstance);

                Console.WriteLine("Solution value: " + cplex.ObjValue + "  Thread :" + Thread.CurrentThread.ManagedThreadId);

                double[] xres = cplex.GetValues(x);

                for (int i = 0; i < mInstance.NumItems; i++)
                {

                    if ((int)Math.Round(xres[i]) == 1)
                    {
                        //                        System.Console.WriteLine("x" + i + " " + xres[i]);
                        mSolution.SetStatusItem(i, KPFSolution.MDKPItemStatus.Using);
                    }
                    else
                    {

                        mSolution.SetStatusItem(i, KPFSolution.MDKPItemStatus.NotUsing);
                    }
                }
            }
            else
            {
                mSolved = false;
                mSolution = null;
                Console.WriteLine("Failed to find a solution.");
            }
            cplex.End();
        }



    }


}
