using GeneXus.Drawing.Text;
using System;
using System.IO;
using System.Linq;
using SkiaSharp;
using System.Collections.Generic;

namespace GeneXus.Drawing;

public sealed class FontFamily : ICloneable, IDisposable
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
			GenericFontFamilies.Monospace => new[] { "Courier New", "Consolas", "Courier", "Menlo", "Monaco", "Lucida Console", "DejaVu Sans Mono" },
			GenericFontFamilies.SansSerif => new[] { "Arial", "Helvetica", "Verdana", "Tahoma", "Trebuchet MS", "Gill Sans", "DejaVu Sans" },
			GenericFontFamilies.Serif => new[] { "Times New Roman", "Georgia", "Garamond", "Palatino", "Book Antiqua", "Baskerville", "DejaVu Serif" },
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
		if (string.IsNullOrEmpty(name))
			throw new ArgumentException("Name can't be empty", nameof(name));
		
		using SKTypeface typeface = SKFontManager.Default.MatchFamily(name);
		if (!typeface.FamilyName.Equals(name)) // MatchFamily() always returns a typeface even when the requested family name is not found
			throw new ArgumentException("Font is not installed", nameof(name));
		
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

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='FontFamily'/>.
	/// </summary>
	public override string ToString() => $"[{GetType().Name}: Name={Name}]";

	/// <summary>
	/// Cleans up resources for this <see cref='FontFamily'/>.
	/// </summary>
	~FontFamily() => Dispose(false);


	#region IEqualitable

	/// <summary>
	/// Indicates whether the specified object is a <see cref='FontFamily'/> and is identical to this <see cref='FontFamily'/>.
	/// </summary>
	public override bool Equals(object obj)
		=> obj is FontFamily f && f.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);

	/// <summary>
	/// Gets a hash code for this <see cref='FontFamily'/>.
	/// </summary>
	public override int GetHashCode() => Name.GetHashCode();

	#endregion


	#region IClonable

	/// <summary>
	/// Creates an exact copy of this <see cref='FontFamily'/>.
	/// </summary>
	public object Clone() => new FontFamily(this);

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

	private void Dispose(bool disposing)
	{
		if (!m_typefacesOwner)
			return;

		foreach (SKTypeface typeface in m_typefaces)
			typeface.Dispose();
	}

	#endregion
	

	#region Properties

	/// <summary>
	/// Gets the name of this <see cref='FontFamily'/>.
	/// </summary>
	public string Name => m_systemFamilyName ?? m_typefaces[0].FamilyName; // assume all typefaces have the same name
	
	#endregion
	
	
	#region Factory

	internal static FontFamily FromFile(string filePath)
	{
		SKData data = SKData.Create(filePath) ?? throw new FileNotFoundException();
		return FromData(data);
	}

	internal static FontFamily FromStream(Stream stream)
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
			throw new ArgumentException("No typefaces were found.", nameof(data));
		
		return new FontFamily(list.ToArray());
	}
	
	#endregion

	
	#region Methods

	internal SKTypeface GetTypeface(FontStyle style)
	{
		bool isBold = (style & FontStyle.Bold) != 0;
		bool isItalic = (style & FontStyle.Italic) != 0;

		SKTypeface typeface = m_typefaces?.FirstOrDefault(t => t.IsBold == isBold && t.IsItalic == isItalic);
		if (typeface != null)
			return typeface;

		if (m_systemFamilyName != null)
		{
			SKFontStyle skFontStyle = isBold ?
				isItalic ? SKFontStyle.BoldItalic : SKFontStyle.Bold :
				isItalic ? SKFontStyle.Italic : SKFontStyle.Normal;
			return SKFontManager.Default.MatchFamily(m_systemFamilyName, skFontStyle);
		}

		if (m_typefaces == null)
			throw new InvalidOperationException("_systemFamilyName and _typefaces can't be both null");

		return m_typefaces[0];
	}
	
	internal int GetTypefaceIndex(FontStyle style)
	{
		return m_typefaces == null ? 0 : Array.IndexOf(m_typefaces, GetTypeface(style));
	}

	/// <summary>
	/// Returns the cell ascent, in design units, of the <see cref='FontFamily'/> of the specified style.
	/// </summary>
	/// <param name="style">A <see cref='FontStyle'/> that contains style information for the font.</param>
	/// <returns>The cell ascent for this <see cref='FontFamily'/> that uses the specified <see cref='FontStyle'/>.</returns>
	public int GetCellAscent(FontStyle style)
	{
		SKTypeface typeface = GetTypeface(style);
		SKFont font = typeface.ToFont(10);
		return (int)Math.Round(Math.Abs(font.Metrics.Ascent * typeface.UnitsPerEm / font.Size));
	}

	/// <summary>
	/// Returns the cell descent, in design units, of the <see cref='FontFamily'/> of the specified style.
	/// </summary>
	/// <param name="style">A <see cref='FontStyle'/> that contains style information for the font.</param>
	/// <returns>The cell descent for this <see cref='FontFamily'/> that uses the specified <see cref='FontStyle'/>.</returns>
	public int GetCellDescent(FontStyle style)
	{
		SKTypeface typeface = GetTypeface(style);
		SKFont font = typeface.ToFont(10);
		return (int)Math.Round(Math.Abs(font.Metrics.Descent * typeface.UnitsPerEm / font.Size));
	}

	/// <summary>
	/// Gets the height, in font design units, of the em square for the specified style.
	/// </summary>
	/// <param name="style">A <see cref='FontStyle'/> for which to get the em height.</param>
	/// <returns>The height of the em square.</returns>
	public int GetEmHeight(FontStyle style) => GetTypeface(style).UnitsPerEm;

	/// <summary>
	/// Returns the line spacing, in design units, of the <see cref='FontFamily'/> of the specified style.
	/// The line spacing is the vertical distance between the base lines of two consecutive lines of text.
	/// </summary>
	/// <param name="style">The <see cref='FontStyle'/> to apply.</param>
	/// <returns>The distance between two consecutive lines of text.</returns>
	public int GetLineSpacing(FontStyle style)
	{
		SKTypeface typeface = GetTypeface(style);
		SKFont font = typeface.ToFont(10);
		return (int)Math.Round(Math.Abs(font.Spacing * typeface.UnitsPerEm / font.Size));
	}

	/// <summary>
	/// Returns the name of this <see cref='FontFamily'/> in the specified language.
	/// </summary>
	/// <param name="language">The language in which the name is returned.</param>
	/// <returns>A <see cref='String'/> that represents the name, in the specified language, of this <see cref='FontFamily'/>.</returns>
	public string GetName(int language) => Name; // NOTE: Language is not supported in SkiaSharp

	/// <summary>
	/// Indicates whether the specified <see cref='FontFamily'/> is available.
	/// </summary>
	/// <param name="style">The <see cref='FontStyle'/> to test.</param>
	public bool IsStyleAvailable(FontStyle style)
	{
		SKTypeface typeface = GetTypeface(style);
		if (m_systemFamilyName != null && typeface.FamilyName.Equals(m_systemFamilyName))
			return false; // font not installed
		
		bool isBold = (style & FontStyle.Bold) != 0;
		bool isItalic = (style & FontStyle.Italic) != 0;
		return typeface.IsBold == isBold && typeface.IsItalic == isItalic;
	}
	
	#endregion
	

	#region Class Properties

	/// <summary>
	/// Returns an array that contains all of the <see cref='FontFamily'/> objects of the system
	/// </summary>
	public static FontFamily[] Families => InstalledFontCollection.Instance.Families;

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
}
