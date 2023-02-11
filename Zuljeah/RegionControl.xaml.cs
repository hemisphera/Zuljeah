using System;
using System.Windows.Controls;
using System.Windows.Media;
using Eos.Mvvm.EventArgs;

namespace WpfApp1;

/// <summary>
/// Interaction logic for RegionControl.xaml
/// </summary>
public partial class RegionControl : UserControl
{

  private static Brush ActiveBrush = new SolidColorBrush(Colors.Orange);

  private static Brush InactiveBrush = new SolidColorBrush(Colors.LightGray);


  public event EventHandler<SetlistItem>? PlayStarted;


  public RegionControl()
  {
    InitializeComponent();
  }

  private void IsActiveBrushConverter_OnOnConvert(object? sender, ConverterEventArgs e)
  {
    e.Result = e.Value is true ? ActiveBrush : InactiveBrush;
  }

}