using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mc3vray
{
    public class Ray
    {
        #region Константи та попередні розрахунки сталих параметрів

        // вхідні дані

        public static double[] Hz;              // вузлові точки глибин
        public static double[] Cz;              // швидкість звуку по вузловим точкам глибин
        public static double[] Kz;              // водні прошарки відповідно вузлових точок швидкості звуку
        public static double[] Yr;              // ординати центрів кіл для кожного водного шару

        public static int i0;                   // номер водного шару джерело звуку
        public static double Hobj;              // глибина 
        public static double Lobj;              // відстань 
        public static double Cobj;              // швідкість

        public static double L_000_090, T_000_090, Amp_000_090;
        public static double L_090_180, T_090_180, Amp_090_180;

        public static int i1;                   // номер водного шару гідроакустичної станції
        public static double Hgas;              // глибина 
        public static double Cgas;              // швідкість

        // вихідні дані

        public static List<double> List_AmR = new List<double>();   // амплітуда променів
        public static List<double> List_AnR = new List<double>();   // кут приходу до гідроакустичної станції
        public static List<double> List_LnR = new List<double>();   // відстаню роходжнення променів від джерела звуку до гідроакустичної станції
        public static List<double> List_TmR = new List<double>();   // час за який промені проходять відстань від джерела звуку до гідроакустичної станції

        // параметри за замовченням

        public static double Ksrf = 0.9;        // коефіціент ослаблення при відбитті від поверхні моря
        public static double Kbtm = 0.7;        // коефіціент ослаблення при відбитті від дна моря
        public static double Kenv = 0.001;      // ослаблення амплітуди променів від пройденої відстані на 1 км
        public static int NRefl = 7;            // розрахунковий параметр загальної кількості відбиттів

        public static double dAngel = 0.001;    // крок зміни кута

        // попередні обчислення додаткових 

        public void PrepareVal()
        {
            // UNDONE: exept parameter

            #region Обчислення кількості відзеркалень

            NRefl = 0;

            double dummy = Ksrf * Kbtm;
            while (dummy > 0.1)
            {
                dummy *= dummy;
                NRefl++;
            }
            NRefl++;

            #endregion

            #region обчислюємо коефіцієнти зміни швидкості звуку за глибиною

            Array.Clear(Kz, 0, Kz.Length);
            Array.Resize(ref Kz, Hz.Length - 1);

            for (i1 = 0; i1 < Kz.Length; i1++)
                Kz[i1] = (Cz[i1 + 1] - Cz[i1] + 0.001) / (Hz[i1 + 1] - Hz[i1]);

            #endregion 

            #region обчислюємо ординати центрів кіл для кожного водного шару

            for (i1 = 0; i1 < Kz.Length; i1++)
                Yr[i1] = Cz[i1] / Kz[i1] - Hz[i1];

            #endregion

            #region обчислюємо номер водного шару в якому знаходиться гідроакустича станція

            for (i1 = 0; i1 < Kz.Length; i1++)
                if (Hgas >= Hz[i1])
                    break;

            Cgas = Cz[i1] + Kz[i1] * (Hgas - Hz[i1]);       // швидкість звуку для глибини гідроакустичної станції

            #endregion
        }

        #endregion

        // ***

        public static bool Progress_000_090 = true;
        public static bool Progress_090_180 = true;

        public void Build( /* double Hobj, double Lobj */ )
        {
            Progress_000_090 = true;
            Progress_090_180 = true;

            #region обчислюємо номер водного шару в якому знаходиться джерело звуку

            for (i0 = 0; i0 < Kz.Length; i0++)
                if (Hobj >= Hz[i0])
                    break;

            Cobj = Cz[i0] + Kz[i0] * (Hobj - Hz[i0]);       // швидкість звуку для глибини джерела звуку

            #endregion

            #region старт двох потоків Str_000_090 та Str_090_180

            Thread Steam_000_090 = new Thread(Str_000_090)
            {
                IsBackground = true
            };
            Steam_000_090.Start();

            Thread Steam_090_180 = new Thread(Str_090_180)
            {
                IsBackground = true
            };
            Steam_090_180.Start();


            while (Progress_000_090 || Progress_090_180)
                ;

            #endregion

            // UNDONE: окремо розглянемо випадок коли коли кут дорівнює 90
            // UNDONE: об'єднуємо масиви <90 та >90
        }

        // ***

        public static void Str_000_090( )
        {       
            L_000_090 = 0; T_000_090 = 0; Amp_000_090 = 0;

            double L = 0;
            int Layer = i0;

            

            double R = 0, X = 0;

            double Angel = 0 + dAngel;
            double BgnAngl, EndAngl;

            double H = Hobj;
            double C = Cz[i0] + Kz[i0] * (Hobj - Hz[i0]);

            int j, i = i0;

            if (Hobj < Hz[Layer])
                Angel = -1;
            else
            {   do
                {
                    BgnAngl = Angel * Math.PI / 180;

                    while (Lobj > L)
                    {
                        /*
                        S.Layer_000_090(    BgnAngl,    // початковий кут
                                            H,          // спочатку початкова глибина джерела звуку, потім вузлові точки по глибині
                                            C,          // спочатку початкова швидкість звуку джерела, потім вузлові точки по глибині
                                            i,          // номер поточного точки водного прошарку
                            out EndAngl,    // кінцевий кут
                            out X,          // радіус кола
                            out R,          // центр кола 
                            out j);         // номер поточного точки водного прошарку
                        */
                        L++;

                       // double lri = (Math.Abs(rFi * Math.Cos(eFi)) > Math.Abs(rFi * Math.Cos(bFi))) ? Math.Abs(rFi * (fi - bFi)) : Math.Abs(rFi * (fi - eFi));

                        //i = j;
                        //H = Hz[j];
                        //C = Cz[j];
                        //BgnAngl = EndAngl;
                    }

                    Angel += dAngel;
                }
                while(BgnAngl > 0)
                ;
            }
        }

        public static void Str_090_180()
        {
            
        }

    }
}
