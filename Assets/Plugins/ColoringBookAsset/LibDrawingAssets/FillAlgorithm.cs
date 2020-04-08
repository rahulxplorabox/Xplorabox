using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace pl.ayground
{
	public class FillAlgorithm
	{


		private class imgPixel
		{
			public int x;
			public int y;

			public imgPixel (int _x, int _y)
			{
				x = _x;
				y = _y;
			}
		}



		private bool compare (Color32 c1, Color32 c2)
		{
			if (c1.b != c2.b)
				return false;
			if (c1.r != c2.r)
				return false;
			if (c1.g != c2.g)
				return false;
			return true;
		}

		/// <summary>
		/// FloodFill function for the image.
		/// </summary>
		/// <returns><c>true</c>, if image was filled, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="image">Image</param>
		/// <param name="color">Color</param>
		/// <param name="drawOnBlack">If set to <c>true</c> draw on black. This is used to avoid painting on black lines and destroying the image</param>
		public bool fillImageWithContainer (int x, int y, DrawableTextureContainer image, Color32 color, bool drawOnBlack)
		{
			int width = image.getWidth ();
			int height = image.getHeight ();
			if (x < 0 || x > width)
				return false;
			if (y < 0 || y >= height)
				return false;
			Color32 targetColor = image.pixels [x + image.offsetTable [y]];
			if (compare (targetColor, color)) {
				return false; // No coloring if the target is already using valid color
			}
			if (compare (targetColor, DrawableTextureContainer.BLACK)) {
				if (!drawOnBlack)
					return false; // No coloring in case you do not want to paint on BLACK and the pixel you targeted is BLACK
			}

			int count = 0;
			Queue<imgPixel> q = new Queue<imgPixel> ();
			q.Enqueue (new imgPixel (x, y));
			do {
				if (count++ > DrawableTextureContainer.COLORING_FAILSAFE_LIMIT)
					break;
				// Take the next pixel from coloring queue
				imgPixel n = q.Dequeue ();
				// Proceed if the pixel in queue is in color that requires change (target color)
				if (compare (image.pixels [n.x + image.offsetTable [n.y]], targetColor)) {
					// In THREE simple steps. 

					//1. Color it.
					image.pixels [n.x + image.offsetTable [n.y]] = color;
					// This two below are helper values that track if I need to color the line above and below my current target pixel. 
					bool InitialBottomAddRequired = true; 
					bool InitialTopAddRequired = true;
					// Add the pixels above and below current pixel to the queue if its required. 
					if (n.y + 1 < height)
					if (compare (image.pixels [n.x + image.offsetTable [n.y + 1]], targetColor)) {
						q.Enqueue (new imgPixel (n.x, n.y + 1));
						// Now it will not be required to add them in next loop
						InitialBottomAddRequired = false;
					}
					if (n.y - 1 > 0)
					if (compare (image.pixels [n.x + image.offsetTable [n.y - 1]], targetColor)) {
						q.Enqueue (new imgPixel (n.x, n.y - 1));
						// Now it will not be required to add them in next loop
						InitialTopAddRequired = false;
					}
				

					// 2. Coloring To the Left
					int tmpX = n.x - 1;
					bool addTopRequired = InitialTopAddRequired;
					bool addBottomRequired = InitialBottomAddRequired;
					while (tmpX >= 0) {
						if (compare (image.pixels [tmpX + image.offsetTable [n.y]], targetColor)) {
							image.pixels [tmpX + image.offsetTable [n.y]] = color; // Here the real color change happens.
							if (n.y > 0) {
								if (compare (image.pixels [tmpX + image.offsetTable [n.y - 1]], targetColor)) {
									if (addTopRequired) {
										// I add the pixel above to the coloring queue if thats required
										q.Enqueue (new imgPixel (tmpX, n.y - 1));
										addTopRequired = false;
									}
								} else {
									// when I reach a pixel above that is not in TargetColor I do need to take note of that so I add the Top pixel again when valid
									addTopRequired = true;
								}
							}
							if (n.y < height - 1) { // same logic for bottom pixels
								if (compare (image.pixels [tmpX + image.offsetTable [n.y + 1]], targetColor)) {
									if (addBottomRequired) {
										q.Enqueue (new imgPixel (tmpX, n.y + 1));
										addBottomRequired = false;
									}
								} else {
									addBottomRequired = true;
								}
							}
							tmpX--;
						} else {
							break;
						}
					}
					//3 Coloring to the right now - same logic as to the left
					tmpX = n.x + 1;
					// seting the top/bottom requirements 
					addTopRequired = InitialTopAddRequired;
					addBottomRequired = InitialBottomAddRequired;
					while (tmpX < width) {
						if (compare (image.pixels [tmpX + image.offsetTable [n.y]], targetColor)) {
							image.pixels [tmpX + image.offsetTable [n.y]] = color;
							if (n.y > 0) {
								if (compare (image.pixels [tmpX + image.offsetTable [n.y - 1]], targetColor)) {
									if (addTopRequired) {
										q.Enqueue (new imgPixel (tmpX, n.y - 1));
										addTopRequired = false;
									}
								} else {
									addTopRequired = true;
								}
							}
							if (n.y < height - 1) {

								if (compare (image.pixels [tmpX + image.offsetTable [n.y + 1]], targetColor)) {
									if (addBottomRequired) {
										q.Enqueue (new imgPixel (tmpX, n.y + 1));
										addBottomRequired = false;
									}
								} else {
									addBottomRequired = true;
								}
							}
							tmpX++;
						} else {
							break;
						}
					}
				} 
			} while (q.Count > 0);
			// Mark the whole image as dirty - could be further optimized to track the real DirtyArea (but will need processing time for tracking the DirtyArea Growth, not doing it right not).
			image.MarkFullAsDirty ();
			return true;
		}
	}
}