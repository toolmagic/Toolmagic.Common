using System;
using System.Globalization;
using System.Windows.Data;

namespace Toolmagic.Common.Presentation
{
	public abstract class ValueConverter<TSource, TTarget> : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Argument.IsNotNull(targetType, nameof(targetType));
			Argument.IsNotNull(culture, nameof(culture));

			if (targetType != typeof(TTarget) && !typeof(TTarget).IsSubclassOf(targetType))
			{
				return null;
			}

			return Convert((TSource) value, parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Argument.IsNotNull(targetType, nameof(targetType));
			Argument.IsNotNull(culture, nameof(culture));

			if (targetType != typeof(TSource) && !typeof(TTarget).IsSubclassOf(targetType))
			{
				return null;
			}

			return ConvertBack((TTarget) value, parameter, culture);
		}

		protected abstract TTarget Convert(TSource value, object parameter, CultureInfo culture);
		protected abstract TSource ConvertBack(TTarget value, object parameter, CultureInfo culture);
	}
}