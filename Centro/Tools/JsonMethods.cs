using System.IO;
using System.Text.Json;

namespace Centro.Model
{
    static class JsonMethods
    {
        public class Credentials
        {
            public string Key { get; set; }
            public string Secret { get; set; }
        }

        public static string SerializeCredentials(Credentials credentials, string fileName = null)
        {
            string json = JsonSerializer.Serialize<Credentials>(credentials);
            if (filename != null)
            {
                StreamWriter writer = new StreamWriter(fileName);
                writer.Write(json);
                writer.Close();

            }
            return json;
        }

        public static Credentials DeserializeCredentials(string fileName)
        {
            string json = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<Credentials>(json);
        }
    }
}
