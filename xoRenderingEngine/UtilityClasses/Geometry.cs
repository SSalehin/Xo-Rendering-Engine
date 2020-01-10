using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace XoREngine {

	public class Geometry : IDisposable {
		public int shadedVAO { get; private set; }
		public int triangleCount { get; private set; }
		private int shadedVBO;
		public int wireframeVAO { get; private set; }
		public int lineCount { get; private set; }
		private int wireframeVBO;
		bool isDisposed;

		public static Geometry CreateFromOBJ(string objFilePath) {
			List<float> shadedAttributeArray = new List<float>();
			List<float> wireframeAttributeArray = new List<float>();

			//Check if the extention is okay
			string[] splittedPath = objFilePath.Split(new char[] { '/', '\\' });
			string fileName = splittedPath[splittedPath.Length - 1];
			string[] splittedName = fileName.Split(new char[] { '.' });
			string extention = splittedName[splittedName.Length - 1];
			if (extention != "obj") throw new WrongExtentionException(fileName, "obj");

			string fileContent;
			using (StreamReader reader = new StreamReader(objFilePath)) {
				fileContent = reader.ReadToEnd();
			}

			//only keep lines with "v", "vt", "vn" or "f" markers 
			string[][] usefulLines =
				fileContent
				.Split(new char[] { '\n' })
				.Select(line => line.Split(new char[] { ' ' }))
				.Where(splittedLine => {
					if (
						splittedLine[0] == "v"
						|| splittedLine[0] == "vt"
						|| splittedLine[0] == "vn"
						|| splittedLine[0] == "f"
					) return true;
					else return false;
				})
				.ToArray();

			//Extract vertexPositions
			Vector3[] positions =
				usefulLines
				.Where(line => line[0] == "v")
				.Select(line => new Vector3(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3])))
				.ToArray();
			//Extract texture co-ordinates
			Vector2[] textureCoordinates =
				usefulLines
				.Where(line => line[0] == "vt")
				.Select(line => new Vector2(float.Parse(line[1]), float.Parse(line[2])))
				.ToArray();
			//Extract vertex normals
			Vector3[] normals =
				usefulLines
				.Where(line => line[0] == "vn")
				.Select(line => new Vector3(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3])))
				.ToArray();

			//Extract face descriptions
			string[][] faceDescriptions =
				usefulLines
				.Where(line => line[0] == "f")
				.ToArray();
			foreach (string[] faceDescription in faceDescriptions) {
				//Get all the positions in advance. It will be necessary if normal is not specified
				string[][] splittedFaceDescription =
					faceDescription
					.Select(description => description.Split(new char[] { '/' }))
					.ToArray();
				//Collect all elements for shaded attribute array
				for (int i = 3; i < faceDescription.Length; i++) {
					float[] currentDescription = GetTriangleDescription(splittedFaceDescription[1], splittedFaceDescription[i - 1], splittedFaceDescription[i]);
					foreach (float element in currentDescription) shadedAttributeArray.Add(element);
				}

				//Collect all elements for wireframe attribute array
				List<Vector3> positionsOnLines = new List<Vector3>();
				for (int i = 1; i < splittedFaceDescription.Length; i++) positionsOnLines.Add(positions[int.Parse(splittedFaceDescription[i][0]) - 1]);
				for (int i = 1; i < positionsOnLines.Count; i++) {
					wireframeAttributeArray.Add(positionsOnLines[i - 1].X);
					wireframeAttributeArray.Add(positionsOnLines[i - 1].Y);
					wireframeAttributeArray.Add(positionsOnLines[i - 1].Z);
					wireframeAttributeArray.Add(positionsOnLines[i].X);
					wireframeAttributeArray.Add(positionsOnLines[i].Y);
					wireframeAttributeArray.Add(positionsOnLines[i].Z);
				}


				float[] GetTriangleDescription(string[] firstVertex, string[] secondVertex, string[] thirdVertex) {
					int describedAttributeCount = firstVertex.Length;
					Vector3[] positionAttributes = new Vector3[] {
						positions[int.Parse(firstVertex[0]) - 1],
						positions[int.Parse(secondVertex[0]) - 1],
						positions[int.Parse(thirdVertex[0]) - 1]
					};
					Vector2[] UVAttributes;
					Vector3[] normalAttributes;

					if (describedAttributeCount == 1) {// Only position is specified
						UVAttributes = new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero };
						//The triangle is composed as follows: first-second, second-third, third-first
						Vector3 normal = CalculateNormals(positionAttributes);
						normalAttributes = new Vector3[] { normal, normal, normal };
					} else if (describedAttributeCount == 2) { // Position and UV is specified
						UVAttributes = new Vector2[] {
							textureCoordinates[int.Parse(firstVertex[1]) - 1],
							textureCoordinates[int.Parse(secondVertex[1]) - 1],
							textureCoordinates[int.Parse(thirdVertex[1]) - 1],
						};
						Vector3 normal = CalculateNormals(positionAttributes);
						normalAttributes = new Vector3[] { normal, normal, normal };
					} else {
						if (firstVertex[1] == "" || firstVertex[1] == " ") { // Position and normal is specified
							UVAttributes = new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero };
							normalAttributes = new Vector3[] {
								normals[int.Parse(firstVertex[2]) - 1],
								normals[int.Parse(secondVertex[2]) - 1],
								normals[int.Parse(thirdVertex[2]) - 1]
							};
						} else { //Position, UV and normal is specified
							UVAttributes = new Vector2[] {
								textureCoordinates[int.Parse(firstVertex[1]) - 1],
								textureCoordinates[int.Parse(secondVertex[1]) - 1],
								textureCoordinates[int.Parse(thirdVertex[1]) - 1],
							};
							normalAttributes = new Vector3[] {
								normals[int.Parse(firstVertex[2]) - 1],
								normals[int.Parse(secondVertex[2]) - 1],
								normals[int.Parse(thirdVertex[2]) - 1]
							};
						}
					}

					List<float> _output = new List<float>();
					for (int i = 0; i < 3; i++) {
						_output.Add(positionAttributes[i].X);
						_output.Add(positionAttributes[i].Y);
						_output.Add(positionAttributes[i].Z);
						_output.Add(UVAttributes[i].X);
						_output.Add(UVAttributes[i].Y);
						_output.Add(normalAttributes[i].X);
						_output.Add(normalAttributes[i].Y);
						_output.Add(normalAttributes[i].Z);
					}
					return _output.ToArray();
				}

				Vector3 CalculateNormals(Vector3[] _positionAttributes) {
					Vector3 firstToSecond = _positionAttributes[1] - _positionAttributes[0];
					Vector3 firstToThird = _positionAttributes[2] - _positionAttributes[0];
					return Vector3.Normalize(Vector3.Cross(firstToSecond, firstToThird));
				}
			}

			//For shaded
			int _shadedVAO = GL.GenVertexArray();
			GL.BindVertexArray(_shadedVAO);
			int _shadedVBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _shadedVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, shadedAttributeArray.Count * sizeof(float), shadedAttributeArray.ToArray(), BufferUsageHint.StaticDraw);
			//For triangles
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
			GL.EnableVertexAttribArray(2);
			GL.BindVertexArray(0);

			int _wireframeVAO = GL.GenVertexArray();
			GL.BindVertexArray(_wireframeVAO);
			int _wireframeVBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _wireframeVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, wireframeAttributeArray.Count * sizeof(float), wireframeAttributeArray.ToArray(), BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);
			GL.BindVertexArray(0);

			return new Geometry(_shadedVBO, _shadedVAO, shadedAttributeArray.Count / 8, _wireframeVBO, _wireframeVAO, wireframeAttributeArray.Count / 2);
			
		}


		private Geometry(int shadedVBO, int shadedVAO, int triangleCount, int wireframeVBO, int wireframeVAO, int lineCount){
			this.shadedVBO = shadedVBO;
			this.shadedVAO = shadedVAO;
			this.triangleCount = triangleCount;
			this.wireframeVBO = wireframeVBO;
			this.wireframeVAO = wireframeVAO;
			this.lineCount = lineCount;
			isDisposed = false;
		}

		public void Dispose() {
			if(!isDisposed){
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.DeleteBuffer(this.shadedVBO);
				GL.DeleteBuffer(this.wireframeVBO);
			}
			GC.SuppressFinalize(this);
		}


	}
	
	
	public class WrongExtentionException : Exception{
		public WrongExtentionException(string fileName, string expectedException)
		: base(fileName + " is expected to have the following extention: " + expectedException) { }

		public WrongExtentionException()
		: base("File did not have expected extention to it's name.") { }
	}
}
