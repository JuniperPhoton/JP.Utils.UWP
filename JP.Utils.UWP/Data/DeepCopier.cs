using System.IO;
using System.Runtime.Serialization;

namespace JP.Utils.Data
{
    public static class ObjectExtensions
    {
        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = ser.ReadObject(ms);
            }
            return (T)retval;
        }
    }
}
