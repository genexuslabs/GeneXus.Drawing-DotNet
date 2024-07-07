using SkiaSharp;
using System.Collections.Generic;

namespace GeneXus.Drawing.Text;

public sealed class InstalledFontCollection : FontCollection
{
	/// <summary>
	/// Initializes a new instance of the <see cref='InstalledFontCollection'/> class.
	/// </summary>
	public InstalledFontCollection() 
	{
		foreach (string name in SKFontManager.Default.FontFamilies)
		{
			List<SKTypeface> typefaces = new();
			foreach (SKFontStyle style in SKFontManager.Default.GetFontStyles(name))
			{
				var typeface = SKFontManager.Default.MatchFamily(name, style);
				typefaces.Add(typeface);
			}

			if (typefaces.Count == 0)
				continue;

			FontFamily fontFamily = new(name, typefaces.ToArray(), true);
			m_families.Add(fontFamily);
		}
	}
}
