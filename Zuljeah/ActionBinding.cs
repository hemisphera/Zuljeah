using System;
using System.Linq;
using Eos.Lib.Io;
using Eos.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zuljeah;

public class ActionBinding : ObservableEntity, IJsonSerializable
{

  public static Type[] TriggerTypeValues { get; } = {
    typeof(KeyTrigger),
    typeof(MidiTrigger)
  };


  public bool Enabled
  {
    get => GetAutoFieldValue<bool>();
    set => SetAutoFieldValue(value);
  }

  public Action? Action
  {
    get => GetAutoFieldValue<Action?>();
    set => SetAutoFieldValue(value);
  }

  public ITrigger? Trigger
  {
    get => GetAutoFieldValue<ITrigger?>();
    set => SetAutoFieldValue(value);
  }

  public Type? Type
  {
    get => Trigger?.GetType();
    set
    {
      Trigger = value == null ? null : (ITrigger)Activator.CreateInstance(value);
      RaisePropertyChanged();
    }
  }


  public ActionBinding()
  {
    Enabled = true;
  }

  public ActionBinding(ITrigger trigger, Action action)
    : this()
  {
    Trigger = trigger;
    Action = action;
  }


  public JObject ToJson(JsonSerializer serializer)
  {
    return new JObject(
      new JProperty(nameof(Enabled), Enabled),
      new JProperty(nameof(Action), Action.Id),
      new JProperty(nameof(Type), Type?.Name),
      new JProperty(nameof(Trigger), Trigger?.Serialize())
    );
  }

  public void FromJson(JObject obj, JsonSerializer serializer)
  {
    var actions = App.Services.GetRequiredService<ActionContainer>();
    var actionId = Guid.Parse(obj.Value<string>(nameof(Action)));
    Action = actions.All.First(a => a.Id == actionId);
    Type = ActionBinding.TriggerTypeValues.FirstOrDefault(t => t.Name == obj.Value<string>(nameof(Type)));
    Trigger?.Deserialize(obj.Value<string>(nameof(Trigger)));
    Enabled = obj.Value<bool>(nameof(Enabled));
  }

}