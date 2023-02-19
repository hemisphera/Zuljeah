﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanford.Multimedia.Midi;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Zuljeah;

public class MidiReceiver : IAsyncDisposable
{

  private InputDevice? Device { get; }

  private PlayerPage Player { get; }

  private ILogger<MidiReceiver> Logger { get; }

  public string DeviceName { get; }


  public MidiReceiver(PlayerPage player, IOptions<ZuljeahConfiguration> config, ILogger<MidiReceiver> logger)
  {
    Logger = logger;
    Player = player;

    logger.LogTrace("MidiLogger initialized.");

    Device = CreateDevice(config.Value.MidiInputDeviceName);
    if (Device != null)
    {
      Device.ChannelMessageReceived += DeviceOnChannelMessageReceived;
      Device.MessageReceived += DeviceOnShortMessageReceived;
      Device.StartRecording();

      var caps = InputDevice.GetDeviceCapabilities(Device.DeviceID);
      DeviceName = caps.name;
      logger.LogTrace($"Listening for MIDI on device '{caps.name}'.");
    }
  }

  private void DeviceOnShortMessageReceived(IMidiMessage message)
  {
    var status = App.Services.GetRequiredService<StatusBroker>();
    status.PulseActivity(message);
  }

  private void DeviceOnChannelMessageReceived(object? sender, ChannelMessageEventArgs e)
  {
    var msg = e.Message;
    Logger.LogDebug($"Received event: {msg.Command} on channel {msg.MidiChannel}, Data1={msg.Data1}, Data2={msg.Data2}");
    Player.InvokeAction(new MidiTrigger(msg));
  }

  private static InputDevice? CreateDevice(string deviceName)
  {
    if (String.IsNullOrEmpty(deviceName)) return null;

    for (var i = 0; i < InputDevice.DeviceCount; i++)
      if (InputDevice.GetDeviceCapabilities(i).name.Equals(deviceName))
        return new InputDevice(i);

    return null;
  }

  public Task Initialize()
  {
    return Task.CompletedTask;
  }

  public async ValueTask DisposeAsync()
  {
    if (Device != null)
    {
      Device.StopRecording();
      Device.Close();
      Device.Dispose();
    }
    await Task.CompletedTask;
  }

}