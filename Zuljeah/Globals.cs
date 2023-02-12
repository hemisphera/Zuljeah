using Eos.Lib.Io.Json;
using Newtonsoft.Json;

namespace Zuljeah
{
  internal static class Globals
  {

    public static readonly JsonSerializer Serializer = new JsonSerializer
    {
      Converters =
      {
        new JsonSerializableConverter()
      }
    };

  }
}
