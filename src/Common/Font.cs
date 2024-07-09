using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace GeneXus.Drawing;

[TypeConverter(typeof(FontConverter))]
[Serializable]
public sealed class Font : IDisposable, ICloneable
{
	/// <summary>
	/// Initializes a new <see cref='Font'/> that uses the specified existing <see cref='Font'/>
	/// and <see cref='FontStyle'/> enumeration.
	/// </summary>
	/// <param name="prototype">The existing <see cref='Font'/> from which to create the new <see cref='Font'/></param>
	/// <param name="newStyle">
	/// The <see cref='FontStyle'/> to apply to the new S<see cref='Font'/>. Multiple
	/// values of the <see cref='FontStyle'/> enumeration can be combined with the OR operator.
	/// </param>
	public Font(Font prototype, FontStyle newStyle)
		: this((FontFamily)prototype.FontFamily.Clone(), prototype.Size, newStyle, prototype.Unit)
	{
		OriginalFontName = prototype.OriginalFontName;
	}

	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified <see cref='Drawing.FontFamily'/> and size.
	/// </summary>
	/// <param name="family">The <see cref='FontFamily'/> of the new <see cref='Font'/>.</param>
	/// <param name="size">The size of the new font in the units specified by the <paramref name="unit"/> parameter.</param>
	/// <param name="style">The <see cref='FontStyle'/> of the new font.</param>
	/// <param name="unit">The <see cref='GraphicsUnit'/> of the new font.</param>
	/// <param name="gdiCharSet">A  <see cref='Byte'/> that specifies a GDI character set to use for this font.</param>
	/// <param name="gdiVerticalFont">A  <see cref='Boolean'/> value indicating whether the new  <see cref='Font'/> is derived from a GDI vertical font..</param>
	public Font(FontFamily family, float size = 12, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Point, byte gdiCharSet = (byte)FONT_CHARSET.DEFAULT_CHARSET, bool gdiVerticalFont = false)
	{
		FontFamily = family ?? throw new ArgumentException("missing family");
		Size = size;
		Style = style;
		Unit = unit;
		GdiCharSet = gdiCharSet;
		GdiVerticalFont = gdiVerticalFont;
	}

	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified <see cref='Drawing.FontFamily'/> and size.
	/// </summary>
	/// <param name="family">The <see cref='FontFamily'/> of the new <see cref='Font'/>.</param>
	/// <param name="size">The size of the new font in the units specified by the <paramref name="unit"/> parameter.</param>
	/// <param name="unit">The <see cref='GraphicsUnit'/> of the new font.</param>
	public Font(FontFamily family, float size, GraphicsUnit unit)
		: this(family, size, FontStyle.Regular, unit) { }

	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified family name and size.
	/// </summary>
	/// <param name="familyName">A string representation of the <see cref='FontFamily'/> of the new <see cref='Font'/>.</param>
	/// <param name="size">The size of the new font in the units specified by the <paramref name="unit"/> parameter.</param>
	/// <param name="style">The <see cref='FontStyle'/> of the new font.</param>
	/// <param name="unit">The <see cref='GraphicsUnit'/> of the new font.</param>
	/// <param name="gdiCharSet">A  <see cref='Byte'/> that specifies a GDI character set to use for this font.</param>
	/// <param name="gdiVerticalFont">A  <see cref='Boolean'/> value indicating whether the new  <see cref='Font'/> is derived from a GDI vertical font..</param>
	public Font(string familyName, float size = 12, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Point, byte gdiCharSet = (byte)FONT_CHARSET.DEFAULT_CHARSET, bool gdiVerticalFont = false)
		: this(FontFamily.Match(familyName).FirstOrDefault() ?? FontFamily.GenericSansSerif, size, style, unit, gdiCharSet, gdiVerticalFont)
	{
		OriginalFontName = familyName;
	}

	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified family name and size.
	/// </summary>
	/// <param name="familyName">A string representation of the <see cref='FontFamily'/> of the new <see cref='Font'/>.</param>
	/// <param name="size">The size of the new font in the units specified by the <paramref name="unit"/> parameter.</param>
	/// <param name="unit">The <see cref='GraphicsUnit'/> of the new font.</param>
	public Font(string familyName, float size, GraphicsUnit unit)
		: this(familyName, size, FontStyle.Regular, unit) { }

	/// <summary>
	///  Cleans up resources for this <see cref='Font'/>.
	/// </summary>
	~Font() => Dispose(false);

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Font'/>.
	/// </summary>
	public override string ToString()
	{
		int index = FontFamily.GetTypefaceIndex(Style);
		string suffix = index > 0 ? $" #{index}" : string.Empty; // show index for debug purposes
		return $"[{GetType().Name}: Name={Name}{suffix}, Size={Size}, Style={Style}, Unit={Unit}]";
	}


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Font'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing) => FontFamily.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	/// Creates an exact copy of this <see cref='Font'/>.
	/// </summary>
	public object Clone() => new Font(this, Style);

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKFont'/> with the coordinates of the specified <see cref='Font'/> .
	/// </summary>
	public static explicit operator SKFont(Font font) => font.Typeface.ToFont(font.Size);

