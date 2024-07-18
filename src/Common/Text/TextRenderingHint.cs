namespace GeneXus.Drawing.Text;

/// <summary>
///  Specifies the quality of text rendering.
/// </summary>
public enum TextRenderingHint
{
	/// <summary>
	///  Glyph with system default rendering hint.
	/// </summary>
	SystemDefault = 0,

	/// <summary>
	///  Glyph bitmap with hinting.
	/// </summary>
	SingleBitPerPixelGridFit = 1,

	/// <summary>
	///  Glyph bitmap without hinting.
	/// </summary>
	SingleBitPerPixel = 2,

	/// <summary>
	///  Anti-aliasing with hinting.
	/// </summary>
	AntiAliasGridFit = 3,

	/// <summary>
	///  Glyph anti-alias bitmap without hinting.
	/// </summary>
	AntiAlias = 4,

	/// <summary>
	///  Glyph CT bitmap with hinting.
	/// </summary>
	ClearTypeGridFit = 5
}
