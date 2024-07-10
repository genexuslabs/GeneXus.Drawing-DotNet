namespace GeneXus.Drawing.Interop;

internal static class GDI_CHARSET
{
	public const byte ANSI_CHARSET = 0x00;
	public const byte DEFAULT_CHARSET = 0x01;
	public const byte SYMBOL_CHARSET = 0x02;
	public const byte MAC_CHARSET = 0x4D;
	public const byte SHIFTJIS_CHARSET = 0x8;
	public const byte HANGUL_CHARSET = 0x81;
	public const byte JOHAB_CHARSET = 0x82;
	public const byte GB2312_CHARSET = 0x86;
	public const byte CHINESEBIG5_CHARSET = 0x88;
	public const byte GREEK_CHARSET = 0xA1;
	public const byte TURKISH_CHARSET = 0xA2;
	public const byte VIETNAMESE_CHARSET = 0xA3;
	public const byte HEBREW_CHARSET = 0xB1;
	public const byte ARABIC_CHARSET = 0xB2;
	public const byte BALTIC_CHARSET = 0xBA;
	public const byte RUSSIAN_CHARSET = 0xCC;
	public const byte THAI_CHARSET = 0xDE;
	public const byte EASTEUROPE_CHARSET = 0xEE;
	public const byte OEM_CHARSET = 0xFF;
}

internal static class GDI_CLIP_PRECISION
{
	public const byte CLIP_DEFAULT_PRECIS = 0x00;
	public const byte CLIP_CHARACTER_PRECIS = 0x01;
	public const byte CLIP_STROKE_PRECIS = 0x02;
	public const byte CLIP_MASK = 0x0F;
	public const byte CLIP_LH_ANGLES = 0x10;
	public const byte CLIP_TT_ALWAYS = 0x20;
	public const byte CLIP_DFA_DISABLE = 0x40;
	public const byte CLIP_EMBEDDED = 0x80;
}

internal static class GDI_FONT_FAMILY
{
	public const byte FF_DONTCARE = 0x00;	 // Don't care or don't know.
	public const byte FF_ROMAN = 0x10;		// Variable stroke width, serifed (Times Roman, Century Schoolbook, etc.).
	public const byte FF_SWISS = 0x20;		// Variable stroke width, sans-serifed (Helvetica, Swiss, etc.).
	public const byte FF_MODERN = 0x30;	   // Constant stroke width, serifed or sans-serifed (Pica, Elite, Courier, etc.).
	public const byte FF_SCRIPT = 0x40;	   // Cursive, etc.
	public const byte FF_DECORATIVE = 0x50;   // Old English, etc.
}

internal static class GDI_OUT_PRECISION
{
	public const byte OUT_DEFAULT_PRECIS = 0x00;
	public const byte OUT_STRING_PRECIS = 0x01;
	public const byte OUT_CHARACTER_PRECIS = 0x02;
	public const byte OUT_STROKE_PRECIS = 0x03;
	public const byte OUT_TT_PRECIS = 0x04;
	public const byte OUT_DEVICE_PRECIS = 0x05;
	public const byte OUT_RASTER_PRECIS = 0x06;
	public const byte OUT_TT_ONLY_PRECIS = 0x07;
	public const byte OUT_OUTLINE_PRECIS = 0x08;
	public const byte OUT_SCREEN_OUTLINE_PRECIS = 0x09;
	public const byte OUT_PS_ONLY_PRECIS = 0x0A;
}

internal static class GDI_PITCH_FONT
{
	public const byte DEFAULT_PITCH = 0x00;
	public const byte FIXED_PITCH = 0x01;
	public const byte VARIABLE_PITCH = 0x02;
}

internal static class GDI_FONT_QUALITY
{
	public const byte DEFAULT_QUALITY = 0x00;
	public const byte DRAFT_QUALITY = 0x01;
	public const byte PROOF_QUALITY = 0x02;
	public const byte NONANTIALIASED_QUALITY = 0x03;
	public const byte ANTIALIASED_QUALITY = 0x04;
	public const byte CLEARTYPE_QUALITY = 0x05;
	public const byte CLEARTYPE_NATURAL_QUALITY = 0x06;
}

public static class GDI_WEIGHT
{
	public const int FW_DONTCARE = 0;
	public const int FW_THIN = 100;
	public const int FW_EXTRALIGHT = 200;
	public const int FW_LIGHT = 300;
	public const int FW_NORMAL = 400;
	public const int FW_MEDIUM = 500;
	public const int FW_SEMIBOLD = 600;
	public const int FW_BOLD = 700;
	public const int FW_EXTRABOLD = 800;
	public const int FW_HEAVY = 900;

	public const int FW_ULTRALIGHT = FW_EXTRALIGHT;
	public const int FW_REGULAR = FW_NORMAL;
	public const int FW_DEMIBOLD = FW_SEMIBOLD;
	public const int FW_ULTRABOLD = FW_EXTRABOLD;
	public const int FW_BLACK = FW_HEAVY;
}