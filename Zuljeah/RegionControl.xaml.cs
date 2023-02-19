using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Eos.Mvvm.EventArgs;

namespace Zuljeah;

/// <summary>
/// Interaction logic for RegionControl.xaml
/// </summary>
public partial class RegionControl : UserControl
{

  private static readonly Brush ActiveBrush = new SolidColorBrush(Colors.Orange);

  private static readonly Brush SelectedBrush = new SolidColorBrush(Colors.LightSkyBlue);

  private static readonly Brush InactiveBrush = new SolidColorBrush(Colors.LightGray);


  public event EventHandler<bool>? SelectedChanged;


  public RegionControl()
  {
    InitializeComponent();
    DataContextChanged += (s, e) =>
    {
      if (e.OldValue is SetlistItem oi) oi.PropertyChanged -= ItemOnPropertyChanged;
      if (e.NewValue is SetlistItem ni) ni.PropertyChanged += ItemOnPropertyChanged;
    };
  }


  private void ItemOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName != nameof(SetlistItem.IsSelected)) return;
    var item = Dispatcher.Invoke(() => DataContext as SetlistItem);
    SelectedChanged?.Invoke(this, item?.IsSelected ?? false);
  }

  private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
  {
    if (DataContext is not SetlistItem item) return;
    item.IsSelected = true;
  }

  private void BorderBrushConverter_OnOnConvert(object? sender, ConverterEventArgs e)
  {
    if (e.Value is not object[] arr || arr.Length < 2) return;
    var isActive = arr[0] is true;
    var isSelected = arr[1] is true;

    if (isActive)
    {
      e.Result = ActiveBrush;
      return;
    }
    if (isSelected)
    {
      e.Result = SelectedBrush;
      return;
    }
    e.Result = InactiveBrush;
  }

}