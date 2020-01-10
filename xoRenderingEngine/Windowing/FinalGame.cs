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
	class FinalGame : GameWindow {
		DrawMode drawMode;
		KeyboardState previousState;

		Camera camera;

		Material houseMaterial;
		Mesh house;

		LightHolder lightHolder;

		DirectionalLight sun;

		public FinalGame(int height, int width, string title)
			: base(height, width, GraphicsMode.Default, title) {
		}

		protected override void OnLoad(EventArgs e) {
			drawMode = DrawMode.Wireframe;
			previousState = Keyboard.GetState();

			camera = new Camera((GameWindow)this);

			houseMaterial = new Material(
				"../../Textures/House.png", // texturePath 
				Vector3.One, //Ambient color
				0.005f, //Ambient Intensity
				1f, //Specular strength
				4 //Shininess
			);
			house = new Mesh(
				new Vector3(0f, 0f, 0f), //Position
				Vector3.Zero, //Rotation
				1f, //Scale
				"../../TestGeometries/Sphere.obj", //Geometry path
				houseMaterial //material
			);

			lightHolder = new LightHolder();

			sun = new DirectionalLight(
				Vector3.One, //direction
				Vector3.One, //color
				1f //intensity
			);

			lightHolder.Add(sun);

			base.OnLoad(e);
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			camera.Update(e, this);

			KeyboardState input = Keyboard.GetState();
			if (input.IsKeyDown(Key.Escape)) Exit();

			//if (input.IsKeyDown(Key.Down)) indicatorPosition.Y -= 0.1f;
			//if (input.IsKeyDown(Key.Up)) indicatorPosition.Y += 0.1f;

			if (previousState.IsKeyDown(Key.Number1) == false && input.IsKeyDown(Key.Number1) == true) drawMode = DrawMode.Wireframe;
			else if (previousState.IsKeyDown(Key.Number2) == false && input.IsKeyDown(Key.Number2) == true) drawMode = DrawMode.Shaded;
			else if (previousState.IsKeyDown(Key.Number3) == false && input.IsKeyDown(Key.Number3) == true) drawMode = DrawMode.WireframeOnShaded;

			previousState = input;

			//spotLight.position = indicatorPosition;

			base.OnUpdateFrame(e);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


			house.Draw(camera, lightHolder, drawMode);

			Context.SwapBuffers();

			base.OnRenderFrame(e);
		}


		protected override void OnResize(EventArgs e) {
			GL.Viewport(0, 0, this.Width, this.Height);
			base.OnResize(e);
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
