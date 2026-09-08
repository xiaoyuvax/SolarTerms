namespace SolarTerms24Net
{
    public class JDate
    {
        public int Y, M, D, h, m;
        public double s;
        public readonly double J2000 = 2451545.0;

        // ΔT 表，与 Python 完全一致
        private readonly double[] dts = [
            -4000,108371.7,-13036.80,392.000,0.0000,
            -500,17201.0,-627.82,16.170,-0.3413,
            -150,12200.6,-346.41,5.403,-0.1593,
            150,9113.8,-328.13,-1.647,0.0377,
            500,5707.5,-391.41,0.915,0.3145,
            900,2203.4,-283.45,13.034,-0.1778,
            1300,490.1,-57.35,2.085,-0.0072,
            1600,120.0,-9.81,-1.532,0.1403,
            1700,10.2,-0.91,0.510,-0.0370,
            1800,13.4,-0.72,0.202,-0.0193,
            1830,7.8,-1.81,0.416,-0.0247,
            1860,8.3,-0.13,-0.406,0.0292,
            1880,-5.4,0.32,-0.183,0.0173,
            1900,-2.3,2.06,0.169,-0.0135,
            1920,21.2,1.69,-0.304,0.0167,
            1940,24.2,1.22,-0.064,0.0031,
            1960,33.2,0.51,0.231,-0.0109,
            1980,51.0,1.29,-0.026,0.0032,
            2000,64.7,-1.66,5.224,-0.2905,
            2150,279.4,732.95,429.579,0.0158,6000
        ];

        public JDate()
        {
            Y = 2000; M = 1; D = 1; h = 12; m = 0; s = 0;
        }

        private static int Int2(double v)
        {
            int x = (int)Math.Floor(v);
            return x < 0 ? x + 1 : x;
        }

        public double DeltaTT(double y)
        {
            int i = 0;
            while (i < 100)
            {
                if (y < dts[i + 5] || i == 95) break;
                i += 5;
            }
            double t1 = (y - dts[i]) / (dts[i + 5] - dts[i]) * 10;
            double t2 = t1 * t1; double t3 = t2 * t1;
            return dts[i + 1] + dts[i + 2] * t1 + dts[i + 3] * t2 + dts[i + 4] * t3;
        }

        public double DeltaTT2(double jd)
        {
            return DeltaTT(jd / 365.2425 + 2000) / 86400.0;
        }

        public double ToJD(bool utc)
        {
            int y = Y, m = M;
            if (m <= 2) { m += 12; y -= 1; }
            int n = 0;
            if (Y * 372 + M * 31 + D >= 588829)
            {
                int cent = Int2(y / 100.0);
                n = 2 - cent + Int2(cent / 4.0);
            }
            n += Int2(365.2500001 * (y + 4716));
            n += Int2(30.6 * (m + 1)) + D;
            double jd = n + ((s / 60 + m) / 60 + h) / 24.0 - 1524.5;
            if (utc) jd += DeltaTT2(jd - J2000);
            return jd;
        }

        public void SetFromJD(double jd, bool utc)
        {
            if (utc) jd -= DeltaTT2(jd - J2000);
            jd += 0.5;
            int A = Int2(jd);
            double F = jd - A;
            if (A > 2299161)
            {
                int D = Int2((A - 1867216.25) / 36524.25);
                A += 1 + D - Int2(D / 4.0);
            }
            A += 1524;
            Y = Int2((A - 122.1) / 365.25);
            int D2 = A - Int2(365.25 * Y);
            M = Int2(D2 / 30.6001);
            D = D2 - Int2(M * 30.6001);
            Y -= 4716; M -= 1;
            if (M > 12) M -= 12;
            if (M <= 2) Y += 1;

            F *= 24; h = Int2(F); F -= h;
            F *= 60; m = Int2(F); F -= m;
            s = F * 60;
        }

        public void SetFromStr(string s)
        {
            Y = int.Parse(s.Substring(0, 4));
            M = int.Parse(s.Substring(4, 2));
            D = int.Parse(s.Substring(6, 2));
            h = int.Parse(s.Substring(9, 2));
            m = int.Parse(s.Substring(11, 2));
            if (int.TryParse(s.AsSpan(13, 2), out int sec)) this.s = sec;
        }

        public string ToStr()
        {
            int hh = h, mm = m;
            double ss = Math.Floor(s + 0.5);
            if (ss >= 60) { ss -= 60; mm++; }
            if (mm >= 60) { mm -= 60; hh++; }
            return string.Concat($"{Y,5:0}".AsSpan(Math.Max(0, $"{Y,5:0}".Length - 5)), $"-{M:00}-{D:00} {hh:00}:{mm:00}:{ss:00}");
        }

        public DateTime ToDateTime()
        {
            int hh = h, mm = m;
            double ss = Math.Floor(s + 0.5);
            if (ss >= 60) { ss -= 60; mm++; }
            if (mm >= 60) { mm -= 60; hh++; }
            return new DateTime(Y, M, D, hh, mm, (int)ss, (int)((ss - (int)ss) * 100));
        }

        public int Dint_dec(double jd, int shiqu, int int_dec)
        {
            double u = jd + 0.5 - DeltaTT2(jd) + shiqu / 24.0;
            return int_dec == 1 ? (int)Math.Floor(u) : (int)(u - Math.Floor(u));
        }
    }
}