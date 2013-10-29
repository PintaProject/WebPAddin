//
// WebPImporter.cs
//
// Author:
//       Cameron White <cameronwhite91@gmail.com>
//
// Copyright (c) 2013 Cameron White
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using Gdk;
using Pinta.Core;

namespace WebPAddin
{
	public class WebPImporter : Pinta.Core.IImageImporter
	{
		/// <summary>
		/// Imports a document into Pinta.
		/// </summary>
		/// <param name='fileName'>
		/// The name of the file to be imported.
		/// </param>
		/// <param name='parent'>
		/// Window to be used as a parent for any dialogs that are shown.
		/// </param>
		public void Import (string filename, Gtk.Window parent)
		{
			int width = -1, height = -1, stride = -1;
			byte[] image_data = null;

			if (!LoadImage (filename, ref image_data, ref width, ref height, ref stride, parent))
				return;

			// Create a new document and add an initial layer.
			Document doc = PintaCore.Workspace.CreateAndActivateDocument (filename, new Size (width, height));
			doc.HasFile = true;
			doc.Workspace.CanvasSize = doc.ImageSize;
			Layer layer = doc.AddNewLayer (Path.GetFileName (filename));

			// Copy over the image data to the layer's surface.
			CopyToSurface (image_data, layer.Surface);
		}

		/// <summary>
		/// Returns a thumbnail of an image.
		/// If the format provides an efficient way to load a smaller version of
		/// the image, it is suggested to use that method to load a thumbnail
		/// no larger than the given width and height parameters. Otherwise, the
		/// returned pixbuf will need to be rescaled by the calling code if it
		/// exceeds the maximum size.
		/// </summary>
		/// <returns>
		/// The thumbnail, or null if the image could not be loaded.
		/// </returns>
		/// <param name='maxWidth'>
		/// The maximum width of the thumbnail.
		/// </param>
		/// <param name='maxHeight'>
		/// The maximum height of the thumbnail.
		/// </param>
		/// <param name='parent'>
		/// Window to be used as a parent for any dialogs that are shown.
		/// </param>
		public Pixbuf LoadThumbnail (string filename, int maxWidth, int maxHeight, Gtk.Window parent)
		{
			int width = -1, height = -1, stride = -1;
			byte[] image_data = null;

			if (!LoadImage (filename, ref image_data, ref width, ref height, ref stride, parent))
				return null;

			using (var surf = new Cairo.ImageSurface (image_data, Cairo.Format.ARGB32, width, height, stride)) {
				return surf.ToPixbuf ();
			}
		}

		/// <summary>
		/// Load a WebP image.
		/// </summary>
		/// <param name="filename">The name of the image file.</param>
		/// <param name="image_data">An array of BGRA values for the image.</param>
		/// <param name="width">The width of the image.</param>
		/// <param name="height">The height of the image.</param>
		/// <param name="stride">The distance (in bytes) between scanlines.</param>
		private static bool LoadImage (string filename, ref byte[] image_data, ref int width,
		                               ref int height, ref int stride, Gtk.Window parent)
		{
			try {
				byte[] raw_data = File.ReadAllBytes (filename);
				uint raw_data_size = (uint)raw_data.Length;
				width = 0;
				height = 0;

				if (NativeMethods.WebPGetInfo (raw_data, raw_data_size, ref width, ref height) == 0)
					throw new FormatException ("Error loading file info");

				stride = width * ColorBgra.SizeOf;
				int output_buffer_size = height * stride;
				image_data = new byte[output_buffer_size];

				UIntPtr result = NativeMethods.WebPDecodeBGRAInto (raw_data, raw_data_size, image_data,
				                                                   output_buffer_size, stride);
				if (result == UIntPtr.Zero)
					throw new FormatException ("Error loading file");
			} catch (DllNotFoundException) {
				NativeMethods.ShowErrorDialog (parent);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Copy image data to the layer's surface.
		/// </summary>
		/// <param name="image_data">Array of image data in RGBA format.</param>
		private static unsafe void CopyToSurface (byte[] image_data, Cairo.ImageSurface surf)
		{
			if (image_data.Length != surf.Data.Length)
				throw new ArgumentException ("Mismatched image sizes");

			surf.Flush ();

			ColorBgra* dst = (ColorBgra *)surf.DataPtr;
			int len = image_data.Length / ColorBgra.SizeOf;

			fixed (byte *src_bytes = image_data) {
				ColorBgra *src = (ColorBgra *)src_bytes;

				for (int i = 0; i < len; ++i) {
					*dst++ = *src++;
				}
			}

			surf.MarkDirty ();
		}
	}
}

