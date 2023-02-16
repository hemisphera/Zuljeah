using System;
using System.Windows.Controls;
using System.Windows.Media;
using Eos.Mvvm.EventArgs;

namespace Zuljeah;

/// <summary>
/// Interaction logic for RegionControl.xaml
/// </summary>
public partial class RegionControl : UserControl
{

  private static readonly Brush ActiveBrush = new SolidColorBrush(Colors.Orange);

  private static readonly Brush InactiveBrush = new SolidColorBrush(Colors.LightGray);


  public RegionControl()
  {
    InitializeComponent();
  }

  private void IsActiveBrushConverter_OnOnConvert(object? sender, ConverterEventArgs e)
  {
    e.Result = e.Value is true ? ActiveBrush : InactiveBrush;
  }

}