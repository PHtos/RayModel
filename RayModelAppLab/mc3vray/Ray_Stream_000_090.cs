using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mc3vray
{
    public class S
    {
        public bool Do_000_090(                 double Angl,    // кут з прошарку в прошарок
                                                double H,       // спочатку початкова глибина джерела звуку, потім вузлові точки по глибині
                                                double C,       // спочатку початкова швидкість звуку джерела, потім вузлові точки по глибині
                                                int i,          // номер поточного точки водного прошарку

                        out double AMP,         // повертаємо амплітуду
                        out double ANG,         // повертаємо кінцевий кут
                        out double TIM,         // повертаємо час
                        out double LNG)         // повертаємо шлях
        {
            AMP = 0;
            ANG = 0;
            TIM = 0;
            LNG = 0;    double BgnAngl = Angl, EndAngl = Angl, X = 0, R = 0, XL = 0;

            int j = i;
            do
            {
                Layer_000_090(              BgnAngl,    // початковий кут
                                            H,          // спочатку початкова глибина джерела звуку, потім вузлові точки по глибині
                                            C,          // спочатку початкова швидкість звуку джерела, потім вузлові точки по глибині
                                            i,          // номер поточного точки водного прошарку
                            out BgnAngl,
                            out EndAngl,    // кінцевий кут
                            out X,          // радіус кола
                            out R,          // центр кола 
                            out j);         // номер поточного точки водного прошарку


            }
            while (true)
            ;
        }

        // пройдений шлях рахує материнська програма
        // відстань контролює материнська програма

        // амплітуду контролює материнська програма

        public static void Layer_000_090(       double Angl,    // кут з прошарку в прошарок
                                                double H,       // спочатку початкова глибина джерела звуку, потім вузлові точки по глибині
                                                double C,       // спочатку початкова швидкість звуку джерела, потім вузлові точки по глибині
                                                int i,          // номер поточного точки водного прошарку

                        out double BgnAngl,     // початковий кут, перерахований по четвертям
                        out double EndAngl,     // кінцевий кут
                        out double X,           // центр кола
                        out double R,           // радіус кола

                        out int j)              // номер поточного точки водного прошарку
        {
            BgnAngl = Angl;
            EndAngl = Angl;

            double BgnA = Angl * Math.PI / 180; // додаткові кути для роботи з дугою кола   
            double EndA = Angl * Math.PI / 180; // додаткові кути для роботи з дугою кола

            R = 0; X = 0;
            j = i;

            #region повне внутрішнє відбиття
            if (BgnAngl == 0 || BgnAngl == 360)
            {
                // UNDONE: повне внутрішнє відбиття
            }
            #endregion

            #region прямий промінь
            if (BgnAngl == 0 || BgnAngl == 360)
            {
                // UNDONE: прямий промінь
            }
            #endregion

            #region промінь напрямлений вверх
            else if (Angl < 90)
            {
                // промінь рухається прямо вверх без заломлювань
                if (Ray_.Kz[j] == 0)                                                             // в цьому випадку немає ані радіуса, ані центра, а початковий кут рівний кінцевом
                    AmpJ_000_090(BgnAngl,                                                       // водний прошарок зміщується на крок вверх
                        out EndAngl, j--, out j, i, H, C, R, BgnA, EndA);

                // промінь рухається прямо вверх з заломленнями
                else
                {
                    R = Ray_.Cz[j + 1] / (Math.Abs(Ray_.Kz[j]) * Math.Sin(BgnA));                 // радіус кола завжди залежить від початкового кута в заданому водному прошарку
                    X = Ray_.Cz[j + 1] / (Ray_.Kz[j] * Math.Tan(BgnA));                           // центр кола також залежить від початкового кута в заданому водному прошарку

                    // промінь вигинається вниз
                    if (Ray_.Kz[j] < 0)
                    {
                        double dummy = Math.Abs(Ray_.Yr[j] + Ray_.Hz[j + 0]);

                        // промінь вигнувся вниз проте перейщов до верхнього водного шару
                        if (dummy < R)
                        {   // HACK: закрив очі на ситуацію коли промінь рухається в прошарку = R
                            BgnA += Math.PI / 2;
                            EndA = Math.PI / 2 + Math.Asin(dummy / R);

                            AmpJ_000_090(Math.Asin(dummy / R) * 180 / Math.PI,                  // водний прошарок зміщується на крок вверх
                                out EndAngl, j--, out j, i, H, C, R, BgnA, EndA);
                        }

                        // промінь вигнувся вниз настільки що лишився в поточному прошарку і пішов до нижнього водного шару 
                        else
                        {   // HACK: закрив очі на ситуацію коли кінцевий кут буде дорівнювати 90 
                            BgnA = Math.PI / 2 + BgnA;
                            EndA = Math.PI / 2 - EndA;

                            AmpJ_000_090(360 - BgnAngl,                                         // водний прошарок залишається незмінним
                                out EndAngl, j, out j, i, H, C, R, BgnA, EndA);
                        }
                    }

                    // промінь вигинається вгору
                    else
                    {
                        double dummy = Math.Abs(Ray_.Yr[j] + Ray_.Hz[j + 0]);

                        // промінь вигнувся вгору проте перейшов до верхнього водного шару
                        if (dummy < R)
                        {   // HACK: закрив очі на ситуацію коли вигнувся на кут быльше 90
                            BgnA += 3 * Math.PI / 2;
                            EndA = 3 * Math.PI / 2 + Math.Asin(dummy / R);

                            AmpJ_000_090(Math.Asin(dummy / R) * 180 / Math.PI,
                                out EndAngl, j--, out j, i, H, C, R, BgnA, EndA);               // водний прошарок зміщується на крок вверх
                        }

                        // промінь вигнувся вгору настільки що лишився в поточному прошарку і пішов назад
                        else
                            // UNDONE: ситуація коли вигинаючись промінь може потрапити до ГАС
                            EndAngl = -1;
                    }
                }
            }
            #endregion

            #region промінь напрямлений вниз
            else if (BgnAngl > 90)
            {
                // промінь рухається прямо вниз без заломлювань
                if (Ray_.Kz[j] == 0)                                                             // в цьому випадку немає ані радіуса, ані центра, а початковий кут рівний кінцевому
                    AmpJ_000_090(BgnAngl,                                                       // водний прошарок зміщується на крок вниз
                        out EndAngl, j++, out j, i, H, C, R, BgnA, EndA);

                // промінь рухається прямо вниз з заломленнями
                else
                {
                    R = Ray_.Cz[j + 0] / Math.Abs(Ray_.Kz[j] * Math.Sin(BgnA));                   // радіус кола завжди залежить від початкового кута в заданому водному прошарку
                    X = Ray_.Cz[j + 0] / (Ray_.Kz[j] * Math.Tan(BgnA));                           // центр кола також залежить від початкового кута в заданому водному прошарку

                    // промінь вигинається вниз
                    if (Ray_.Kz[j] < 0)
                    {
                        double dummy = Math.Abs(Ray_.Yr[j] + Ray_.Hz[j + 1]);

                        // промінь вигнувся вниз проте перейщов до попереднього водного шару
                        if (dummy < R)
                        {
                            BgnA -= 3 * Math.PI / 2;
                            EndA = Math.Asin(dummy / R);

                            AmpJ_000_090(270 + Math.Asin(dummy / R) * 180 / Math.PI,            // водний прошарок зміщується на крок вниз
                                out EndAngl, j++, out j, i, H, C, R, BgnA, EndA);
                        }

                        // промінь вигнувся вниз настільки що лишився в поточному прошарку і пішов назад
                        else
                            // HACK: закрив очі на ситуацію коли вигинаючись промінь може потрапити до ГАС
                            EndAngl = -1;
                    }

                    // промінь вигинається вгору
                    else
                    {
                        double dummy = Math.Abs(Ray_.Yr[j] + Ray_.Hz[j + 1]);

                        // промінь вигнувся вгору проте перейщов до попереднього водного шару
                        if (dummy < R)
                        {   // HACK: закрив очі на ситуацію коли промінь рухається в прошарку = R
                            BgnA -= Math.PI / 2;
                            EndA = Math.PI + Math.Asin(dummy / R);

                            AmpJ_000_090(270 + Math.Asin(dummy / R) * 180 / Math.PI,            // водний прошарок зміщується на крок вниз
                                out EndAngl, j++, out j, i, H, C, R, BgnA, EndA);
                        }

                        // промінь вигнувся вгору настільки що лишився в поточному прошарку і пішов до верхнього водного шару
                        else
                        {   // HACK: закрив очі на ситуацію коли кінцевий кут буде дорівнювати 90
                            BgnA -= Math.PI / 2;                                                // промінь по дузі пішов вверх
                            EndA = 7 * Math.PI / 2 - EndA;

                            AmpJ_000_090(360 - BgnAngl,                                         // водний прошарок залишається незмінним
                                out EndAngl, j, out j, i, H, C, R, BgnA, EndA);
                        }
                    }
                }
            }
            #endregion
        }


        public static void AmpJ_000_090(double BgnAngl, out double EndAngl,
                                                int j0, out int j1,
                                                int i, double H, double C, double R, double BgnA, double EndA)
        {
            EndAngl = BgnAngl;  j1 = j0;

            #region Основні розразунки 
            if (j1 <= 0)                            // якщо це вже поверхня, то:
            {
                j1 = 0;                             // перевірка на дурня

                EndAngl = 360 - BgnAngl;            // промінь віддзеркалюється від поверхні моря і змінює напрямок
                Ray_.Amp_000_090 *= Ray_.Ksrf;        // а також зменшує амплітуду сигналу
            }
            else if (j1 >= Ray_.Kz.Length - 1)       // якщо це вже дно, то:
            {
                j1 = Ray_.Kz.Length - 1;             // перевірка на дурня

                EndAngl = 360 - BgnAngl;            // промінь віддзеркалюється від поверхні дна і змінює напрямок,
                Ray_.Amp_000_090 *= Ray_.Kbtm;        // а також зменшує амплітуду сигналу
            }


            if (Ray_.Kz[i] == 0)
            {
                Ray_.L_000_090 = Math.Abs(H - Ray_.Hz[i]) / Math.Sin(BgnAngl * Math.PI / 180);
                // TODO: Ray.x_000_090
            }
            else
            {
                Ray_.L_000_090 = Math.Abs(R * (EndA - BgnA));
                Ray_.x_000_090 = R * Math.Abs(Math.Cos(EndA) - Math.Cos(BgnA));
            }
                Ray_.T_000_090 = 2 * Ray_.L_000_090 / (C + Ray_.Cz[j1]);

            #endregion

            //#region Перевірка влучання променя в об'єкт

            //if (Ray.Amp_000_090 > 0.05)
            //{
            //    if (XL + R * Math.Cos(EndAngl) < Ray.Lobj)
            //    {
            //        H = Ray.Hz[j];
            //        C = Ray.Cz[j];
            //        XL += R * Math.Cos(EndAngl);

            //        i = j;
            //        BgnAngl = EndAngl;
            //    }
            //    else
            //    {
            //        // HACK: check
            //        double aA = Math.Atan2(2 * Math.PI + Ray.Hobj - Ray.Yr[i], Ray.Lobj - XL);

            //        if (Math.Abs(XL + R * Math.Cos(aA) - Ray.Lobj) < 0.1 && Math.Abs(Math.Abs(Ray.Yr[i] + R * Math.Sin(aA)) - Ray.Hobj) < 0.1)
            //        {
            //            // TODO: tilt
            //            // TODO: BgnA, EndA
            //            XL += R * Math.Cos(aA);

            //            return true;
            //        }
            //        else
            //            return false;
            //    }
            //}
            //else
            //    // HACK: if tilt
            //    return false;

            //#endregion
        }

    }
}
