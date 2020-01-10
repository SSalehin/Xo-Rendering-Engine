using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XoREngine {
	public class Mesh {
		public Vector3 Position {
			get { return this.position; }
			set {
				this.position = value;
				ReaclculateModel();
			}
		}
		public Vector3 Rotation {
			get { return this.rotation; }
			set {
				this.rotation = value;
				ReaclculateModel();
			}
		}
		public float Scale {
			get { return this.scale; }
			set {
				this.scale = value;
				ReaclculateModel();
			}
		}

		private Vector3 position;
		private Vector3 rotation;
		private float scale;

		public readonly Geometry geometry;
		public readonly Shader shader;
		public Material material;

		private Matrix4 model;

		public Mesh(Vector3 position, Vector3 rotation, float scale, string geometryPath, Material material){
			this.Position = position;
			this.Rotation = rotation;
			this.Scale = scale;
			this.geometry = Geometry.CreateFromOBJ(geometryPath);
			this.material = material;
			this.shader = new Shader("../../Shaders/Lit/Vertex.glsl", "../../Shaders/Lit/CombinedFragment.glsl");
		}

		public void PrepareDraw(Camera camera) {
			shader.Use();
			material.PrepareDraw(model, camera, shader);
		}

		public void Draw() {
			GL.BindVertexArray(geometry.shadedVAO);
			GL.DrawArrays(PrimitiveType.Triangles, 0, geometry.triangleCount);
		}

		private void ReaclculateModel() {
			this.model =
					Matrix4.CreateTranslation(this.position) *
					Matrix4.CreateRotationX(this.rotation.X) *
					Matrix4.CreateRotationY(this.rotation.Y) *
					Matrix4.CreateRotationZ(this.rotation.Z) *
					Matrix4.CreateScale(this.scale);
		}
	}
}
