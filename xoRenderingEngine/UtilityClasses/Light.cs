using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace XoREngine {
	public abstract class Light {
		protected Vector3 color;
		protected float intensity;

		protected Light() {
			this.color = Vector3.One;
			this.intensity = 1f;
		}

		protected Light(Vector3 color, float intensity){
			this.color = color;
			this.intensity = intensity;
		}

		public abstract void SetValues(Shader shader, int index);
	}

	public class DirectionalLight : Light {
		public Vector3 direction;	

		public DirectionalLight(Vector3 direction, Vector3 color, float intensity) : base(color, intensity){
			this.direction = direction;
		}

		public override void SetValues(Shader shader, int index) {
			shader.SetVector3("directionalLights[" + index + "].direction", this.direction);
			shader.SetVector3("directionalLights[" + index + "].color", this.color);
			shader.SetFloat("directionalLights[" + index + "].intensity", this.intensity);
		}
	}

	public class PointLight : Light {
		public Vector3 position;
		public float constant = 0.05f;
		public float linear = 0.05f;
		public float quadratic = 0.05f;

		public PointLight(Vector3 position) : base() {
			this.position = position;
		}

		public PointLight(Vector3 position, Vector3 color, float intensity) : base(color, intensity) {
			this.position = position;
		}

		public PointLight(Vector3 position, Vector3 color, float intensity, float constant, float linear, float quadratic) : base(color, intensity) {
			this.position = position;
			this.constant = constant;
			this.linear = linear;
			this.quadratic = quadratic;
		}

		public override void SetValues(Shader shader, int index) {
			shader.SetVector3("pointLights[" + index + "].position", this.position);
			shader.SetVector3("pointLights[" + index + "].color", this.color);
			shader.SetFloat("pointLights[" + index + "].intensity", this.intensity);
			shader.SetFloat("pointLights[" + index + "].constant", this.constant);
			shader.SetFloat("pointLights[" + index + "].linear", this.linear);
			shader.SetFloat("pointLights[" + index + "].quadratic", this.quadratic);
		}
	}

	public class SpotLight : Light {
		public Vector3 position;
		public Vector3 direction;
		public float cutOffAngle = 30f;
		public float smoothingAngle = 5f;
		public float constant = 0.5f;
		public float linear = 0.1f;
		public float quadratic = 0.05f;

		public SpotLight(Vector3 position, Vector3 direction)
		: base() {
			this.position = position;
			this.direction = direction;
		}

		public SpotLight(Vector3 position, Vector3 direction, Vector3 color, float intensity, float cutOffAngle, float smoothingAngle)
		: base (color, intensity){
			this.position = position;
			this.direction = direction;
			this.cutOffAngle = cutOffAngle;
			this.smoothingAngle = smoothingAngle;
		}

		public SpotLight(Vector3 position, Vector3 direction, Vector3 color, float intensity, float cutOffAngle, float smoothingAngle, float constant, float linear, float quadratic)
		: base(color, intensity) {
			this.position = position;
			this.direction = direction;
			this.cutOffAngle = cutOffAngle;
			this.smoothingAngle = smoothingAngle;
			this.constant = constant;
			this.linear = linear;
			this.quadratic = quadratic;
		}

		public override void SetValues(Shader shader, int index) {
			shader.SetVector3("spotLights[" + index + "].position", this.position);
			shader.SetVector3("spotLights[" + index + "].direction", this.direction);
			shader.SetFloat("spotLights[" + index + "].cutOffAngle", this.cutOffAngle);
			shader.SetFloat("spotLights[" + index + "].smoothingAngle", this.smoothingAngle);
			shader.SetVector3("spotLights[" + index + "].color", this.color);
			shader.SetFloat("spotLights[" + index + "].intensity", this.intensity);
			shader.SetFloat("spotLights[" + index + "].constant", this.constant);
			shader.SetFloat("spotLights[" + index + "].linear", this.linear);
			shader.SetFloat("spotLights[" + index + "].quadratic", this.quadratic);
		}
	}

	public class LightHolder {
		private List<Light> lightList;

		public LightHolder() {
			lightList = new List<Light>();
		}

		public void Add(Light light) {
			if (!lightList.Contains(light)) lightList.Add(light);
		}

		public void Remove(Light light){
			if (lightList.Contains(light)) lightList.Remove(light);
		}

		public void SetLights(Shader shader) {
			List<Light> directionalLights =
				lightList
				.Where(light => light as DirectionalLight != null)
				.ToList();
			shader.SetInt("directionalLightCount", directionalLights.Count);
			for (int i = 0; i < directionalLights.Count; i++) directionalLights[i].SetValues(shader, i);

			List<Light> pointLights =
				lightList
				.Where(light => light as PointLight != null)
				.ToList();
			shader.SetInt("pointLightCount", pointLights.Count);
			for (int i = 0; i < pointLights.Count; i++) pointLights[i].SetValues(shader, i);

			List<Light> spotLights =
				lightList
				.Where(light => light as SpotLight != null)
				.ToList();
			shader.SetInt("spotLightCount", spotLights.Count);
			for (int i = 0; i < spotLights.Count; i++) spotLights[i].SetValues(shader, i);
		}
	}
}
