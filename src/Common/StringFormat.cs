
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using GeneXus.Drawing.Text;
using SkiaSharp;

namespace GeneXus.Drawing;

public sealed class StringFormat : ICloneable, IDisposable
{
	private float TabOffset = 0;
	private float[] TabStops = Array.Empty<float>();
	private CharacterRange[] Ranges = Array.Empty<CharacterRange>();

	/// <summary>
	///  Initializes a new instance of the <see cref='StringFormat'/> class.
	/// </summary>
	public StringFormat()
		: this(0, 0) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='StringFormat'/> class with the specified <see cref='StringFormatFlags'/>.
	/// </summary>
	public StringFormat(StringFormatFlags options)
		: this(options, 0) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='StringFormat'/> class with the specified
	///  <see cref='StringFormatFlags'/> and language.
	/// </summary>
	public StringFormat(StringFormatFlags options, int language)
	{
		FormatFlags = options;
		DigitSubstitutionLanguage = language;
	}

	/// <summary>
	///  Initializes a new instance of the <see cref='StringFormat'/> class from the specified
	///  existing <see cref='StringFormat'/>.
	/// </summary>
	public StringFormat(StringFormat format)
		=> format.MemberwiseClone();

	/// <summary>
	///  Cleans up resources for this <see cref='StringFormat'/>.
	/// </summary>
	~StringFormat() => Dispose(false);

