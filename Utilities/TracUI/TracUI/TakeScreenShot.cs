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
			new Thread(delegate()
			{
				Thread.Sleep(60*1000);
				fs.Flush();
			}).Start();
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