using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace _000_BoolLammp.Helper.Conveter
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            Type type = value.GetType();
            if (!type.IsEnum)
                return DependencyProperty.UnsetValue;

            string name = value.ToString();
            FieldInfo fi = type.GetField(name);
            if (fi == null)
                return name;

            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes?.FirstOrDefault()?.Description ?? name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value?.ToString();
            if (string.IsNullOrEmpty(strValue) || !targetType.IsEnum)
                return DependencyProperty.UnsetValue;

            return Enum.TryParse(targetType, strValue, true, out object result)
                ? result
                : DependencyProperty.UnsetValue;
        }
    }
    //[ValueConversion(typeof(Enum), typeof(String))]
    //public class EnumDescriptionConverter : IValueConverter
    //{
    //   public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null)
    //            return DependencyProperty.UnsetValue; // 或 return string.Empty;
    //        FieldInfo fi = value.GetType().GetField(value.ToString());
    //        if (fi == null)
    //            return value.ToString(); // 或抛出异常
    //        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
    //        return attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].Description) ? attributes[0].Description : value.ToString();
    //    }
    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        //return Enum.Parse(value.GetType(), value.ToString());
    //        if (value == null || !targetType.IsEnum)
    //            return DependencyProperty.UnsetValue;

    //        string strValue = value.ToString();
    //        if (Enum.TryParse(targetType, strValue, out object result))
    //            return result;

    //        return DependencyProperty.UnsetValue; // 或处理错误
    //    }

    //}


}
