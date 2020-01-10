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
	class ZGame : GameWindow {
		Camera camera;
		UnlitMesh manMesh;
		DrawMode drawMode = DrawMode.Wireframe;

		KeyboardState previousState;

		public ZGame(int height, int width, string title)
			: base(height, width, GraphicsMode.Default, title) {
		}

		protected override void OnResize(EventArgs e) {
			GL.Viewport(0, 0, this.Width, this.Height);
			base.OnResize(e);
		}

		protected override void OnLoad(EventArgs e) {
			DefaultGraphicsBehaviourLoader.InitializeDefaultGraphicsBehaviour();
			camera = new Camera((GameWindow)this);

			manMesh = new UnlitMesh("../../TestGeometries/man.obj", "../../Textures/manTexture.png");

			previousState = Keyboard.GetState();

			base.OnLoad(e);
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			camera.Update(e, this);

			KeyboardState input = Keyboard.GetState();
			if (input.IsKeyDown(Key.Escape)) Exit();

			if (previousState.IsKeyDown(Key.Number1) == false && input.IsKeyDown(Key.Number1) == true) drawMode = DrawMode.Wireframe;
			else if (previousState.IsKeyDown(Key.Number2) == false && input.IsKeyDown(Key.Number2) == true) drawMode = DrawMode.Shaded;
			else if (previousState.IsKeyDown(Key.Number3) == false && input.IsKeyDown(Key.Number3) == true) drawMode = DrawMode.WireframeOnShaded;

			previousState = input;
			base.OnUpdateFrame(e);
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			manMesh.Draw(Matrix4.Identity, camera.view, camera.projection, drawMode);

			

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
