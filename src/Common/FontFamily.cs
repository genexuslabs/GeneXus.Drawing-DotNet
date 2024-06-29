using GeneXus.Drawing.Text;
using System;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace GeneXus.Drawing;

public class FontFamily : IDisposable
{
	private readonly string m_systemFamilyName; // To look in the default fonts
	private readonly SKTypeface[] m_typefaces; // This were loaded from a file
	private readonly bool m_typefacesOwner; // To know if we need to dispose the typefaces

	private FontFamily(SKTypeface[] typefaces)
	{
		if (typefaces.Length == 0)
			throw new ArgumentException("At least 1 typeface is required", nameof(typefaces));

		m_typefaces = typefaces;
		m_typefacesOwner = true; // the typefaces will be disposed with this object
	}

	private FontFamily(FontFamily family)
	{
		m_systemFamilyName = family.m_systemFamilyName;
		m_typefaces = family.m_typefaces;
	}
	
	private FontFamily(FontFamily[] families)
	{
		if (families.Length == 0)
			throw new ArgumentException("At least 1 base family is required", nameof(families));

		List<SKTypeface> typefaces = new();
		foreach (FontFamily family in families)
		{
			m_systemFamilyName ??= family.m_systemFamilyName; // assuming all have the same name
			if (family.m_typefaces != null)
				typefaces.AddRange(family.m_typefaces);
		}

		if (typefaces.Count > 0)
			m_typefaces = typefaces.ToArray();
	}

	public static FontFamily FromFile(string filePath)
	{
		SKData data = SKData.Create(filePath) ?? throw new FileNotFoundException();
		return FromData(data);
	}

	public static FontFamily FromStream(Stream stream)
	{
		SKData data = SKData.Create(stream);
		return FromData(data);
	}
	
	private static FontFamily FromData(SKData data)
	{
		int index = 0;
		List<SKTypeface> list = new();
		while (true)
		{
			SKTypeface typeface = SKFontManager.Default.CreateTypeface(data, index++);
			if (typeface == null)
				break;
			list.Add(typeface);
		}

		if (list.Count == 0)
			throw new ArgumentException("No font found");
		
		return new FontFamily(list.ToArray());
	}
	
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
	/// Initializes a new <see cref='FontFamily'/> with the specified name.
	/// </summary>
	/// <param name="name">The name of the new <see cref='FontFamily'/>.</param>
	/// <exception cref='ArgumentException'><see cref='name'/> is an empty string or the font is not installed.</exception>
	public FontFamily(string name)
	{
		m_systemFamilyName = name;
	}
	
	/// <summary>
	/// Initializes a new <see cref='FontFamily'/> in the specified <see cref='FontCollection'/> with the specified name.
	/// </summary>
	/// <param name="name">The name of the new <see cref='FontFamily'/>.</param>
	/// <param name="collection">The <see cref='FontCollection'/> that contains this <see cref='FontFamily'/>.</param>
	/// <exception cref='ArgumentException'><see cref='name'/> is an empty string or the font is not in the collection.</exception>
	public FontFamily(string name, FontCollection collection)
		: this(Match(name, collection.Families) ?? throw new ArgumentException("missing family from collection", nameof(name))) { }

	private static FontFamily[] Match(string name, FontFamily[] families)
	{
		FontFamily[] matched = families.Where(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
		return matched.Length == 0 ? null : matched;
	}
	
	internal static FontFamily Match(string name)
	{
		FontFamily[] matched = Match(name, Families);
		if (matched == null)
			return GenericSansSerif;
		
		if (matched.Length == 1)
			return matched[0];
		
		return new FontFamily(matched);
	}

	#region IEqualitable

	/// <summary>
	/// Indicates whether the specified object is a <see cref='FontFamily'/> and is identical to this <see cref='FontFamily'/>.
	/// </summary>
	public override bool Equals(object obj)
		=> obj is FontFamily ff
		&& ff.m_typeface.FamilyName.Equals(m_typeface.FamilyName, StringComparison.OrdinalIgnoreCase)
		&& ff.m_index == m_index;

	/// <summary>
	/// Gets a hash code for this <see cref='FontFamily'/>.
	/// </summary>
	public override int GetHashCode() => Name.GetHashCode();

	#endregion


	#region IDisposable

	/// <summary>
	/// Cleans up resources for this <see cref='FontFamily'/>.
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