	#endregion


	#region Properties

	/// <summary>
	/// Gets the face name of this <see cref='Font'/>.
	/// </summary>
	public string Name => OriginalFontName ?? FontFamily.Name;

	/// <summary>
	/// Gets the name of the <see cref='Font'/> originally specified.
	/// </summary>
	public string OriginalFontName { get; private set; }

	/// <summary>
	/// Gets the <see cref='Drawing.FontFamily'/> associated with this Font.
	/// </summary>
	public FontFamily FontFamily { get; private set; }

	/// <summary>
	/// Gets the weight of this <see cref='Font'/>.
	/// </summary>
	public int Weight => Typeface.FontWeight;

	/// <summary>
	/// Gets the width of this <see cref='Font'/>.
	/// </summary>
	public int Width => Typeface.FontWidth;

	/// <summary>
	/// Gets the line spacing of this <see cref='Font'/>.
	/// The Height property is the value returned by GetHeight method, rounded up to the nearest integer.
	/// </summary>
	public int Height => (int)Math.Ceiling(GetHeight());

	/// <summary>
	/// Gets the size of this <see cref='Font'/>.
	/// </summary>
	public float Size { get; private set; }

	/// <summary>
	/// Gets the slant of this <see cref='Font'/>.
	/// </summary>
	public SlantType Slant => Typeface.FontSlant switch
	{
		SKFontStyleSlant.Oblique => SlantType.Oblique,
		SKFontStyleSlant.Italic => SlantType.Italic,
		SKFontStyleSlant.Upright => SlantType.Normal,
		_ => throw new Exception("missing slant type")
	};

	/// <summary>
	/// Gets style information for this <see cref='Font'/>.
	/// </summary>
	public FontStyle Style { get; private set; }

	/// <summary>
	/// Gets the FamilyName associated with this <see cref='Font'/>.
	/// </summary>
	public string FamilyName => FontFamily.Name;

	/// <summary>
	/// Gets the FaceName associated with this <see cref='Font'/>.
	/// </summary>
	public string FaceName
	{
		get
		{
			string widthName = Width switch
			{
				1 => "Ultra Condensed",
				2 => "Extra Condensed",
				3 => "Condensed",
				4 => "Semi Condensed",
				5 => "Normal",
				6 => "Semi Expanded",
				7 => "Expanded",
				8 => "Extra Expanded",
				9 => "Ultra Expanded",
				_ => string.Empty
			};

			string weightName = Weight switch
			{
				< 100 => "Extra Thin",
				< 275 => "Thin",
				< 300 => "Extra Light",
				< 350 => "Light",
				< 400 => "Semi Light",
				< 500 => "Normal",
				< 600 => "Medium",
				< 700 => "Semi Bold",
				< 800 => "Bold",
				< 900 => "Extra Bold",
				< 950 => "Black",
				< 999 => "Extra Black",
				_ => string.Empty
			};

			string slantName = Slant switch
			{
				SlantType.Normal => "Normal",
				SlantType.Italic => "Italic",
				SlantType.Oblique => "Oblique",
				_ => string.Empty
			};

			// Reference: https://referencesource.microsoft.com/#PresentationCore/Core/CSharp/MS/Internal/FontFace/FontDifferentiator.cs,29
			StringBuilder faceName = new();

			AppendStyle(widthName);
			AppendStyle(weightName);
			AppendStyle(slantName);

			return faceName.Append(faceName.Length > 0 ? string.Empty : "Regular").ToString();

			void AppendStyle(string value)
			{
				if (value.Equals("Normal", StringComparison.OrdinalIgnoreCase)) return;
				faceName.Append(faceName.Length > 0 ? " " : string.Empty).Append(value.Replace(" ", string.Empty));
			}
		}
	}

	/// <summary>
	/// Gets a value that indicates whether this <see cref='Font'/> has the italic style applied.
	/// </summary>
	public bool Italic => Typeface.IsItalic;

	/// <summary>
	/// Gets a value that indicates whether this <see cref='Font'/> is bold.
	/// </summary>
	public bool Bold => Typeface.IsBold;

	/// <summary>
	/// Gets a value indicating whether this <see cref='Font'/> is underlined.
	/// </summary>
	public bool Underline => Metrics is { UnderlineThickness: > 0, UnderlinePosition: 0f };

	/// <summary>
	/// Gets a value indicating whether this <see cref='Font'/> is strikeout (has a line through it).
	/// </summary>
	public bool Strikeout => Metrics is { StrikeoutThickness: > 0, StrikeoutPosition: 0f };

