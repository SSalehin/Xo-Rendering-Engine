using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XoREngine {
	static class Configuration {
		public static string wireframeVertextShaderPath = "../../Shaders/Wireframe/Vertex.glsl";
		public static string wireframeFragmentShaderPath = "../../Shaders/Wireframe/Fragment.glsl";

		public static string unlitVertextShaderPath = "../../Shaders/Unlit/Vertex.glsl";
		public static string unlitFragmentShaderPath = "../../Shaders/Unlit/Fragment.glsl";

		public static string defaultTexturePath = "../../Textures/default.jpg";

		public static Vector4 defaultWireframeColor = Vector4.One;

		public static Vector4 defaultClearColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);

		public static float minimmumFOV = 1f;
		public static float maximmumFOV = 90f;
	}
	public enum DrawMode {
		Wireframe, Shaded, WireframeOnShaded
	}
}
