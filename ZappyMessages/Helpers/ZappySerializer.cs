using Newtonsoft.Json;

namespace ZappyMessages.Helpers
{
    public static class ZappySerializer
    {
        static JsonSerializerSettings _settings;

        static ZappySerializer()
        {
            _settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            };
        }


        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }
        public static T DeserializeObject<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj, _settings);
        }

    }
}
