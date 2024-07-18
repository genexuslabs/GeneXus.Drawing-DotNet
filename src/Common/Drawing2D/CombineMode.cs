namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies how different clipping regions can be combined.
/// </summary>
public enum CombineMode
{
	/// <summary>
	///  One clipping region is replaced by another.
	/// </summary>
	Replace = 0,

	/// <summary>
	///  Two clipping regions are combined by taking their intersection.
	/// </summary>
	Intersect = 1,

	/// <summary>
	///  Two clipping regions are combined by taking the union of both.
	/// </summary>
	Union = 2,

	/// <summary>
	///  Two clipping regions are combined by taking only the areas enclosed by one or the other region, but not both.
	/// </summary>
	Xor = 3,

	/// <summary>
	///  Specifies that the existing region is replaced by the result of the new region being removed from the existing 
	///  region. Said differently, the new region is excluded from the existing region.
	/// </summary>
	Exclude = 4,

	/// <summary>
	///  Specifies that the existing region is replaced by the result of the existing region being removed from the new
	///  region. Said differently, the existing region is excluded from the new region.
	/// </summary>
	Complement = 5
}
