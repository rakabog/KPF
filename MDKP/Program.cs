using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPF
{
    class Program
    {
        static void Main(string[] args)
        {


            KPFInstance TestInstance = new KPFInstance();

            /*
            TestInstance.Load_CB("c:\\primeri\\MDKP\\mknapcb8.txt",0);
            MDKPCplex T = new MDKPCplex(TestInstance);
            T.TimeLimit = 600;
            MDKPProblem Problem = new MDKPProblem(TestInstance);
            Problem.TimeLimit = 2000 * 1000;
            Problem.InitRandom(1);
            Problem.TimeLimit = 600 * 1000;
            Problem.SolveFixSet(25,5, 10000, 25,50,0.1);
            Problem.SaveIntermediate("mknapcb8_P25_MI25_TLF0.1_K5_M50.txt");


            TestInstance.Load_CB("c:\\primeri\\MDKP\\mknapcb9.txt", 0);
            T = new MDKPCplex(TestInstance);
            T.TimeLimit = 600;
            Problem = new MDKPProblem(TestInstance);
            Problem.TimeLimit = 2000 * 1000;
            Problem.InitRandom(1);
            Problem.TimeLimit = 600 * 1000;
            Problem.SolveFixSet(25, 5, 10000, 25, 50, 0.1);
            Problem.SaveIntermediate("mknapcb9_P25_MI25_TLF0.1_K5_M50.txt");
            */


            KPFExperiments Exp = new KPFExperiments();
//                                    Exp.GenerateTables("Table.txt");
                         Exp.SolveMaxBinVarAll();
                         Exp.SolvePopsizeAll();

            //                        Exp.SolveParamTest();
            //   Exp.GenerateTimeTable("TableTime.txt");
            //            Exp.GenerateParamResultFile("ResFile.txt",3,1);

            /*
            Exp.LoadAllRuns("Resmknapcb9_0_25_", "c:\\primeri\\MDKP\\Res_P100_MI25_TLF0.1_K5_M25\\", ".\\Res\\", "P100_MI25_TLF0.1_K5_M25", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_25_", "c:\\primeri\\MDKP\\Res_P50_MI25_TLF0.1_K5_M25\\", ".\\Res\\", "P50_MI25_TLF0.1_K5_M25", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_25_", "c:\\primeri\\MDKP\\Res_P200_MI25_TLF0.1_K5_M25\\", ".\\Res\\", "P200_MI25_TLF0.1_K5_M25", 10);
            */
            /*
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI25_TLF0.1_K5_M50", ".\\Res\\", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI50_TLF0.1_K5_M50", ".\\Res\\", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI100_TLF0.1_K5_M50", ".\\Res\\", 10);


            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P50_MI100_TLF0.1_K5_M50", ".\\Res\\", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI100_TLF0.1_K5_M50", ".\\Res\\", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P200_MI100_TLF0.1_K5_M50", ".\\Res\\", 10);

            */
            /*
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 116056);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI50_TLF0.1_K5_M50", ".\\Res\\", 10, 116056);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI100_TLF0.1_K5_M50", ".\\Res\\", 10, 116056);



            Exp.LoadAllRuns("Resmknapcb5_0_50_", "c:\\primeri\\MDKP\\", "P25_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 59187);
            Exp.LoadAllRuns("Resmknapcb5_0_50_", "c:\\primeri\\MDKP\\", "P50_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 59187);
            Exp.LoadAllRuns("Resmknapcb5_0_50_", "c:\\primeri\\MDKP\\", "P100_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 59187);
            Exp.LoadAllRuns("Resmknapcb5_0_50_", "c:\\primeri\\MDKP\\", "P200_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 59187);
            */

//            KPFInstance Instance = new KPFInstance();
//            ;




            //            Instance.Load("c:\\primeri\\KPF\\MF\\500\\02_id_102_objs_500_size_1500_sets_3000_maxNumConflicts_2_maxCost_15_seme_1518.txt");

    //        Instance.Load("c:\\primeri\\KPF\\MF\\500\\01_id_1101_objs_500_size_1500_sets_4000_maxNumConflicts_2_maxCost_15_seme_2097.txt");
            
                      //            Instance.Load("c:\\primeri\\KPF\\LK\\500\\01_id_101b_objs_500_size_1500_sets_3000_maxNumConflicts_2_maxCost_15_seme_2097.txt");
                      //            Instance.Load("c:\\primeri\\KPF\\LK\\1000\\01_id_116b_objs_1000_size_3000_sets_6000_maxNumConflicts_2_maxCost_15_seme_2558.txt");
//                                   Instance.Load("c:\\primeri\\KPF\\MF\\1000\\01_id_1116_objs_1000_size_3000_sets_8000_maxNumConflicts_2_maxCost_15_seme_2558.txt");
                      /*

                                  KPFProblem Problem = new KPFProblem(Instance);
                                  Problem.RCLSize = 5;
                                  Problem.SolveGreedy();
                      /**/


                      //            KPFCplex Solver = new KPFCplex(Instance);
                      //            Exp.SolveAll();

                      //             Solver.Solve();
            /*          
                      KPFProblem Problem = new KPFProblem(Instance);
                      Problem.TimeLimit = 2000 * 1000;
                      Problem.InitRandom(1);
                      Problem.TimeLimit = 600 * 1000;
                      Problem.RCLSize = 5;    
                      Problem.SolveFixSet(100, 5, 10000,250, 50, 0.1);
            /*          */



            System.Console.ReadKey();
        }
    }
}