	/// <summary>
	/// Gets a value indicating whether the <see cref='Font'/> is a member of SystemFonts.
	/// </summary>
	public bool IsSystemFont => false; // there are no system fonts like it is explained in https://learn.microsoft.com/en-us/dotnet/api/system.drawing.systemfonts?view=net-8.0

	/// <summary>
	/// Gets the name of this <see cref='Font'/>.
	/// </summary>
	[Browsable(false)]
	public string SystemFontName => string.Empty;

	/// <summary>
	/// Gets the em-size, in points, of this <see cref='Font'/>.
	/// </summary>
	/// <returns>The em-size, in points, of this <see cref='Font'/></returns>
	[Browsable(false)]
	public float SizeInPoints => Unit switch
	{
		GraphicsUnit.World => throw new NotSupportedException("World unit conversion is not supported."),
		GraphicsUnit.Display => Size * 72 / DPI, // Assuming display unit is pixels
		GraphicsUnit.Pixel => Size * 72 / DPI, // 1 pixel = 72 points per inch / Dots Per Inch
		GraphicsUnit.Point => Size, // Already in points
		GraphicsUnit.Inch => Size * 72, // 1 inch = 72 points
		GraphicsUnit.Document => Size * 72 / 300, // 1 document unit = 1/300 inch
		GraphicsUnit.Millimeter => Size * 72 / 25.4f, // 1 millimeter = 1/25.4 inch
		_ => throw new ArgumentException("Invalid GraphicsUnit")
	};
	
	/// <summary>
	/// Gets the unit of measure for this <see cref='Font'/>.
	/// </summary>
	/// <returns>A <see cref='GraphicsUnit'/> that represents the unit of measure for this <see cref='Font'/>.</returns>
	public GraphicsUnit Unit { get; private set; }

	/// <summary>
	/// Returns the GDI char set for this instance of a font. This will only
	/// be valid if this font was created from a classic GDI font definition,
	/// like a LOGFONT or HFONT, or it was passed into the constructor.
	/// </summary>
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public byte GdiCharSet { get; private set; }

	/// <summary>
	/// Determines if this font was created to represent a GDI vertical font. This will only be valid if this font
	/// was created from a classic GDIfont definition, like a LOGFONT or HFONT, or it was passed into the constructor.
	/// </summary>
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool GdiVerticalFont { get; private set; }

	#endregion


	#region Methods

	/// <summary>
	/// Returns the line spacing of this <see cref='Font'/>.
	/// </summary>
	public float GetHeight()
		=> Metrics.Descent - Metrics.Ascent + Metrics.Leading;

	#endregion


	#region Extras

	/* 
	 * NOTE: This implementation is not part of System.Drawing.Font 
	 * nor System.Windows.Media.Fonts libraries
	 */

	/// <summary>
	/// Returns the embedded <see cref='Font'/> count from a filename.
	/// </summary>
	public static int GetFontCount(string filepath)
	{
		var data = File.ReadAllBytes(filepath);
		using var stream = new MemoryStream(data);
		return GetFontCount(stream);
	}

	/// <summary>
	/// Returns the embedded <see cref='Font'/> count from a file stream.
	/// </summary>
	public static int GetFontCount(Stream stream)
	{
		using var reader = new BinaryReader(stream);
		string type = new(reader.ReadChars(4));
		if (type.Equals("ttcf") || type.Equals("OTTO")) // check ttc/otc type in file header
		{
			int version = BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);
			if (version != 0x00010000 && version != 0x00020000)
				throw new ArgumentException($"unrecognized ttc/otc version 0x{version:X8}");
			int count = BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);
			return count;
		}
		return 1;
	}

	#endregion


	#region Utilties

	private enum FONT_CHARSET
	{
		ANSI_CHARSET = 0x00,
		DEFAULT_CHARSET = 0x01,
		SYMBOL_CHARSET = 0x02,
		SHIFTJIS_CHARSET = 0x80,
		HANGUL_CHARSET = 0x81,
		GB2312_CHARSET = 0x86,
		CHINESEBIG5_CHARSET = 0x88,
		GREEK_CHARSET = 0xA1,
		TURKISH_CHARSET = 0xA2,
		HEBREW_CHARSET = 0xB1,
		ARABIC_CHARSET = 0xB2,
		BALTIC_CHARSET = 0xBA,
		RUSSIAN_CHARSET = 0xCC,
		THAI_CHARSET = 0xDE,
		EE_CHARSET = 0xEE,
		OEM_CHARSET = 0xFF
	}

	private SKTypeface Typeface => FontFamily.GetTypeface(Style);

	private SKFontMetrics Metrics => Typeface.ToFont(Size).Metrics;

	private static int DPI
	{
		get
		{
			using var surface = SKSurface.Create(new SKImageInfo(50, 50));
			return (int)(100f * surface.Canvas.DeviceClipBounds.Width / surface.Canvas.LocalClipBounds.Width);
		}
	}

	#endregion
}
