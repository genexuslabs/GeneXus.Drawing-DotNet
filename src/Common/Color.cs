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
	/// Gets the 32-bit ARGB value of this <see cref='Color'/>  structure.
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

	public static Color Transparent => new(SKColors.Transparent);
	public static Color AliceBlue => new(SKColors.AliceBlue);
	public static Color AntiqueWhite => new(SKColors.AntiqueWhite);
	public static Color Aqua => new(SKColors.Aqua);
	public static Color Aquamarine => new(SKColors.Aquamarine);
	public static Color Azure => new(SKColors.Azure);
	public static Color Beige => new(SKColors.Beige);
	public static Color Bisque => new(SKColors.Bisque);
	public static Color Black => new(SKColors.Black);
	public static Color BlanchedAlmond => new(SKColors.BlanchedAlmond);
	public static Color Blue => new(SKColors.Blue);
	public static Color BlueViolet => new(SKColors.BlueViolet);
	public static Color Brown => new(SKColors.Brown);
	public static Color BurlyWood => new(SKColors.BurlyWood);
	public static Color CadetBlue => new(SKColors.CadetBlue);
	public static Color Chartreuse => new(SKColors.Chartreuse);
	public static Color Chocolate => new(SKColors.Chocolate);
	public static Color Coral => new(SKColors.Coral);
	public static Color CornflowerBlue => new(SKColors.CornflowerBlue);
	public static Color Cornsilk => new(SKColors.Cornsilk);
	public static Color Crimson => new(SKColors.Crimson);
	public static Color Cyan => new(SKColors.Cyan);
	public static Color DarkBlue => new(SKColors.DarkBlue);
	public static Color DarkCyan => new(SKColors.DarkCyan);
	public static Color DarkGoldenrod => new(SKColors.DarkGoldenrod);
	public static Color DarkGray => new(SKColors.DarkGray);
	public static Color DarkGreen => new(SKColors.DarkGreen);
	public static Color DarkKhaki => new(SKColors.DarkKhaki);
	public static Color DarkMagenta => new(SKColors.DarkMagenta);
	public static Color DarkOliveGreen => new(SKColors.DarkOliveGreen);
	public static Color DarkOrange => new(SKColors.DarkOrange);
	public static Color DarkOrchid => new(SKColors.DarkOrchid);
	public static Color DarkRed => new(SKColors.DarkRed);
	public static Color DarkSalmon => new(SKColors.DarkSalmon);
	public static Color DarkSeaGreen => new(SKColors.DarkSeaGreen);
	public static Color DarkSlateBlue => new(SKColors.DarkSlateBlue);
	public static Color DarkSlateGray => new(SKColors.DarkSlateGray);
	public static Color DarkTurquoise => new(SKColors.DarkTurquoise);
	public static Color DarkViolet => new(SKColors.DarkViolet);
	public static Color DeepPink => new(SKColors.DeepPink);
	public static Color DeepSkyBlue => new(SKColors.DeepSkyBlue);
	public static Color DimGray => new(SKColors.DimGray);
	public static Color DodgerBlue => new(SKColors.DodgerBlue);
	public static Color Firebrick => new(SKColors.Firebrick);
	public static Color FloralWhite => new(SKColors.FloralWhite);
	public static Color ForestGreen => new(SKColors.ForestGreen);
	public static Color Fuchsia => new(SKColors.Fuchsia);
	public static Color Gainsboro => new(SKColors.Gainsboro);
	public static Color GhostWhite => new(SKColors.GhostWhite);
	public static Color Gold => new(SKColors.Gold);
	public static Color Goldenrod => new(SKColors.Goldenrod);
	public static Color Gray => new(SKColors.Gray);
	public static Color Green => new(SKColors.Green);
	public static Color GreenYellow => new(SKColors.GreenYellow);
	public static Color Honeydew => new(SKColors.Honeydew);
	public static Color HotPink => new(SKColors.HotPink);
	public static Color IndianRed => new(SKColors.IndianRed);
	public static Color Indigo => new(SKColors.Indigo);
	public static Color Ivory => new(SKColors.Ivory);
	public static Color Khaki => new(SKColors.Khaki);
	public static Color Lavender => new(SKColors.Lavender);
	public static Color LavenderBlush => new(SKColors.LavenderBlush);
	public static Color LawnGreen => new(SKColors.LawnGreen);
	public static Color LemonChiffon => new(SKColors.LemonChiffon);
	public static Color LightBlue => new(SKColors.LightBlue);
	public static Color LightCoral => new(SKColors.LightCoral);
	public static Color LightCyan => new(SKColors.LightCyan);
	public static Color LightGoldenrodYellow => new(SKColors.LightGoldenrodYellow);
	public static Color LightGray => new(SKColors.LightGray);
	public static Color LightGreen => new(SKColors.LightGreen);
	public static Color LightPink => new(SKColors.LightPink);
	public static Color LightSalmon => new(SKColors.LightSalmon);
	public static Color LightSeaGreen => new(SKColors.LightSeaGreen);
	public static Color LightSkyBlue => new(SKColors.LightSkyBlue);
	public static Color LightSlateGray => new(SKColors.LightSlateGray);
	public static Color LightSteelBlue => new(SKColors.LightSteelBlue);
	public static Color LightYellow => new(SKColors.LightYellow);
	public static Color Lime => new(SKColors.Lime);
	public static Color LimeGreen => new(SKColors.LimeGreen);
	public static Color Linen => new(SKColors.Linen);
	public static Color Magenta => new(SKColors.Magenta);
	public static Color Maroon => new(SKColors.Maroon);
	public static Color MediumAquamarine => new(SKColors.MediumAquamarine);
	public static Color MediumBlue => new(SKColors.MediumBlue);
	public static Color MediumOrchid => new(SKColors.MediumOrchid);
	public static Color MediumPurple => new(SKColors.MediumPurple);
	public static Color MediumSeaGreen => new(SKColors.MediumSeaGreen);
	public static Color MediumSlateBlue => new(SKColors.MediumSlateBlue);
	public static Color MediumSpringGreen => new(SKColors.MediumSpringGreen);
	public static Color MediumTurquoise => new(SKColors.MediumTurquoise);
	public static Color MediumVioletRed => new(SKColors.MediumVioletRed);
	public static Color MidnightBlue => new(SKColors.MidnightBlue);
	public static Color MintCream => new(SKColors.MintCream);
	public static Color MistyRose => new(SKColors.MistyRose);
	public static Color Moccasin => new(SKColors.Moccasin);
	public static Color NavajoWhite => new(SKColors.NavajoWhite);
	public static Color Navy => new(SKColors.Navy);
	public static Color OldLace => new(SKColors.OldLace);
	public static Color Olive => new(SKColors.Olive);
	public static Color OliveDrab => new(SKColors.OliveDrab);
	public static Color Orange => new(SKColors.Orange);
	public static Color OrangeRed => new(SKColors.OrangeRed);
	public static Color Orchid => new(SKColors.Orchid);
	public static Color PaleGoldenrod => new(SKColors.PaleGoldenrod);
	public static Color PaleGreen => new(SKColors.PaleGreen);
	public static Color PaleTurquoise => new(SKColors.PaleTurquoise);
	public static Color PaleVioletRed => new(SKColors.PaleVioletRed);
	public static Color PapayaWhip => new(SKColors.PapayaWhip);
	public static Color PeachPuff => new(SKColors.PeachPuff);
	public static Color Peru => new(SKColors.Peru);
	public static Color Pink => new(SKColors.Pink);
	public static Color Plum => new(SKColors.Plum);
	public static Color PowderBlue => new(SKColors.PowderBlue);
	public static Color Purple => new(SKColors.Purple);
	public static Color Red => new(SKColors.Red);
	public static Color RebeccaPurple => new("#663399");
	public static Color RosyBrown => new(SKColors.RosyBrown);
	public static Color RoyalBlue => new(SKColors.RoyalBlue);
	public static Color SaddleBrown => new(SKColors.SaddleBrown);
	public static Color Salmon => new(SKColors.Salmon);
	public static Color SandyBrown => new(SKColors.SandyBrown);
	public static Color SeaGreen => new(SKColors.SeaGreen);
	public static Color SeaShell => new(SKColors.SeaShell);
	public static Color Sienna => new(SKColors.Sienna);
	public static Color Silver => new(SKColors.Silver);
	public static Color SkyBlue => new(SKColors.SkyBlue);
	public static Color SlateBlue => new(SKColors.SlateBlue);
	public static Color SlateGray => new(SKColors.SlateGray);
	public static Color Snow => new(SKColors.Snow);
	public static Color SpringGreen => new(SKColors.SpringGreen);
	public static Color SteelBlue => new(SKColors.SteelBlue);
	public static Color Tan => new(SKColors.Tan);
	public static Color Teal => new(SKColors.Teal);
	public static Color Thistle => new(SKColors.Thistle);
	public static Color Tomato => new(SKColors.Tomato);
	public static Color Turquoise => new(SKColors.Turquoise);
	public static Color Violet => new(SKColors.Violet);
	public static Color Wheat => new(SKColors.Wheat);
	public static Color White => new(SKColors.White);
	public static Color WhiteSmoke => new(SKColors.WhiteSmoke);
	public static Color Yellow => new(SKColors.Yellow);
	public static Color YellowGreen => new(SKColors.YellowGreen);

	#endregion
}
