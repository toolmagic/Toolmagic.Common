using System;
using System.Globalization;
using System.Windows.Data;

namespace Toolmagic.Common.Presentation
{
	public abstract class MultiValueConverter<TSource1, TSource2, TTarget> : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			Argument.IsNotNull(values, nameof(values));
			Argument.AreEqual(2, values.Length, nameof(values));
			Argument.IsNotNull(targetType, nameof(targetType));

			if (targetType != typeof(TTarget) && !targetType.IsSubclassOf(typeof(TTarget)))
			{
				return null;
			}

			return Convert((TSource1) values[0], (TSource2) values[1], parameter, culture);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			Argument.IsNotNull(targetTypes, nameof(targetTypes));
			Argument.AreEqual(2, targetTypes.Length, nameof(targetTypes));

			if (targetTypes[0] != typeof(TSource1) && !targetTypes[0].IsSubclassOf(typeof(TSource1)))
			{
				return null;
			}

			if (targetTypes[1] != typeof(TSource2) && !targetTypes[1].IsSubclassOf(typeof(TSource2)))
			{
				return null;
			}

			var result = ConvertBack((TTarget) value, parameter, culture);

			if (result == null)
			{
				return null;
			}

			return new object[] {result.Item1, result.Item2};
		}

		protected abstract TTarget Convert(TSource1 value1, TSource2 value2, object parameter, CultureInfo culture);
		protected abstract Tuple<TSource1, TSource2> ConvertBack(TTarget value, object parameter, CultureInfo culture);
	}
}