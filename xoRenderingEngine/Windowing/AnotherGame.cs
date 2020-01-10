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
	class AnotherGame : GameWindow {
		Camera camera;
		DrawMode drawMode;

		KeyboardState previousState;

		UnlitMesh indicator;
		Vector3 indicatorPosition = new Vector3(0f, 20f, 0f);

		Material manMaterial, woodMaterial, groundMaterial;
		Mesh manMesh, woodMesh, groundMesh;

		LightHolder lightHolder = new LightHolder();
		DirectionalLight sun;
		SpotLight spotLight;

		public AnotherGame(int height, int width, string title)
			: base(height, width, GraphicsMode.Default, title) {
		}

		protected override void OnResize(EventArgs e) {
			GL.Viewport(0, 0, this.Width, this.Height);
			base.OnResize(e);
		}

		protected override void OnLoad(EventArgs e) {
			DefaultGraphicsBehaviourLoader.InitializeDefaultGraphicsBehaviour();
			GL.CullFace(CullFaceMode.FrontAndBack);
			//GL.Enable(GL.CullFace);
			camera = new Camera((GameWindow)this);
			camera.Move(new Vector3(0f, -13f, -32f));

			indicator = new UnlitMesh("../../TestGeometries/point.obj", "../../Textures/default.jpg");

			manMaterial = new Material(
				"../../Textures/manTexture.png", // texturePath 
				Vector3.One, //Ambient color
				0.005f, //Ambient Intensity
				1f, //Specular strength
				4 //Shininess
			);
			manMesh = new Mesh(
				new Vector3(0f, 0f, 0f), //Position
				Vector3.Zero, //Rotation
				1f, //Scale
				"../../TestGeometries/Man.obj", //Geometry path
				manMaterial //material
			);

			woodMaterial = new Material(
				"../../Textures/WoodTexture.png", // texturePath 
				Vector3.One, //Ambient color
				0.05f, //Ambient Intensity
				5f, //Specular strength
				10 //Shininess
			);
			woodMesh = new Mesh(
				new Vector3(-3f, 0f, 3f), //Position
				Vector3.Zero, //Rotation
				3f, //Scale
				"../../TestGeometries/TestGeometry.obj", //Geometry path
				woodMaterial //material
			);

			groundMaterial = new Material(
				"../../Textures/pebbleTexture.jpg", // texturePath 
				Vector3.One, //Ambient color
				0.05f, //Ambient Intensity
				5f, //Specular strength
				10 //Shininess
			);
			groundMesh = new Mesh(
				new Vector3(0f, 0f, 3f), //Position
				Vector3.Zero, //Rotation
				3f, //Scale
				"../../TestGeometries/Ground.obj", //Geometry path
				groundMaterial //material
			);

			sun = new DirectionalLight(
				new Vector3(0f, -1f, 0f), //Direction
				Vector3.One, //Color 
				1f //Intensity
			);
			//lightHolder.Add(sun);
			spotLight = new SpotLight(
				indicatorPosition, //position
				-1 * Vector3.UnitY, //direction
				Vector3.One, //color
				600f, //intensity
				40f, 
				0.1f,
				0.1f,
				0.1f,
				0.01f
			);
			lightHolder.Add(spotLight);

			previousState = Keyboard.GetState();

			base.OnLoad(e);
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			camera.Update(e, this);

			KeyboardState input = Keyboard.GetState();
			if (input.IsKeyDown(Key.Escape)) Exit();

			if (input.IsKeyDown(Key.Down)) indicatorPosition.Y -= 0.1f;
			if (input.IsKeyDown(Key.Up)) indicatorPosition.Y += 0.1f;

			if (previousState.IsKeyDown(Key.Number1) == false && input.IsKeyDown(Key.Number1) == true) drawMode = DrawMode.Wireframe;
			else if (previousState.IsKeyDown(Key.Number2) == false && input.IsKeyDown(Key.Number2) == true) drawMode = DrawMode.Shaded;
			else if (previousState.IsKeyDown(Key.Number3) == false && input.IsKeyDown(Key.Number3) == true) drawMode = DrawMode.WireframeOnShaded;

			previousState = input;

			spotLight.position = indicatorPosition;

			base.OnUpdateFrame(e);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			indicator.Draw(Matrix4.CreateTranslation(indicatorPosition), camera.view, camera.projection, DrawMode.WireframeOnShaded);

			
			manMesh.Draw(camera, lightHolder, drawMode);

			woodMesh.Draw(camera, lightHolder, drawMode);

			groundMesh.Draw(camera, lightHolder, drawMode);

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
