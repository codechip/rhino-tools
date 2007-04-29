#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;

namespace TracUI
{
	public partial class TakeScreenShot : Form
	{
		[DllImport("winmm.dll")]
		private static extern bool PlaySound(string lpszName, int hModule, int dwFlags);

		private bool dragging;
		private Point prevOffset;

		public TakeScreenShot()
		{
			InitializeComponent();
		}

		private void TakeScreenShot_DoubleClick(object sender, EventArgs e)
		{
			int hdcSrc = User32.GetWindowDC(User32.GetDesktopWindow()),
				hdcDest = GDI32.CreateCompatibleDC(hdcSrc),
				hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, Width, Height);
			GDI32.SelectObject(hdcDest, hBitmap);
			Point pointToScreen = PointToScreen(ClientRectangle.Location);
			GDI32.BitBlt(hdcDest, 0, 0, Width, Height, hdcSrc, pointToScreen.X, pointToScreen.Y, 0x00CC0020);
			Bitmap image = Image.FromHbitmap(new IntPtr(hBitmap));
			BackgroundImage = image;

			Cleanup(hBitmap, hdcSrc, hdcDest);
			PlayCameraClick();
			Close();
		}

		private static void PlayCameraClick()
		{
			Stream wav = Assembly.GetExecutingAssembly().GetManifestResourceStream("TracUI.cameraclick.wav");
			string file = Path.GetTempFileName();
			byte[] buffer = new byte[1024];
			FileStream fs = File.Create(file, 1024, FileOptions.WriteThrough);
			int readBytes;
			do
			{
				readBytes = wav.Read(buffer, 0, buffer.Length);
				fs.Write(buffer, 0, readBytes);
			} while (readBytes != 0);
			fs.Dispose();
			PlaySound(file, 0, 1);
		}

		private void Cleanup(int hBitmap, int hdcSrc, int hdcDest)
		{
			User32.ReleaseDC(User32.GetDesktopWindow(), hdcSrc);
			GDI32.DeleteDC(hdcDest);
			GDI32.DeleteObject(hBitmap);
		}

		private void TakeScreenShot_MouseDown(object sender, MouseEventArgs e)
		{
			dragging = true;
			Point pointToScreen = PointToScreen(e.Location);
			prevOffset = new Point(pointToScreen.X - Location.X, pointToScreen.Y - Location.Y);
		}

		private void TakeScreenShot_MouseMove(object sender, MouseEventArgs e)
		{
			if (dragging)
			{
				Point pointToScreen = PointToScreen(e.Location);
				Location = new Point(pointToScreen.X - prevOffset.X, pointToScreen.Y - prevOffset.Y);
			}
		}

		private void TakeScreenShot_MouseUp(object sender, MouseEventArgs e)
		{
			dragging = false;
		}

		private void TakeScreenShot_SizeChanged(object sender, EventArgs e)
		{
			BackgroundImage = null;
		}
	}

	internal class GDI32
	{
		[DllImport("GDI32.dll")]
		public static extern bool BitBlt(
			int hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, int hdcSrc, int nXSrc, int nYSrc, int dwRop);

		[DllImport("GDI32.dll")]
		public static extern int CreateCompatibleBitmap(int hdc, int nWidth, int nHeight);

		[DllImport("GDI32.dll")]
		public static extern int CreateCompatibleDC(int hdc);

		[DllImport("GDI32.dll")]
		public static extern bool DeleteDC(int hdc);

		[DllImport("GDI32.dll")]
		public static extern bool DeleteObject(int hObject);

		[DllImport("GDI32.dll")]
		public static extern int GetDeviceCaps(int hdc, int nIndex);

		[DllImport("GDI32.dll")]
		public static extern int SelectObject(int hdc, int hgdiobj);
	}

	internal class User32
	{
		[DllImport("User32.dll")]
		public static extern int GetDesktopWindow();

		[DllImport("User32.dll")]
		public static extern int GetWindowDC(int hWnd);

		[DllImport("User32.dll")]
		public static extern int ReleaseDC(int hWnd, int hDC);
	}
}