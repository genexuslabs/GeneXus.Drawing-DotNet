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
	/// <param name="prototype">The existing <see cref='Font'/> from which to create the new <see cref='Font'/>.</param>
	/// <param name="newStyle">
	/// The <see cref='FontStyle'/> to apply to the new <see cref='Font'/>. Multiple
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
	/// <param name="gdiCharSet">A <see cref='Byte'/> that specifies a GDI character set to use for this font.</param>
	/// <param name="gdiVerticalFont">A <see cref='Boolean'/> value indicating whether the new <see cref='Font'/> is derived from a GDI vertical font.</param>
	public Font(FontFamily family, float size = 12, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Point, byte gdiCharSet = Interop.GDI_CHARSET.DEFAULT_CHARSET, bool gdiVerticalFont = false)
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
	/// <param name="gdiCharSet">A <see cref='Byte'/> that specifies a GDI character set to use for this font.</param>
	/// <param name="gdiVerticalFont">A <see cref='Boolean'/> value indicating whether the new <see cref='Font'/> is derived from a GDI vertical font.</param>
	public Font(string familyName, float size = 12, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Point, byte gdiCharSet = Interop.GDI_CHARSET.DEFAULT_CHARSET, bool gdiVerticalFont = false)
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
	/// <returns>The em-size, in points, of this <see cref='Font'/>.</returns>
	[Browsable(false)]
	public float SizeInPoints => Size * Graphics.GetFactor(Graphics.DPI.Y, Unit, GraphicsUnit.Point);
	
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
	/// Returns the line spacing, in pixels, of this <see cref='Font'/>.
	/// </summary>
	public float GetHeight()
	{
		using var graphics = new Graphics(new SKBitmap(1, 1)) { PageUnit = Unit };
		return GetHeight(graphics);
	}

	/// <summary>
	/// Returns the line spacing, in the current unit of a specified Graphics, of this <see cref='Font'/>.
	/// </summary>
	public float GetHeight(Graphics graphics)
		=> GetHeight(graphics.PageUnit, graphics.DpiY);

	/// <summary>
	/// Returns the height, in pixels, of this <see cref='Font'/> when drawn to a device with the specified vertical resolution.
	/// </summary>
	public float GetHeight(float dpi)
		=> GetHeight(Unit, dpi);

	/// <summary>
	/// Returns the height, in pixels, of this <see cref='Font'/> when drawn to a device with the specified vertical resolution
	/// and with the specified <see cref='GraphicsUnit'>.
	/// </summary>
	private float GetHeight(GraphicsUnit unit, float dpi)
		=> (Metrics.Descent - Metrics.Ascent + Metrics.Leading) * Graphics.GetFactor(dpi, unit, GraphicsUnit.Pixel);

	/// <summary>
	/// Creates a <see cref="Font"/> from the specified handle to a device context (HDC).
	/// </summary>
	/// <returns>The newly created <see cref="Font"/>.</returns>
	public static Font FromHdc(IntPtr hdc)
		=> throw new NotSupportedException("unsupported by skia.");

	/// <summary>
	/// Creates a <see cref='Font'/> from the specified Windows handle.
	/// </summary>
	/// <returns>The newly created <see cref="Font"/>.</returns>
	public static Font FromHfont(IntPtr hfont)
		=> throw new NotSupportedException("unsupported by skia.");

	/// <inheritdoc cref="FromLogFont(object)"/>
	public static Font FromLogFont(in Interop.LOGFONT logFont)
		=> FromLogFont(logFont, IntPtr.Zero);

	/// <inheritdoc cref="FromLogFont(object, IntPtr)"/>
	public static Font FromLogFont(in Interop.LOGFONT logFont, IntPtr hdc)
		=> FromLogFont(logFont as object, hdc);

	/// <summary>
	/// Creates a <see cref="Font"/> from the given <see cref="Interop.LOGFONT"/> using the screen device context.
	/// </summary>
	/// <param name="logFont">A boxed <see cref="Interop.LOGFONT"/>.</param>
	/// <returns>The newly created <see cref="Font"/>.</returns>
	public static Font FromLogFont(object logFont)
		=> FromLogFont(logFont, IntPtr.Zero);

	/// <summary>
	/// Creates a <see cref="Font"/> from the given <see cref="Interop.LOGFONT"/> using the given device context.
	/// </summary>
	/// <param name="logFont">A boxed <see cref="Interop.LOGFONT"/>.</param>
	/// <param name="hdc">Handle to a device context (HDC).</param>
	/// <returns>The newly created <see cref="Font"/>.</returns>
	public static Font FromLogFont(object logFont, IntPtr hdc)
		=> throw new NotSupportedException("unsupported by skia.");

	/// <summary>
	/// Returns a handle to this <see cref='Font'/>.
	/// </summary>
	public IntPtr ToHfont()
		=> throw new NotSupportedException("unsupported by skia.");

	/// <inheritdoc cref="ToLogFont(object)"/>
	public void ToLogFont(out Interop.LOGFONT logFont)
		=> ToLogFont(out logFont, null);

	/// <inheritdoc cref="ToLogFont(object, object)"/>
	public void ToLogFont(out Interop.LOGFONT logFont, Graphics graphics)
		=> ToLogFont(logFont = new Interop.LOGFONT(), graphics);

	/// <summary>
	/// Creates a GDI logical font (<see cref="Interop.LOGFONT"/>) structure from this <see cref="Font"/>.
	/// </summary>
	/// <param name="logFont">An <see cref="Object"/> to represent the <see cref="Interop.LOGFONT"/> structure that this method creates.</param>
	public void ToLogFont(object logFont)
		=> ToLogFont(logFont, null);

	/// <summary>
	/// Creates a GDI logical font (<see cref="Interop.LOGFONT"/>) structure from this <see cref="Font"/> and Graphics.
	/// </summary>
	/// <param name="logFont">An <see cref="Object"/> to represent the <see cref="Interop.LOGFONT"/> structure that this method creates.</param>
	/// <param name="graphics">A Graphics that provides additional information for the <see cref="Interop.LOGFONT"/> structure.</param>
	public void ToLogFont(object logFont, Graphics graphics)
		=> throw new NotSupportedException("unsupported by skia.");

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

	private SKTypeface Typeface => FontFamily.GetTypeface(Style);

	private SKFontMetrics Metrics => Typeface.ToFont(Size).Metrics;

	#endregion
}
