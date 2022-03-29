namespace bHapticsOSC.Config
{
    public class DevicesConfig : IniFile
    {
        public Device Head;

        public Device Vest;

        public Device ArmLeft;
        public Device ArmRight;

        public Device HandLeft;
        public Device HandRight;

        public Device FootLeft;
        public Device FootRight;

        internal DevicesConfig(string filepath) : base(filepath) { }

        internal override void Load()
        {
            if (Head == null)
                Head = new Device() { Category = "Head", Parent = this };
            Head.Load();

            if (Vest == null)
                Vest = new Device() { Category = "Vest", Parent = this };
            Vest.Load();

            if (ArmLeft == null)
                ArmLeft = new Device() { Category = "Arm Left", Parent = this };
            ArmLeft.Load();

            if (ArmRight == null)
                ArmRight = new Device() { Category = "Arm Right", Parent = this };
            ArmRight.Load();

            if (HandLeft == null)
                HandLeft = new Device() { Category = "Hand Left", Parent = this };
            HandLeft.Load();

            if (HandRight == null)
                HandRight = new Device() { Category = "Hand Right", Parent = this };
            HandRight.Load();

            if (FootLeft == null)
                FootLeft = new Device() { Category = "Foot Left", Parent = this };
            FootLeft.Load();

            if (FootRight == null)
                FootRight = new Device() { Category = "Foot Right", Parent = this };
            FootRight.Load();
        }

        public class Device
        {
            internal IniFile Parent;
            internal string Category;

            public bool Enabled;
            public int Intensity;

            internal void Load()
            {
                Enabled = Parent.GetBool(Category, nameof(Enabled), true, true);
                Intensity = Parent.GetInt(Category, nameof(Intensity), 100, true);
            }
        }
    }
}
