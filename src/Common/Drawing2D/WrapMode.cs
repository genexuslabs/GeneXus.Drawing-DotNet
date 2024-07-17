namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies how a texture or gradient is tiled when it is smaller than the area being filled.
/// </summary>
public enum WrapMode
{
	/// <summary>
	///  Tiles the gradient or texture.
	/// </summary>
	Tile = 0,

	/// <summary>
	///  Reverses the texture or gradient horizontally and then tiles the texture or gradient.
	/// </summary>
	TileFlipX = 1,

	/// <summary>
	///  Reverses the texture or gradient vertically and then tiles the texture or gradient.
	/// </summary>
	TileFlipY = 2,

	/// <summary>
	///  Reverses the texture or gradient horizontally and vertically and then tiles the texture or gradient.
	/// </summary>
	TileFlipXY = 3,

	/// <summary>
	///  The texture or gradient is not tiled.
	/// </summary>
	Clamp = 4
}
