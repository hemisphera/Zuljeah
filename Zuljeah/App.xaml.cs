﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Eos.Mvvm;
using Eos.Mvvm.DataTemplates;
using Eos.Mvvm.EventArgs;
using Hsp.Reaper.ApiClient;
using Hsp.Reaper.ApiClient.JobScheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Zuljeah;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

  private static Microsoft.Extensions.Hosting.IHost Host { get; set; }

  public static IServiceProvider Services => Host.Services;

  public static MainVm MainVmInstance => Services.GetRequiredService<MainVm>();


  protected override void OnStartup(StartupEventArgs e)
  {

    var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
      .ConfigureServices((context, services) =>
      {
        services.AddLogging(b =>
        {
          b.AddFile(context.Configuration.GetSection("Logging"));
        });
        services.AddOptions<ZuljeahConfiguration>()
          .Bind(context.Configuration);

        services.AddSingleton(sp =>
        {
          var config = sp.GetRequiredService<IOptions<ZuljeahConfiguration>>();
          return new ReaperApiClient(config.Value.ReaperUri);
        });

        services.AddSingleton<Setlist>();
        services.AddSingleton<MainVm>();
        services.AddSingleton<ActionBindingsEditor>();
        services.AddSingleton<PlayerPage>();
        services.AddSingleton<ActionContainer>();
        services.AddSingleton<MidiReceiver>();
      });

    Host = builder.Build();

    UiSettings.ViewLocator = new ViewLocator(Assembly.GetExecutingAssembly());
    UiSettings.DialogService = new BasicDialogService();

    Task.Run(async () =>
    {
      await App.Services.GetRequiredService<MidiReceiver>().Initialize();
      await App.MainVmInstance.Initialize(e.Args);
    });
    base.OnStartup(e);
  }

  protected override void OnExit(ExitEventArgs e)
  {
    Task.Run(async () =>
    {
      var client = Services.GetRequiredService<ReaperApiClient>();
      await client.DisposeAsync();
      var receiver = Services.GetRequiredService<MidiReceiver>();
      await receiver.DisposeAsync();
    }).Wait();
  }

  private void TimespanNoMillisecondsConverter_OnOnConvert(object? sender, ConverterEventArgs e)
  {
    if (e.Value == null) return;
    var ts = (TimeSpan)e.Value;
    e.Result = TimeSpan.FromSeconds(Math.Round(ts.TotalSeconds, 0));
  }

}