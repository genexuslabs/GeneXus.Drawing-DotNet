using GeneXus.Drawing.Text;
using System;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace GeneXus.Drawing;

public abstract class FontFamily : IDisposable, ICloneable
{
	/// <summary>
	/// Creates a human-readable string that represents this <see cref='FontFamily'/>.
	/// </summary>
	public override string ToString() => $"[{GetType().Name}: Name={Name}]";
	
	public virtual void Dispose() { }
	
	#region Properties

	/// <summary>
	/// Gets the name of this <see cref='FontFamily'/>.
	/// </summary>
	public abstract string Name { get; }

	/// <summary>
	/// Gets the Face associated with this <see cref='FontFamily'/>.
	/// </summary>
	public abstract string Face { get; }

	/// <summary>
	/// Gets a value that indicates whether this <see cref='FontFamily'/> has the italic style applied.
	/// </summary>
	public abstract bool IsItalic { get; }

	/// <summary>
	/// Gets a value that indicates whether this <see cref='FontFamily'/> is bold.
	/// </summary>
	public abstract bool IsBold { get; }
	
	/// <summary>
	/// Gets the weight of this <see cref='FontFamily'/>.
	/// </summary>
	public abstract int Weight { get; }
	
	/// <summary>
	/// Gets the width of this <see cref='FontFamily'/>.
	/// </summary>
	public abstract int Width { get; }
	
	/// <summary>
	/// Gets the slant of this <see cref='Font'/>.
	/// </summary>
	public abstract SlantType Slant { get; }
	
	#endregion
	
	#region Methods

	/// <summary>
	/// Returns the cell ascent of this <see cref='FontFamily'/>.
	/// </summary>
	public abstract int GetCellAscent();

	/// <summary>
	/// Returns the cell descent of this <see cref='FontFamily'/>.
	/// </summary>
	public abstract int GetCellDescent();

	/// <summary>
	/// Gets the height, of the em square of this <see cref='FontFamily'/>.
	/// </summary>
	public abstract int GetEmHeight();

	///<summary>
	/// Returns the distance between two consecutive lines of text for this
	/// <see cref='FontFamily'/> with the specified <see cref='FontFamily'/>.
	/// </summary>
	public abstract int GetLineSpacing();

	/// <summary>
	/// Returns the name of this <see cref='FontFamily'/> in the specified language.
	/// </summary>
	public abstract string GetName(int language);

	/// <summary>
	/// Indicates whether the specified <see cref='FontFamily'/> is available.
	/// </summary>
	public abstract bool IsStyleAvailable(FontStyle style);

	/// <summary>
	/// Creates an exact copy of this <see cref='FontFamily'/>.
	/// </summary>
	public abstract object Clone();
	
	#endregion
	
	#region Generic Font Families

	/// <summary>
	/// Returns an array that contains all of the <see cref='FontFamily'/> objects associated with the current
	/// graphics context.
	/// </summary>
	public static FontFamily[] Families => new InstalledFontCollection().Families;

	/// <summary>
	///  Gets a generic monospace <see cref='FontFamily'/>.
	/// </summary>
	public static FontFamily GenericMonospace => new SkiaFontFamily(GenericFontFamilies.Monospace);

	/// <summary>
	///  Gets a generic SansSerif <see cref='FontFamily'/>.
	/// </summary>
	public static FontFamily GenericSansSerif => new SkiaFontFamily(GenericFontFamilies.SansSerif);

	/// <summary>
	///  Gets a generic Serif <see cref='FontFamily'/>.
	/// </summary>
	public static FontFamily GenericSerif => new SkiaFontFamily(GenericFontFamilies.Serif);

	#endregion
}

public static class FontFamilyFactory
{
	/// <summary>
	/// Initializes a new instance of the <see cref='FontFamily'/> class from the specified filename name
	/// and optional index for font collection.
	/// </summary>
	public static FontFamily Create(string fontPath, int index = 0) => new SkiaFontFamily(fontPath, index);
	
	/// <summary>
	/// Initializes a new instance of the <see cref='FontFamily'/> class from the specified stream name
	/// and optional index for font collection.
	/// </summary>
	public static FontFamily Create(Stream stream, int index = 0) => new SkiaFontFamily(stream, index);
	
	/// <summary>
	/// Initializes a new instance of the <see cref='FontFamily'/> class in the specified
	/// <see cref='FontCollection'/> and with the specified name.
	/// </summary>
	public static FontFamily Create(string name, FontCollection fontCollection) => new SkiaFontFamily(name, fontCollection);

	/// <summary>
	/// Initializes a new instance of the <see cref='SkiaFontFamily'/> class from the specified generic font family.
	/// </summary>
	public static FontFamily Create(GenericFontFamilies genericFamily) => new SkiaFontFamily(genericFamily);
}

internal sealed class SkiaFontFamily : FontFamily
{
	internal readonly int m_index;
	private readonly SKData m_data;
	private readonly SKTypeface m_typeface;

	private SkiaFontFamily(SKData skData, int index)
	{
		m_index = index;
		m_data = skData;
		m_typeface = SKTypeface.FromData(m_data, m_index);
		if (m_typeface == null) throw new ArgumentException("file does not exist or is an invalid font file");
	}

	public SkiaFontFamily(string fontPath, int index = 0)
		: this(File.Exists(fontPath) ? SKData.Create(fontPath) : throw new FileNotFoundException(fontPath), index) { }

