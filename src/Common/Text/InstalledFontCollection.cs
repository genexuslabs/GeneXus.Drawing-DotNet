namespace GeneXus.Drawing.Text;

public class InstalledFontCollection : FontCollection
{
	/// <summary>
	///  Initializes a new instance of the <see cref='InstalledFontCollection'/> class.
	/// </summary>
	public InstalledFontCollection() : base()
	{
		foreach (var font in Font.SystemFonts)
			m_families.Add(font.FontFamily);
	}
}
