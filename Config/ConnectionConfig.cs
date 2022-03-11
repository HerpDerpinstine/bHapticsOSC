namespace bHapticsOSC.Config
{
    internal class ConnectionConfig : IniFile
    {
        internal string Address;
        internal int Port;
        internal int UpdateRate;

        internal ConnectionConfig(string filepath) : base(filepath) { }

        internal override void Load()
        {
            Address = GetString("Connection", nameof(Address), "127.0.0.1", true);
            Port = GetInt("Connection", nameof(Port), 9001, true);
            UpdateRate = GetInt("Threading", nameof(UpdateRate), 100, true);
        }
    }
}
