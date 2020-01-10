using System;
using System.IO;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace XoREngine {
	public class Shader : IDisposable {
		public int Handle;
		private bool isDisposed = false;

		public Shader(string vertexShaderPath, string fragmentShaderPath){
			//Read vertex shader source and compile it
			string vertexShaderSource;
			using(StreamReader reader = new StreamReader(vertexShaderPath, Encoding.UTF8)){
				vertexShaderSource = reader.ReadToEnd();
			}
			int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, vertexShaderSource);
			GL.CompileShader(vertexShader);
			string vertexShaderInfoLog = GL.GetShaderInfoLog(vertexShader);
			if (vertexShaderInfoLog != String.Empty) throw new VertexShaderCompilationException(vertexShaderInfoLog);

			//Read fragment shader source and compile it
			string fragmentShaderSource;
			using (StreamReader reader = new StreamReader(fragmentShaderPath, Encoding.UTF8)) {
				fragmentShaderSource = reader.ReadToEnd();
			}
			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, fragmentShaderSource);
			GL.CompileShader(fragmentShader);
			string fragmentShaderInfoLog = GL.GetShaderInfoLog(fragmentShader);
			if (fragmentShaderInfoLog != String.Empty) throw new FragmentShaderCompilationException(fragmentShaderInfoLog);

			//Create the actual shader program by attaching individual shaders and linking it
			Handle = GL.CreateProgram();
			GL.AttachShader(Handle, vertexShader);
			GL.AttachShader(Handle, fragmentShader);
			GL.LinkProgram(Handle);

			//Compiled data is copied to the shader program when linked
			//Individual shaders also need not to be attached
			//So detatch and delete them
			GL.DetachShader(Handle, vertexShader);
			GL.DeleteShader(vertexShader);
			GL.DetachShader(Handle, fragmentShader);
			GL.DeleteShader(fragmentShader);
		}

		public void Use() {
			GL.UseProgram(Handle);
		}

		public void SetMatrix4(string matrixName, Matrix4 matrix){
			int location = GL.GetUniformLocation(this.Handle, matrixName);
			GL.UniformMatrix4(location, false, ref matrix);
		}

		public void SetVector4(string vectorName, Vector4 vector){
			int location = GL.GetUniformLocation(this.Handle, vectorName);
			GL.Uniform4(location, vector.X, vector.Y, vector.Z, vector.W);
		}

		public void SetVector3(string vectorName, Vector3 vector) {
			int location = GL.GetUniformLocation(this.Handle, vectorName);
			GL.Uniform3(location, vector.X, vector.Y, vector.Z);
		}

		public void SetFloat(string name, float value) {
			int location = GL.GetUniformLocation(this.Handle, name);
			GL.Uniform1(location, value);
		}

		public void SetInt(string name, int value) {
			int location = GL.GetUniformLocation(this.Handle, name);
			GL.Uniform1(location, value);
		}

		public void SetBool(string name, bool value) {
			int location = GL.GetUniformLocation(this.Handle, name);
			if(value == true) GL.Uniform1(location, 10f);
			else GL.Uniform1(location, 0f);
		}

		public int GetAttributeLocation(string attributeName){
			return GL.GetAttribLocation(Handle, attributeName);
		}

		public void Dispose() {
			if (!isDisposed) {
				GL.DeleteProgram(Handle);
				isDisposed = true;
			}
			GC.SuppressFinalize(this);
		}

		~Shader() {
			//if(!isDisposed) GL.DeleteProgram(Handle);
		}
	}	

	public class VertexShaderCompilationException : Exception 
	{
		public VertexShaderCompilationException()
		: base("Vertex shader could not be compiled.") { }

		public VertexShaderCompilationException(string message)
		: base("Vertex shader could not be compiled: " + message) { }
	}

	public class FragmentShaderCompilationException : Exception {
		public FragmentShaderCompilationException()
		: base("Fragment shader could not be compiled.") { }

		public FragmentShaderCompilationException(string message)
		: base("Fragment shader could not be compiled: " + message) { }
	}
}
