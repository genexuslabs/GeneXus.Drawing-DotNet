namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies the system to use when evaluating coordinates.
/// </summary>
public enum CoordinateSpace
{
	/// <summary>
	///  Specifies that coordinates are in the world coordinate context. World coordinates are 
	///  used in a nonphysical environment, such as a modeling environment.
	/// </summary>
	World = 0,

	/// <summary>
	///  Specifies that coordinates are in the page coordinate context. Their units are defined 
	///  by the <see cref='Graphics.PageUnit'/> property, and must be one of the elements of 
	///  the <see cref='GraphicsUnit'/> enumeration.
	/// </summary>
	Page = 1,

	/// <summary>
	///  Specifies that coordinates are in the device coordinate context. On a computer screen 
	///  the device coordinates are usually measured in pixels.
	/// </summary>
	Device = 2
}
