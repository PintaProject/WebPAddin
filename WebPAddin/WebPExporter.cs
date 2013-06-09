//
// WebPExporter.cs
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
using System.Runtime.InteropServices;
using Gtk;
using Pinta.Core;

namespace WebPAddin
{
	public class WebPExporter : Pinta.Core.IImageExporter
	{
		/// <summary>
		/// Exports a document to a file.
		/// </summary>
		/// <param name='document'>
		/// The document to be saved.
		/// </param>
		/// <param name='fileName'>
		/// File name to save to.
		/// </param>
		/// <param name='parent'>
		/// Window to be used as a parent for any dialogs that are shown.
		/// </param>
		public void Export (Document document, string fileName, Window parent)
		{
			using (var surface = document.GetFlattenedImage ()) {
				// Encode the image.
				var output = IntPtr.Zero;
				uint length = NativeMethods.WebPEncodeBGRA (surface.Data, surface.Width, surface.Height,
				                                            surface.Stride, 80, ref output);
				byte[] data = new byte[length];
				Marshal.Copy (output, data, 0, (int)length);
				// The caller is responsible for calling free() with the array allocated by WebPEncodeBGRA.
				NativeMethods.Free (output);

				// Save the encoded data to the file.
				File.WriteAllBytes (fileName, data);
			}
		}
	}
}

