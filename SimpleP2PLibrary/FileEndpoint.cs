using System;

namespace SimpleP2PLibrary
{
    public class FileEndpoint
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }

        //The equals method is the be able to find similar instances of the object
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(FileEndpoint))
            {
                return false;
            }
            FileEndpoint other = (FileEndpoint)obj;
            if (this.IPAddress == other.IPAddress && this.Port == other.Port)
            {
                return true;
            }
            return false;
        }
    }
}
