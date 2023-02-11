using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eos.Lib.Io.Json;
using Newtonsoft.Json;

namespace WpfApp1
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
