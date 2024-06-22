using System;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public class Font : IDisposable, ICloneable
{
	internal readonly FontFamily m_family;
	internal readonly float m_size;
	internal readonly string m_original;

	/// <summary>
	/// Initializes a new System.Drawing.Font that uses the specified existing <see cref='Font'/>
	/// and <see cref='FontStyle'/> enumeration.
	/// </summary>
	/// <param name="prototype">The existing <see cref='Font'/> from which to create the new <see cref='Font'/></param>
	/// <param name="newStyle">
	/// The <see cref='FontStyle'/> to apply to the new S<see cref='Font'/>. Multiple
	/// values of the <see cref='FontStyle'/> enumeration can be combined with the OR operator.
	/// </param>
	public Font(Font prototype, FontStyle newStyle)
	{
		m_family = prototype.FontFamily;
		// TODO set newStyle
	}
	
	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified <see cref='Drawing.FontFamily'/> and size.
	/// </summary>
	public Font(FontFamily family, float size = 12)
	{
		m_family = family ?? throw new ArgumentException("missing family");
		m_size = size;
	}

	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified family name and size.
	/// </summary>
	public Font(string familyName, float size = 12)
		: this(SystemFonts.Select(f => f.m_family).FirstOrDefault(ff => ff is SkiaFontFamily sff && sff.MatchFamily(familyName)) ?? new UnknownFontFamily(familyName), size)
	{
		m_original = familyName;
	}

	/// <summary>
	/// Initializes a new <see cref='Font'/> using the specified family name, size, styl.
	/// </summary>
	public Font(string familyName, float size, FontStyle style, GraphicsUnit unit)
		: this(familyName, size)
	{
		// TODO set style and unit
	}
	
	/// <summary>
	///  Cleans up resources for this <see cref='Font'/>.
	/// </summary>
	~Font() => Dispose(false);

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Font'/>.
	/// </summary>
	public override string ToString()
	{
		string suffix = m_family is SkiaFontFamily { m_index: > 0 } sff ? $"#{sff.m_index}" : string.Empty;
		return $"[{GetType().Name}: Name={Name}{suffix}, Size={Size}]";
	}


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Font'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	protected virtual void Dispose(bool disposing) => m_family.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	/// Creates an exact copy of this <see cref='Font'/>.
	/// </summary>
	public object Clone() => new Font((FontFamily)m_family.Clone());

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKFont'/> with the coordinates of the specified <see cref='Font'/> .
	/// </summary>
	public static explicit operator SKFont(Font font) => (font.m_family as SkiaFontFamily)?.GetFont(font.m_size);

	#endregion


	#region Properties

	/// <summary>
	/// Gets the face name of this <see cref='Font'/>.
	/// </summary>
	public string Name => m_family is SkiaFontFamily sff ? $"{sff.Name} {sff.Face}" : m_family.Name;

	/// <summary>
	/// Gets the name of the <see cref='Font'/> originally specified.
	/// </summary>
	public string OriginalFontName => m_original;

	/// <summary>
	/// Gets the <see cref='Drawing.FontFamily'/> associated with this Font.
	/// </summary>
	public FontFamily FontFamily => m_family;

	/// <summary>
	/// Gets the weight of this <see cref='Font'/>.
	/// </summary>
	public int Weight => m_family.Weight;

	/// <summary>
	/// Gets the width of this <see cref='Font'/>.
	/// </summary>
	public int Width => m_family.Width;

	/// <summary>
	/// Gets the line spacing of this <see cref='Font'/>.
	/// The Height property is the value returned by GetHeight method, rounded up to the nearest integer.
	/// </summary>
	public int Height => (int)Math.Ceiling(GetHeight());

	/// <summary>
	/// Gets the size of this <see cref='Font'/>.
	/// </summary>
	public float Size => m_size;

	/// <summary>
	/// Gets the slant of this <see cref='Font'/>.
	/// </summary>
	public SlantType Slant => m_family.Slant;

	/// <summary>
	/// Gets style information for this <see cref='Font'/>.
	/// </summary>
	public FontStyle Style
	{
		get
		{
			FontStyle style = FontStyle.Regular;

			if (Italic)
				style |= FontStyle.Italic;

			if (Bold)
				style |= FontStyle.Bold;

			if (Underline)
				style |= FontStyle.Underline;

			if (Strikeout)
				style |= FontStyle.Strikeout;

			return style;
		}
	}

	/// <summary>
	/// Gets the FamilyName associated with this <see cref='Font'/>.
	/// </summary>
	public string FamilyName => m_family.Name;

	/// <summary>
	/// Gets the FaceName associated with this <see cref='Font'/>.
	/// </summary>
	public string FaceName => m_family.Face;

	/// <summary>
	/// Gets a value that indicates whether this <see cref='Font'/> has the italic style applied.
	/// </summary>
	public bool Italic => m_family.IsItalic;

	/// <summary>
	/// Gets a value that indicates whether this <see cref='Font'/> is bold.
	/// </summary>
	public bool Bold => m_family.IsBold;

	private SKFontMetrics Metrics => m_family is SkiaFontFamily sff ? sff.GetFont(m_size).Metrics : new SKFontMetrics();
	
	/// <summary>
	/// Gets a value indicating whether this <see cref='Font'/> is underlined.
	/// </summary>
	public bool Underline => Metrics is { UnderlineThickness: > 0, UnderlinePosition: 0f };

	/// <summary>
	/// Gets a value indicating whether this <see cref='Font'/> is strikeout (has a line through it).
	/// </summary>
	public bool Strikeout => Metrics is { StrikeoutThickness: > 0, StrikeoutPosition: 0f };

	/// <summary>
	/// Gets a value indicating whether the <see cref='Font'/> is a member of SystemFonts.
	/// </summary>
	public bool IsSystemFont
	{
		get
		{
			var fontFullName = ToString();
			return SystemFonts.Any(sf => fontFullName.Equals(sf.ToString()));
		}
	}

	/// <summary>
	/// Gets a system <see cref='Font'/> collection.
	/// </summary>
	public static ICollection<Font> SystemFonts
	{
		get
		{
			s_SystemFonts ??= GetFonts(SYSTEM_FONT_PATH);
			return s_SystemFonts;
		}
	}

	private static ICollection<Font> s_SystemFonts;

	/// <summary>
	/// Gets the em-size, in points, of this <see cref='Font'/>.
	/// </summary>
	/// <returns>The em-size, in points, of this <see cref='Font'/></returns>
	[Browsable(false)]
	public float SizeInPoints
	{
		get
		{
			// TODO calculate it for other units
			Debug.Assert(Unit == GraphicsUnit.Point);
			return Size;
		}
	}
	
	/// <summary>
	/// Gets the unit of measure for this <see cref='Font'/>.
	/// </summary>
	/// <returns>A <see cref='GraphicsUnit'/> that represents the unit of measure for this <see cref='Font'/>.</returns>
	public GraphicsUnit Unit => GraphicsUnit.Point;
	
	#endregion


	#region Methods

	/// <summary>
	/// Returns the line spacing of this <see cref='Font'/>.
	/// </summary>
	public float GetHeight() => Metrics.Descent - Metrics.Ascent + Metrics.Leading;

	/// <summary>
	/// Returns a <see cref='Font'/> collection in the specified location.
	/// </summary>
	public static ICollection<Font> GetFonts(string location)
	{
		var fonts = new ConcurrentBag<Font>();
		var files = Directory.EnumerateFiles(location);
		Parallel.ForEach(files, fontFile =>
		{
			if (FONT_EXTENSIONS.Contains(Path.GetExtension(fontFile)))
			{
				var family = FontFamilyFactory.Create(fontFile);
				var font = new Font(family);
				fonts.Add(font);
			}
		});
		return fonts.ToList();
	}

	#endregion


	#region Extras

	/* 
	 * NOTE: This implementation is not part of System.Drawing.Font 
	 * nor System.Windows.Media.Fonts libraries
	 */

	/// <summary>
	/// Returns the embedded <see cref='Font'/> count from a filename.
	/// </summary>
	public static int GetFontCount(string filepath)
	{
		var data = File.ReadAllBytes(filepath);
		using var stream = new MemoryStream(data);
		return GetFontCount(stream);
	}

	/// <summary>
	/// Returns the embedded <see cref='Font'/> count from a file stream.
	/// </summary>
	public static int GetFontCount(Stream stream)
	{
		using var reader = new BinaryReader(stream);
		string type = new(reader.ReadChars(4));
		if (type.Equals("ttcf") || type.Equals("OTTO")) // check ttc/otc type in file header
		{
			int version = BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);
			if (version != 0x00010000 && version != 0x00020000)
				throw new ArgumentException($"unrecognized ttc/otc version 0x{version:X8}");
			int count = BitConverter.ToInt32(reader.ReadBytes(4).Reverse().ToArray(), 0);
			return count;
		}
		return 1;
	}

	#endregion


	#region Utilities

	private static readonly string SYSTEM_FONT_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
	private static readonly string[] FONT_EXTENSIONS = { ".ttf", ".otf", ".eot", ".woff", ".woff2" };

	#endregion
}
