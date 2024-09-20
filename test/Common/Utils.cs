using System;
using System.IO;
using System.Linq;

namespace GeneXus.Drawing.Test;

internal abstract class Utils
{
	private static readonly string IMAGE_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "images");

	private static readonly double MAX_COLOR_DISTANCE = GetColorDistance(
		Color.FromArgb(0, 0, 0, 0), 
		Color.FromArgb(255, 255, 255, 255));

	public static RectangleF GetBoundingRectangle(PointF[] points)
    {
        if (points == null || points.Length == 0)
            throw new ArgumentException("The points array cannot be null or empty.", nameof(points));
		
        float xMin = points.Min(pt => pt.X);
		float xMax = points.Max(pt => pt.X);
		float yMin = points.Min(pt => pt.Y);
		float yMax = points.Max(pt => pt.Y);
		return new(xMin, yMin, xMax - xMin, yMax - yMin);
    }

	public static PointF GetCenterPoint(PointF[] points)
	{
		float xCenter = points.Average(pt => pt.X);
		float yCenter = points.Average(pt => pt.Y);
		return new(xCenter, yCenter);
	}

	public static double GetColorDistance(Color color1, Color color2)
	{
		double aDiff = Math.Pow(color1.A - color2.A, 2);
		double rDiff = Math.Pow(color1.R - color2.R, 2);
		double gDiff = Math.Pow(color1.G - color2.G, 2);
		double bDiff = Math.Pow(color1.B - color2.B, 2);
		return Math.Sqrt(aDiff + rDiff + gDiff + bDiff);
	}

	public static Color GetAverageColor(Bitmap bm, int x, int y, int radius)
	{
		int aSum = 0, rSum = 0, gSum = 0, bSum = 0, count = 0;
		for (int dy = -radius; dy <= radius; dy++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                int nx = x + dx;
                int ny = y + dy;

				if (nx < 0 || ny < 0 || nx >= bm.Width || ny >= bm.Height)
					continue;

				Color color = bm.GetPixel(nx, ny);
				rSum += color.R;
				gSum += color.G;
				bSum += color.B;
				aSum += color.A;
				count++;
            }
        }
		return Color.FromArgb(aSum / count, rSum / count, gSum / count, bSum / count);
	}

	public static float GetSimilarity(Bitmap bm1, Bitmap bm2, double tolerance = 0.1, int window = 3)
	{
		float hits = 0f; // compare pixel by pixel considering the average in a box of window size
		if (bm1.Size == bm2.Size)
		{
			int radius = window / 2;
			double threshold = tolerance * MAX_COLOR_DISTANCE;
			
			for (int i = 0; i < bm1.Width; i++)
			{
				for (int j = 0; j < bm1.Height; j++)
				{
					Color avg1 = GetAverageColor(bm1, i, j, radius);
					Color avg2 = GetAverageColor(bm2, i, j, radius);

					hits += GetColorDistance(avg1, avg2) < threshold ? 1 : 0;
				}
			}
		}
		return hits / (bm1.Width * bm1.Height);
	}

	public static float CompareImage(string filename, Brush brush, bool save = false)
	{
		string filepath = Path.Combine(IMAGE_PATH, filename);
		using var im = Image.FromFile(filepath);
		using var bm = new Bitmap(im);
		
		var gu = GraphicsUnit.Pixel;
		using var bg = new Bitmap(bm.Width, bm.Height);
		using var g = Graphics.FromImage(bg);
		g.FillRectangle(brush, bg.GetBounds(ref gu));

		return CompareImage(filename, bg, save);
	}

	public static float CompareImage(string filename, Bitmap bg, bool save = false)
	{
		string filepath = Path.Combine(IMAGE_PATH, filename);
		using var im = Image.FromFile(filepath);
		using var bm = new Bitmap(im);

		if (save)
		{
			string savepath = Path.Combine(IMAGE_PATH, ".out", filename);

			string dirpath = Path.GetDirectoryName(savepath);
			if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);

			bg.Save(savepath);
		}

		return GetSimilarity(bg, bm);
	}
}