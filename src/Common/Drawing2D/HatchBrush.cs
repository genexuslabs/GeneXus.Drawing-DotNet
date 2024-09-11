using System;
using System.Collections.Generic;
using SkiaSharp;

namespace GeneXus.Drawing.Drawing2D;

public sealed class HatchBrush : Brush
{
	private readonly Color m_fore;
	private readonly Color m_back;
	private readonly HatchStyle m_style;

	/// <summary>
	///  Initializes a new instance of the <see cref="HatchBrush"/> class with the 
	///  specified <see cref="Drawing2D.HatchStyle"/> enumeration, foreground color, and background color.
	/// </summary>
	public HatchBrush(HatchStyle hatchStyle, Color foreColor, Color backColor)
		: base(new SKPaint { IsDither = true })
	{
		m_fore = foreColor;
		m_back = backColor;
		m_style = hatchStyle;

		UpdateShader(() => { });
	}

	/// <summary>
	///  Initializes a new instance of the <see cref="HatchBrush"/> class with the 
	///  specified <see cref="Drawing2D.HatchStyle"/> enumeration and foreground color.
	/// </summary>
	public HatchBrush(HatchStyle hatchStyle, Color foreColor)
		: this(hatchStyle, foreColor, Color.Transparent) { }

	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='HatchBrush'/>.
	/// </summary>
	public override object Clone()
		=> new HatchBrush(m_style, m_fore, m_back);

	#endregion


	#region Properties

	/// <summary>
	///  Gets the color of spaces between the hatch lines drawn by this <see cref='HatchBrush'/> object.
	/// </summary>
	public Color BackgroundColor => m_back;

	/// <summary>
	///  Gets the color of hatch lines drawn by this <see cref='HatchBrush'/> object.
	/// </summary>
	public Color ForegroundColor => m_fore;

	/// <summary>
	///  Gets the hatch style of this <see cref='HatchBrush'/> object.
	/// </summary>
	public HatchStyle HatchStyle => m_style;

	#endregion


	#region Utilities

