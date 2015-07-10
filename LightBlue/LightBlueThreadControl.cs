using System;
using System.Threading;

namespace LightBlue
{
    public static class LightBlueThreadControl
    {
        [ThreadStatic]
        private static CancellationTokenSource Cts;

        public static CancellationToken CancellationToken
        {
            get
            {
                if (Cts == null) Cts = new CancellationTokenSource();
                return Cts.Token;
            }
        }

        internal static CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (Cts == null) Cts = new CancellationTokenSource();
                return Cts;
            }
        }
    }
}
