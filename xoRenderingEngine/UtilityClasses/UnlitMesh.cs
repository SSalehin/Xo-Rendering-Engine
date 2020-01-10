using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace XoREngine {
	public class UnlitMesh : IDisposable {
		private Geometry geometry;
		private Texture2D texture;
		private Shader unlitShader, wireframeShader;

		public Vector4 wireframeColor { get; set; }

		public UnlitMesh(string geometryPath, string texturePath){
			geometry = Geometry.CreateFromOBJ(geometryPath);
			texture = new Texture2D(texturePath);
			unlitShader = new Shader(Configuration.unlitVertextShaderPath, Configuration.unlitFragmentShaderPath);
			wireframeShader = new Shader(Configuration.wireframeVertextShaderPath, Configuration.wireframeFragmentShaderPath);
			wireframeColor = Configuration.defaultWireframeColor;
		}

		public void Draw(Matrix4 model, Matrix4 view, Matrix4 projection, DrawMode mode = DrawMode.Shaded) {
			if (mode == DrawMode.Shaded) DrawShaded(model, view, projection);
			else if (mode == DrawMode.Wireframe) DrawWireframe(model, view, projection);
			else if (mode == DrawMode.WireframeOnShaded){
				DrawShaded(model, view, projection); 
				DrawWireframe(model, view, projection);
			}
		}

		private void DrawShaded(Matrix4 model, Matrix4 view, Matrix4 projection) {
			unlitShader.Use();
			unlitShader.SetMatrix4("model", model);
			unlitShader.SetMatrix4("view", view);
			unlitShader.SetMatrix4("projection", projection);

			texture.Use();
			GL.BindVertexArray(geometry.shadedVAO);
			GL.DrawArrays(PrimitiveType.Triangles, 0, geometry.triangleCount);
		}

		private void DrawWireframe(Matrix4 model, Matrix4 view, Matrix4 projection) {
			wireframeShader.Use();
			wireframeShader.SetMatrix4("model", model);
			wireframeShader.SetMatrix4("view", view);
			wireframeShader.SetMatrix4("projection", projection);
			
			wireframeShader.SetVector4("inColor", new Vector4(1f, 1f, 1f, 1f));
			GL.BindVertexArray(geometry.wireframeVAO);
			GL.DrawArrays(PrimitiveType.Lines, 0, geometry.lineCount);
		}

		public void Dispose() {
			geometry.Dispose();
			texture.Dispose();
			unlitShader.Dispose();
			wireframeShader.Dispose();
		}
	}
}
