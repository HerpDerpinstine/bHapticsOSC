namespace bHapticsOSC.Config
{
    internal class ConnectionConfig : IniFile
    {
        internal int Port;
        internal int UpdateRate;

        internal ConnectionConfig(string filepath) : base(filepath) { }

        internal override void Load()
        {
            Port = GetInt("Connection", nameof(Port), 9001, true);
            UpdateRate = GetInt("Threading", nameof(UpdateRate), 100, true);
        }
    }
}
