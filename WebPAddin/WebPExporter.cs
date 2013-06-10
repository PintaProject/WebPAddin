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
		/// ID for the WebP quality factor setting. This is used when saving or retrieving
		/// the user's preferred quality setting from Pinta's settings.xml file.
		/// </summary>
		private const string WebPQualityFactorSetting = "webp-quality";

		/// <summary>Default quality factor to use when no previous setting exists.</summary>
		private const int DefaultQualityFactor = 85;

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
			// Retrieve the last quality factor setting the user selected, or the default value.
			int quality_factor = PintaCore.Settings.GetSetting<int> (WebPQualityFactorSetting,
			                                                         DefaultQualityFactor);

			// Don't repeatedly prompt the user after they've already selected a
			// quality setting for this document.
			if (!PintaCore.Workspace.ActiveDocument.HasBeenSavedInSession) {
				var dialog = new WebPSettingsDialog (parent, quality_factor);
				try {
					if ((ResponseType)dialog.Run () == ResponseType.Ok) {
						quality_factor = dialog.QualityFactor;
					} else {
						return;
					}
				} finally {
					dialog.Destroy ();
				}
			}

			// Merge all of the layers and convert the image to WebP.
			using (var surface = document.GetFlattenedImage ()) {
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

			// Save the chosen quality setting to Pinta's settings file so that it
			// can be the default the next time a file is saved.
			PintaCore.Settings.PutSetting (WebPQualityFactorSetting, quality_factor);
		}
	}
}

