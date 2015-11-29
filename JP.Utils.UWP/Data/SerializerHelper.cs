using JP.Utils.Debug;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using Windows.Storage;

namespace JP.Utils.Data
{
    public class SerializerHelper
    {
        /// <summary>
        /// 序列化为JSON格式并保存在独立储存里
        /// 注意ImageSource无法序列化
        /// 需要在属性添加Attribute[IgnoreDataMember]来避免序列化ImageSource
        /// </summary>
        /// <typeparam name="T">需要序列化的类的类型</typeparam>
        /// <param name="objectToBeSer">被序列化的对象</param>
        /// <param name="fileName">要保存到独立储存的文件名</param>
        public async static Task<bool> SerializerToJson<T>(object objectToBeSer, string fileName, bool isReplace=true)
        {
            try
            {
                T objecttojson = (T)objectToBeSer;
                string jsonString;
                using (var ms = new MemoryStream())
                {
                    new DataContractJsonSerializer(objecttojson.GetType()).WriteObject(ms, objecttojson);
                    jsonString = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
                }

                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(fileName, (isReplace ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.GenerateUniqueName));
                await FileIO.WriteTextAsync(file, jsonString);

                return true;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(SerializerHelper), "SerializerToJson<T>");
                return false;
            }

        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">要反序列化获得的对象</typeparam>
        /// <param name="filename">储存在独立储存的文件名</param>
        /// <returns>返回反序列化后的对象，如果文件不存在，返回一个Object类型的对象</returns>
        public async static Task<T> DeserializeFromJsonByFileName<T>(string filename)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(filename);
                string jsonString = await FileIO.ReadTextAsync(file);

                return DeSerializeFromJsonStr<T>(jsonString);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static T DeSerializeFromJsonStr<T>(string jsonStr)
        {
            try
            {
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
                    {
                        T objectToReturn = (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
                        return objectToReturn;
                    }
                }
                else return default(T);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        //以下为序列化XML
        // 注意目标类必须包含     [XmlRoot(ElementName = "entry")]
        // [XmlElement("id")]         [XmlAttribute("href")] 类似的属性标记
        // 比如：
        // [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
        //public class Feed : ViewModelBase
        //{
        //    private string title;
        //    [XmlElement("title")]
        //    public string Title{get;set;}

        //    private string id;
        //    [XmlElement("id")]
        //    public string ID{get;set;}

        //    private ObservableCollection<Entry> entries;
        //    [XmlElement("entry")]
        //    public ObservableCollection<Entry> Entries{get;set;}

        /// <summary>
        /// 序列化成XML
        /// </summary>
        /// <typeparam name="T">源对象的类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>返回xml</returns>
        public async static Task<string> SerializeToXml<T>(T obj)
        {
            if (obj != null)
            {
                var ser = new XmlSerializer(typeof(T));
                using (var writer = new StringWriter())
                {
                    ser.Serialize(writer, obj);
                    await writer.FlushAsync();
                    return writer.GetStringBuilder().ToString();
                }
            }
            else return null;
        }

        /// <summary>
        /// 从XML反序列化为对象
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="xml">xml字符串</param>
        /// <returns>返回该对象</returns>
        /// /// 注意目标类必须包含     [XmlRoot(ElementName = "entry")]
        /// [XmlElement("id")]         [XmlAttribute("href")] 类似的属性标记
        public static T DeserializeFromXml<T>(string xml)
        {
            T result = default(T);
            if (!String.IsNullOrEmpty(xml))
            {
                var ser = new XmlSerializer(typeof(T));
                using (var reader = new StringReader(xml))
                {
                    result = (T)ser.Deserialize(reader);
                }
            }
            return result;
        }


    }
}
