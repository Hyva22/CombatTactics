using Newtonsoft.Json;

namespace Network.Game
{
    public abstract class PersistantObject
    {
        public long id;

        //public long ID { get => id; set => id = value; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static T FromJson<T>(string jsonString) where T : PersistantObject
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
