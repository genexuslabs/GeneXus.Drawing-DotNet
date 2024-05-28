using GeneXus.Drawing.Text;
using System;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace GeneXus.Drawing;

public class FontFamily : IDisposable
{
	internal readonly int m_index;
	internal readonly SKData m_data;
	internal readonly SKTypeface m_typeface;

	internal FontFamily(SKData skData, int index)
	{
		m_index = index;
		m_data = skData;
		m_typeface = SKTypeface.FromData(m_data, m_index);
		if (m_typeface == null) throw new ArgumentException("file does not exist or is an invalid font file");
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='FontFamily'/> class from the specified filename name
	/// and optional index for font collection.
	/// </summary>
	public FontFamily(string name, int index = 0)
		: this(File.Exists(name) ? SKData.Create(name) : new Font(name).FontFamily.m_data, index) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='FontFamily'/> class from the specified stream name
	/// and optional index for font collection.
	/// </summary>
	public FontFamily(Stream stream, int index = 0)
		: this(SKData.Create(stream), index) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='FontFamily'/> class in the specified
	///  <see cref='FontCollection'/> and with the specified name.
	/// </summary>
	public FontFamily(string name, FontCollection fontCollection)
		: this(fontCollection.Families.FirstOrDefault(ff => ff.MatchFamily(name))?.m_data
			  ?? throw new ArgumentException($"missing family from collection", nameof(name)), 0) { }

	/// <summary>
	///  Cleans up resources for this <see cref='FontFamily'/>.
	/// </summary>
	~FontFamily() => Dispose();

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='FontFamily'/>.
	/// </summary>
	public override string ToString() => $"[{GetType().Name}: Name={m_typeface.FamilyName}]";


	#region IEqualitable

	/// <summary>
	/// Indicates whether the specified object is a <see cref='FontFamily'/> and is identical to this <see cref='FontFamily'/>.
	/// </summary>
	public override bool Equals(object obj)
		=> obj is FontFamily ff
		&& ff.m_typeface.FamilyName.Equals(m_typeface.FamilyName, StringComparison.OrdinalIgnoreCase)
		&& ff.m_index == m_index;

	/// <summary>
	///  Gets a hash code for this <see cref='FontFamily'/>.
	/// </summary>
	public override int GetHashCode() => Name.GetHashCode();

	#endregion


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='FontFamily'/>.
	/// </summary>
	public void Dispose()
	{
		m_typeface.Dispose();
		m_data.Dispose();
	}

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKTypeface'/> with the coordinates of the specified <see cref='FontFamily'/> .
	/// </summary>
	public static explicit operator SKTypeface(FontFamily fontFamily) => fontFamily.m_typeface;

	#endregion


	#region Properties

	/// <summary>
	/// Gets the name of this <see cref='FontFamily'/>.
	/// </summary>
	public string Name => m_typeface.FamilyName;

	#endregion


	#region Methods

	internal SKFont m_font => GetFont(10);
	internal SKFont GetFont(float size) => m_typeface.ToFont(size);

	/// <summary>
	/// Returns the cell ascent of this <see cref='FontFamily'/>.
	/// </summary>
	public float GetCellAscent() => Math.Abs(m_font.Metrics.Ascent * GetEmHeight() / m_font.Size);

	/// <summary>
	/// Returns the cell descent of this <see cref='FontFamily'/>.
	/// </summary>
	public float GetCellDescent() => Math.Abs(m_font.Metrics.Descent * GetEmHeight() / m_font.Size);

	/// <summary>
	/// Gets the height, of the em square of this <see cref='FontFamily'/>.
	/// </summary>
	public float GetEmHeight() => m_typeface.UnitsPerEm;

	/// Returns the distance between two consecutive lines of text for this <see cref='FontFamily'/> with the
	/// specified <see cref='FontStyle'/>.
	/// </summary>
	public float GetLineSpacing() => Math.Abs(m_font.Spacing * GetEmHeight() / m_font.Size);

	#endregion


	#region Utilities

	internal int Weight => m_typeface.FontWeight;
	private string WeightName => Weight switch
	{
		int n when n < 100 => "Extra Thin",
		int n when n < 275 => "Thin",
		int n when n < 300 => "Extra Light",
		int n when n < 350 => "Light",
		int n when n < 400 => "Semi Light",
		int n when n < 500 => "Normal",
		int n when n < 600 => "Medium",
		int n when n < 700 => "Semi Bold",
		int n when n < 800 => "Bold",
		int n when n < 900 => "Extra Bold",
		int n when n < 950 => "Black",
		int n when n < 999 => "Extra Black",
		_ => string.Empty
	};

	internal int Width => m_typeface.FontWidth;
	private string WidthName => Width switch
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

	internal SlantType Slant => m_typeface.FontSlant switch
	{
		SKFontStyleSlant.Oblique => SlantType.Oblique,
		SKFontStyleSlant.Italic => SlantType.Italic,
		SKFontStyleSlant.Upright => SlantType.Normal,
		_ => throw new Exception("missing slant type")
	};
	private string SlantName => Slant switch
	{
		SlantType.Normal => "Normal",
		SlantType.Italic => "Italic",
		SlantType.Oblique => "Oblique",
		_ => string.Empty
	};

	internal string Face
	{
		get
		{
			// Reference: https://referencesource.microsoft.com/#PresentationCore/Core/CSharp/MS/Internal/FontFace/FontDifferentiator.cs,29
			var faceName = new StringBuilder();
			void AppendStyle(string value)
			{
				if (value.Equals("Normal", StringComparison.OrdinalIgnoreCase)) return;
				faceName.Append(faceName.Length > 0 ? " " : string.Empty).Append(value.Replace(" ", string.Empty));
			}

			AppendStyle(WidthName);
			AppendStyle(WeightName);
			AppendStyle(SlantName);

			return faceName.Append(faceName.Length > 0 ? string.Empty : "Regular").ToString();
		}
	}

	internal bool MatchFamily(string familyName) // TODO: Improve this code
		=> new string[] { Name, $"{Name} {Face}", $"{Name}-{Face}" }.Any(candidateName
			=> candidateName.Equals(familyName, StringComparison.OrdinalIgnoreCase));

	#endregion
}
