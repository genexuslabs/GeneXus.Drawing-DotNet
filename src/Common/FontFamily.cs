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
			  ?? throw new ArgumentException($"missing family from collection", nameof(name)), 0)
	{ }

	/// <summary>
	/// Initializes a new <see cref='FontFamily'/> from the specified generic font family.
	/// </summary>
	/// <param name="genericFamily">
	/// The <see cref='GenericFontFamilies'/> from which to create the new <see cref='FontFamily'/>.
	/// </param>
	public FontFamily(GenericFontFamilies genericFamily)
		: this(GetGenericFontFamily(genericFamily)) { }

	private static FontFamily GetGenericFontFamily(GenericFontFamilies genericFamily)
	{
		string[] candidates = genericFamily switch // NOTE: Define a set of predefined fonts
		{
			GenericFontFamilies.Monospace => new[] { "Courier New", "Consolas", "Courier", "Menlo", "Monaco", "Lucida Console" },
			GenericFontFamilies.SansSerif => new[] { "Arial", "Helvetica", "Verdana", "Tahoma", "Trebuchet MS", "Gill Sans" },
			GenericFontFamilies.Serif => new[] { "Times New Roman", "Georgia", "Garamond", "Palatino", "Book Antiqua", "Baskerville" },
			_ => throw new ArgumentException($"invalid generic font value {genericFamily}", nameof(genericFamily))
		};
		foreach (string candidate in candidates)
		{
			FontFamily family = Families.FirstOrDefault(f => f.Name.Equals(candidate, StringComparison.OrdinalIgnoreCase));
			if (family != null)
				return family;
		}
		throw new ArgumentException("Generic font family is not installed", nameof(genericFamily));
	}

	/// <summary>
	///  Cleans up resources for this <see cref='FontFamily'/>.
	/// </summary>
	~FontFamily() => Dispose(false);

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
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	protected virtual void Dispose(bool disposing)
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

	/// <summary>
	///  Returns an array that contains all of the <see cref='FontFamily'/> objects associated with the current
	///  graphics context.
	/// </summary>
	public static FontFamily[] Families => new InstalledFontCollection().Families;

	/// <summary>
	///  Gets a generic monospace <see cref='FontFamily'/>.
	/// </summary>
	public static FontFamily GenericMonospace => new(GenericFontFamilies.Monospace);

	/// <summary>
	///  Gets a generic SansSerif <see cref='FontFamily'/>.
	/// </summary>
	public static FontFamily GenericSansSerif => new(GenericFontFamilies.SansSerif);

	/// <summary>
	///  Gets a generic Serif <see cref='FontFamily'/>.
	/// </summary>
	public static FontFamily GenericSerif => new(GenericFontFamilies.Serif);

	#endregion


	#region Methods

	internal SKFont m_font => GetFont(10);
	internal SKFont GetFont(float size) => m_typeface.ToFont(size);

	/// <summary>
	/// Returns the cell ascent of this <see cref='FontFamily'/>.
	/// </summary>
	public int GetCellAscent() => (int)Math.Abs(m_font.Metrics.Ascent * GetEmHeight() / m_font.Size);

	/// <summary>
	/// Returns the cell descent of this <see cref='FontFamily'/>.
	/// </summary>
	public int GetCellDescent() => (int)Math.Abs(m_font.Metrics.Descent * GetEmHeight() / m_font.Size);

	/// <summary>
	/// Gets the height, of the em square of this <see cref='FontFamily'/>.
	/// </summary>
	public int GetEmHeight() => m_typeface.UnitsPerEm;

	/// Returns the distance between two consecutive lines of text for this <see cref='FontFamily'/> with the
	/// specified <see cref='FontStyle'/>.
	/// </summary>
	public int GetLineSpacing() => (int)Math.Abs(m_font.Spacing * GetEmHeight() / m_font.Size);

	/// <summary>
	///  Returns the name of this <see cref='FontFamily'/> in the specified language.
	/// </summary>
	public string GetName(int language) => Name; // NOTE: Language is not suppored in SkiaSharp

	/// <summary>
	///  Indicates whether the specified <see cref='FontStyle'/> is available.
	/// </summary>
	public bool IsStyleAvailable(FontStyle style) => (new Font(this).Style & style) == style;

	#endregion


	internal bool MatchFamily(string familyName) // TODO: Improve this code
		=> new string[] { Name, $"{Name} {Face}", $"{Name}-{Face}" }.Any(candidateName
			=> candidateName.Equals(familyName, StringComparison.OrdinalIgnoreCase));

	#endregion
}
