using System.Threading;

namespace bHapticsOSC.Utils
{
    public abstract class ThreadedTask
    {
        internal static int UpdateRate = 100; // ms
        private Thread thread;

        public abstract bool BeginInitInternal();
        public void BeginInit()
        {
            if (BeginInitInternal())
                RunThread();
        }

        public abstract bool EndInitInternal();
        public void EndInit()
        {
            if (EndInitInternal())
                KillThread();
        }

        public abstract void WithinThread();
        private void RunThread()
        {
            if (thread == null)
                thread = new Thread(WithinThread);

            if (thread.IsAlive)
                return;
            thread.Start();
        }
        private void KillThread()
        {
            if (thread == null)
                return;
            thread.Abort();
        }
    }
}
