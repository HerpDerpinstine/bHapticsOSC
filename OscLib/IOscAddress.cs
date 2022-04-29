namespace OscLib
{
    public interface IOscAddress
    {
        public abstract string GetAddressPrefix();
        public abstract string[] GetAddressBook();
    }
}
