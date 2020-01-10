using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XoREngine {
	public class Material : IDisposable {
		private Texture2D texture;
		public readonly Vector3 ambientColor;
		public readonly float ambientStrength;
		public readonly float specularStrength;
		public readonly float shininess;

		private bool isDisposed = false;

		public Material(string texturePath, Vector3 ambientColor, float ambientStrength, float specularStrength, float shininess){
			this.texture = new Texture2D(texturePath);
			this.ambientColor = ambientColor;
			this.ambientStrength = ambientStrength;
			this.specularStrength = specularStrength;
			this.shininess = shininess;
		}

		public void PrepareDraw(Matrix4 model, Camera camera, Shader shader){
			shader.Use();
			shader.SetMatrix4("model", model);
			shader.SetMatrix4("view", camera.view);
			shader.SetMatrix4("projection", camera.projection);
			shader.SetVector3("viewPosition", camera.Position);

			texture.Use();
			shader.SetVector3("material.ambientColor", this.ambientColor);
			shader.SetFloat("material.ambientStrength", this.ambientStrength);
			shader.SetFloat("material.specularStrength", this.specularStrength);
			shader.SetFloat("material.shininess", this.shininess);
		}

		public void Dispose() {
			if(!isDisposed){
				texture.Dispose();
			}
			GC.SuppressFinalize(this);
		}
	}
}
