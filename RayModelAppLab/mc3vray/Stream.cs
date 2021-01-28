using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mc3vray
{
    public class S
    {

        // пройдений шлях рахує материнська програма
        // відстань контролює материнська програма

        // амплітуду контролює материнська програма
        // TODO: розібратись з четвертямт кутів

        public static void Layer_000_090(   double Angl,        // початковий кут
                                            double H,           // спочатку початкова глибина джерела звуку, потім вузлові точки по глибині
                                            double C,           // спочатку початкова швидкість звуку джерела, потім вузлові точки по глибині
                                            int i,              // номер поточного точки водного прошарку

                        out double BgnAngl, // TODO: дадти на виході початковий кут, перераховувати по четвертям
                        out double EndAngl, // кінцевий кут
                        out double X,       // центр кола
                        out double R,       // радіус кола
                        
                        out int j)          // номер поточного точки водного прошарку
        {

            double dX   =   0;
            BgnAngl = Angl;
            EndAngl = Angl;

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
                if (Ray.Kz[j] == 0)                                                     // в цьому випадку немає ані радіуса, ані центра, а початковий кут рівний кінцевом
                    AmpJ_000_090(BgnAngl, out EndAngl, j--, out j, i, H, C, R);         // водний прошарок зміщується на крок вверх
                // промінь рухається прямо вверх з заломленнями
                else
                {
                    R = Ray.Cz[j + 1] / (Math.Abs(Ray.Kz[j]) * Math.Sin(BgnAngl));      // радіус кола завжди залежить від початкового кута в заданому водному прошарку
                    X = Ray.Cz[j + 1] / (Ray.Kz[j] * Math.Tan(BgnAngl));                // центр кола також залежить від початкового кута в заданому водному прошарку

                    // промінь вигинається вниз
                    if (Ray.Kz[j] < 0)
                    {   // промінь вигнувся вниз проте перейщов до верхнього водного шару
                        double dummy = Ray.Yr[j] + Ray.Hz[j + 0];
                        // HACK: якщо == R, то промінь рухається в прошарку
                        if (Math.Abs(dummy) < R)
                        {
                            AmpJ_000_090(Math.Asin(dummy) / R, out EndAngl, j--, out j, i, H, C, R);        // водний прошарок зміщується на крок вверх

                            // TODO: pi/2 + alpha (beta)
                            dX = R * Math.Abs(Math.Cos(BgnAngl) - Math.Cos(EndAngl));
                        }
                        // промінь вигнувся вниз настільки що лишився в поточному прошарку і пішов до нижнього водного шару 
                        // HACK: закрив очі на ситуацію коли кінцевий кут буде дорівнювати 90 
                        else
                        {
                            AmpJ_000_090(2 * Math.PI - BgnAngl, out EndAngl, j, out j, i, H, C, R);         // водний прошарок залишається незмінним

                            dX = R * Math.Abs(Math.Cos(Math.PI-BgnAngl) - Math.Cos(BgnAngl));
                        }
                    }

                    // промінь вигинається вгору
                    else
                    {   // промінь вигнувся вгору проте перейшов до верхнього водного шару
                        // HACK: якщо == R, то промінь рухається в прошарку
                        if (Math.Abs(Ray.Yr[j] + Ray.Hz[j + 0]) < R)
                        {
                            // TODO: EndAngl = 

                            AmpJ_000_090(EndAngl, out EndAngl, j--, out j, i, H, C, R);    // водний прошарок зміщується на крок вверх

                            // TODO: 3*pi/2 + alpha (beta)
                            dX = R * Math.Abs(Math.Cos(BgnAngl) - Math.Cos(EndAngl));
                        }
                        // промінь вигнувся вгору настільки що лишився в поточному прошарку і пішов назад
                        // HACK: закрив очі на ситуацію коли вигинаючись промінь може потрапити до ГАС
                        else
                            EndAngl = -1;
                    }
                }
            }
            #endregion

            #region промінь напрямлений вниз
            else if (BgnAngl > 90)
            {
                // промінь рухається прямо вниз без заломлювань
                if (Ray.Kz[j] == 0)                                                     // в цьому випадку немає ані радіуса, ані центра, а початковий кут рівний кінцевому
                    AmpJ_000_090(BgnAngl, out EndAngl, j++, out j, i, H, C, R);            // водний прошарок зміщується на крок вниз
                // промінь рухається прямо вниз з заломленнями
                else
                {
                    R = Ray.Cz[j + 0] / (Math.Abs(Ray.Kz[j]) * Math.Sin(BgnAngl));      // радіус кола завжди залежить від початкового кута в заданому водному прошарку
                    X = Ray.Cz[j + 0] / (Ray.Kz[j] * Math.Tan(BgnAngl));                // центр кола також залежить від початкового кута в заданому водному прошарку

                    // промінь вигинається вниз
                    if (Ray.Kz[j] < 0)
                    {   // промінь вигнувся вниз проте перейщов до попереднього водного шару
                        if (Math.Abs(Ray.Yr[j] + Ray.Hz[j + 1]) < R)
                        {
                            // TODO: EndAngl = 

                            AmpJ_000_090(BgnAngl, out EndAngl, j++, out j, i, H, C, R);    // водний прошарок зміщується на крок вниз

                            // TODO: L = 
                        }
                        // промінь вигнувся вниз настільки що лишився в поточному прошарку і пішов назад
                        // HACK: закрив очі на ситуацію коли вигинаючись промінь може потрапити до ГАС
                        else
                            EndAngl = -1;
                    }

                    // промінь вигинається вгору
                    else
                    {   // промінь вигнувся вгору проте перейщов до попереднього водного шару
                        double dummy = Math.Abs(Ray.Yr[j] + Ray.Hz[j + 1]);
                        // HACK: якщо == R, то промінь рухається в прошарку
                        if (dummy < R)
                        {
                            // TODO: EndAngl = 

                            AmpJ_000_090(BgnAngl, out EndAngl, j++, out j, i, H, C, R);    // водний прошарок зміщується на крок вниз

                            // TODO: L = 
                        }
                        // промінь вигнувся вгору настільки що лишився в поточному прошарку і пішов до верхнього водного шару
                        // HACK: закрив очі на ситуацію коли кінцевий кут буде дорівнювати 90 
                        else
                        {
                            EndAngl = Math.PI - BgnAngl;                                // промінь по дузі пішов вверх

                            AmpJ_000_090(EndAngl, out EndAngl, j, out j, i, H, C, R);      // водний прошарок залишається незмінним

                            // TODO: L = 
                        }
                    }
                }
            }
            #endregion
        }

        public static void AmpJ_000_090(    double BgnAngl, out double EndAngl,
                                            int j0,          out int j1,
                                            int i, double H, double C, double R)
        {
            EndAngl = BgnAngl;

            j1 = j0;
            if (j1 <= 0)                        // якщо це вже поверхня, то:
            {
                j1 = 0;                         // перевірка на дурня

                EndAngl = Math.PI - BgnAngl;    // промінь віддзеркалюється від поверхні і змінює напрямок
                Ray.Amp_000_090 *= Ray.Ksrf;    // а також зменшує амплітуду сигналу
            }
            else if (j1 >= Ray.Kz.Length - 1)   // якщо це вже дно, то:
            {
                j1 = Ray.Kz.Length - 1;         // перевірка на дурня

                EndAngl = Math.PI - BgnAngl;    // промінь віддзеркалюється від поверхні і змінює напрямок,
                Ray.Amp_000_090 *= Ray.Kbtm;    // а також зменшує амплітуду сигналу
            }

            if (Ray.Kz[i] == 0)
                Ray.L_000_090 = Math.Abs(H - Ray.Hz[i]) / Math.Sin(BgnAngl);
            else
                Ray.L_000_090 += Math.Abs(R * (EndAngl - BgnAngl));
                Ray.T_000_090 = 2 * Ray.L_000_090 / (C + Ray.Cz[j1]);
        }


        public static void Layer_090_180()
        {

        }
    }
}
