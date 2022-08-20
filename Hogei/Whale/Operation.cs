using System.Text.Json;
using System.Text.Json.Serialization;
namespace Hogei;

public struct Operation
{
    public List<KeySpecifier> Keys { get; }
    public TimeSpan Wait { get; }

    public Operation(ICollection<KeySpecifier> keys, TimeSpan wait)
    {
        Keys = keys.ToList();
        Wait = wait;
    }

    public static Dictionary<string, List<Operation>> GetDictionaryFromJson(string path)
    {
        var tmp = JsonSerializer.Deserialize<Dictionary<string, List<_Operation>>>(File.ReadAllText(path));
        if (tmp == null)
        {
            throw new FileNotFoundException();
        }

        var ret = new Dictionary<string, List<Operation>>();
        foreach (var pair in tmp)
        {
            ret.Add(pair.Key, pair.Value.Select(operation => operation.Transfer()).ToList());
        }
        return ret;
    }

    struct _Operation
    {
        [JsonInclude]
        [JsonPropertyName("keys")]
        public List<string> Keys { get; private set; }
        [JsonInclude]
        [JsonPropertyName("wait")]
        public TimeSpan Wait { get; private set; }

        [JsonConstructor]
        public _Operation(List<string> keys, TimeSpan wait)
        {
            Keys = keys;
            Wait = wait;
        }

        public Operation Transfer()
        {
            return new Operation(Keys.Select(key => (KeySpecifier)key[0]).ToList(), Wait);
        }
    }
}
