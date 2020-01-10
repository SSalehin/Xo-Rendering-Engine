using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using XoREngine;

namespace XoREngine {
	public static class DefaultGraphicsBehaviourLoader {
		public static void InitializeDefaultGraphicsBehaviour() {
			DefaultTextureBehaviourLoader.InitializeDefaultTextureBehaviour();
			//GL.ClearColor(0.2f, 0.3f, 0.5f, 1.0f);
			GL.ClearColor(Configuration.defaultClearColor.X, Configuration.defaultClearColor.Y, Configuration.defaultClearColor.Z, Configuration.defaultClearColor.W);
			GL.Enable(EnableCap.DepthTest);
		}
	}

	static class DefaultTextureBehaviourLoader {
		//Texture wrap mode : same for S, T and R coordinates
		private static TextureWrapMode defaultTextureWrapMode = TextureWrapMode.Repeat; 

		//TextureFiltering
		private static TextureMinFilter defaultTextureMinFilter = TextureMinFilter.Nearest;
		private static TextureMagFilter defaultTextureMagFilter = TextureMagFilter.Linear; 

		public static void InitializeDefaultTextureBehaviour() {
			//Set Wrap mode
			//For 2D:
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)defaultTextureWrapMode);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)defaultTextureWrapMode);
			//For 3D:
			GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapR, (int)defaultTextureWrapMode);
			GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapS, (int)defaultTextureWrapMode);
			GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureWrapT, (int)defaultTextureWrapMode);

			//Set filters:
			//For 2D:
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)defaultTextureMinFilter);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)defaultTextureMagFilter);
			//For 3D:
			GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMinFilter, (int)defaultTextureMinFilter);
			GL.TexParameter(TextureTarget.Texture3D, TextureParameterName.TextureMagFilter, (int)defaultTextureMagFilter);
		}
	}
}
