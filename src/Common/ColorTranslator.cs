using System;
using System.Globalization;

namespace GeneXus.Drawing;

/// <summary>
/// Translates colors to and from <see cref='Color'/> objects.
/// </summary>
public static class ColorTranslator
{
	// COLORREF is 0x00BBGGRR
	private const int R_SHIFT = 0,  G_SHIFT = 8, B_SHIFT = 16;
	private const int OLE_SYS_COLOR_FLAG = unchecked((int)0x80000000);

	#region Methods

	/// <summary>
	/// Translates the specified <see cref='Color'/> to an Ole color.
	/// </summary>
	public static int ToOle(Color c)
		=> c.IsKnownColor && c.IsSystemColor ? throw new NotImplementedException() : ToWin32(c);

	/// <summary>
	/// Translates the specified <see cref='Color'/> to an Html string color representation.
	/// </summary>
	public static string ToHtml(Color c)
		=> c.IsNamedColor ? c.Name : c.Hex;

	/// <summary>
	/// Translates the specified <see cref='Color'/> to a Win32 color.
	/// </summary>
	public static int ToWin32(Color c)
		=> c.R << R_SHIFT | c.G << G_SHIFT | c.B << B_SHIFT;

	/// <summary>
	/// Translates an Html color representation to a <see cref='Color'/>.
	/// </summary>
	public static Color FromHtml(string htmlColor)
	{
		if (htmlColor.StartsWith("#"))
			return Color.FromHex(htmlColor);

		if (htmlColor.StartsWith("0x", StringComparison.OrdinalIgnoreCase) || htmlColor.StartsWith("&h", StringComparison.OrdinalIgnoreCase))
			return Color.FromArgb(Convert.ToInt32(htmlColor.Substring(2), 16));

		var provider = (NumberFormatInfo)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo));
		if (int.TryParse(htmlColor, NumberStyles.Integer, provider, out int value))
			return Color.FromArgb(value);

		return Color.FromName(htmlColor);
	}

	/// <summary>
	/// Translates an Ole color value to a <see cref='Color'/>.
	/// </summary>
	public static Color FromOle(int oleColor)
		=> (oleColor & OLE_SYS_COLOR_FLAG) == 0 ? Color.FromArgb((int)RefToArgb((uint)oleColor)) : throw new NotImplementedException();
	
	/// <summary>
	/// Translates an Win32 color value to a <see cref='Color'/>.
	/// </summary>
	public static Color FromWin32(int win32Color)
		=> FromOle(win32Color);

	#endregion


	#region Helpers

	private static uint RefToArgb(uint value)
		=> ((value >> R_SHIFT) & 0xFF) << 16
		 | ((value >> G_SHIFT) & 0xFF) << 8
		 | ((value >> B_SHIFT) & 0xFF) << 0
		 | 0xFFu << 24;

	#endregion
}