
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace GeneXus.Drawing;

public class ColorConverter : TypeConverter
{
	private static readonly Lazy<StandardValuesCollection> s_valuesLazy = new(() =>
	{
		var set = new HashSet<Color>();
		foreach (PropertyInfo prop in typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static))
			if (prop.GetValue(null) is Color color)
				set.Add(color);
		return new StandardValuesCollection(set.OrderBy(c => c, new ColorComparer()).ToList());
	});

	/// <summary>
	///  Initializes a new instance of the <see cref='ColorConverter'/> class.
	/// </summary>
	public ColorConverter() { }


	# region TypeConverter overrides

	/// <summary>
	///  Determines if this converter can convert an object in the given source type to the native type of the converter.
	/// </summary>
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		=> sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

	/// <summary>
	///  Returns a value indicating whether this converter can convert an object to the given destination type using the context.
	/// </summary>
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		=> destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);

	/// <summary>
	///  Converts the given object to the converter's native type.
	/// </summary>
	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		=> value is string strValue ? ColorTranslator.ConvertFromString(strValue, culture ?? CultureInfo.CurrentCulture)
		:  base.ConvertFrom(context, culture, value);

	/// <summary>
	///  Converts the specified object to another type.
	/// </summary>
	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == null) 
			throw new ArgumentNullException(nameof(destinationType));

		if (value is Color c)
		{
			if (destinationType == typeof(string))
			{
				if (c.IsEmpty)
					return string.Empty;

				if (c.IsKnownColor)
					return c.Name;

				if (c.IsNamedColor)
					return $"'{c.Name}'";

				culture ??= CultureInfo.CurrentCulture;

				var converter = TypeDescriptor.GetConverter(typeof(int));
				var components = c.A < 255 ? new[] { c.A, c.R, c.G, c.B } : new[] { c.R, c.G, c.B };

				var sep = string.Concat(culture.TextInfo.ListSeparator, " ");
				var args = components.Select(comp => converter.ConvertToString(context, culture, comp));
				return string.Join(sep, args);
			}
			else if (destinationType == typeof(InstanceDescriptor))
			{
				MemberInfo member = null;
				object[] args = new object[] { };

				if (c.IsEmpty)
				{
					member = typeof(Color).GetField("Empty");
				}
				else if (c.IsKnownColor)
				{
					member = typeof(Color).GetProperty(c.Name);
				}
				else if (c.IsNamedColor)
				{
					member = typeof(Color).GetMethod("FromName", new[] { typeof(string) });
					args = new object[] { c.Name };
				}
				else if (c.A < 255)
				{
					member = typeof(Color).GetMethod("FromArgb", new[] { typeof(int), typeof(int), typeof(int), typeof(int) });
					args = new object[] { c.A, c.R, c.G, c.B };
				}
				else
				{
					member = typeof(Color).GetMethod("FromArgb", new[] { typeof(int), typeof(int), typeof(int) });
					args = new object[] { c.R, c.G, c.B };
				}

				if (member != null)
					return new InstanceDescriptor(member, args);
				return null;
			}
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}

	/// <summary>
	///  Retrieves a collection containing a set of standard values for the data type for which this validator 
	///  is designed. This will return null if the data type does not support a standard set of values.
	/// </summary>
	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		=> s_valuesLazy.Value;

	/// <summary>
	///  Determines if this object supports a standard set of values that can be chosen from a list.
	/// </summary>
	public override bool GetStandardValuesSupported(ITypeDescriptorContext context) 
		=> true;

	#endregion


	#region Utilities

	private sealed class ColorComparer : IComparer<Color>
	{
		public int Compare(Color left, Color right) => string.CompareOrdinal(left.Name, right.Name);
	}

	#endregion
}