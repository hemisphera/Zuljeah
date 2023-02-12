using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eos.Lib.Io;
using Hsp.Reaper.ApiClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WpfApp1;

public class Setlist : IJsonSerializable
{

  public string? Filename { get; private set; }

  public string? Name { get; set; }

  public List<SetlistItem> Items { get; set; } = new();


  internal async Task UpdateFromReaper(ReaperApiClient client)
  {
    try
    {
      var regions = await client.ListRegions();
      var itemsToUpdate = Items.ToList();
      foreach (var region in regions)
      {
        var setlistItem = itemsToUpdate.FirstOrDefault(i => i.RegionId == region.RegionId);
        if (setlistItem == null) continue;
        setlistItem.LoadRegion(region);
        itemsToUpdate.Remove(setlistItem);
      }

      foreach (var setlistItem in itemsToUpdate)
        setlistItem.LoadRegion(null);
    }
    catch
    {
      // ignore
    }
  }


  public async Task Load(string filename)
  {
    var json = await File.ReadAllTextAsync(filename, Encoding.UTF8);
    FromJson(JObject.Parse(json), Globals.Serializer);
    Filename = filename;
  }

  public void FromJson(JObject jo, JsonSerializer serializer)
  {
    Name = jo.Value<string>(nameof(Name));
    Items.Clear();
    Items.AddRange(jo[nameof(Items)]?.ToObject<SetlistItem[]>() ?? Array.Empty<SetlistItem>());
  }

  public async Task Save(string? filename = null)
  {
    if (String.IsNullOrEmpty(filename))
      filename = Filename;
    if (String.IsNullOrEmpty(filename))
      throw new NotSupportedException("No filename has been specified.");

    var json = JToken.FromObject(this, Globals.Serializer);
    await File.WriteAllTextAsync(filename, json.ToString(Formatting.Indented), Encoding.UTF8);
    Filename = filename;
  }

  public JObject ToJson(JsonSerializer serializer)
  {
    return new JObject(
      new JProperty(nameof(Name), Name),
      new JProperty(nameof(Items), JToken.FromObject(Items))
    );
  }

}