using System;
using System.Reflection;
using System.Windows;
using Eos.Mvvm;
using Eos.Mvvm.DataTemplates;
using Eos.Mvvm.EventArgs;
using Microsoft.Extensions.DependencyInjection;

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
    base.OnStartup(e);

    var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
      .ConfigureServices((context, services) =>
      {
        services.AddOptions<ZuljeahConfiguration>()
          .Bind(context.Configuration);
        services.AddSingleton<MainVm>();
      });

    Host = builder.Build();

    /*
    var builder = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)


    Configuration = builder.Build();

    var serviceCollection = new ServiceCollection();
    ConfigureServices(serviceCollection);

    ServiceProvider = serviceCollection.BuildServiceProvider();
    */

    UiSettings.ViewLocator = new ViewLocator(Assembly.GetExecutingAssembly());
    UiSettings.DialogService = new BasicDialogService();

    App.MainVmInstance.Initialize(e.Args);
  }

  private void TimespanNoMillisecondsConverter_OnOnConvert(object? sender, ConverterEventArgs e)
  {
    if (e.Value == null) return;
    var ts = (TimeSpan)e.Value;
    e.Result = TimeSpan.FromSeconds(Math.Round(ts.TotalSeconds, 0));
  }

}