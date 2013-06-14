//
// NativeMethods.cs
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
using System.Runtime.InteropServices;

namespace WebPAddin
{
	public class NativeMethods
	{
		[DllImport ("webp")]
		public static extern int WebPGetInfo(byte[] data, uint data_size, ref int width, ref int height);

		[DllImport ("webp")]
		public static extern UIntPtr WebPDecodeBGRAInto(byte[] data, uint data_size, byte[] output_buffer,
		                                                int output_buffer_size, int output_stride);

		[DllImport ("webp")]
		public static extern uint WebPEncodeBGRA(byte[] data, int width, int height, int stride,
		                                         float quality_factor, ref IntPtr output);

		[DllImport ("libc", EntryPoint="free")]
		public static extern void Free (IntPtr ptr);
	}
}

