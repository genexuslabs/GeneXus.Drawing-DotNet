using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneXus.Drawing.Imaging;

public sealed class ColorPalette
{
	private readonly int m_flags;
	private readonly Color[] m_entries;

	internal ColorPalette(int flags, Color[] entries)
	{
		m_flags = flags;
		m_entries = entries;
	}

	/// <summary>
	///  Create a custom color palette.
	/// </summary>
	/// <param name="customColors">Color entries for the palette.</param>
	public ColorPalette(params Color[] colors)
		: this(0, colors) { }

	/// <summary>
	///  Create an optimal color palette based on the colors in a given bitmap.
	/// </summary>
	public static ColorPalette CreateOptimalPalette(int colors, bool useTransparentColor, Bitmap bitmap)
	{
		var entries = QuantizeColors(bitmap, colors, useTransparentColor);
		int flags = 0x0;
		if (HasAlphaInfo(entries))
			flags |= 0x1;
		if (IsGrayScaled(entries))
			flags |= 0x2;
		if (IsHalftone(entries))
			flags |= 0x4;
		return new ColorPalette(flags, entries);
	}


	#region Properties

	/// <summary>
	///  Specifies how to interpret the color information in the array of colors.
	///  - 0x01: The color values in the array contain alpha information.
	///  - 0x02: The colors in the array are grayscale values.
	///  - 0x04: The colors in the array are halftone values.
	/// </summary>
	public int Flags => m_flags;

	/// <summary>
	///  Specifies an array of <see cref='Color'/> objects.
	/// </summary>
	public Color[] Entries => m_entries;

	#endregion


	#region Utilities

	private static Color[] QuantizeColors(Bitmap bitmap, int count, bool useTransparentColor)
	{
		var pixels = new List<Color>();
		for (int x = 0; x < bitmap.Width; x++)
			for (int y = 0; y < bitmap.Height; y++)
				pixels.Add(bitmap.GetPixel(x, y));
		return pixels // median cut cuantization
			.Where(c => useTransparentColor || c != Color.Transparent)
			.GroupBy(c => new { c.R, c.G, c.B })
			.OrderByDescending(g => g.Count())
			.Take(count)
			.Select(g => g.First())
			.ToArray();
	}

	private static bool HasAlphaInfo(Color[] colors)
		=> colors.Any(c => c.A > 255);

	private static bool IsGrayScaled(Color[] colors)
		=> colors.All(c => c.R == c.G && c.G == c.B);

	private static bool IsHalftone(Color[] colors)
	{
		const float DOT_SIZE_VAR_THRESHOLD = 0.2f;
		const float DOT_SPACING_VAR_THRESHOLD = 0.2f;

		for (int i = 1; i < colors.Length; i++)
		{
			var currColor = colors[i];
			var prevColor = colors[i - 1];

			var currDotSize = currColor.GetBrightness();
			var prevDotSize = prevColor.GetBrightness();
			var dotSizeVariation = Math.Abs(currDotSize - prevDotSize) / Math.Max(currDotSize, prevDotSize);
			if (dotSizeVariation > DOT_SIZE_VAR_THRESHOLD)
				return false;

			var currDotSpacing = 1 - currColor.GetBrightness();
			var prevDotSpacing = 1 - prevColor.GetBrightness();
			var dotSpacingVariation = Math.Abs(currDotSpacing - prevDotSpacing) / Math.Max(currDotSpacing, prevDotSpacing);
			if (dotSpacingVariation > DOT_SPACING_VAR_THRESHOLD)
				return false;
		}

		return true;
	}

	#endregion
}
