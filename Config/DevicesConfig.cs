namespace bHapticsOSC.Config
{
    internal class DevicesConfig : IniFile
    {
        internal Device Head;

        internal Device VestFront;
        internal Device VestBack;

        internal Device ArmLeft;
        internal Device ArmRight;

        internal Device HandLeft;
        internal Device HandRight;

        internal Device FootLeft;
        internal Device FootRight;

        internal DevicesConfig(string filepath) : base(filepath) { }

        internal override void Load()
        {
            if (Head == null)
                Head = new Device() { Category = "Head", Parent = this };
            Head.Load();

            if (VestFront == null)
                VestFront = VestFront = new Device() { Category = "Vest Front", Parent = this };
            VestFront.Load();

            if (VestBack == null)
                VestBack = new Device() { Category = "Vest Back", Parent = this };
            VestBack.Load();

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

        internal class Device
        {
            internal IniFile Parent;
            internal string Category;

            internal bool Enabled;
            internal int Intensity;

            internal void Load()
            {
                Enabled = Parent.GetBool(Category, nameof(Enabled), true, true);
                Intensity = Parent.GetInt(Category, nameof(Intensity), 100, true);
            }
        }
    }
}
