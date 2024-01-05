namespace Validator_Management_Tool.Tools
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Text;
    using System.Windows.Data;

    using Skyline.DataMiner.CICD.Validators.Common.Model;

    using Validator_Management_Tool.Model;

    [ValueConversion(typeof(string), typeof(bool))]
    public class EmptyStringToBooleanConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.IsNullOrEmpty((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.IsNullOrEmpty((string)value);
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    [ValueConversion(typeof(uint), typeof(Certainty))]
    public class CertaintyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint)
            {
                return (Certainty)Int32.Parse(value.ToString());
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Certainty)
            {
                return UInt32.Parse(((int)value).ToString());
            }
            return value;
        }
    }

    [ValueConversion(typeof(uint), typeof(FixImpact))]
    public class FixImpactConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint)
            {
                return (FixImpact)Int32.Parse(value.ToString());
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FixImpact)
            {
                return UInt32.Parse(((int)value).ToString());
            }
            return value;
        }
    }

    [ValueConversion(typeof(Category), typeof(int))]
    public class CategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Category)
            {
                return (int)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return (Category)Enum.Parse(typeof(Category), value.ToString());
            }
            return value;
        }
    }

    [ValueConversion(typeof(Source), typeof(uint))]
    public class SourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Source)
            {
                return (int)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint)
            {
                return (Source)Enum.Parse(typeof(Source), value.ToString());
            }
            return value;
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReadOnlyObservableCollection<object> collection = (ReadOnlyObservableCollection<object>)value;
            StringBuilder total = new StringBuilder();
            int count = 0;
            foreach (CollectionViewGroup group in collection)
            {
                Check check = (Check)group.Items[0];
                if (count == 0)
                {
                    total.Append(check.CheckId);
                }
                else
                {
                    total.Append(", ").Append(check.CheckId);
                }

                count++;
            }

            return total.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class CheckIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReadOnlyObservableCollection<object> collection = (ReadOnlyObservableCollection<object>)value;

            if (collection.Count != 0)
            {
                var check = (Check)collection[0];
                return check.CheckId.ToString();
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Empty;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class CheckBubbelUpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReadOnlyObservableCollection<object> collection = (ReadOnlyObservableCollection<object>)value;
            bool error = false;

            foreach (Check check in collection)
            {
                if (check.Error)
                {
                    error = true;
                }
            }

            return error;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class NamespaceBubbelUpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReadOnlyObservableCollection<object> collection = (ReadOnlyObservableCollection<object>)value;
            bool error = false;

            foreach (CollectionViewGroup group in collection)
            {
                foreach (Check check in group.Items)
                {
                    if (check.Error)
                    {
                        error = true;
                    }
                }
            }

            return error;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class CategoryBubbelUpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReadOnlyObservableCollection<object> collection = (ReadOnlyObservableCollection<object>)value;
            bool error = false;

            foreach (CollectionViewGroup group1 in collection)
            {
                foreach (CollectionViewGroup group2 in group1.Items)
                {
                    foreach (Check check in group2.Items)
                    {
                        if (check.Error)
                        {
                            error = true;
                        }
                    }
                }
            }

            return error;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
