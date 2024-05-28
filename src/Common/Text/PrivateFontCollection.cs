using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GeneXus.Drawing.Text;

public class PrivateFontCollection : FontCollection
{
	/// <summary>
	///  Initializes a new instance of the <see cref='PrivateFontCollection'/> class.
	/// </summary>
	public PrivateFontCollection() : base()
	{ }

	/// <summary>
	///  Adds a font from the specified file to this <see cref='PrivateFontCollection'/>.
	/// </summary>
	public void AddFontFile(string filename)
	{
		var fontFamily = new FontFamily(filename);
		m_families.Add(fontFamily);
	}

	/// <summary>
	///  Adds a font contained in system memory to this <see cref='PrivateFontCollection'/>.
	/// </summary>
	public void AddMemoryFont(IntPtr memory, int length)
	{
		byte[] fontData = new byte[length];
		Marshal.Copy(memory, fontData, 0, length);

		using var stream = new MemoryStream(fontData);

		var fontFamily = new FontFamily(stream);
		m_families.Add(fontFamily);
	}
}