	private static readonly Dictionary<HatchStyle, (int Width, uint[] Pattern)> HATCH_DATA = new()
	{
		{ HatchStyle.Horizontal,				(8, new uint[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }) },
        { HatchStyle.Vertical,					(8, new uint[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }) },
        { HatchStyle.ForwardDiagonal,			(8, new uint[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 }) },
        { HatchStyle.BackwardDiagonal,			(8, new uint[] { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 }) },
        { HatchStyle.Cross,						(8, new uint[] { 0xFF, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }) },
        { HatchStyle.DiagonalCross, 			(8, new uint[] { 0x81, 0x42, 0x24, 0x18, 0x18, 0x24, 0x42, 0x81 }) },
        { HatchStyle.Percent05, 				(8, new uint[] { 0x80, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00 }) },
        { HatchStyle.Percent10, 				(8, new uint[] { 0x80, 0x00, 0x08, 0x00 }) },
        { HatchStyle.Percent20, 				(4, new uint[] { 0x08, 0x00, 0x02, 0x00 }) },
        { HatchStyle.Percent25, 				(4, new uint[] { 0x08, 0x02 }) },
        { HatchStyle.Percent30, 				(4, new uint[] { 0x0A, 0x04, 0x0A, 0x01 }) },
        { HatchStyle.Percent40, 				(8, new uint[] { 0xAA, 0x55, 0xAA, 0x51, 0xAA, 0x55, 0xAA, 0x15 }) },
        { HatchStyle.Percent50, 				(2, new uint[] { 0x02, 0x01 }) },
        { HatchStyle.Percent60, 				(4, new uint[] { 0x0E, 0x05, 0x0B, 0x05 }) },
        { HatchStyle.Percent70, 				(4, new uint[] { 0x07, 0x0D }) },
        { HatchStyle.Percent75, 				(4, new uint[] { 0x07, 0x0F, 0x0D, 0x0F }) },
        { HatchStyle.Percent80, 				(8, new uint[] { 0xEF, 0xFF, 0x7E, 0xFF }) },
        { HatchStyle.Percent90, 				(8, new uint[] { 0xFF, 0xFF, 0xFF, 0xF7, 0xFF, 0xFF, 0xFF, 0x7F }) },
        { HatchStyle.LightDownwardDiagonal,		(4, new uint[] { 0x08, 0x04, 0x02, 0x01 }) },
        { HatchStyle.LightUpwardDiagonal,		(4, new uint[] { 0x01, 0x02, 0x04, 0x08 }) },
        { HatchStyle.DarkDownwardDiagonal,		(4, new uint[] { 0x0C, 0x06, 0x03, 0x09 }) },
        { HatchStyle.DarkUpwardDiagonal,		(4, new uint[] { 0x03, 0x06, 0x0C, 0x09 }) },
        { HatchStyle.WideDownwardDiagonal,		(8, new uint[] { 0xC1, 0xE0, 0x70, 0x38, 0x1C, 0x0E, 0x07, 0x83 }) },
        { HatchStyle.WideUpwardDiagonal,		(8, new uint[] { 0x83, 0x07, 0x0E, 0x1C, 0x38, 0x70, 0xE0, 0xC1 }) },
        { HatchStyle.LightVertical,				(4, new uint[] { 0x08, 0x08, 0x08, 0x08 }) },
        { HatchStyle.LightHorizontal,			(4, new uint[] { 0x0F, 0x00, 0x00, 0x00 }) },
        { HatchStyle.NarrowVertical,			(2, new uint[] { 0x01, 0x01 }) },
        { HatchStyle.NarrowHorizontal,			(2, new uint[] { 0x03, 0x00 }) },
        { HatchStyle.DarkVertical,				(4, new uint[] { 0x0C, 0x0C, 0x0C, 0x0C }) },
        { HatchStyle.DarkHorizontal,			(4, new uint[] { 0x0F, 0x0F, 0x00, 0x00 }) },
        { HatchStyle.DashedDownwardDiagonal,	(4, new uint[] { 0x00, 0x00, 0x08, 0x04, 0x02, 0x01, 0x00, 0x00 }) },
        { HatchStyle.DashedUpwardDiagonal,		(4, new uint[] { 0x00, 0x00, 0x01, 0x02, 0x04, 0x08, 0x00, 0x00 }) },
        { HatchStyle.DashedHorizontal,			(8, new uint[] { 0xF0, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00 }) },
        { HatchStyle.DashedVertical,			(8, new uint[] { 0x80, 0x80, 0x80, 0x80, 0x08, 0x08, 0x08, 0x08 }) },
        { HatchStyle.SmallConfetti,				(8, new uint[] { 0x80, 0x08, 0x40, 0x03, 0x10, 0x01, 0x20, 0x04 }) },
        { HatchStyle.LargeConfetti,				(8, new uint[] { 0xB1, 0x30, 0x03, 0x1B, 0xD8, 0xC0, 0x06, 0x8D }) },
        { HatchStyle.ZigZag,					(8, new uint[] { 0x81, 0x42, 0x24, 0x18 }) },
        { HatchStyle.Wave,						(8, new uint[] { 0x00, 0x18, 0x25, 0xC0 }) },
        { HatchStyle.DiagonalBrick,				(8, new uint[] { 0x01, 0x02, 0x04, 0x08, 0x18, 0x24, 0x42, 0x81 }) },
        { HatchStyle.HorizontalBrick,			(8, new uint[] { 0xFF, 0x80, 0x80, 0x80, 0xFF, 0x08, 0x08, 0x08 }) },
        { HatchStyle.Weave, 					(8, new uint[] { 0x88, 0x54, 0x22, 0x45, 0x88, 0x14, 0x22, 0x51 }) },
        { HatchStyle.Plaid, 					(8, new uint[] { 0xAA, 0x55, 0xAA, 0x55, 0xF0, 0xF0, 0xF0, 0xF0 }) },
        { HatchStyle.Divot, 					(8, new uint[] { 0x00, 0x10, 0x08, 0x10, 0x00, 0x80, 0x01, 0x80 }) },
        { HatchStyle.DottedGrid, 				(8, new uint[] { 0xAA, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00 }) },
        { HatchStyle.DottedDiamond, 			(8, new uint[] { 0x80, 0x00, 0x22, 0x00, 0x08, 0x00, 0x22, 0x00  }) },
        { HatchStyle.Shingle, 					(8, new uint[] { 0x03, 0x84, 0x48, 0x30, 0x0C, 0x02, 0x01, 0x01 }) },
        { HatchStyle.Trellis,					(4, new uint[] { 0x0F, 0x06, 0x0F, 0x09 }) },
        { HatchStyle.Sphere,					(8, new uint[] { 0x77, 0x89, 0x8F, 0x8F, 0x77, 0x98, 0xF8, 0xF8 }) },
        { HatchStyle.SmallGrid, 				(4, new uint[] { 0x0F, 0x08, 0x08, 0x08 }) },
        { HatchStyle.SmallCheckerBoard,			(4, new uint[] { 0x09, 0x06, 0x06, 0x09 }) },
        { HatchStyle.LargeCheckerBoard,			(8, new uint[] { 0xF0, 0xF0, 0xF0, 0xF0, 0x0F, 0x0F, 0x0F, 0x0F }) },
        { HatchStyle.OutlinedDiamond,			(8, new uint[] { 0x82, 0x44, 0x28, 0x10, 0x28, 0x44, 0x82, 0x01 }) },
        { HatchStyle.SolidDiamond,				(8, new uint[] { 0x10, 0x38, 0x7C, 0xFE, 0x7C, 0x38, 0x10, 0x00 }) }
	};

	private void UpdateShader(Action action)
	{
		action();

		if (!HATCH_DATA.TryGetValue(m_style, out var data))
			throw new NotImplementedException($"hatch style {m_style}.");

		using var bitmap = new SKBitmap(data.Width, data.Pattern.Length);
		using var canvas = new SKCanvas(bitmap);
		using var paint = new SKPaint
		{
			Color = m_fore.m_color,
			Style = SKPaintStyle.Fill,
			IsAntialias = false
		};

		canvas.Clear(m_back.m_color);

		for (int x = 0; x < data.Width; x++)
		{
			for (int y = 0; y < data.Pattern.Length; y++)
			{
				int offset = data.Width - x;
				uint mask = 1u << offset - 1;
				if ((data.Pattern[y] & mask) == mask)
					canvas.DrawPoint(x, y, paint);
			}
		}

		m_paint.Shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
	}


	#endregion
}
