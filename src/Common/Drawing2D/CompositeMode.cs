namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies how the source colors are combined with the background colors.
/// </summary>
public enum CompositeMode
{
	/// <summary>
	///  Specifies that when a color is rendered, it overwrites the background color.
	/// </summary>
	SourceOver = 0,

	/// <summary>
	///  Specifies that when a color is rendered, it is blended with the background 
	///  color. The blend is determined by the alpha component of the color being rendered.
	/// </summary>
	SourceCopy = 1
}
