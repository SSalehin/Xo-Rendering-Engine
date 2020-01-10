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
	class PresentationGame : GameWindow {
		Camera camera;
		UnlitMesh lamp;


		UnlitMesh unlitMesh;
		Vector3 unlitMeshPosition = new Vector3(0f, 0f, -3f);

		Material woodMaterial, metalMaterial, brickMaterial;
		Mesh woodMesh, metalMesh, brickMesh;

		LightHolder lightHolder;
		PointLight whitePointLight;
		DirectionalLight sunLight;


		public PresentationGame(int height, int width, string title)
			: base(height, width, GraphicsMode.Default, title) {
		}

		protected override void OnResize(EventArgs e) {
			GL.Viewport(0, 0, this.Width, this.Height);
			base.OnResize(e);
		}

		protected override void OnLoad(EventArgs e) {
			DefaultGraphicsBehaviourLoader.InitializeDefaultGraphicsBehaviour();
			camera = new Camera((GameWindow)this);
			camera.Move(new Vector3(0f, 0f, 6f));

			lamp = new UnlitMesh("../../TestGeometries/point.obj", "../../Textures/default.jpg");

			unlitMesh = new UnlitMesh("../../TestGeometries/TestGeometry.obj", "../../Textures/WoodTexture.png");


			brickMaterial = new Material(
				"../../Textures/BrickTexture.png", // texturePath 
				Vector3.One, //Ambient color
				0.05f, //Ambient Intensity
				1f, //Specular strength
				4 //Shininess
			);
			brickMesh = new Mesh(
				new Vector3(-3f, 0f, 0f), //Position
				Vector3.Zero, //Rotation
				1f, //Scale
				"../../TestGeometries/TestGeometry.obj", //Geometry path
				brickMaterial //material
			);

			woodMaterial = new Material(
				"../../Textures/WoodTexture.png", // texturePath 
				Vector3.One, //Ambient color
				0.05f, //Ambient Intensity
				5f, //Specular strength
				10 //Shininess
			);
			woodMesh = new Mesh(
				new Vector3(0f, 0f, 3f), //Position
				Vector3.Zero, //Rotation
				1f, //Scale
				"../../TestGeometries/TestGeometry.obj", //Geometry path
				woodMaterial //material
			);

			metalMaterial = new Material(
				"../../Textures/MetalTexture.png", // texturePath 
				Vector3.One, //Ambient color
				0.05f, //Ambient Intensity
				20f, //Specular strength
				20 //Shininess
			);
			metalMesh = new Mesh(
				new Vector3(3f, 0f, 0f), //Position
				Vector3.Zero, //Rotation
				1f, //Scale
				"../../TestGeometries/TestGeometry.obj", //Geometry path
				metalMaterial //material
			);


			lightHolder = new LightHolder();

			whitePointLight = new PointLight(
				new Vector3(0f, 3f, 0f), //position
				new Vector3(1f, 1f, 1f), //color
				0.5f//intensity
			);
			lightHolder.Add(whitePointLight);

			sunLight = new DirectionalLight(
				new Vector3(1f, -1f, 1f), //Direction
				Vector3.One, //Color 
				0.5f //Intensity
			);
			lightHolder.Add(sunLight);

			base.OnLoad(e);
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			camera.Update(e, this);

			KeyboardState input = Keyboard.GetState();
			if (input.IsKeyDown(Key.Escape)) Exit();
			
			base.OnUpdateFrame(e);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			lamp.Draw(Matrix4.CreateTranslation(0f, 3f, 0f), camera.view, camera.projection);

			unlitMesh.Draw(Matrix4.CreateTranslation(unlitMeshPosition), camera.view, camera.projection, DrawMode.WireframeOnShaded);


			brickMesh.PrepareDraw(camera);
			lightHolder.SetLights(brickMesh.shader);
			brickMesh.Draw();

			woodMesh.PrepareDraw(camera);
			lightHolder.SetLights(woodMesh.shader);
			woodMesh.Draw();

			metalMesh.PrepareDraw(camera);
			lightHolder.SetLights(metalMesh.shader);
			metalMesh.Draw();

			Context.SwapBuffers();
			base.OnRenderFrame(e);
		}

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
