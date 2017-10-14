using System.Threading;

namespace JP.Utils.Network
{
    public static class CTSFactory
    {
        public static CancellationTokenSource MakeCTS(int timeout)
        {
            return new CancellationTokenSource(timeout);
        }

        public static CancellationTokenSource MakeCTS()
        {
            return new CancellationTokenSource();
        }
    }
}