using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
[DebuggerDisplay("{NameAndARGBValue}")]
public readonly struct Color : IEquatable<Color>
{
	private readonly string m_name;
	private readonly int m_index;
	internal readonly SKColor m_color;

	private static readonly Dictionary<SKColor, string> m_KnownColorNames;

	static Color()
	{
		m_KnownColorNames = new Dictionary<SKColor, string>();
		PropertyInfo[] properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
		foreach (PropertyInfo property in properties.Where(prop => prop.PropertyType == typeof(Color)))
			if (property.GetValue(null) is Color color)
				m_KnownColorNames[color.m_color] = property.Name;
	}

	internal Color(SKColor color, string name = null, int index = 0)
	{
		m_color = color;
		m_name = name;
		m_index = index;
	}

	private Color(SKColor color, KnownColor knownColor)
		: this(color, knownColor.ToString(), (int)knownColor) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Color'/> structure with the specified 
	/// alpha, red, green and blue values.
	/// </summary>
	public Color(int alpha, int red, int green, int blue)
		: this(CreateFromRgba(red, green, blue, alpha)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Color'/> structure with the specified 
	/// hexadecimal string value RRGGBBAA or AARRGGBB if argb flag is enabled.
	/// </summary>
	public Color(string hex, bool argb = false)
		: this(CreateFromHex(hex, argb)) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Color'/>.
	/// </summary>
	public override readonly string ToString()
	{
		string name = IsNamedColor ? Name : IsEmpty ? "Empty" : $"A={A}, R={R}, G={G}, B={B}";
		return $"{nameof(Color)} [{name}]";
	}


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKColor'/> with the coordinates of the specified <see cref='Color'/> .
	/// </summary>
	public static explicit operator SKColor(Color color) => color.m_color;

	/// <summary>
	/// Tests whether two <see cref='Color'/> objects are identical.
	/// </summary>
	public static bool operator ==(Color left, Color right) => left.m_color == right.m_color;

	/// <summary>
	/// Tests whether two <see cref='Color'/> objects are different.
	/// </summary>
	public static bool operator !=(Color left, Color right) => left.m_color != right.m_color;

	#endregion


	#region IEquatable

	/// <summary>
	/// Tests whether a <see cref='Color'/> has the same values
	/// as this Color.
	/// </summary>
	public readonly bool Equals(Color other) => m_color.Equals(other.m_color);

	/// <summary>
	/// Tests whether <paramref name="obj"/> is a <see cref='Color'/> with the same values
	/// as this Color.
	/// </summary>
	public override readonly bool Equals(object obj) => obj is Color color && Equals(color);

	/// <summary>
	/// Returns a hash code.
	/// </summary>
	public override readonly int GetHashCode() => m_color.GetHashCode();

	#endregion


	#region Fields

	/// <summary>
	/// Creates a new instance of the <see cref='Color'/> class with member data left uninitialized.
	/// </summary>
	public static readonly Color Empty = new(SKColor.Empty, null, -1);

	#endregion


	#region Properties

	/// <summary>
	/// Gets a value indicating whether this <see cref='Color'/> is empty.
	/// </summary>
	public readonly bool IsEmpty => m_color == SKColor.Empty && m_index < 0;

	/// <summary>
	/// Gets a value indicating whether this <see cref='Color'/> structure is a predefined color. 
	/// Predefined colors are represented by the elements of the <see cref='KnownColor'/> enumeration.
	/// </summary>
	public readonly bool IsKnownColor
		=> m_index > 0 && m_index <= (int)KnownColor.RebeccaPurple;

	/// <summary>
	/// Gets a value indicating whether this <see cref='Color'/> structure is a system color. A system 
	/// color is a color that is used in a Windows display element. System colors are represented by 
	/// elements of the <see cref='KnownColor'/> enumeration.
	/// </summary>
	public readonly bool IsSystemColor
		=> m_index >= (int)KnownColor.ActiveBorder && m_index <= (int)KnownColor.WindowText
		|| m_index >= (int)KnownColor.ButtonFace && m_index <= (int)KnownColor.MenuHighlight;

	/// <summary>
	/// Gets a value indicating whether this <see cref='Color'/> is a named color.
	/// </summary>
	public readonly bool IsNamedColor => m_name?.Length > 0;

	/// <summary>
	/// Gets the alpha component value of this <see cref='Color'/> structure.
	/// </summary>
	public readonly int A => m_color.Alpha;

	/// <summary>
	/// Gets the red component value of this <see cref='Color'/> structure.
	/// </summary>
	public readonly int R => m_color.Red;

	/// <summary>
	/// Gets the green component value of this <see cref='Color'/> structure.
	/// </summary>
	public readonly int G => m_color.Green;

	/// <summary>
	/// Gets the blue component value of this <see cref='Color'/> structure.
	/// </summary>
	public readonly int B => m_color.Blue;

	/// <summary>
	/// Gets the name component value of this <see cref='Color'/> structure.
	/// </summary>
	public readonly string Name
	{
		get
		{
			if (IsNamedColor || IsKnownColor)
				return m_name;

			if (m_KnownColorNames.TryGetValue(m_color, out string name))
				return name;

			return ToArgb().ToString("x");
		}
	}

	/// <summary>
	/// Gets the hexadecimal representation in #RRGGBBAA (or #RGBA if can be reduced) of this <see cref='Color'/> structure.
	/// </summary>
	public readonly string Hex
	{
		get
		{
			// Get hex values
			string hexR = HexR;
			string hexG = HexG;
			string hexB = HexB;
			string hexA = HexA;

			// CHeck reduce hex
			if (hexR[0] == hexR[1] && hexG[0] == hexG[1] && hexB[0] == hexB[1] && hexA[0] == hexA[1])
			{
				hexR = hexR[0].ToString();
				hexG = hexG[0].ToString();
				hexB = hexB[0].ToString();
				hexA = hexA[0].ToString();
			}

			// Ignore alpha if opaque
			hexA = HexA.Equals("ff") ? "" : hexA;

			// Return RGBA value
			return $"#{hexR}{hexG}{hexB}{hexA}".ToUpper();
		}
	}

	#endregion


	#region Factory

	/// <summary>
	/// Creates a <see cref='Color'/> structure from the hexadecimal representation in RRGGBBAA (or AARRGGBB if argb flag enabled) values.
	/// </summary>
	public static Color FromHex(string hex, bool argb = false)
		=> new(hex, argb);

	/// <summary>
	/// Creates a <see cref='Color'/> structure from the four 8-bit ARGB components (alpha, red, green, and blue) values.
	/// </summary>
	public static Color FromArgb(int alpha, int red, int green, int blue)
		=> new(alpha, red, green, blue);

	/// <summary>
	/// Creates a <see cref='Color'/> structure from the four 8-bit RGB components (red, green, and blue) values.
	/// </summary>
	public static Color FromArgb(int red, int green, int blue)
		=> FromArgb(255, red, green, blue);

	/// <summary>
	/// Creates a <see cref='Color'/> structure from the specified base <see cref='Color'/> structure, but 
	/// with the new specified alpha value.
	/// </summary>
	public static Color FromArgb(int alpha, Color baseColor)
		=> FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);

	/// <summary>
	/// Creates a <see cref='Color'/> structure from a 32-bit ARGB value.
	/// </summary>
	public static Color FromArgb(int argb)
	{
		byte a = (byte)((argb >> 24) & 0xFF);
		byte r = (byte)((argb >> 16) & 0xFF);
		byte g = (byte)((argb >> 8) & 0xFF);
		byte b = (byte)((argb >> 0) & 0xFF);
		return FromArgb(a, r, g, b);
	}

	/// <summary>
	/// Creates a <see cref='Color'/> structure from the specified name of a predefined color.
	/// </summary>
	public static Color FromName(string name)
	{
		PropertyInfo property = typeof(Color).GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
		if (property?.PropertyType == typeof(Color) && property.GetValue(null) is Color color)
			return new(color.m_color, property.Name);
		return Empty;
	}

	/// <summary>
	/// Creates a <see cref='Color'/> structure from the specified predefined <see cref='KnownColor'/> color.
	/// </summary>
	public static Color FromKnownColor(KnownColor color)
	{
		var knownName = color.ToString();
		var knownColor = FromName(knownName); // NOTE: system colors are not supported and will be returned as empty
		return new(knownColor.m_color, knownColor.m_name ?? knownName, (int)color);
	}

	#endregion


	#region Methods

	/// <summary>
	/// Gets the 32-bit ARGB value of this <see cref='Color'/> structure.
	/// </summary>
	public readonly int ToArgb() => (A << 24) | (R << 16) | (G << 8) | B;

	/// <summary>
	/// Gets the hue-saturation-lightness (HSL) lightness value for this <see cref='Color'/> structure.
	/// </summary>
	public readonly float GetBrightness() => HSL.Luminosity / 100.0f;

	/// <summary>
	/// Gets the hue-saturation-lightness (HSL) saturation value for this <see cref='Color'/> structure.
	/// </summary>
	public readonly float GetSaturation() => HSL.Saturation / 100.0f;

	/// <summary>
	/// Gets the hue-saturation-lightness (HSL) hue value for this <see cref='Color'/> structure.
	/// </summary>
	public readonly float GetHue() => HSL.Hue;

	/// <summary>
	/// Gets the <see cref='KnownColor'/> value of this <see cref='Color'/> structure.
	/// </summary>
	public readonly KnownColor ToKnownColor() => Enum.TryParse(Name, out KnownColor color) ? color : 0;

	/// <summary>
	/// Linearly interpolates two <see cref='Color'/> by a given amount clamped between 0 and 1.
	/// </summary>
	public static Color Blend(Color color1, Color color2, float amount)
	{
		if (amount <= 0) return color1;
		if (amount >= 1) return color2;
		int r = (int)(color1.R + (color2.R - color1.R) * amount);
		int g = (int)(color1.G + (color2.G - color1.G) * amount);
		int b = (int)(color1.B + (color2.B - color1.B) * amount);
		int a = (int)(color1.A + (color2.A - color1.A) * amount);
		return FromArgb(a, r, g, b);
	}

	#endregion


	#region Utilities

	private readonly string NameAndARGBValue => $"{{Name = {Name}, ARGB = ({A}, {R}, {G}, {B})}}";

	private readonly string HexA => A.ToString("x2");
	private readonly string HexR => R.ToString("x2");
	private readonly string HexG => G.ToString("x2");
	private readonly string HexB => B.ToString("x2");

	private readonly (float Hue, float Saturation, float Luminosity) HSL
	{
		get
		{
			m_color.ToHsl(out float hue, out float saturation, out float luminosity);
			return (hue, saturation, luminosity);
		}
	}

	private static bool IsHexDigit(char c)
		=> (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');

	private static int ParseHex(string value, int start, int length = 2)
		=> int.Parse(value.Substring(start, length), System.Globalization.NumberStyles.HexNumber);

	private static SKColor CreateFromRgba(int red, int green, int blue, int alpha)
	{
		byte r = (byte)red;
		byte g = (byte)green;
		byte b = (byte)blue;
		byte a = (byte)alpha;
		return new SKColor(r, g, b, a);
	}

	private static SKColor CreateFromHex(string hex, bool argb)
	{
		hex = hex.TrimStart('#').ToLower();
		if (!hex.All(IsHexDigit))
			throw new ArgumentException($"invalid hex digit in #{hex}", nameof(hex));

		// expand for reduced hex
		hex = hex.Length switch
		{
			3 => string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2], 'f', 'f'),
			4 => string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2], hex[3], hex[3]),
			6 => string.Concat(hex, "ff"),
			8 => hex,
			_ => throw new ArgumentException($"invalid hex length #{hex}", nameof(hex))
		};

		// change rgba to argb
		if (argb)
			hex = string.Concat(hex[6], hex[7], hex[0], hex[1], hex[2], hex[3], hex[4], hex[5]);

		int r = ParseHex(hex, 0);
		int g = ParseHex(hex, 2);
		int b = ParseHex(hex, 4);
		int a = ParseHex(hex, 6);
		return CreateFromRgba(r, g, b, a);
	}

	#endregion


	#region NamedColors

	public static Color Transparent => new(SKColors.Transparent, KnownColor.Transparent);
	public static Color AliceBlue => new(SKColors.AliceBlue, KnownColor.AliceBlue);
	public static Color AntiqueWhite => new(SKColors.AntiqueWhite, KnownColor.AntiqueWhite);
	public static Color Aqua => new(SKColors.Aqua, KnownColor.Aqua);
	public static Color Aquamarine => new(SKColors.Aquamarine, KnownColor.Aquamarine);
	public static Color Azure => new(SKColors.Azure, KnownColor.Azure);
	public static Color Beige => new(SKColors.Beige, KnownColor.Beige);
	public static Color Bisque => new(SKColors.Bisque, KnownColor.Bisque);
	public static Color Black => new(SKColors.Black, KnownColor.Black);
	public static Color BlanchedAlmond => new(SKColors.BlanchedAlmond, KnownColor.BlanchedAlmond);
	public static Color Blue => new(SKColors.Blue, KnownColor.Blue);
	public static Color BlueViolet => new(SKColors.BlueViolet, KnownColor.BlueViolet);
	public static Color Brown => new(SKColors.Brown, KnownColor.Brown);
	public static Color BurlyWood => new(SKColors.BurlyWood, KnownColor.BurlyWood);
	public static Color CadetBlue => new(SKColors.CadetBlue, KnownColor.CadetBlue);
	public static Color Chartreuse => new(SKColors.Chartreuse, KnownColor.Chartreuse);
	public static Color Chocolate => new(SKColors.Chocolate, KnownColor.Chocolate);
	public static Color Coral => new(SKColors.Coral, KnownColor.Coral);
	public static Color CornflowerBlue => new(SKColors.CornflowerBlue, KnownColor.CornflowerBlue);
	public static Color Cornsilk => new(SKColors.Cornsilk, KnownColor.Cornsilk);
	public static Color Crimson => new(SKColors.Crimson, KnownColor.Crimson);
	public static Color Cyan => new(SKColors.Cyan, KnownColor.Cyan);
	public static Color DarkBlue => new(SKColors.DarkBlue, KnownColor.DarkBlue);
	public static Color DarkCyan => new(SKColors.DarkCyan, KnownColor.DarkCyan);
	public static Color DarkGoldenrod => new(SKColors.DarkGoldenrod, KnownColor.DarkGoldenrod);
	public static Color DarkGray => new(SKColors.DarkGray, KnownColor.DarkGray);
	public static Color DarkGreen => new(SKColors.DarkGreen, KnownColor.DarkGreen);
	public static Color DarkKhaki => new(SKColors.DarkKhaki, KnownColor.DarkKhaki);
	public static Color DarkMagenta => new(SKColors.DarkMagenta, KnownColor.DarkMagenta);
	public static Color DarkOliveGreen => new(SKColors.DarkOliveGreen, KnownColor.DarkOliveGreen);
	public static Color DarkOrange => new(SKColors.DarkOrange, KnownColor.DarkOrange);
	public static Color DarkOrchid => new(SKColors.DarkOrchid, KnownColor.DarkOrchid);
	public static Color DarkRed => new(SKColors.DarkRed, KnownColor.DarkRed);
	public static Color DarkSalmon => new(SKColors.DarkSalmon, KnownColor.DarkSalmon);
	public static Color DarkSeaGreen => new(SKColors.DarkSeaGreen, KnownColor.DarkSeaGreen);
	public static Color DarkSlateBlue => new(SKColors.DarkSlateBlue, KnownColor.DarkSlateBlue);
	public static Color DarkSlateGray => new(SKColors.DarkSlateGray, KnownColor.DarkSlateGray);
	public static Color DarkTurquoise => new(SKColors.DarkTurquoise, KnownColor.DarkTurquoise);
	public static Color DarkViolet => new(SKColors.DarkViolet, KnownColor.DarkViolet);
	public static Color DeepPink => new(SKColors.DeepPink, KnownColor.DeepPink);
	public static Color DeepSkyBlue => new(SKColors.DeepSkyBlue, KnownColor.DeepSkyBlue);
	public static Color DimGray => new(SKColors.DimGray, KnownColor.DimGray);
	public static Color DodgerBlue => new(SKColors.DodgerBlue, KnownColor.DodgerBlue);
	public static Color Firebrick => new(SKColors.Firebrick, KnownColor.Firebrick);
	public static Color FloralWhite => new(SKColors.FloralWhite, KnownColor.FloralWhite);
	public static Color ForestGreen => new(SKColors.ForestGreen, KnownColor.ForestGreen);
	public static Color Fuchsia => new(SKColors.Fuchsia, KnownColor.Fuchsia);
	public static Color Gainsboro => new(SKColors.Gainsboro, KnownColor.Gainsboro);
	public static Color GhostWhite => new(SKColors.GhostWhite, KnownColor.GhostWhite);
	public static Color Gold => new(SKColors.Gold, KnownColor.Gold);
	public static Color Goldenrod => new(SKColors.Goldenrod, KnownColor.Goldenrod);
	public static Color Gray => new(SKColors.Gray, KnownColor.Gray);
	public static Color Green => new(SKColors.Green, KnownColor.Green);
	public static Color GreenYellow => new(SKColors.GreenYellow, KnownColor.GreenYellow);
	public static Color Honeydew => new(SKColors.Honeydew, KnownColor.Honeydew);
	public static Color HotPink => new(SKColors.HotPink, KnownColor.HotPink);
	public static Color IndianRed => new(SKColors.IndianRed, KnownColor.IndianRed);
	public static Color Indigo => new(SKColors.Indigo, KnownColor.Indigo);
	public static Color Ivory => new(SKColors.Ivory, KnownColor.Ivory);
	public static Color Khaki => new(SKColors.Khaki, KnownColor.Khaki);
	public static Color Lavender => new(SKColors.Lavender, KnownColor.Lavender);
	public static Color LavenderBlush => new(SKColors.LavenderBlush, KnownColor.LavenderBlush);
	public static Color LawnGreen => new(SKColors.LawnGreen, KnownColor.LawnGreen);
	public static Color LemonChiffon => new(SKColors.LemonChiffon, KnownColor.LemonChiffon);
	public static Color LightBlue => new(SKColors.LightBlue, KnownColor.LightBlue);
	public static Color LightCoral => new(SKColors.LightCoral, KnownColor.LightCoral);
	public static Color LightCyan => new(SKColors.LightCyan, KnownColor.LightCyan);
	public static Color LightGoldenrodYellow => new(SKColors.LightGoldenrodYellow, KnownColor.LightGoldenrodYellow);
	public static Color LightGray => new(SKColors.LightGray, KnownColor.LightGray);
	public static Color LightGreen => new(SKColors.LightGreen, KnownColor.LightGreen);
	public static Color LightPink => new(SKColors.LightPink, KnownColor.LightPink);
	public static Color LightSalmon => new(SKColors.LightSalmon, KnownColor.LightSalmon);
	public static Color LightSeaGreen => new(SKColors.LightSeaGreen, KnownColor.LightSeaGreen);
	public static Color LightSkyBlue => new(SKColors.LightSkyBlue, KnownColor.LightSkyBlue);
	public static Color LightSlateGray => new(SKColors.LightSlateGray, KnownColor.LightSlateGray);
	public static Color LightSteelBlue => new(SKColors.LightSteelBlue, KnownColor.LightSteelBlue);
	public static Color LightYellow => new(SKColors.LightYellow, KnownColor.LightYellow);
	public static Color Lime => new(SKColors.Lime, KnownColor.Lime);
	public static Color LimeGreen => new(SKColors.LimeGreen, KnownColor.LimeGreen);
	public static Color Linen => new(SKColors.Linen, KnownColor.Linen);
	public static Color Magenta => new(SKColors.Magenta, KnownColor.Magenta);
	public static Color Maroon => new(SKColors.Maroon, KnownColor.Maroon);
	public static Color MediumAquamarine => new(SKColors.MediumAquamarine, KnownColor.MediumAquamarine);
	public static Color MediumBlue => new(SKColors.MediumBlue, KnownColor.MediumBlue);
	public static Color MediumOrchid => new(SKColors.MediumOrchid, KnownColor.MediumOrchid);
	public static Color MediumPurple => new(SKColors.MediumPurple, KnownColor.MediumPurple);
	public static Color MediumSeaGreen => new(SKColors.MediumSeaGreen, KnownColor.MediumSeaGreen);
	public static Color MediumSlateBlue => new(SKColors.MediumSlateBlue, KnownColor.MediumSlateBlue);
	public static Color MediumSpringGreen => new(SKColors.MediumSpringGreen, KnownColor.MediumSpringGreen);
	public static Color MediumTurquoise => new(SKColors.MediumTurquoise, KnownColor.MediumTurquoise);
	public static Color MediumVioletRed => new(SKColors.MediumVioletRed, KnownColor.MediumVioletRed);
	public static Color MidnightBlue => new(SKColors.MidnightBlue, KnownColor.MidnightBlue);
	public static Color MintCream => new(SKColors.MintCream, KnownColor.MintCream);
	public static Color MistyRose => new(SKColors.MistyRose, KnownColor.MistyRose);
	public static Color Moccasin => new(SKColors.Moccasin, KnownColor.Moccasin);
	public static Color NavajoWhite => new(SKColors.NavajoWhite, KnownColor.NavajoWhite);
	public static Color Navy => new(SKColors.Navy, KnownColor.Navy);
	public static Color OldLace => new(SKColors.OldLace, KnownColor.OldLace);
	public static Color Olive => new(SKColors.Olive, KnownColor.Olive);
	public static Color OliveDrab => new(SKColors.OliveDrab, KnownColor.OliveDrab);
	public static Color Orange => new(SKColors.Orange, KnownColor.Orange);
	public static Color OrangeRed => new(SKColors.OrangeRed, KnownColor.OrangeRed);
	public static Color Orchid => new(SKColors.Orchid, KnownColor.Orchid);
	public static Color PaleGoldenrod => new(SKColors.PaleGoldenrod, KnownColor.PaleGoldenrod);
	public static Color PaleGreen => new(SKColors.PaleGreen, KnownColor.PaleGreen);
	public static Color PaleTurquoise => new(SKColors.PaleTurquoise, KnownColor.PaleTurquoise);
	public static Color PaleVioletRed => new(SKColors.PaleVioletRed, KnownColor.PaleVioletRed);
	public static Color PapayaWhip => new(SKColors.PapayaWhip, KnownColor.PapayaWhip);
	public static Color PeachPuff => new(SKColors.PeachPuff, KnownColor.PeachPuff);
	public static Color Peru => new(SKColors.Peru, KnownColor.Peru);
	public static Color Pink => new(SKColors.Pink, KnownColor.Pink);
	public static Color Plum => new(SKColors.Plum, KnownColor.Plum);
	public static Color PowderBlue => new(SKColors.PowderBlue, KnownColor.PowderBlue);
	public static Color Purple => new(SKColors.Purple, KnownColor.Purple);
	public static Color Red => new(SKColors.Red, KnownColor.Red);
	public static Color RebeccaPurple => new(SKColor.Parse("#663399"), KnownColor.RebeccaPurple);
	public static Color RosyBrown => new(SKColors.RosyBrown, KnownColor.RosyBrown);
	public static Color RoyalBlue => new(SKColors.RoyalBlue, KnownColor.RoyalBlue);
	public static Color SaddleBrown => new(SKColors.SaddleBrown, KnownColor.SaddleBrown);
	public static Color Salmon => new(SKColors.Salmon, KnownColor.Salmon);
	public static Color SandyBrown => new(SKColors.SandyBrown, KnownColor.SandyBrown);
	public static Color SeaGreen => new(SKColors.SeaGreen, KnownColor.SeaGreen);
	public static Color SeaShell => new(SKColors.SeaShell, KnownColor.SeaShell);
	public static Color Sienna => new(SKColors.Sienna, KnownColor.Sienna);
	public static Color Silver => new(SKColors.Silver, KnownColor.Silver);
	public static Color SkyBlue => new(SKColors.SkyBlue, KnownColor.SkyBlue);
	public static Color SlateBlue => new(SKColors.SlateBlue, KnownColor.SlateBlue);
	public static Color SlateGray => new(SKColors.SlateGray, KnownColor.SlateGray);
	public static Color Snow => new(SKColors.Snow, KnownColor.Snow);
	public static Color SpringGreen => new(SKColors.SpringGreen, KnownColor.SpringGreen);
	public static Color SteelBlue => new(SKColors.SteelBlue, KnownColor.SteelBlue);
	public static Color Tan => new(SKColors.Tan, KnownColor.Tan);
	public static Color Teal => new(SKColors.Teal, KnownColor.Teal);
	public static Color Thistle => new(SKColors.Thistle, KnownColor.Thistle);
	public static Color Tomato => new(SKColors.Tomato, KnownColor.Tomato);
	public static Color Turquoise => new(SKColors.Turquoise, KnownColor.Turquoise);
	public static Color Violet => new(SKColors.Violet, KnownColor.Violet);
	public static Color Wheat => new(SKColors.Wheat, KnownColor.Wheat);
	public static Color White => new(SKColors.White, KnownColor.White);
	public static Color WhiteSmoke => new(SKColors.WhiteSmoke, KnownColor.WhiteSmoke);
	public static Color Yellow => new(SKColors.Yellow, KnownColor.Yellow);
	public static Color YellowGreen => new(SKColors.YellowGreen, KnownColor.YellowGreen);

	#endregion
}
