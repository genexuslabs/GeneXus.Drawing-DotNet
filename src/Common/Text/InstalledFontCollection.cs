using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneXus.Drawing.Text;

public class InstalledFontCollection : FontCollection
{
	private static readonly string SYSTEM_FONT_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
	private static readonly string[] FONT_EXTENSIONS = { ".ttf", ".otf", ".eot", ".woff", ".woff2" };
	
	public static readonly FontCollection Instance = new InstalledFontCollection();
	
	/// <summary>
	///  Initializes a new instance of the <see cref='InstalledFontCollection'/> class.
	/// </summary>
	private InstalledFontCollection()
	{
		string fontPath = SYSTEM_FONT_PATH; // this may be empty in some linux
		if (!string.IsNullOrEmpty(fontPath))
			m_families.AddRange(GetFontFamilies(SYSTEM_FONT_PATH));
		m_families.AddRange(GetDefaultFontFamilies());
	}
	
	/// <summary>
	/// Returns a <see cref='FontFamily'/> collection in the specified location.
	/// </summary>
	public static IEnumerable<FontFamily> GetFontFamilies(string location)
	{
		return (from fontFile in Directory.EnumerateFiles(location)
			where FONT_EXTENSIONS.Contains(Path.GetExtension(fontFile))
			select FontFamily.FromFile(fontFile)).ToList();
	}

	private static IEnumerable<FontFamily> GetDefaultFontFamilies()
	{
		return SKFontManager.Default.FontFamilies.Select(name => new FontFamily(name));
	}
}
