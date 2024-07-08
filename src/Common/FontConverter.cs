using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace GeneXus.Drawing;

/// <summary>
/// Converts <see cref="Font" /> objects from one data type to another.
/// </summary>
public class FontConverter : TypeConverter
{
	internal class UnitName
	{
		internal readonly string m_name;
		internal readonly GraphicsUnit m_unit;

		internal static readonly UnitName[] Names = {
			new("world", GraphicsUnit.World),
			new("display", GraphicsUnit.Display),
			new("px", GraphicsUnit.Pixel),
			new("pt", GraphicsUnit.Point),
			new("in", GraphicsUnit.Inch),
			new("doc", GraphicsUnit.Document),
			new("mm", GraphicsUnit.Millimeter)
		};

		private UnitName(string name, GraphicsUnit unit)
		{
			m_name = name;
			m_unit = unit;
		}
	}

	/// <summary>
	/// <see cref='FontNameConverter'/> is a type converter that is used
	/// to convert a font name to and from various other representations.
	/// </summary>
	private sealed class FontNameConverter : TypeConverter
	{
		private StandardValuesCollection _values;

		/// <summary>
		/// Determines if this converter can convert an object in the given source type to
		/// the native type of the converter.
		/// </summary>
		/// <param name="context">
		/// An <see cref='ITypeDescriptorContext'/> that can be used to extract additional
		/// information about the environment this converter is being invoked from. This
		/// may be null, so you should always check. Also, properties on the context object
		/// may return null.
		/// </param>
		/// <param name="sourceType">The type you wish to convert from.</param>
		/// <returns>true if the converter can perform the conversion; otherwise, false.</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the given object to the converter's native type.
		/// </summary>
		/// <param name="context">
		/// An <see cref='ITypeDescriptorContext'/> that can be used to extract additional
		/// information about the environment this converter is being invoked from. This
		/// may be null, so you should always check. Also, properties on the context object
		/// may return null.
		/// </param>
		/// <param name="culture">A <see cref='CultureInfo'/> to use to perform the conversion</param>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted object.</returns>
		/// <exception cref="NotSupportedException">The conversion cannot be completed.</exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string s)
				return MatchFontName(s, context);

			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Retrieves a collection containing a set of standard values for the data type
		/// this converter is designed for.
		/// </summary>
		/// <param name="context">
		/// An <see cref='ITypeDescriptorContext'/> that can be used to extract additional
		/// information about the environment this converter is being invoked from. This
		/// may be null, so you should always check. Also, properties on the context object
		/// may return null.
		/// </param>
		/// <returns>A collection containing a standard set of valid values, or null. The default is null.</returns>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (_values != null)
				return _values;

			FontFamily[] families = FontFamily.Families;
			Hashtable hashtable = new();
			foreach (FontFamily fontFamily in families)
			{
				string name = fontFamily.Name;
				hashtable[name.ToLower(CultureInfo.InvariantCulture)] = name;
			}

			object[] array = new object[hashtable.Values.Count];
			hashtable.Values.CopyTo(array, 0);
			Array.Sort(array, Comparer.Default);
			_values = new StandardValuesCollection(array);

			return _values;
		}

		/// <summary>
		/// Determines if the list of standard values returned from the <see cref="GetStandardValues"/>
		/// method is an exclusive list.
		/// </summary>
		/// <param name="context">
		/// An <see cref='ITypeDescriptorContext'/> that can be used to extract additional
		/// information about the environment this converter is being invoked from. This
		/// may be null, so you should always check. Also, properties on the context object
		/// may return null.
		/// </param>
		/// <returns>
		/// true if the collection returned from <see cref='GetStandardValues'/>
		/// is an exclusive list of possible values; otherwise, false. The default is false.
		/// </returns>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		/// <summary>
		/// Determines if this object supports a standard set of values that can be picked from a list.
		/// </summary>
		/// <param name="context">
		/// An <see cref='ITypeDescriptorContext'/> that can be used to extract additional
		/// information about the environment this converter is being invoked from. This
		/// may be null, so you should always check. Also, properties on the context object
		/// may return null.
		/// </param>
		/// <returns>
		/// true if <see cref="GetStandardValues"/> should be called to find a common set 
		/// of values the object supports; otherwise, false.
		/// </returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private string MatchFontName(string name, ITypeDescriptorContext context)
		{
			string candidate = null;
			string lowerName = name.ToLower(CultureInfo.InvariantCulture);
			foreach (string fontName in GetStandardValues(context))
			{
				string lowerFontName = fontName.ToLower(CultureInfo.InvariantCulture);
				if (lowerFontName.Equals(lowerName))
					return fontName;

				if (lowerFontName.StartsWith(name) && (candidate == null || lowerFontName.Length <= candidate.Length))
					candidate = fontName;
			}

			return candidate ?? name;
		}

