﻿using SkiaSharp;
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
	/// Initializes a new instance of the <see cref='InstalledFontCollection'/> class.
	/// </summary>
	public InstalledFontCollection()
	{
		string fontPath = SYSTEM_FONT_PATH; // this may be empty in some linux
		if (!string.IsNullOrEmpty(fontPath))
		{
			IEnumerable<FontFamily> systemFontFamilies = Directory.EnumerateFiles(SYSTEM_FONT_PATH)
				.Where(fontFile => FONT_EXTENSIONS.Contains(Path.GetExtension(fontFile)))
				.Select(FontFamily.FromFile).ToList();
			m_families.AddRange(systemFontFamilies);
		}

		IEnumerable<FontFamily> defaultFontFamilies = SKFontManager.Default.FontFamilies.Select(name => new FontFamily(name));
		m_families.AddRange(defaultFontFamilies);
	}
}
