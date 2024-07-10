using System.Runtime.InteropServices;

namespace GeneXus.Drawing.Interop;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct LOGFONT
{
	private const int LF_FACESIZE = 32;

	/// <summary>
	/// The height of the font's character cell or character.
	/// </summary>
	public int lfHeight;

	/// <summary>
	/// The average width of characters in the font.
	/// </summary>
	public int lfWidth;

	/// <summary>
	/// The angle, in tenths of degrees, between the escapement vector and the x-axis.
	/// </summary>
	public int lfEscapement;

	/// <summary>
	/// The angle, in tenths of degrees, between each character's base line and the x-axis.
	/// </summary>
	public int lfOrientation;

	/// <summary>
	/// The weight of the font in the range 0 through 1000.
	/// </summary>
	public int lfWeight;

	/// <summary>
	/// Specifies an italic font if set to 1.
	/// </summary>
	public byte lfItalic;

	/// <summary>
	/// Specifies an underlined font if set to 1.
	/// </summary>
	public byte lfUnderline;

	/// <summary>
	/// Specifies a strikeout font if set to 1.
	/// </summary>
	public byte lfStrikeOut;

	/// <summary>
	/// The character set used by the font.
	/// </summary>
	public byte lfCharSet;

	/// <summary>
	/// The output precision, which defines how closely the output must match the requested font's 
	/// height, width, character orientation, escapement, and pitch.
	/// </summary>
	public byte lfOutPrecision;

	/// <summary>
	/// The clipping precision, which defines how to clip characters that are partially outside
	/// the clipping region.
	/// </summary>
	public byte lfClipPrecision;

	/// <summary>
	/// The output quality, which defines how carefully the font must match the attributes of
	/// the requested font.
	/// </summary>
	public byte lfQuality;

	/// <summary>
	/// The pitch and family of the font.
	/// </summary>
	public byte lfPitchAndFamily;

	/// <summary>
	/// A string specifying the typeface name of the font.
	/// </summary>
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
	public string lfFaceName;

	internal readonly string AsString()
		=> $"lfHeight={lfHeight}, lfWidth={lfWidth}, lfEscapement={lfEscapement}, lfOrientation={lfOrientation}, "
		 + $"lfWeight={lfWeight}, lfItalic={lfItalic}, lfUnderline={lfUnderline}, lfStrikeOut={lfStrikeOut}, "
		 + $"lfCharSet={lfCharSet}, lfOutPrecision={lfOutPrecision}, lfClipPrecision={lfClipPrecision}, "
		 + $"lfQuality={lfQuality}, lfPitchAndFamily={lfPitchAndFamily}, lfFaceName={lfFaceName}";
}