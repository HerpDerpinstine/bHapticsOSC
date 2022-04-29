using System.Threading;

namespace OscLib.Utils
{
    public abstract class ThreadedTask
    {
        private Thread thread;

        public bool IsAlive()
            => (thread == null) ? false : thread.IsAlive;

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
            if (IsAlive())
                KillThread();

            thread = new Thread(WithinThread);
            thread.Start();
        }
        private void KillThread()
        {
            if (!IsAlive())
                return;

            thread.Abort();
            thread = null;
        }
    }
}