		private void OnInstalledFontsChanged(object sender, EventArgs e)
		{
			_values = null;
		}
	}

	/// <summary>
	/// Converts font units to and from other unit types.
	/// </summary>
	private class FontUnitConverter : EnumConverter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref='FontUnitConverter'/> class.
		/// </summary>
		public FontUnitConverter()
			: base(typeof(GraphicsUnit)) { }

		/// <summary>
		/// Returns a collection of standard values valid for the <see cref='Font'/> type.
		/// </summary>
		/// <param name="context">An <see cref='ITypeDescriptorContext'/> that provides a format context.</param>
		/// <returns>A collection containing a standard set of valid values, or null. The default is null.</returns>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (base.Values == null)
			{
				base.GetStandardValues(context);
				ArrayList arrayList = new(base.Values);
				arrayList.Remove(GraphicsUnit.Display);
				base.Values = new StandardValuesCollection(arrayList);
			}

			return base.Values;
		}
	}

	private FontNameConverter _fontNameConverter;

	/// <summary>
	/// Determines whether this converter can convert an object in the specified source
	/// type to the native type of the converter.
	/// </summary>
	/// <param name="context">
	/// A formatter context. This object can be used to get additional information about
	/// the environment this converter is being called from. This may be null, so you
	/// should always check. Also, properties on the context object may also return null.
	/// </param>
	/// <param name="sourceType">The type you want to convert from</param>
	/// <returns>This method returns true if this object can perform the conversion.</returns>
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
	}

	/// <summary>
	/// Gets a value indicating whether this converter can convert an object to the given
	/// destination type using the context.
	/// </summary>
	/// <param name="context">An <see cref='ITypeDescriptorContext'/> object that provides a format context.</param>
	/// <param name="destinationType">A <see cref='Type'/> object that represents the type you want to convert to.</param>
	/// <returns>This method returns true if this converter can perform the conversion; otherwise, false.</returns>
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
	}

	/// <summary>
	/// Converts the specified object to the native type of the converter.
	/// </summary>
	/// <param name="context">
	/// A formatter context. This object can be used to get additional information about
	/// the environment this converter is being called from. This may be null, so you
	/// should always check. Also, properties on the context object may also return null.
	/// </param>
	/// <param name="culture">A <see cref='CultureInfo'/> object that specifies the culture used to represent the font.</param>
	/// <param name="value">The object to convert.</param>
	/// <returns>The converted object.</returns>
	/// <exception cref="NotSupportedException">The conversion could not be performed.</exception>
	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is not string text)
			return base.ConvertFrom(context, culture, value);

		string trimmedText = text.Trim();
		if (trimmedText.Length == 0)
			return null;

		culture ??= CultureInfo.CurrentCulture;

		char separator = culture.TextInfo.ListSeparator[0];
		string familyName = trimmedText;
		float emSize = 8.25f;
		FontStyle fontStyle = FontStyle.Regular;
		GraphicsUnit unit = GraphicsUnit.Point;

		int separatorIndex = trimmedText.IndexOf(separator);
		if (separatorIndex > 0)
		{
			familyName = trimmedText.Substring(0, separatorIndex);
			if (separatorIndex < trimmedText.Length - 1)
			{
				string sizeAndUnit;
				string style = null;

				int styleIndex = trimmedText.IndexOf("style=");
				if (styleIndex != -1)
				{
					style = trimmedText.Substring(styleIndex, trimmedText.Length - styleIndex);
					if (!style.StartsWith("style="))
						throw GetFormatException(trimmedText, separator);

					sizeAndUnit = trimmedText.Substring(separatorIndex + 1, styleIndex - separatorIndex - 1);
				}
				else
				{
					sizeAndUnit = trimmedText.Substring(separatorIndex + 1, trimmedText.Length - separatorIndex - 1);
				}

				ParseSizeTokens(sizeAndUnit, separator, out string sizeText, out string unitText);
				if (sizeText != null)
				{
					try
					{
						emSize = (float)TypeDescriptor.GetConverter(typeof(float)).ConvertFromString(context, culture, sizeText);
					}
					catch
					{
						throw GetFormatException(trimmedText, separator);
					}
				}

				if (unitText != null)
				{
					unit = ParseGraphicsUnits(unitText);
				}

				if (style != null)
				{
					int indexEqual = style.IndexOf("=");
					style = style.Substring(indexEqual + 1, style.Length - "style=".Length);
					foreach (string stylePart in style.Split(separator))
					{
						try
						{
							fontStyle |= (FontStyle)Enum.Parse(typeof(FontStyle), stylePart.Trim(), ignoreCase: true);
						}
						catch (Exception ex)
						{
							if (ex is InvalidEnumArgumentException)
								throw;

							throw GetFormatException(trimmedText, separator);
						}

						const FontStyle validFontStyle = FontStyle.Bold | FontStyle.Italic | FontStyle.Underline | FontStyle.Strikeout;
						if ((fontStyle | validFontStyle) != validFontStyle)
						{
							throw new InvalidEnumArgumentException("style", (int)fontStyle, typeof(FontStyle));
						}
					}
				}
			}
		}

		_fontNameConverter ??= new FontNameConverter();
		familyName = (string)_fontNameConverter.ConvertFrom(context, culture, familyName);
		return new Font(familyName, emSize, fontStyle, unit);
	}

	/// <summary>
	/// Converts the specified object to another type.
	/// </summary>
	/// <param name="context">
	/// A formatter context. This object can be used to get additional information about
	/// the environment this converter is being called from. This may be null, so you
	/// should always check. Also, properties on the context object may also return null.
	/// </param>
	/// <param name="culture">
	/// A <see cref='CultureInfo'/> object that specifies the culture used to represent the object.
	/// </param>
	/// <param name="value">The object to convert.</param>
	/// <param name="destinationType">The data type to convert the object to.</param>
	/// <returns>The converted object.</returns>
	/// <exception cref="NotSupportedException">The conversion was not successful.</exception>
	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == null)
			throw new ArgumentNullException(nameof(destinationType));

		if (destinationType == typeof(string))
		{
			if (value is not Font font)
				return string.Empty;

			culture ??= CultureInfo.CurrentCulture;

			string separator = culture.TextInfo.ListSeparator + " ";
			int parts = 2;
			if (font.Style != 0)
				parts++;

			string[] array = new string[parts];
			array[0] = font.Name;
			array[1] = TypeDescriptor.GetConverter(font.Size).ConvertToString(context, culture, font.Size) + GetGraphicsUnitText(font.Unit);
			if (font.Style != 0)
				array[2] = "style=" + font.Style.ToString("G");

			return string.Join(separator, array);
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}

	/// <summary>
	/// Creates an object of this type by using a specified set of property values for the object.
	/// </summary>
	/// <param name="context">A type descriptor through which additional context can be provided.</param>
	/// <param name="propertyValues">
	/// A dictionary of new property values. The dictionary contains a series of name-value
	/// pairs, one for each property returned from the <see cref='GetProperties'/>
	/// method.
	/// </param>
	/// <returns>
	/// The newly created object, or null if the object could not be created. The default
	/// implementation returns null. Useful for creating non-changeable objects that have changeable properties.
	/// </returns>
	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
			throw new ArgumentNullException(nameof(propertyValues));

		object familyObj = propertyValues["Name"] ?? "Tahoma";
		object sizeObj = propertyValues["Size"] ?? 8f;
		object unitObj = propertyValues["Unit"] ?? GraphicsUnit.Point;
		object isBoldObj = propertyValues["Bold"] ?? false;
		object isItalicObj = propertyValues["Italic"] ?? false;
		object isStrikeoutObj = propertyValues["Strikeout"] ?? false;
		object isUnderlineObj = propertyValues["Underline"] ?? false;
		if (familyObj is not string family
			|| sizeObj is not float size
			|| unitObj is not GraphicsUnit unit
			|| isBoldObj is not bool isBold
			|| isItalicObj is not bool isItalic
			|| isStrikeoutObj is not bool isStrikeout
			|| isUnderlineObj is not bool isUnderline)
		{
			throw new ArgumentException("Property value with incorrect type");
		}

		FontStyle style = FontStyle.Regular;
		if (isBold)
			style |= FontStyle.Bold;

		if (isItalic)
			style |= FontStyle.Italic;

		if (isStrikeout)
			style |= FontStyle.Strikeout;

		if (isUnderline)
			style |= FontStyle.Underline;

		return new Font(family, size, style, unit);
	}

	/// <summary>
	/// Determines whether changing a value on this object should require a call to the
	/// <see cref='CreateInstance'/> method to create a new value.
	/// </summary>
	/// <param name="context">A type descriptor through which additional context can be provided.</param>
	/// <returns>
	/// This method returns true if the CreateInstance object should be called when a
	/// change is made to one or more properties of this object; otherwise, false.
	/// </returns>
	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	private static ArgumentException GetFormatException(string text, char separator)
	{
		string validFormat = string.Format(CultureInfo.CurrentCulture, "name{0} size[units[{0} style=style1[{0} style2{0} ...]]]", separator);
		return new ArgumentException($"Failed to parse font text, input: {text}, valid format: {validFormat}");
	}

	private static string GetGraphicsUnitText(GraphicsUnit units)
	{
		return UnitName.Names.FirstOrDefault(n => n.m_unit == units)?.m_name ?? "";
	}

	/// <summary>
	/// Retrieves the set of properties for this type. By default, a type does not have
	/// any properties to return.
	/// </summary>
	/// <param name="context">A type descriptor through which additional context can be provided.</param>
	/// <param name="value">The value of the object to get the properties for.</param>
	/// <param name="attributes">An array of <see cref='Attribute'/> objects that describe the properties.</param>
	/// <returns>
	/// The set of properties that should be exposed for this data type. If no properties
	/// should be exposed, this may return null. The default implementation always returns
	/// null. An easy implementation of this method can call the <see cref='TypeConverter.GetProperties'/>
	/// method for the correct data type.
	/// </returns>
	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Font), attributes);
		return properties.Sort(new[] { "Name", "Size", "Unit", "Weight" });
	}

	/// <summary>
	/// Determines whether this object supports properties. The default is false.
	/// </summary>
	/// <param name="context">A type descriptor through which additional context can be provided.</param>
	/// <returns>
	/// This method returns true if the <see cref="GetPropertiesSupported(ITypeDescriptorContext)"/>
	/// method should be called to find the properties of this object; otherwise, false.
	/// </returns>
	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	private static void ParseSizeTokens(string text, char separator, out string size, out string unit)
	{
		size = null;
		unit = null;
		text = text.Trim();
		int length = text.Length;
		if (length <= 0)
			return;

		int index = 0;
		while (index < length && !char.IsLetter(text[index]))
			index++;

		char[] trimChars = { separator, ' ' };
		if (index > 0)
		{
			size = text.Substring(0, index);
			size = size.Trim(trimChars);
		}

		if (index < length)
		{
			unit = text.Substring(index);
			unit = unit.TrimEnd(trimChars);
		}
	}

	private static GraphicsUnit ParseGraphicsUnits(string units)
	{
		UnitName unitName = UnitName.Names.FirstOrDefault(un => string.Equals(un.m_name, units, StringComparison.OrdinalIgnoreCase));
		if (unitName == null)
			throw new ArgumentException(nameof(units));
		return unitName.m_unit;
	}
}