	/// <summary>
	///  Converts this <see cref='StringFormat'/> to a human-readable string.
	/// </summary>
	public override string ToString() => $"[StringFormat, FormatFlags={FormatFlags}]";


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='GraphicsPath'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing) { }

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='GraphicsPath'/>.
	/// </summary>
	public object Clone() => new StringFormat(this);

	#endregion


	#region Properties

	/// <summary>
	///  Specifies text alignment information.
	/// </summary>
	public StringAlignment Alignment { get; set; } = StringAlignment.Near;

	/// <summary>
	///  Gets the <see cref='StringDigitSubstitute'/> for this <see cref='StringFormat'/>.
	/// </summary>
	public StringDigitSubstitute DigitSubstitutionMethod { get; private set; } = StringDigitSubstitute.None;

	/// <summary>
	///  Gets the language of <see cref='StringDigitSubstitute'/> for this <see cref='StringFormat'/>.
	/// </summary>
	public int DigitSubstitutionLanguage { get; private set; } = CultureInfo.InvariantCulture.LCID;

	/// <summary>
	///  Gets or sets a <see cref='StringFormatFlags'/> that contains formatting information.
	/// </summary>
	public StringFormatFlags FormatFlags { get; set; } = 0;

	/// <summary>
	///  Gets a generic default <see cref='StringFormat'/>.
	/// </summary>
	public static StringFormat GenericDefault => new();

	/// <summary>
	///  Gets a generic typographic <see cref='StringFormat'/>.
	/// </summary>
	public static StringFormat GenericTypographic => new()
	{
		FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit | StringFormatFlags.NoClip,
		Trimming = StringTrimming.None
	};

	/// <summary>
	///  Gets or sets the <see cref='Drawing.HotkeyPrefix'/> for this <see cref='StringFormat'/> .
	/// </summary>
	public HotkeyPrefix HotkeyPrefix { get; set; } = HotkeyPrefix.None;

	/// <summary>
	///  Gets or sets the line alignment.
	/// </summary>
	public StringAlignment LineAlignment { get; set; } = StringAlignment.Near;

	/// <summary>
	///  Gets or sets the <see cref='StringTrimming'/> for this <see cref='StringFormat'/>.
	/// </summary>
	public StringTrimming Trimming { get; set; } = StringTrimming.None;

	#endregion


	#region Methods

	/// <summary>
	///  Gets the ranges count for this <see cref='StringFormat'/> object.
	/// </summary>
	internal int GetMeasurableCharacterRangeCount()
		=> Ranges.Length;

	/// <summary>
	///  Gets the tab stops for this <see cref='StringFormat'/> object.
	/// </summary>
	public float[] GetTabStops(out float firstTabOffset)
	{
		firstTabOffset = TabOffset;
		return TabStops;
	}

	/// <summary>
	///  Specifies the language and method to be used when local digits are substituted for western digits.
	/// </summary>
	public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
	{
		DigitSubstitutionLanguage = language;
		DigitSubstitutionMethod = substitute;
	}

	/// <summary>
	///  Sets the measure of characters to the specified range.
	/// </summary>
	public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
		=> Ranges = ranges;

	/// <summary>
	///  Sets tab stops for this <see cref='StringFormat'/>.
	/// </summary>
	public void SetTabStops(float firstTabOffset, float[] tabStops)
	{
		TabOffset = firstTabOffset;
		TabStops = tabStops;
	}

	#endregion


	#region Utilties

	internal static readonly string[] BREAKLINES = new string[]
	{
		"\r\n",
		"\n",
		"\u2028", // unicode's line separator
		"\u2029", // unicode's paragraph separator
		"\u0085", // unicode's next line
	};

	internal static readonly char[] CONTROL = Enumerable.Range(0x00, 0xFFFF)
		.Select(Convert.ToChar)
		.Where(char.IsControl)
		.ToArray();

	internal static readonly Dictionary<char, char> CONTROL_CHARACTERS = new()
	{
		{ '\u0000', '␀' },  // Null
		{ '\u0001', '␁' },  // Start of Heading
		{ '\u0002', '␂' },  // Start of Text
		{ '\u0003', '␃' },  // End of Text
		{ '\u0004', '␄' },  // End of Transmission
		{ '\u0005', '␅' },  // Enquiry
		{ '\u0006', '␆' },  // Acknowledge
		{ '\u0007', '␇' },  // Bell
		{ '\u0008', '␈' },  // Backspace
		{ '\u0009', '␉' },  // Horizontal Tab
		{ '\u000A', '␊' },  // Line Feed
		{ '\u000B', '␋' },  // Vertical Tab
		{ '\u000C', '␌' },  // Form Feed
		{ '\u000D', '␍' },  // Carriage Return
		{ '\u000E', '␎' },  // Shift Out
		{ '\u000F', '␏' },  // Shift In
		{ '\u0010', '␐' },  // Data Link Escape
		{ '\u0011', '␑' },  // Device Control 1
		{ '\u0012', '␒' },  // Device Control 2
		{ '\u0013', '␓' },  // Device Control 3
		{ '\u0014', '␔' },  // Device Control 4
		{ '\u0015', '␕' },  // Negative Acknowledge
		{ '\u0016', '␖' },  // Synchronous Idle
		{ '\u0017', '␗' },  // End of Transmission Block
		{ '\u0018', '␘' },  // Cancel
		{ '\u0019', '␙' },  // End of Medium
		{ '\u001A', '␚' },  // Substitute
		{ '\u001B', '␛' },  // Escape
		{ '\u001C', '␜' },  // File Separator
		{ '\u001D', '␝' },  // Group Separator
		{ '\u001E', '␞' },  // Record Separator
		{ '\u001F', '␟' },  // Unit Separator
		{ '\u007F', '␡' },  // Delete
		{ '\u0085', '␤' },  // Next Line
		{ '\u00A0', '␣' },  // Non-Breaking Space
		{ '\u1680', ' ' },  // Ogham Space Mark
		{ '\u2000', ' ' },  // En Quad
		{ '\u2001', ' ' },  // Em Quad
		{ '\u2002', ' ' },  // En Space
		{ '\u2003', ' ' },  // Em Space
		{ '\u2004', ' ' },  // Three-Per-Em Space
		{ '\u2005', ' ' },  // Four-Per-Em Space
		{ '\u2006', ' ' },  // Six-Per-Em Space
		{ '\u2007', ' ' },  // Figure Space
		{ '\u2008', ' ' },  // Punctuation Space
		{ '\u2009', ' ' },  // Thin Space
		{ '\u200A', ' ' },  // Hair Space
		{ '\u200B', '​' },  // Zero Width Space
		{ '\u200C', '‌' },  // Zero Width Non-Joiner
		{ '\u200D', '‍' },  // Zero Width Joiner
		{ '\u200E', '‎' },  // Left-to-Right Mark
		{ '\u200F', '‏' },  // Right-to-Left Mark
		{ '\u2028', '␤' },  // Line Separator
		{ '\u2029', '¶' },  // Paragraph Separator
		{ '\u202A', '‪' },  // Left-to-Right Embedding
		{ '\u202B', '‫' },  // Right-to-Left Embedding
		{ '\u202C', '‬' },  // Pop Directional Formatting
		{ '\u202D', '‭' },  // Left-to-Right Override
		{ '\u202E', '‮' },  // Right-to-Left Override
		{ '\u2060', '⁠' },  // Word Joiner
		{ '\u2061', '⁡' },  // Function Application
		{ '\u2062', '⁢' },  // Invisible Times
		{ '\u2063', '⁣' },  // Invisible Separator
		{ '\u2064', '⁤' }   // Invisible Plus
	};

	#endregion


	#region Helpers

	internal string[] ApplyRanges(string text)
	{
		var substrings = new List<string>();
		foreach (var range in Ranges)
		{
			int idx = Math.Max(0, range.First);
			int end = Math.Min(idx + range.Length, idx + text.Length);
			if (idx == end)
				continue;
			string substring = text.Substring(idx, end - idx);
			substrings.Add(substring);
		}
		return substrings.ToArray();
	}

	internal string ApplyDirection(string text)
		=> FormatFlags.HasFlag(StringFormatFlags.DirectionVertical)
			? string.Join("\n", text.Split(BREAKLINES, StringSplitOptions.None).Reverse())
			: text;


	internal string ApplyHotkey(string text, out int[] indexes)
	{
		var hkIndexes = new List<int>();

		var sb = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			var c = text[i];
			if (c == '&')
			{
				if (i < text.Length - 1 && text[i + 1] == '&')
				{
					i++; // skip next '&'
				}
				else
				{
					if (HotkeyPrefix == HotkeyPrefix.Show)
						hkIndexes.Add(sb.Length);
					if (HotkeyPrefix != HotkeyPrefix.None)
						continue;
				}
			}
			sb.Append(c);
		}
		indexes = hkIndexes.ToArray();
		return sb.ToString();
	}


	internal string ApplyTabStops(string text)
	{
		var tabStops = GetTabStops(out var tabOffset);
		if (tabStops.Length > 0)
		{
			var fill = string.Empty.PadLeft((int)tabOffset, ' ');

			var lines = text.Split('\n');
			for (int i = 0; i < lines.Length; i++)
			{
				var columns = lines[i].Split('\t');
				for (int j = 0; j < columns.Length; j++)
				{
					int pad = j < tabStops.Length ? (int)tabStops[j] : columns[j].Length;
					columns[j] = columns[j].PadLeft(pad, ' ');
				}
				lines[i] = fill + string.Join("", columns);
			}
			text = string.Join("", lines);
		}
		return text;
	}


	internal string ApplyDigitSubstitution(string text)
	{
		var sb = new StringBuilder(text);
		if (DigitSubstitutionMethod != StringDigitSubstitute.None)
		{
			var lcid = DigitSubstitutionMethod == StringDigitSubstitute.User 
				? CultureInfo.CurrentCulture.LCID 
				: DigitSubstitutionLanguage; 
				
			var culture = new CultureInfo(lcid);

			var format = culture.NumberFormat;
			for (int i = 0; i < format.NativeDigits.Length; i++)
				sb.Replace(i.ToString(), format.NativeDigits[i]);
		}
		return sb.ToString();
	}


	internal string ApplyControlEscape(string text)
	{
		var sb = new StringBuilder(text);
		if (FormatFlags.HasFlag(StringFormatFlags.DisplayFormatControl))
		{
			var controlChars = CONTROL_CHARACTERS;
			foreach (char chr in CONTROL)
				if (!controlChars.ContainsKey(chr))
					controlChars.Add(chr, '�');

			foreach (var kvp in controlChars)
				sb.Replace(kvp.Key, kvp.Value);
		}
		return sb.ToString();
	}


	internal string ApplyWrapping(string text, SKRect boundBox, Func<string, float> measureText)
	{
		if (FormatFlags.HasFlag(StringFormatFlags.NoWrap))
			return text;

		float accWidth = 0;

		var sb = new StringBuilder();
		foreach (var line in text.Split(BREAKLINES, StringSplitOptions.None))
		{
			foreach (var word in line.Split(' '))
			{
				var curWidth = measureText(word);
				if (Math.Ceiling(accWidth += curWidth) >= boundBox.Width)
				{
					sb.Append('\n').Append(' ');
					accWidth = curWidth;
				}
				sb.Append(word).Append(' ');
			}
			sb.Length--; // remove last space
		}
		return sb.ToString();
	}


	private string ApplyTrimming(IEnumerable<string> tokens, string endToken, SKRect boundBox, Func<string, float> measureText)
	{
		float accWidth = 0;
		float endLen = measureText(endToken);

		var sb = new StringBuilder();
		var tokenList = tokens.ToArray();
		for (int i = 0; i < tokenList.Length; i++)
		{
			string token = tokenList[i];

			float curWidth = measureText(token);
			if (token == "&")
			{
				if (i < tokenList.Length - 1 && tokenList[i + 1] == "&")
				{
					i++; // skip next '&'
				}
				else if (HotkeyPrefix != HotkeyPrefix.None)
				{
					curWidth = 0;
				}
			}

			if (Math.Ceiling((accWidth += curWidth) + endLen) >= boundBox.Width)
			{
				if (FormatFlags.HasFlag(StringFormatFlags.DirectionRightToLeft))
					sb.Insert(0, endToken);
				else
					sb.Append(endToken);
				break;
			}
			sb.Append(token);
		}

		var trim = (string s) => FormatFlags.HasFlag(StringFormatFlags.DirectionRightToLeft) ? s.TrimStart() : s.TrimEnd();
		return trim(sb.ToString());
	}


	internal string ApplyTrimming(string text, SKRect boundBox, float lineHeight, Func<string, float> measureText)
	{
		var lines = text.Split(BREAKLINES, StringSplitOptions.None);
		if (FormatFlags.HasFlag(StringFormatFlags.LineLimit))
			lines = lines.Take((int)(boundBox.Height / lineHeight)).ToArray();

		for (int i = 0; i < lines.Length; i++)
		{
			string line = lines[i];

			if (!FormatFlags.HasFlag(StringFormatFlags.MeasureTrailingSpaces))
				line = FormatFlags.HasFlag(StringFormatFlags.DirectionRightToLeft) ? line.TrimStart() : line.TrimEnd();

			line = Trimming switch
			{
				StringTrimming.None
					=> line,

				StringTrimming.Character
					=> ApplyTrimming(line.Select(chr => chr.ToString()), "", boundBox, measureText),

				StringTrimming.Word
					=> ApplyTrimming(line.Split(' '), "", boundBox, measureText),

				StringTrimming.EllipsisCharacter
					=> ApplyTrimming(line.Select(chr => chr.ToString()), "…", boundBox, measureText),

				StringTrimming.EllipsisWord
					=> ApplyTrimming(line.Split(' '), "…", boundBox, measureText),

				StringTrimming.EllipsisPath
					=> ApplyTrimming(line.Split(Path.PathSeparator), "…", boundBox, measureText),

				_ => throw new NotImplementedException($"trimming value {Trimming}")
			};

			lines[i] = line;
		}

		return string.Join("\n", lines).Trim();
	}

	#endregion
}
