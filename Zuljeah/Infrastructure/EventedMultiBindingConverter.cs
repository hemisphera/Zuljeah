using System;
using System.Globalization;
using System.Windows.Data;
using Eos.Mvvm.EventArgs;

namespace Zuljeah.Infrastructure;

public class EventedMultiConverter : IMultiValueConverter
{

  public event EventHandler<ConverterEventArgs> OnConvert;

  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    var e = new ConverterEventArgs(values, targetType, parameter, culture);
    var onConvert = this.OnConvert;
    if (onConvert != null)
      onConvert(this, e);
    return e.Result;
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotSupportedException();
  }

}