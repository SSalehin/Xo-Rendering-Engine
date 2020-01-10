using System;
using OpenTK;
using OpenTK.Input;

namespace XoREngine {
	public class Camera {
		public Vector3 Position{ get { return position; } private set { } }
		private Vector3 position;

		public Vector3 frontDirection;

		public float moveSpeed = 10f;
		public float lookSensitivity = 10f;

		bool mouseJustEntered = true;
		KeyboardState lastKeyboardInput;
		MouseState lastMouseInput;

		private float yaw, pitch;

		public float fov;

		public Matrix4 view, projection;

		

		public Camera(Vector3 initialPosition, Vector3 lookDirection, float fov, GameWindow window) {
			window.CursorVisible = false;
			position = initialPosition;
			frontDirection = lookDirection;
			lastKeyboardInput = Keyboard.GetState();
			lastMouseInput = Mouse.GetState();
		}

		public Camera(GameWindow window) {
			position = new Vector3(0f, 0f, 3f);
			frontDirection = new Vector3(0f, 0f, -1f);
			fov = 45f;
			window.CursorVisible = false;
			lastKeyboardInput = Keyboard.GetState();
			lastMouseInput = Mouse.GetState();
		}

		private void RecalculateFront() {
			frontDirection.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
			frontDirection.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
			frontDirection.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
			frontDirection = Vector3.Normalize(frontDirection);
		}

		public void Update(FrameEventArgs e, GameWindow window) {
			KeyboardState input = Keyboard.GetState();
			MouseState mouseInput = Mouse.GetState();

			if (!window.Focused) return;
			if (input.IsKeyDown(Key.W)) position += frontDirection * moveSpeed * (float)e.Time;
			if (input.IsKeyDown(Key.S)) position -= frontDirection * moveSpeed * (float)e.Time;
			if (input.IsKeyDown(Key.A)) position -= Vector3.Normalize(Vector3.Cross(frontDirection, Vector3.UnitY)) * moveSpeed * (float)e.Time;
			if (input.IsKeyDown(Key.D)) position += Vector3.Normalize(Vector3.Cross(frontDirection, Vector3.UnitY)) * moveSpeed * (float)e.Time;
			if (input.IsKeyDown(Key.Q)) position -= Vector3.UnitY * moveSpeed * (float)e.Time;
			if (input.IsKeyDown(Key.E)) position += Vector3.UnitY * moveSpeed * (float)e.Time;

			if (mouseJustEntered) {
				lastMouseInput = Mouse.GetState();
				mouseJustEntered = false;
			} else {
				yaw += (mouseInput.X - lastMouseInput.X) * lookSensitivity * (float)e.Time;
				pitch -= (mouseInput.Y - lastMouseInput.Y) * lookSensitivity * (float)e.Time;
				if (pitch > 89.0f) pitch = 89.0f;
				else if (pitch < -89.0f) pitch = -89.0f;
			}

			RecalculateFront();

			view = Matrix4.LookAt(position, position + frontDirection, Vector3.UnitY);
			projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)window.Width / window.Height, 0.01f, 1000f);

			lastKeyboardInput = input;
			lastMouseInput = mouseInput;
		}

		public void OnMouseMove(MouseMoveEventArgs e, GameWindow window) {
			if (window.Focused) Mouse.SetPosition(window.X + window.Width / 2f, window.Y + window.Height / 2f);
		}

		public void OnMouseWheel(MouseWheelEventArgs e) {
			fov -= e.DeltaPrecise * 10;
			if (fov >= Configuration.maximmumFOV) fov = Configuration.maximmumFOV;
			else if (fov <= Configuration.minimmumFOV) fov = Configuration.minimmumFOV;
		}

		public void Move(Vector3 amounts){
			position -= amounts;
		}
	}
}
