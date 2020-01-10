using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace XoREngine {
	public class Texture2D : IDisposable {
		private int Handle;
		bool isDisposed;

		public Texture2D(string imageFilePath){
			Handle = GL.GenTexture();
			this.Use();

			Image<Rgba32> image = (Image<Rgba32>)Image.Load(imageFilePath);
			image.Mutate(x => x.Flip(FlipMode.Vertical));

			Rgba32[] tempPixels = image.GetPixelSpan().ToArray();

			byte[] pixels = image.GetPixelSpan()
							.ToArray()
							.Aggregate(
								new List<byte>(), 
								(acc, pixel) => { 
									acc.Add(pixel.R);
									acc.Add(pixel.G);
									acc.Add(pixel.B);
									acc.Add(pixel.A);
									return acc;
								}
							).ToArray();
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			isDisposed = false;
		}

		public void Use(TextureUnit contextTextureUnit = TextureUnit.Texture0){
			GL.ActiveTexture(contextTextureUnit);
			GL.BindTexture(TextureTarget.Texture2D, Handle);
		}

		public void Dispose() {
			if (isDisposed) {
				GL.DeleteTexture(Handle);
				isDisposed = true;
				GC.SuppressFinalize(this);
			}
		}
	}
}
