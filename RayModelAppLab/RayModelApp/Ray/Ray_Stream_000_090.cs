using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayModelApp
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
            LNG = 0;    double BgnA = Angl, EndA = Angl, X = 0, R = 0, XL = 0;



            #region Джерело знаходиться між шарами (промінь рухається вверх 000_090)

            /*
            if (H != Ray_.Hz[i + 1])
            {
                // гіпотетична ситуація, коли об'єкт знаходиться на поверхні і випромінює вверх
                if (H == Ray_.Hz[i + 0])
                    return false;

                // джерело знаходиться між шарами і випромінює вверх
                else
                {   // промінь рухається прямо вверх без заломлювань
                    if (Ray_.Kz[i] == 0)                                                        // в цьому випадку немає ані радіуса, ані центра, а початковий кут рівний кінцевом
                        i--;                                                                    // водний прошарок зміщується на крок вверх
                    // промінь рухається прямо вверх з заломленнями
                    else
                    {   double dummy = Math.Sin(Angl * Math.PI / 180);

                        R = Ray_.Cz[i + 1] / (Math.Abs(Ray_.Kz[i]) * dummy);                    // радіус кола завжди залежить від початкового кута в заданому водному прошарку
                        X = Ray_.Cz[i + 1] / (Ray_.Kz[i] * dummy);                              // центр кола також залежить від початкового кута в заданому водному прошарку

                        dummy = Math.Abs(C / Ray_.Kz[i] + Ray_.Hz[i] - H) / R;

                        // промінь вигинається вниз
                        if (Ray_.Kz[j] < 0)
                        {   // промінь вигнувся вниз проте перейщов до верхнього водного шару
                            if (dummy < 1)
                            {
                                bA += Math.PI / 2;
                                eA = Math.PI / 2 + Math.Asin(dummy);

                                EndA = Math.Asin(dummy) * Ray_.Grd; j--;                // водний прошарок зміщується на крок вверх
                            }
                            // промінь вигнувся вниз настільки що лишився в поточному прошарку і пішов до нижнього водного шару 
                            else
                            {
                                bA = Math.PI / 2 + bA;
                                eA = Math.PI / 2 - eA;

                                EndA = 360 - BgnA;                                        // водний прошарок залишається незмінним
                            }
                        }

                        // промінь вигинається вгору
                        else
                        {   // промінь вигнувся вгору проте перейшов до верхнього водного шару
                            if (dummy < 1)
                            {
                                bA += 3 * Math.PI / 2;
                                eA = 3 * Math.PI / 2 + Math.Asin(dummy);

                                EndA = Math.Asin(dummy) * Ray_.Grd; i--;                    // водний прошарок зміщується на крок вверх
                            }
                            // TODO: промінь вигнувся вгору настільки що лишився в поточному прошарку і пішов назад
                            else
                            {
                                bA = 0;
                                eA = 0;

                                EndA = -1;
                            }
                        }
                    }
                }
            }
            */
            #endregion





            int j = i;

            do
            {
                Layer_000_090(              BgnA,    // початковий кут
                                            i,          // номер поточного точки водного прошарку по глибині
                            out BgnA,
                            out EndA,    // кінцевий кут
                            out X,          // радіус кола
                            out R,          // центр кола 
                            out j);         // номер поточного точки водного прошарку


            }
            while (EndA != -1)
            ;


            return true;
        }

        // пройдений шлях рахує материнська програма
        // відстань контролює материнська програма

        // амплітуду контролює материнська програма

        public static void Layer_000_090(       double Angl,    // кут з прошарку в прошарок
                                                int i,          // номер поточного точки водного прошарку
                        out double BgnA,        // початковий кут, перерахований по четвертям
                        out double EndA,        // кінцевий кут
                        out double X,           // центр кола
                        out double R,           // радіус кола

                        out int j)              // номер поточного точки водного прошарку
        {
            BgnA = Angl;
            EndA = BgnA;

            double Yr = 0;
            R = 0; X = 0;
            j = i;

            //    double dummy = Ray_.Yr[i] - (Ray_.Hz[i + 1] - Ray_.Hz[i + 0]);
            double bA = Angl * Math.PI / 180; // додаткові кути для роботи з дугою кола   
            double eA = Angl * Math.PI / 180; // додаткові кути для роботи з дугою кола

            double x_ = 0, l_ = 0, t_ = 0;

            #region повне внутрішнє відбиття
            if (BgnA == 0 || BgnA == 360)
            {
                // UNDONE: повне внутрішнє відбиття
            }
            #endregion

            #region прямий промінь
            if (BgnA == 0 || BgnA == 360)
            {
                // UNDONE: прямий промінь
            }
            #endregion

            // HACK: закрив очі на ситуацію коли промінь рухається в прошарку = R

            #region промінь напрямлений вверх 0 - 90
            else if (Angl < 90)
            {
                // промінь рухається прямо вверх без заломлювань
                if (Ray_.Kz[j - 1] == 0)                                                                // в цьому випадку немає ані радіуса, ані центра, а початковий кут рівний кінцевому
                {
                    s1(--j, BgnA, out EndA);                                                            // водний прошарок зміщується на крок вверх

                    l_ = Ray_.L_000_090 + Math.Abs((Ray_.Hz[j] - Ray_.Hz[i]) / Math.Sin(BgnA * Math.PI / 180));
                    x_ = Ray_.x_000_090 + Math.Abs((Ray_.Hz[j] - Ray_.Hz[i]) / Math.Atan(BgnA * Math.PI / 180));
                }
                // промінь рухається прямо вверх з заломленнями
                else
                {
                    R = Ray_.Cz[j] / (Ray_.Kz[j - 1] * Math.Cos(bA));

                    // промінь вигинається вгору 
                    if (Ray_.Kz[j - 1] < 0)
                    {
                        X = Ray_.Cz[j - 0] / (Ray_.Kz[j - 1] / Math.Tan(bA));

                        // промінь вигнувся вгору проте перейщов до верхнього водного шару
                        if (Ray_.Cz[j - 0] / (R * Ray_.Kz[j - 1]) < 1)                                  // перевірємо чи модуль дельти центра кола і наступного прошарку більше радіусу |Yr - H[j-1]| < R
                        {
                            //  bA = Math.Acos(Ray_.Cz[j - 0] / (R * Ray_.Kz[j - 1]));
                            eA = Math.Acos(Ray_.Cz[j - 1] / (R * Ray_.Kz[j - 1]));

                            s1(--j, eA * Ray_.Grd, out EndA);                                           // водний прошарок зміщується на крок вверх

                            bA += 3 * Math.PI / 2;
                            eA += 3 * Math.PI / 2;
                        }
                        // теоритично неможлива ситуація, коли промінь вигнувся вгору настільки, що пішов у зворотньому напрямку
                        else
                            return;
                    }
                    // промінь вигинається вниз
                    else
                    {
                        X = Ray_.Cz[j] / (Ray_.Kz[j - 1] * Math.Tan(bA));
                                                                                                            // Yr < 0 (Kz < 0 і центр кола нижче водного прошарку H[j-0]), 
                        Yr = Math.Abs(Ray_.Cz[j] / Ray_.Kz[j - 1] + Ray_.Hz[j - 1]) / R;                       // перевірємо чи модуль дельти центра кола і наступного прошарку більше радіусу |Yr - H[j-1]| < R

                        // промінь вигнувся вниз проте перейщов до верхнього водного шару
                        if (Yr < 1)
                        {
                            s1(--j, Math.Asin(Yr) * Ray_.Grd, out EndA);                            // водний прошарок зміщується на крок вверх

                            bA += Math.PI / 2;
                            eA = Math.PI / 2 + Math.Asin(Yr);
                        }
                        // промінь вигнувся вниз настільки що лишився в поточному прошарку і пішов до нижнього водного шару 
                        else
                        {
                            s1(j, 360 - BgnA, out EndA);                                                    // водний прошарок залишається незмінним

                            bA += Math.PI / 2;
                            eA = Math.PI / 2 - eA;
                        }
                    }





                    l_ = Ray_.L_000_090 + R * Math.Abs(eA - bA);
                    x_ = Ray_.x_000_090 + R * Math.Abs(Math.Cos(eA) - Math.Cos(bA));
                }
                t_ = Ray_.T_000_090 + 2 * l_ / (Ray_.Cz[i] + Ray_.Cz[i - 1]);
            }
            #endregion

            #region промінь напрямлений вниз 270 - 360
            else if (BgnA > 90)
            {
                // промінь рухається прямо вниз без заломлювань
                if (Ray_.Kz[j - 0] == 0)                                                        // в цьому випадку немає ані радіуса, ані центра, а початковий кут рівний кінцевому
                {
                    s1(++j, BgnA, out EndA);                                                    // водний прошарок зміщується на крок вниз

                    l_ = Ray_.L_000_090 + Math.Abs((Ray_.Hz[j] - Ray_.Hz[i]) / Math.Cos((BgnA - 270) * Math.PI / 180));
                    x_ = Ray_.x_000_090 + Math.Abs((Ray_.Hz[j] - Ray_.Hz[i]) * Math.Atan((BgnA - 270) * Math.PI / 180));
                }
                // промінь рухається прямо вниз з заломленнями
                else
                {
                    R = Ray_.Cz[j] / Math.Abs(Ray_.Kz[j - 0] * Math.Sin(bA));
                    X = Ray_.Cz[j] / (Ray_.Kz[j - 0] * Math.Tan(bA));

                    // промінь вигинається вгору 
                    if (Ray_.Kz[j - 0] < 0)
                    {
                        double dummy = Math.Abs(Ray_.Yr[j] + Ray_.Hz[j + 1]) / R;               // Yr < 0 (Kz < 0 і центр кола вище водного прошарку H[j+0]), 
                                                                                                // перевірємо чи модуль дельти центра кола і наступного прошарку більше радіусу |Yr - H[j+1]| < R
                                                                                                // промінь вигнувся вгору проте перейщов до попереднього водного шару
                        X = Ray_.Cz[j + 1] / (Ray_.Kz[j - 0] / Math.Tan(bA));

                        if (dummy < 1)
                        {
                            s1(++j, 270 + Math.Asin(dummy) * Ray_.Grd, out EndA);          // водний прошарок зміщується на крок вниз

                            bA -= Math.PI / 2;
                            eA = Math.PI + Math.Asin(dummy);
                        }
                        // промінь вигнувся вгору настільки що лишився в поточному прошарку і пішов до верхнього водного шару
                        else
                        {
                            s1(j, 360 - BgnA, out EndA);                                        // водний прошарок залишається незмінним

                            bA -= 1 * Math.PI / 2;
                            eA = 7 * Math.PI / 2 - eA;
                        }




                    }
                    // промінь вигинається вниз
                    else
                    {
                        double dummy = Math.Asin(Math.Abs(Ray_.Yr[j] + Ray_.Hz[j + 0]) / R);    // Yr < 0 (Kz < 0 і центр кола нижче водного прошарку H[j+1]), 

                        s1(++j, 270 + dummy * Ray_.Grd, out EndA);                         // водний прошарок зміщується на крок вниз

                        bA -= 3 * Math.PI / 2;
                        eA = dummy;
                    }

                    l_ = Ray_.L_000_090 + R * Math.Abs(eA - bA);
                    x_ = Ray_.x_000_090 + R * Math.Abs(Math.Cos(eA) - Math.Cos(bA));
                }
                // HACH: не завжди так
                t_ = Ray_.T_000_090 + 2 * l_ / (Ray_.Cz[j] + Ray_.Cz[j + 1]);
            }
            #endregion

            //

            #region Перевірка влучання променя в об'єкт

            if (x_ < Ray_.Lobj)
            {
                double Alpha = 2 * Math.PI + Math.Atan2(Ray_.Hobj - Ray_.Yr[i], Ray_.Lobj - (Ray_.x_000_090 + X));

                if (    Math.Abs(Ray_.Lobj - (R * Math.Cos(Alpha) + Ray_.x_000_090 + X)) < 0.1 &&
                        Math.Abs(Ray_.Hobj - (R * Math.Sin(Alpha) + Ray_.Yr[i])) < 0.1 )
                {



                    //double lri = (Math.Abs(rFi * Math.Cos(eFi)) > Math.Abs(rFi * Math.Cos(bFi))) ? Math.Abs(rFi * (fi - bFi)) : Math.Abs(rFi * (fi - eFi));
                    //tmr += 2 * lri / (cz[iFi + 1] + cz[iFi]);
                }
            }
            else
            {
                Ray_.L_000_090 = l_;
                Ray_.T_000_090 = t_;

                Ray_.x_000_090 = x_;
            }
            

            //if (Ray.Amp_000_090 > 0.05)
            //{
            //    if (XL + R * Math.Cos(EndA) < Ray.Lobj)
            //    {
            //        H = Ray.Hz[j];
            //        C = Ray.Cz[j];
            //        XL += R * Math.Cos(EndA);

            //        i = j;
            //        BgnA = EndA;
            //    }
            //    else
            //    {
            //        // HACK: check


            //        if (Math.Abs(XL + R * Math.Cos(aA) - Ray.Lobj) < 0.1 && Math.Abs(Math.Abs(Ray.Yr[i] + R * Math.Sin(aA)) - Ray.Hobj) < 0.1)
            //        {
            //            // TODO: tilt
            //            // TODO: bA, eA
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

            #endregion
        }

        public static void s1(int j, double BgnA, out double EndA)
        {
            EndA = BgnA;

            #region Перевірка відзеркалення від поверхні
            // якщо це вже поверхня:
            if (j <= 0)
            {
                j = 0;                                  // перевірка на дурня
                Ray_.Amp_000_090 *= Ray_.Ksrf;          // амплітуда сигналу зменшується 

                EndA = 360 - BgnA;                      // промінь віддзеркалюється від поверхні моря і змінює напрямок
                                                        // HACH: EndA < 90
                if (EndA < 90)
                    EndA = 360 - BgnA;
            }
            // якщо це вже дно, то:
            else if (j >= Ray_.Kz.Length - 1)
            {
                j = Ray_.Kz.Length - 1;                 // перевірка на дурня
                Ray_.Amp_000_090 *= Ray_.Kbtm;          // амплітуда сигналу зменшується

                EndA = 360 - BgnA;                      // промінь віддзеркалюється від поверхні дна і змінює напрямок
                // HACH: EndA > 90
                if (EndA > 90)
                    EndA = 360 - BgnA;
            }
            #endregion
        }
    }
}
