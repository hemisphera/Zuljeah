using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sanford.Multimedia.Midi;

namespace Zuljeah;

public class MidiReceiver
{

  private InputDevice? Device { get; }

  private PlayerPage Player { get; }


  public MidiReceiver(PlayerPage player, IOptions<ZuljeahConfiguration> config)
  {
    Device = CreateDevice(config.Value.MidiInputDeviceName);
    Player = player;
    if (Device != null)
      Device.MessageReceived += DeviceOnMessageReceived;
  }

  private void DeviceOnMessageReceived(IMidiMessage message)
  {
    if (message is ChannelMessage msg)
      Player.InvokeAction(new MidiTrigger(msg));
  }

  private InputDevice? CreateDevice(string deviceName)
  {
    if (String.IsNullOrEmpty(deviceName)) return null;

    for (var i = 0; i < InputDevice.DeviceCount; i++)
      if (InputDevice.GetDeviceCapabilities(i).name.Equals(deviceName))
        return new InputDevice(i);

    return null;
  }

}