using SkiaSharp;
using System;

namespace GeneXus.Drawing.Drawing2D;

public sealed class GraphicsState : MarshalByRefObject
{
    internal int m_index;
    internal SKCanvas m_canvas;

    internal GraphicsState(int state) => m_index = state;
}