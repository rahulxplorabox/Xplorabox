using System;
using UnityEngine;
using UnityEngine.UI;

namespace pl.ayground
{
	/// <summary>
	/// Drawable texture container - a data structure to handle drawing operations with ease
	/// </summary>
	public class DrawableTextureContainer
	{
		public static Color32 BLACK = new Color32 (0, 0, 0, 255);
		public static Color32 WHITE = new Color32 (255, 255, 255, 255);
		public static int COLORING_FAILSAFE_LIMIT = 256000;

		/// <summary>
		/// Resolve TextureFormat that shall be used. ARGB32 is the preferred one.
		/// </summary>
		/// <returns>The TextureFormat supported by the system.</returns>
		public static TextureFormat TextureFormatUsed ()
		{
			if (SystemInfo.SupportsTextureFormat (TextureFormat.ARGB32)) {
				return TextureFormat.ARGB32;
			}
			if (SystemInfo.SupportsTextureFormat (TextureFormat.RGBA4444)) {
				return TextureFormat.RGBA4444;
			}
			if (SystemInfo.SupportsTextureFormat (TextureFormat.RGBA32)) {
				return TextureFormat.RGBA32;
			}

			if (SystemInfo.SupportsTextureFormat (TextureFormat.RGBAFloat)) {
				return TextureFormat.RGBAFloat;
			}
			// fail :(
			throw new NotSupportedException("Unable to find a supported TextureFormat on current platform. DrawableTextureContainer.TextureFormatUsed()");

		}

		/// <summary>
		/// Rectangle on a texture - used to handle DirtyArea processing.
		/// </summary>
		private class TextureRegion
		{
			public int x = 0;
			public int y = 0;
			public int width = 0;
			public int height = 0;

			public TextureRegion (int _x, int _y, int _width, int _height, int _textureWidth, int _textureHeight)
			{
				x = Math.Max (_x, 0);
				y = Math.Max (_y, 0);
				width = Math.Min (_width, _textureWidth);
				height = Math.Min (_height, _textureHeight);
			}

			public TextureRegion ()
			{
				x = 0;
				y = 0;
				width = 0;
				height = 0;
			}

			public override string ToString ()
			{
				return "Dirty area (x,y) = " + x + "," + y + " width: " + width + ", height:" + height;
			}

		}


		/// <summary>
		/// This is where I store all the image pixels as Color32 entries
		/// </summary>
		public Color32[] pixels;
		/// <summary>
		/// And here goes the rows offset table (used to get the row start position)
		/// </summary>
		public int[] offsetTable;

		private Texture2D texture;
		private int width = -1;
		private int height = -1;
		private bool isDirty = false;
		private TextureRegion dirtyArea = new TextureRegion ();

		public int getWidth ()
		{ 
			return width; 
		}

		public int getHeight ()
		{
			return height;
		}

		private TextureRegion getDirtyArea ()
		{
			return dirtyArea;
		}

		/// <summary>
		/// Returns the image as Pixels array
		/// </summary>
		/// <returns>The pixels.</returns>
		public Color32[] getPixels ()
		{
			return pixels;
		}

		/// <summary>
		/// Sets whole image as dirty
		/// </summary>
		public void MarkFullAsDirty ()
		{
			addDirtyZone (0, 0, width, height);
		}


