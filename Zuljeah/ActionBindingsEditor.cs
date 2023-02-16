using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Eos.Lib.Io.Json;
using Eos.Mvvm;
using Eos.Mvvm.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Zuljeah;

public class ActionBindingsEditor : AsyncItemsViewModelBase<ActionBinding>, IPage
{

  private static string Filename =>
    Path.Combine(
      Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
      "bindings.json");

  public string Title => "Bindings";

  public Action[] AllActions
  {
    get
    {
      var container = App.Services.GetRequiredService<ActionContainer>();
      return container.All;
    }
  }


  public ActionBindingsEditor()
  {
  }


  protected override async Task<IEnumerable<ActionBinding>> GetItems()
  {
    return await ValueTask.FromResult(Items.ToArray());
  }



  public async Task Load()
  {
    var container = App.Services.GetRequiredService<ActionContainer>();
    ActionBinding[] newItems;
    if (File.Exists(Filename))
    {
      var content = await File.ReadAllTextAsync(Filename);
      newItems = JsonConvert.DeserializeObject<ActionBinding[]>(content, new JsonSerializableConverter()) ?? Array.Empty<ActionBinding>();
    }
    else
    {
      newItems = new ActionBinding[]
      {
        new(new KeyTrigger(Key.Enter), container.Play),
        new(new KeyTrigger(Key.Escape), container.Stop),
        new(new KeyTrigger(Key.Space), container.Pause)
      };
    }

    foreach (var item in newItems)
      Items.Add(item);
  }

  [UiCommand(Caption = "Save", Page = "Editor", Group = "Edit", Image = "Save")]
  public async Task Save()
  {
    await File.WriteAllTextAsync(Filename, JsonConvert.SerializeObject(Items, Formatting.Indented, new JsonSerializableConverter()));
  }


  public Task Activate()
  {
    return Task.CompletedTask;
  }

  [UiCommand(Caption = "Close", Page = "Editor", Group = "Edit", Image = "Close")]
  public async Task ClosePage()
  {
    await App.MainVmInstance.ClosePage(this);
  }

}