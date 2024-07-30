using System;
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
		: base(new SKPaint { })
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

	private void UpdateShader(Action action)
	{
		action();

		var size = 10f;
		using var pattern = new SKBitmap((int)size, (int)size);

		using var canvas = new SKCanvas(pattern);
		canvas.Clear(m_back.m_color);

		var paint = new SKPaint
		{
			Color = m_fore.m_color,
			Style = SKPaintStyle.Stroke,
			StrokeWidth = 1,
			IsAntialias = true
		};

		switch (m_style)
		{
			case HatchStyle.Horizontal:
				canvas.DrawLine(0, size / 2, size, size / 2, paint);
				break;

			case HatchStyle.Vertical:
				canvas.DrawLine(size / 2, 0, size / 2, size, paint);
				break;

			case HatchStyle.ForwardDiagonal:
				canvas.DrawLine(0, 0, size, size, paint);
				break;

			case HatchStyle.BackwardDiagonal:
				canvas.DrawLine(size, 0, 0, size, paint);
				break;

			case HatchStyle.Cross:
				canvas.DrawLine(0, size / 2, size, size / 2, paint);
				canvas.DrawLine(size / 2, 0, size / 2, size, paint);
				break;

			case HatchStyle.DiagonalCross:
				canvas.DrawLine(0, 0, size, size, paint);
				canvas.DrawLine(size, 0, 0, size, paint);
				break;

			case HatchStyle.DashedDownwardDiagonal:
				paint.PathEffect = SKPathEffect.CreateDash(new float[] { 4, 2 }, 0);
				canvas.DrawLine(0, 0, size, size, paint);
				break;

			case HatchStyle.DashedUpwardDiagonal:
				paint.PathEffect = SKPathEffect.CreateDash(new float[] { 4, 2 }, 0);
				canvas.DrawLine(0, size, size, 0, paint);
				break;

			case HatchStyle.DashedHorizontal:
				paint.PathEffect = SKPathEffect.CreateDash(new float[] { 4, 2 }, 0);
				canvas.DrawLine(0, size / 2, size, size / 2, paint);
				break;

			case HatchStyle.DashedVertical:
				paint.PathEffect = SKPathEffect.CreateDash(new float[] { 4, 2 }, 0);
				canvas.DrawLine(size / 2, 0, size / 2, size, paint);
				break;

			case HatchStyle.SmallConfetti:
				DrawConfettiPattern(canvas, paint, size, 3);
				break;

			case HatchStyle.LargeConfetti:
				DrawConfettiPattern(canvas, paint, size, 5);
				break;

			case HatchStyle.ZigZag:
				for (int i = 0; i < size; i += 2)
				{
					canvas.DrawLine(i, 0, i + 1, size, paint);
					canvas.DrawLine(i + 1, size, i + 2, 0, paint);
				}
				break;

			case HatchStyle.Wave:
				for (int i = 0; i < size; i += 2)
				{
					canvas.DrawLine(i, size / 2, i + 1, 0, paint);
					canvas.DrawLine(i + 1, 0, i + 2, size / 2, paint);
					canvas.DrawLine(i + 2, size / 2, i + 3, size, paint);
					canvas.DrawLine(i + 3, size, i + 4, size / 2, paint);
				}
				break;

			case HatchStyle.Weave:
				for (int i = 0; i < size; i += 4)
				{
					canvas.DrawLine(i, 0, i, size, paint);
					canvas.DrawLine(0, i, size, i, paint);
				}
				break;

			case HatchStyle.Plaid:
				for (int i = 0; i < size; i += 2)
				{
					canvas.DrawLine(i, 0, i, size, paint);
					canvas.DrawLine(0, i, size, i, paint);
				}
				break;

			case HatchStyle.Divot:
				for (int i = 0; i < size; i += 4)
				{
					canvas.DrawLine(i, 0, i + 2, size, paint);
					canvas.DrawLine(i + 2, 0, i + 4, size, paint);
				}
				break;

			case HatchStyle.Shingle:
				for (int i = 0; i < size; i += 4)
				{
					canvas.DrawLine(i, 0, i + 2, size, paint);
					canvas.DrawLine(i + 2, 0, i + 4, size, paint);
				}
				break;

			case HatchStyle.Trellis:
				for (int i = 0; i < size; i += 2)
				{
					canvas.DrawLine(i, 0, i, size, paint);
					canvas.DrawLine(0, i, size, i, paint);
				}
				break;

			case HatchStyle.Sphere:
				for (int i = 0; i < size; i += 4)
					for (int j = 0; j < size; j += 4)
						canvas.DrawCircle(i + 2, j + 2, 2, paint);
				break;

			case HatchStyle.SmallGrid:
				for (int i = 0; i < size; i += 2)
				{
					canvas.DrawLine(i, 0, i, size, paint);
					canvas.DrawLine(0, i, size, i, paint);
				}
				break;

			case HatchStyle.DottedGrid:
				for (int i = 0; i < size; i += 2)
					for (int j = 0; j < size; j += 2)
						canvas.DrawPoint(i, j, paint);
				break;

			case HatchStyle.DottedDiamond:
				for (int i = 0; i < size; i += 2)
				{
					canvas.DrawPoint(i, i, paint);
					canvas.DrawPoint(size - i, i, paint);
				}
				break;

			case HatchStyle.SolidDiamond:
				for (int i = 0; i < size; i += 4)
				{
					canvas.DrawLine(i, 0, i + 2, size, paint);
					canvas.DrawLine(i + 2, 0, i + 4, size, paint);
				}
				break;

			case HatchStyle.OutlinedDiamond:
				for (int i = 0; i < size; i += 4)
				{
					canvas.DrawLine(i, 0, i + 2, size, paint);
					canvas.DrawLine(i + 2, 0, i + 4, size, paint);
				}
				break;

			case HatchStyle.DiagonalBrick:
				for (int i = 0; i < size; i += 4)
				{
					canvas.DrawLine(i, 0, i + 2, size, paint);
					canvas.DrawLine(i + 2, 0, i + 4, size, paint);
				}
				break;

			case HatchStyle.HorizontalBrick:
				for (int i = 0; i < size; i += 4)
				{
					canvas.DrawLine(0, i, size, i, paint);
					canvas.DrawLine(0, i + 2, size, i + 2, paint);
				}
				break;

			case HatchStyle.SmallCheckerBoard:
				DrawCheckerBoardPattern(canvas, paint, m_fore.m_color, m_back.m_color, size, 2);
				break;

			case HatchStyle.LargeCheckerBoard:
				DrawCheckerBoardPattern(canvas, paint, m_fore.m_color, m_back.m_color, size, 4);
				break;

			case HatchStyle.Percent05:
				DrawPercentagePattern(canvas, paint, size, 5);
				break;

			case HatchStyle.Percent10:
				DrawPercentagePattern(canvas, paint, size, 10);
				break;

			case HatchStyle.Percent20:
				DrawPercentagePattern(canvas, paint, size, 20);
				break;

			case HatchStyle.Percent25:
				DrawPercentagePattern(canvas, paint, size, 25);
				break;

			case HatchStyle.Percent30:
				DrawPercentagePattern(canvas, paint, size, 30);
				break;

			case HatchStyle.Percent40:
				DrawPercentagePattern(canvas, paint, size, 40);
				break;

			case HatchStyle.Percent50:
				DrawPercentagePattern(canvas, paint, size, 50);
				break;

			case HatchStyle.Percent60:
				DrawPercentagePattern(canvas, paint, size, 60);
				break;

			case HatchStyle.Percent70:
				DrawPercentagePattern(canvas, paint, size, 70);
				break;

			case HatchStyle.Percent75:
				DrawPercentagePattern(canvas, paint, size, 75);
				break;

			case HatchStyle.Percent80:
				DrawPercentagePattern(canvas, paint, size, 80);
				break;

			case HatchStyle.Percent90:
				DrawPercentagePattern(canvas, paint, size, 90);
				break;

			// TODO: add other styles
			default:
				throw new NotSupportedException($"Hatch style {m_style} is not supported.");
		}

		m_paint.Shader = SKShader.CreateBitmap(pattern, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
	}

	private static void DrawPercentagePattern(SKCanvas canvas, SKPaint paint, float size, int percent)
	{
		var rectSize = (int)(size * percent / 100.0);
		for (int y = 0; y < size; y += rectSize)
			for (int x = 0; x < size; x += rectSize)
				canvas.DrawRect(new SKRect(x, y, x + rectSize, y + rectSize), paint);
	}

	private static void DrawConfettiPattern(SKCanvas canvas, SKPaint paint, float size, int pieces)
	{
		var isize = (int)Math.Ceiling(size);
		var rand = new Random();
		for (int i = 0; i < pieces; i++)
		{
			int x = rand.Next(isize);
			int y = rand.Next(isize);
			canvas.DrawPoint(x, y, paint);
		}
	}

	private static void DrawCheckerBoardPattern(SKCanvas canvas, SKPaint paint, SKColor fore, SKColor back, float size, int checkerSize)
	{
		paint.Color = back;
		for (int i = 0; i < size; i += checkerSize)
		{
			for (int j = 0; j < size; j += checkerSize)
			{
				paint.Color = paint.Color == fore ? back : fore;
				canvas.DrawRect(i, j, checkerSize, checkerSize, paint);
			}
		}
	}


	#endregion
}