	public SkiaFontFamily(Stream stream, int index = 0)
		: this(SKData.Create(stream), index) { }

	public SkiaFontFamily(string name, FontCollection fontCollection)
		: this((fontCollection.Families.FirstOrDefault(ff => ff is SkiaFontFamily sff && sff.MatchFamily(name)) as SkiaFontFamily)?.m_data
			  ?? throw new ArgumentException($"missing family from collection", nameof(name)), 0)
	{ }

	public SkiaFontFamily(GenericFontFamilies genericFamily)
		: this(GetGenericFontFamily(genericFamily).m_data, 0) { }

	/// <summary>
	/// Cleans up resources for this <see cref='SkiaFontFamily'/>.
	/// </summary>
	~SkiaFontFamily() => Dispose(false);
	

	#region IEqualitable

	/// <summary>
	/// Indicates whether the specified object is a <see cref='FontFamily'/> and is identical to this <see cref='FontFamily'/>.
	/// </summary>
	public override bool Equals(object obj)
		=> obj is SkiaFontFamily ff
		&& ff.m_typeface.FamilyName.Equals(m_typeface.FamilyName, StringComparison.OrdinalIgnoreCase)
		&& ff.m_index == m_index;

	/// <summary>
	/// Gets a hash code for this <see cref='SkiaFontFamily'/>.
	/// </summary>
	public override int GetHashCode() => Name.GetHashCode();

	#endregion


	#region IDisposable

	/// <summary>
	/// Cleans up resources for this <see cref='SkiaFontFamily'/>.
	/// </summary>
	public override void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing)
	{
		m_typeface.Dispose();
		m_data.Dispose();
	}

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKTypeface'/> with the coordinates of the specified <see cref='SkiaFontFamily'/> .
	/// </summary>
	public static explicit operator SKTypeface(SkiaFontFamily fontFamily) => fontFamily.m_typeface;

	#endregion


	#region Properties

	/// <summary>
	/// Gets the name of this <see cref='FontFamily'/>.
	/// </summary>
	public override string Name => m_typeface.FamilyName;

	#endregion


	#region Methods

	private SKFont m_font => GetFont(10);
	internal SKFont GetFont(float size) => m_typeface.ToFont(size);

	public override int GetCellAscent() => (int)Math.Abs(m_font.Metrics.Ascent * GetEmHeight() / m_font.Size);
	public override int GetCellDescent() => (int)Math.Abs(m_font.Metrics.Descent * GetEmHeight() / m_font.Size);
	public override int GetEmHeight() => m_typeface.UnitsPerEm;
	public override int GetLineSpacing() => (int)Math.Abs(m_font.Spacing * GetEmHeight() / m_font.Size);
	public override string GetName(int language) => Name; // NOTE: Language is not supported in SkiaSharp
	public override bool IsStyleAvailable(FontStyle style) => (new Font(this).Style & style) == style;
	public override object Clone() => new SkiaFontFamily(SKData.CreateCopy(m_data.ToArray()), m_index);

	#endregion


	#region Utilities

	public override bool IsItalic => m_typeface.IsItalic;
	public override bool IsBold => m_typeface.IsBold;
	
	public override int Weight => m_typeface.FontWeight;
	private string WeightName => Weight switch
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

	public override int Width => m_typeface.FontWidth;
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

	public override SlantType Slant => m_typeface.FontSlant switch
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

	public override string Face
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
	
	private static SkiaFontFamily GetGenericFontFamily(GenericFontFamilies genericFamily)
	{
		var candidates = genericFamily switch // NOTE: Define a set of predefined fonts
		{
			GenericFontFamilies.Monospace => new[] { "Courier New", "Consolas", "Courier", "Menlo", "Monaco", "Lucida Console" },
			GenericFontFamilies.SansSerif => new[] { "Arial", "Helvetica", "Verdana", "Tahoma", "Trebuchet MS", "Gill Sans" },
			GenericFontFamilies.Serif => new[] { "Times New Roman", "Georgia", "Garamond", "Palatino", "Book Antiqua", "Baskerville" },
			_ => throw new ArgumentException($"invalid generic font value {genericFamily}", nameof(genericFamily))
		};
		foreach (string candidate in candidates)
			if (Font.SystemFonts.FirstOrDefault(f => f.FamilyName.Equals(candidate, StringComparison.OrdinalIgnoreCase)) is Font font)
				return font.FontFamily as SkiaFontFamily;
		throw new ArgumentException($"invalid generic font family", nameof(genericFamily));
	}

	internal bool MatchFamily(string familyName) // TODO: Improve this code
		=> new[] { Name, $"{Name} {Face}", $"{Name}-{Face}" }.Any(candidateName
			=> candidateName.Equals(familyName, StringComparison.OrdinalIgnoreCase));

	#endregion
}

internal sealed class UnknownFontFamily : FontFamily
{
	public UnknownFontFamily(string name) { Name = name; }
	public override string Name { get; }
	public override string Face => string.Empty;
	public override bool IsItalic => false;
	public override bool IsBold => false;
	public override int Weight => 0;
	public override int Width => 0;
	public override SlantType Slant => SlantType.Normal;
	public override int GetCellAscent() => 0;
	public override int GetCellDescent() => 0;
	public override int GetEmHeight() => 0;
	public override int GetLineSpacing() => 0;
	public override string GetName(int language) => Name;
	public override bool IsStyleAvailable(FontStyle style) => false;
	public override object Clone() => new UnknownFontFamily(Name);
}
