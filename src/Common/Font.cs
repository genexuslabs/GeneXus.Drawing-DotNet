using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public class Font : IDisposable, ICloneable
{
	private readonly FontFamily m_family;
	private readonly string m_original;
	private readonly float m_size;
	private readonly FontStyle m_style;
	private readonly GraphicsUnit m_unit;

	/// <summary>
	/// Initializes a new System.Drawing.Font that uses the specified existing <see cref='Font'/>
	/// and <see cref='FontStyle'/> enumeration.
	/// </summary>
	/// <param name="prototype">The existing <see cref='Font'/> from which to create the new <see cref='Font'/></param>
	/// <param name="newStyle">
	/// The <see cref='FontStyle'/> to apply to the new S<see cref='Font'/>. Multiple
	/// values of the <see cref='FontStyle'/> enumeration can be combined with the OR operator.
	/// </param>
	public Font(Font prototype, FontStyle newStyle)
	{
		m_family = prototype.FontFamily;
		m_original = prototype.m_original;
		m_size = prototype.m_size;
		m_unit = prototype.m_unit;
		m_style = newStyle;
	}

	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified <see cref='Drawing.FontFamily'/> and size.
	/// </summary>
	/// <param name="family">The <see cref='FontFamily'/> of the new <see cref='Font'/>.</param>
	/// <param name="size">The size of the new font in the units specified by the <paramref name="unit"/> parameter.</param>
	/// <param name="style">The <see cref='FontStyle'/> of the new font.</param>
	/// <param name="unit">The <see cref='GraphicsUnit'/> of the new font.</param>
	public Font(FontFamily family, float size = 12, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Point)
	{
		m_family = family ?? throw new ArgumentException("missing family");
		m_size = size;
		m_style = style;
		m_unit = unit;
	}

	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified family name and size.
	/// </summary>
	/// <param name="familyName">A string representation of the <see cref='FontFamily'/> of the new <see cref='Font'/>.</param>
	/// <param name="size">The size of the new font in the units specified by the <paramref name="unit"/> parameter.</param>
	/// <param name="style">The <see cref='FontStyle'/> of the new font.</param>
	/// <param name="unit">The <see cref='GraphicsUnit'/> of the new font.</param>
	public Font(string familyName, float size = 12, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Point)
		: this(FontFamily.Match(familyName), size, style, unit)
	{
		m_original = familyName;
	}
	
	/// <summary>
	///  Cleans up resources for this <see cref='Font'/>.
	/// </summary>
	~Font() => Dispose(false);

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Font'/>.
	/// </summary>
	public override string ToString()
	{
		return $"[{GetType().Name}: Name={Name}, Size={Size}, Style={Style}, Unit={Unit}]";
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

	protected virtual void Dispose(bool disposing) => m_family.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	/// Creates an exact copy of this <see cref='Font'/>.
	/// </summary>
	public object Clone()
	{
		if (m_original != null)
			return new Font(m_original, m_size, m_style, m_unit);
		else
			return new Font((FontFamily)m_family.Clone(), m_size, m_style, m_unit);
	}

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKFont'/> with the coordinates of the specified <see cref='Font'/> .
	/// </summary>
	public static explicit operator SKFont(Font font) => font.Typeface.ToFont(font.m_size);

	private SKTypeface Typeface => m_family.GetTypeface(m_style);

	#endregion


	#region Properties

	/// <summary>
	/// Gets the face name of this <see cref='Font'/>.
	/// </summary>
	public string Name => m_original ?? m_family.Name;

	/// <summary>
	/// Gets the name of the <see cref='Font'/> originally specified.
	/// </summary>
	public string OriginalFontName => m_original;

	/// <summary>
	/// Gets the <see cref='Drawing.FontFamily'/> associated with this Font.
	/// </summary>
	public FontFamily FontFamily => m_family;

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
	public float Size => m_size;

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
	public FontStyle Style => m_style;

	/// <summary>
	/// Gets the FamilyName associated with this <see cref='Font'/>.
	/// </summary>
	public string FamilyName => m_family.Name;

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

	private SKFontMetrics Metrics => Typeface.ToFont(m_size).Metrics;
	
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
	/// </summary>
		{
	/// <summary>
	/// Gets the unit of measure for this <see cref='Font'/>.
	/// </summary>
	/// <returns>A <see cref='GraphicsUnit'/> that represents the unit of measure for this <see cref='Font'/>.</returns>
	public GraphicsUnit Unit => m_unit;
	
	#endregion


	#region Methods

	/// <summary>
	/// Returns the line spacing of this <see cref='Font'/>.
	/// </summary>
	public float GetHeight() => Metrics.Descent - Metrics.Ascent + Metrics.Leading;

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
}
