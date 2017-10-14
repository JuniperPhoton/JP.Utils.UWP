namespace JP.Utils.UWP.Data
{
    public static class Maths
    {
        public static double Clamp(this double x, double min, double max)
        {
            if (x <= min) return min;
            if (x >= max) return max;
            return x;
        }

        public static int Clamp(this int x, int min, int max)
        {
            if (x <= min) return min;
            if (x >= max) return max;
            return x;
        }

        public static float Clamp(this float x, float min, float max)
        {
            if (x <= min) return min;
            if (x >= max) return max;
            return x;
        }

        public static float ToF2(this float x)
        {
            return float.Parse(x.ToString("f2"));
        }

        public static float ToF2(this double x)
        {
            return float.Parse(x.ToString("f2"));
        }

        public static float ToF1(this float x)
        {
            return float.Parse(x.ToString("f1"));
        }

        public static float ToF1(this double x)
        {
            return float.Parse(x.ToString("f1"));
        }
    }
}