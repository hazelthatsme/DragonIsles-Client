using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class NoiseGeneratorPerlin
{

    private int[] d;
    public double a;
    public double b;
    public double c;
    private static double[] e = new double[] { 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, 0.0D, -1.0D, 0.0D };
    private static double[] f = new double[] { 1.0D, 1.0D, -1.0D, -1.0D, 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D };
    private static double[] g = new double[] { 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, 1.0D, -1.0D, -1.0D, 1.0D, 1.0D, -1.0D, -1.0D, 0.0D, 1.0D, 0.0D, -1.0D };
    private static double[] h = new double[] { 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, 0.0D, -1.0D, 0.0D };
    private static double[] i2 = new double[] { 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, 1.0D, -1.0D, -1.0D, 1.0D, 1.0D, -1.0D, -1.0D, 0.0D, 1.0D, 0.0D, -1.0D };

    public NoiseGeneratorPerlin()
    {
        Random random = new Random();

        d = new int[512];
        a = random.NextDouble() * 256.0D;
        b = random.NextDouble() * 256.0D;
        c = random.NextDouble() * 256.0D;

        int i;

        for (i = 0; i < 256; d[i] = i++) { }

        for (i = 0; i < 256; ++i)
        {
            int j = random.Next(256 - i) + i;
            int k = d[i];

            d[i] = d[j];
            d[j] = k;
            d[i + 256] = d[i];
        }
    }

    public NoiseGeneratorPerlin(Random random)
    {
        d = new int[512];
        a = random.NextDouble() * 256.0D;
        b = random.NextDouble() * 256.0D;
        c = random.NextDouble() * 256.0D;

        int i;

        for (i = 0; i < 256; d[i] = i++) { }

        for (i = 0; i < 256; ++i)
        {
            int j = random.Next(256 - i) + i;
            int k = d[i];

            d[i] = d[j];
            d[j] = k;
            d[i + 256] = d[i];
        }
    }

    public double funcB(double d0, double d1, double d2)
    {
        return d1 + d0 * (d2 - d1);
    }

    public double funcA(int i, double d0, double d1)
    {
        int j = i & 15;

        return h[j] * d0 + i2[j] * d1;
    }

    public double funcA(int i, double d0, double d1, double d2)
    {
        int j = i & 15;

        return e[j] * d0 + f[j] * d1 + g[j] * d2;
    }

    public void funcA(double[] adouble, double d0, double d1, double d2, int i, int j, int k, double d3, double d4, double d5, double d6)
    {
        int l;
        int i1;
        double d7;
        double d8;
        double d9;
        int j1;
        double d10;
        int k1;
        int l1;
        int i2;
        int j2;

        if (j == 1)
        {
            double d11 = 0.0D;
            double d12 = 0.0D;

            j2 = 0;
            double d13 = 1.0D / d6;

            for (int k2 = 0; k2 < i; ++k2)
            {
                d7 = d0 + k2 * d3 + a;
                int l2 = (int)d7;

                if (d7 < l2)
                {
                    --l2;
                }

                int i3 = l2 & 255;

                d7 -= l2;
                d8 = d7 * d7 * d7 * (d7 * (d7 * 6.0D - 15.0D) + 10.0D);

                for (j1 = 0; j1 < k; ++j1)
                {
                    d9 = d2 + j1 * d5 + c;
                    k1 = (int)d9;
                    if (d9 < k1)
                    {
                        --k1;
                    }

                    l1 = k1 & 255;
                    d9 -= k1;
                    d10 = d9 * d9 * d9 * (d9 * (d9 * 6.0D - 15.0D) + 10.0D);
                    l = d[i3] + 0;
                    int j3 = d[l] + l1;
                    int k3 = d[i3 + 1] + 0;

                    i1 = d[k3] + l1;
                    d11 = funcB(d8, funcA(d[j3], d7, d9), funcA(d[i1], d7 - 1.0D, 0.0D, d9));
                    d12 = funcB(d8, funcA(d[j3 + 1], d7, 0.0D, d9 - 1.0D), funcA(d[i1 + 1], d7 - 1.0D, 0.0D, d9 - 1.0D));
                    double d14 = funcB(d10, d11, d12);

                    i2 = j2++;
                    adouble[i2] += d14 * d13;
                }
            }
        }
        else
        {
            l = 0;
            double d15 = 1.0D / d6;

            i1 = -1;
            double d16 = 0.0D;

            d7 = 0.0D;
            double d17 = 0.0D;

            d8 = 0.0D;

            for (j1 = 0; j1 < i; ++j1)
            {
                d9 = d0 + j1 * d3 + a;
                k1 = (int)d9;
                if (d9 < k1)
                {
                    --k1;
                }

                l1 = k1 & 255;
                d9 -= k1;
                d10 = d9 * d9 * d9 * (d9 * (d9 * 6.0D - 15.0D) + 10.0D);

                for (int l3 = 0; l3 < k; ++l3)
                {
                    double d18 = d2 + l3 * d5 + c;
                    int i4 = (int)d18;

                    if (d18 < i4)
                    {
                        --i4;
                    }

                    int j4 = i4 & 255;

                    d18 -= (double)i4;
                    double d19 = d18 * d18 * d18 * (d18 * (d18 * 6.0D - 15.0D) + 10.0D);

                    for (int k4 = 0; k4 < j; ++k4)
                    {
                        double d20 = d1 + k4 * d4 + b;
                        int l4 = (int)d20;

                        if (d20 < l4)
                        {
                            --l4;
                        }

                        int i5 = l4 & 255;

                        d20 -= l4;
                        double d21 = d20 * d20 * d20 * (d20 * (d20 * 6.0D - 15.0D) + 10.0D);

                        if (k4 == 0 || i5 != i1)
                        {
                            i1 = i5;
                            int j5 = d[l1] + i5;
                            int k5 = d[j5] + j4;
                            int l5 = d[j5 + 1] + j4;
                            int i6 = d[l1 + 1] + i5;

                            j2 = this.d[i6] + j4;
                            int j6 = this.d[i6 + 1] + j4;

                            d16 = funcB(d10, funcA(d[k5], d9, d20, d18), funcA(d[j2], d9 - 1.0D, d20, d18));
                            d7 = funcB(d10, funcA(d[l5], d9, d20 - 1.0D, d18), funcA(d[j6], d9 - 1.0D, d20 - 1.0D, d18));
                            d17 = funcB(d10, funcA(d[k5 + 1], d9, d20, d18 - 1.0D), funcA(d[j2 + 1], d9 - 1.0D, d20, d18 - 1.0D));
                            d8 = funcB(d10, funcA(d[l5 + 1], d9, d20 - 1.0D, d18 - 1.0D), funcA(d[j6 + 1], d9 - 1.0D, d20 - 1.0D, d18 - 1.0D));
                        }

                        double d22 = funcB(d21, d16, d7);
                        double d23 = funcB(d21, d17, d8);
                        double d24 = funcB(d19, d22, d23);

                        i2 = l++;
                        adouble[i2] += d24 * d15;
                    }
                }
            }
        }
    }

}