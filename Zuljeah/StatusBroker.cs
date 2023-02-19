using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Eos.Mvvm;
using Sanford.Multimedia.Midi;

namespace Zuljeah;

public class StatusBroker : ObservableEntity
{

  public string? Version { get; }

  public string? MidiDeviceName { get; }

  public string MidiActivity
  {
    get => GetAutoFieldValue<string>(String.Empty, nameof(MidiActivity));
    private set
    {
      SetAutoFieldValue(value);
      if (!String.IsNullOrEmpty(MidiActivity)) LastMidiActivity = DateTime.Now;
    }
  }

  private DateTime LastMidiActivity { get; set; }


  public StatusBroker(MidiReceiver receiver)
  {
    var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
    Version = fvi.FileVersion;

    MidiDeviceName = String.IsNullOrEmpty(receiver.DeviceName) ? "<No MIDI Device>" : receiver.DeviceName;
  }


  public async Task PulseActivity(IMidiMessage message)
  {
    if (String.IsNullOrEmpty(MidiActivity))
      MidiActivity = "MIDI";
    await Task.Delay(TimeSpan.FromSeconds(0.5));
    if (DateTime.Now.Subtract(LastMidiActivity) >= TimeSpan.FromSeconds(0.5))
      MidiActivity = "";
  }

}