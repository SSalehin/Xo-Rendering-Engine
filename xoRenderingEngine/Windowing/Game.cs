using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using XoREngine;

namespace Windowing {
	class Game : GameWindow {
		int itr = 0;
		DrawMode drawmode = DrawMode.Shaded;
		Camera camera;

		Geometry lampGeometry;
		Shader lampShader;
		private readonly Vector3[] _cubePositions =
		{
			new Vector3(0.0f, 0.0f, 0.0f),
			new Vector3(2.0f, 5.0f, -15.0f),
			new Vector3(-1.5f, -2.2f, -2.5f),
			new Vector3(-3.8f, -2.0f, -12.3f),
			new Vector3(2.4f, -0.4f, -3.5f),
			new Vector3(-1.7f, 3.0f, -7.5f),
			new Vector3(1.3f, -2.0f, -2.5f),
			new Vector3(1.5f, 2.0f, -2.5f),
			new Vector3(1.5f, 0.2f, -1.5f),
			new Vector3(-1.3f, 1.0f, -1.5f)
		};

		Mesh[] meshes;
		Material material;

		Vector3 lightColor = Vector3.One;
		DirectionalLight light = new DirectionalLight(-1 * Vector3.UnitY, Vector3.One, 1f);
		//DirectionalLight secondLight = new DirectionalLight(-1 * Vector3.UnitY, Vector3.UnitY, 1f);
		PointLight pointLight0, pointLight1;
		SpotLight spotLight;
		LightHolder lightHolder = new LightHolder();

		Vector3 lightPosition = new Vector3(0f, 0.0f, 5.0f);

		public Game (int height, int width, string title)
			: base (height, width, GraphicsMode.Default, title) { 
		}

		protected override void OnResize(EventArgs e) {
			GL.Viewport(0, 0, this.Width, this.Height);
			base.OnResize(e);
		}

		protected override void OnLoad(EventArgs e) {
			DefaultGraphicsBehaviourLoader.InitializeDefaultGraphicsBehaviour();
			camera = new Camera((GameWindow)this);
			camera.Move(new Vector3(5f, 0f, 2f));

			lampGeometry = Geometry.CreateFromOBJ("../../TestGeometries/sphere.obj");
			lampShader = new Shader("../../Shaders/Lit/Vertex.glsl", "../../Shaders/Lit/lampFragment.glsl");


			material = new Material("../../brickTexture.jpg", new Vector3(1.0f,1.0f, 1.0f), 0.05f, 0.5f, 32f);
			meshes = new Mesh[_cubePositions.Length];
			for(int i=0; i<meshes.Length; i++){
				meshes[i] = new Mesh(
					_cubePositions[i],
					new Vector3(1.0f, 0.3f, 0.5f) * 20f * i,
					1f,
					"../../TestGeometries/BoxPositionAndUV.obj",
					material
				);
			}

			lightHolder.Add(light);
			//lightHolder.Add(secondLight);

			pointLight0 = new PointLight(Vector3.One * 4, Vector3.UnitZ, 2f, 0.5f, 0.1f, 0.05f);
			pointLight1 = new PointLight(Vector3.One * -4, Vector3.UnitX, 2f, 0.5f, 0.1f, 0.05f);
			lightHolder.Add(pointLight0);
			lightHolder.Add(pointLight1);

			spotLight = new SpotLight(
				lightPosition,
				camera.frontDirection,
				Vector3.UnitY,
				60f,
				25f,
				2f
			);
			lightHolder.Add(spotLight);

			base.OnLoad(e);
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			camera.Update(e, (GameWindow)this);

			KeyboardState input = Keyboard.GetState();
			if (input.IsKeyDown(Key.Escape)) Exit();
			
			base.OnUpdateFrame(e);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			itr++;
			itr %= 3600;
			Matrix4 lightModel = Matrix4.CreateTranslation(lightPosition);

			spotLight.direction = camera.frontDirection;

			foreach (Mesh mesh in meshes) {
				mesh.PrepareDraw(camera);
				lightHolder.SetLights(mesh.shader);
				mesh.Draw();
			}
			

			
			///-------------Light Object---------------///
			lampShader.Use();
			lampShader.SetMatrix4("model", lightModel);
			lampShader.SetMatrix4("view", camera.view);
			lampShader.SetMatrix4("projection", camera.projection);
			lampShader.SetVector3("color", lightColor);

			GL.BindVertexArray(lampGeometry.shadedVAO);
			GL.DrawArrays(PrimitiveType.Triangles, 0, lampGeometry.triangleCount);

			/////////////////////////////


			Context.SwapBuffers();
			base.OnRenderFrame(e);
		}

		protected override void OnUnload(EventArgs e) {

		}

		//CAREFUL!!!
		//DO NOT DELETE THIS METHOD!!!
		protected override void OnMouseMove(MouseMoveEventArgs e) {
			camera.OnMouseMove(e, (GameWindow)this);
			base.OnMouseMove(e);
		}
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			camera.OnMouseWheel(e);
			base.OnMouseWheel(e);
		}
	}
}
