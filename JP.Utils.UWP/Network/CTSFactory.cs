using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
