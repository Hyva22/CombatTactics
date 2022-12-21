using Newtonsoft.Json;

namespace Network.Game
{
    /// <summary>
    /// Attributes that should be serialized need to be public.
    /// </summary>
    public abstract class PersistantObject
    {
        public long id;

        /// <summary>
        /// Converts this object to a Json string, using Newtonsoft.Json
        /// </summary>
        /// <returns>Json string</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Creates an object from a Json string, using Newtonsoft.Json
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="jsonString">String to convert</param>
        /// <returns></returns>
        public static T FromJson<T>(string jsonString) where T : PersistantObject
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
