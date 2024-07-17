
using SkiaSharp;

namespace GeneXus.Drawing;

public sealed class SolidBrush : Brush
{
	public SolidBrush(Color color)
		: base(new SKPaint { Color = color.m_color }) { }


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='TextureBrush'/>.
	/// </summary>
	public override object Clone()
		=> new SolidBrush(new Color(m_paint.Color));

	#endregion


	#region Properties

	/// <summary>
	///  Gets or sets the color of this <see cref='SolidBrush'/> object.
	/// </summary>
	public Color Color
	{
		get => new(m_paint.Color);
		set => m_paint.Color = value.m_color;
	}

	#endregion
}
