using System;
using UnityEngine;

namespace pl.ayground
{
	/// <summary>
	/// Supporting class required for BradleyLocalTreshold logic. Check https://en.wikipedia.org/wiki/Summed_area_table for more info.
	/// </summary>
	public class IntegralImage
	{
		
		public int Width { get { return width; } }

		public int Height { get { return height; } }

		private float[,] image; 
		private int height;
		private int width;

		/// <summary>
		/// Initializes a new instance of the <see cref="pl.ayground.IntegralImage"/> class.
		/// </summary>
		/// <param name="container">DrawableContainer to use as a base</param>
		public IntegralImage (DrawableTextureContainer container)
		{
			width = container.getWidth ();
			height = container.getHeight (); 
			image = new float[width + 1, height + 1];
			for (int y = 1; y < height + 1; y++) {
				float rowSum = 0;
				for (int x = 1; x < width + 1; x++) {
					rowSum += container.getGrayFloat (x - 1, y - 1);
					image [x, y] = rowSum + image [x, y - 1];
				}
			}
		}

		/// <summary>
		/// Gets the rectangle mean unsafe = return sum divided by actual rectangles size. This is unsafe method - do not call for OutOfBounds conditions :) 
		/// </summary>
		/// <returns>pixels sum divided by actual rectangles size</returns></returns>
		/// <param name="x1">The topLeft x value.</param>
		/// <param name="y1">The topLeft y value.</param>
		/// <param name="x2">The bottomRight x value.</param>
		/// <param name="y2">The bottomRight y value.</param>
		public float getRectangleMeanUnsafe (int x1, int y1, int x2, int y2)
		{
			x2++;
			y2++;
			return (float)((double)(image [x2, y2] + image [x1, y1] - image [x1, y2] - image [x2, y1]) / (double)((x2 - x1) * (y2 - y1)));
		}
	}
}