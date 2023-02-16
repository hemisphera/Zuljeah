using System;
using System.Linq;
using Sanford.Multimedia.Midi;

namespace Zuljeah;

public class MidiTrigger : ITrigger
{

  public ChannelCommand Command { get; set; }

  public byte Channel { get; set; }

  public byte Data1 { get; set; }

  public byte Data2 { get; set; }


  public MidiTrigger()
  {
  }

  public MidiTrigger(ChannelMessage message)
    : this()
  {
    Command = message.Command;
    Channel = (byte)message.MidiChannel;
    Data1 = (byte)message.Data1;
    Data2 = (byte)message.Data2;
  }


  public string Serialize()
  {
    return String.Join(
      "",
      new[] { (byte)Command, Channel, Data1, Data2 }
        .Select(b => $"{b:X2}"));
  }

  public void Deserialize(string data)
  {
    var arr = String.IsNullOrEmpty(data)
      ? new byte[4]
      : Enumerable.Range(0, data.Length)
          .Where(x => x % 2 == 0)
          .Select(x => Convert.ToByte(data.Substring(x, 2), 16))
          .ToArray();
    Command = (ChannelCommand)arr[0];
    Channel = arr[1];
    Data1 = arr[2];
    Data2 = arr[3];
  }

  public bool Equals(ITrigger? other)
  {
    if (other is not MidiTrigger t2) return false;
    return
      t2.Command == Command &&
      t2.Channel == Channel &&
      t2.Data1 == Data1 &&
      t2.Data2 == Data2;
  }

}