		/// <summary>
		/// Converts to pure B&W image, using the ratio as the treshold
		/// </summary>
		/// <param name="ratio">0 to 1 treshold ratio for conversion</param>
		public void ConvertToPureBW (float ratio)
		{
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					ConvertPixelToPureBW (x, y, ratio);
				}
			}
			texture.SetPixels32 (pixels);
			texture.Apply ();
		}

		private void ConvertPixelToPureBW (int x, int y, float ratio)
		{
			Color32 c = pixels [x + offsetTable [y]];
			float pixelRatio = (float)(c.r + c.g + c.b) / (float)(255 * 3);
			if (pixelRatio > ratio) {
				pixels [x + offsetTable [y]] = DrawableTextureContainer.WHITE;
			} else {
				pixels [x + offsetTable [y]] = DrawableTextureContainer.BLACK;
			}
		}

		/// <summary>
		/// Initialize the container based on Texture2D source, and does some conversions if required.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="convertToPureBW">If set to <c>true</c> convert to pure B&W </param>
		/// <param name="doBradleyTreshold">If set to <c>true</c> do bradley treshold [binarize].</param>
		public DrawableTextureContainer (Texture2D source, bool convertToPureBW, bool doBradleyTreshold)
		{
			width = source.width;
			height = source.height;
			dirtyArea = new TextureRegion ();
			isDirty = false;
			texture = new Texture2D (width, height, DrawableTextureContainer.TextureFormatUsed (), false);
			texture.SetPixels32 (source.GetPixels32 ());
			texture.Apply ();
			pixels = new Color32[width * height];
			pixels = source.GetPixels32 ();

			offsetTable = new int[height];
			for (int i = 0; i < height; i++) {
				offsetTable [i] = i * width;
			}
			if (convertToPureBW) {
				if (doBradleyTreshold) {
					BradleyLocalTreshold ();
				} else {
					ConvertToPureBW (0.5f);
				}
			}
		}

		private void BradleyLocalTreshold ()
		{
			int windowSize = 41;
			float pixelBrightnessDifferenceLimit = 0.15f;
			IntegralImage integralImage = new IntegralImage (this);
			int widthM1 = width - 1;
			int heightM1 = height - 1;
			int radius = windowSize / 2;
			float avgBrightnessPart = 1.0f - pixelBrightnessDifferenceLimit;
			for (int x = 0; x < width; x++) {
				int x1 = Math.Max (x - radius, 0);
				int x2 = Math.Min (x + radius, widthM1);
				for (int y = 0; y < height; y++) {
					int y1 = Math.Max (y - radius, 0);
					int y2 = Math.Min (y + radius, heightM1);
					float source = getGrayFloat (x, y);
					float avg = integralImage.getRectangleMeanUnsafe (x1, y1, x2, y2);
					if (source <= avg * avgBrightnessPart) {
						pixels [x + y * width] = DrawableTextureContainer.BLACK;
					} else {
						pixels [x + y * width] = DrawableTextureContainer.WHITE;
					}
				}
			}
			texture.SetPixels32 (pixels);
			texture.Apply ();
		}

		/// <summary>
		/// Gets the pixel grayscale value as float - used for BradleyTreshold
		/// </summary>
		/// <returns>grayscale value of a pixel or 0 if OutOfBounds occured</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public float getGrayFloat (int x, int y)
		{
			try {
				Color32 c = pixels [x + y * width];
				float r = (float)c.r / 255;
				float g = (float)c.g / 255;
				float b = (float)c.b / 255;
				return new Color (r, g, b, 1).grayscale;
			} catch (Exception e) {
				Debug.Log ("OutOfBounds while trying to getGrayFloat at :" + x + "," + y + "\n" + e);
				return 0;
			}
		}


		/// <summary>
		/// Flood fill a color in the provided location
		/// </summary>
		/// <returns><c>true</c>, if the image was indeed painted, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color to use for flood fill</param>
		public bool PaintBucketTool (int x, int y, Color32 color)
		{
			bool retVal = new FillAlgorithm ().fillImageWithContainer (x, y, this, color, false);
			texture.Apply ();
			return retVal;
		}


		private bool IsInImageBounds (int x, int y)
		{
			if (x < 0)
				return false;
			if (y < 0)
				return false;
			if (x > width)
				return false;
			if (y > height)
				return false;
			return true;
		}

		/// <summary>
		/// Gets the color of the pixel.
		/// </summary>
		/// <returns>The pixel color.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Color32 getPixelColor (int x, int y)
		{
			return pixels [x + offsetTable [y]];
		}

		/// <summary>
		/// Random PaintBucketTool execution - random location and color.
		/// </summary>
		public void RandomPaintBucketTool ()
		{
			int x = UnityEngine.Random.Range (0, width);
			int y = UnityEngine.Random.Range (0, height);
			Color32 c = new Color32 ((byte)UnityEngine.Random.Range (0, 255), (byte)UnityEngine.Random.Range (0, 255), (byte)UnityEngine.Random.Range (0, 255), 255);
			PaintBucketTool (x, y, c);
		}

		/// <summary>
		/// Random PaintBucketTool execution - set your location and color will be randomized.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void RandomPaintBucketToolInPosition (int x, int y)
		{
			Color32 c = new Color32 ((byte)UnityEngine.Random.Range (0, 255), (byte)UnityEngine.Random.Range (0, 255), (byte)UnityEngine.Random.Range (0, 255), 255);
			PaintBucketTool (x, y, c);
		}


		private void addDirtyZone (int x, int y, int width, int height)
		{
			isDirty = true;
			if (dirtyArea.width == 0) {
				dirtyArea = new TextureRegion (x, y, width, height, this.width, this.height);
			} else {
				int newStartX = Math.Min (x, dirtyArea.x);
				int newStartY = Math.Min (y, dirtyArea.y);
				int newEndX = Math.Max (x + width, dirtyArea.x + dirtyArea.width);
				int newEndY = Math.Max (y + height, dirtyArea.y + dirtyArea.height);
				dirtyArea = new TextureRegion (newEndX, newStartY, newEndX - newStartX, newEndY - newStartY, this.width, this.height);
			}
		}

		private Color32[] getDirtyAreaAsColorArray ()
		{
			int w = (int)dirtyArea.width;
			int h = (int)dirtyArea.height;
			Color32[] retVal = new Color32[w * h];
			int startingPixelPosition = dirtyArea.y * this.width;
			for (int row = 0; row < h; row++) {
				for (int cell = 0; cell < w; cell++) {
					retVal [row * w + cell] = pixels [startingPixelPosition + dirtyArea.x + cell + row * this.width];
				}
			}
			return retVal;
		}

		/// <summary>
		/// Returns current image as a Texture2D for display purposes
		/// </summary>
		/// <returns>current state of the image as a Texture2D</returns>
		public Texture2D getTexture ()
		{
			if (isDirty) {
				if (this.dirtyArea.width > 0) {
					texture.SetPixels32 (dirtyArea.x, dirtyArea.y, dirtyArea.width, dirtyArea.height, getDirtyAreaAsColorArray ());
					texture.Apply ();
					isDirty = false;
					dirtyArea = new TextureRegion ();
				} else {
					isDirty = false;
					texture.SetPixels32 (pixels);
					texture.Apply ();
				}
			}
			return texture;
		}
	}
}
