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

  public string Title => "Trigger Bindings";

  public string Icon => "Action";

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



  [UiCommand(Caption = "Delete", Page = "Editor", Group = "Edit", Image = "Delete")]
  public async Task Delete()
  {
    if (SelectedItem == null) return;
    await DispatchAsync(() => Items.Remove(SelectedItem));
    await Task.CompletedTask;
  }

  public async Task Load()
  {
    if (File.Exists(Filename))
    {
      var content = await File.ReadAllTextAsync(Filename);
      var newItems = JsonConvert.DeserializeObject<ActionBinding[]>(content, new JsonSerializableConverter()) ??
                     Array.Empty<ActionBinding>();
      foreach (var item in newItems)
        Items.Add(item);
    }
    else
      await InitDefaults();
  }

  [UiCommand(Caption = "Init Defaults", Page = "Editor", Group = "Edit", Image = "ReOpen")]
  public async Task InitDefaults()
  {
    var container = App.Services.GetRequiredService<ActionContainer>();
    var newItems = new ActionBinding[]
    {
      new(new KeyTrigger(Key.Enter), container.Play),
      new(new KeyTrigger(Key.Escape), container.Stop),
      new(new KeyTrigger(Key.Space), container.Pause),
      new(new KeyTrigger(Key.Down), container.MoveNextAction),
      new(new KeyTrigger(Key.Up), container.MovePreviousAction)
    };
    await DispatchAsync(() =>
    {
      Items.Clear();
      foreach (var item in newItems)
        Items.Add(item);
    });
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

  public async Task Deactivate()
  {
    await Save();
  }

}