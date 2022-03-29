namespace bHapticsOSC.Config
{
    public class ConnectionConfig : IniFile
    {
        public int Port;
        public int UpdateRate;

        internal ConnectionConfig(string filepath) : base(filepath) { }

        internal override void Load()
        {
            Port = GetInt("Connection", nameof(Port), 9001, true);
            UpdateRate = GetInt("Threading", nameof(UpdateRate), 100, true);
        }
    }
}
