//
// WebPSettingsDialog.cs
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
using Gtk;
using Mono.Addins;

namespace WebPAddin
{
	public class WebPSettingsDialog : Dialog
	{
		private HScale quality_factor;

		public WebPSettingsDialog (Window parent, int default_quality) : base (
			AddinManager.CurrentLocalizer.GetString ("WebP Quality"), parent, DialogFlags.Modal,
			Stock.Cancel, ResponseType.Cancel, Stock.Ok, ResponseType.Ok)
		{
			BorderWidth = 6;
			VBox.Spacing = 3;

			// Ensure that users can press OK to immediately close the
			// dialog if they don't need to adjust the setting.
			DefaultResponse = ResponseType.Ok;

			var label = new Label (AddinManager.CurrentLocalizer.GetString ("Quality Factor:"));
			label.Xalign = 0;
			VBox.PackStart (label, false, false, 0);

			quality_factor = new HScale (1, 100, 1);
			quality_factor.Value = default_quality;
			VBox.PackStart (quality_factor, false, false, 0);

			VBox.ShowAll ();

			AlternativeButtonOrder = new int[] { (int)ResponseType.Ok, (int)ResponseType.Cancel };
		}

		public int QualityFactor { get {return (int)quality_factor.Value; } }
	}
}

