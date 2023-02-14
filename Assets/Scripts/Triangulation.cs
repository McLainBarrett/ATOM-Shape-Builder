using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulation : MonoBehaviour {

	private class Vertex {
		public Vector2 Position;
		public Vertex Prev, Next;
		public int Index;
		public Vertex() {}
		public Vertex(Vector2 Position, Vertex Prev, Vertex Next) {
			this.Position = Position;
			this.Prev = Prev;
			this.Next = Next;
		}
	}

	private class Triangle {
		public Vertex a, b, c;
		public Triangle(Vertex a, Vertex b, Vertex c) {
			this.a = a;
			this.b = b;
			this.c = c;
		}
	}

	static public List<ushort> Triangulate(List<Vector2> points) {
		List<Vertex> verts = Vertify(points);
		List<Triangle> tris = new List<Triangle>();
		while (verts.Count > 3) {
			var tri = Clip(ref verts, Random.Range(0, verts.Count));
			if (tri != null)
				tris.Add(tri);
		}
		tris.Add(new Triangle(verts[0],verts[1],verts[2]));

		List<ushort> ushorts = new List<ushort>();
		foreach (var tri in tris) {
			ushorts.Add((ushort)tri.a.Index);
			ushorts.Add((ushort)tri.b.Index);
			ushorts.Add((ushort)tri.c.Index);
		}
		return ushorts;
	}

	static private List<Vertex> Vertify(List<Vector2> points) {
		List<Vertex> verts = new List<Vertex>();
		for (int i = 0; i < points.Count; i++) {
			var vert = new Vertex();
			vert.Position = points[i];
			vert.Index = i;

			if (i > 0) {
				vert.Prev = verts[verts.Count-1];
				vert.Prev.Next = vert;
			}				
			if (i == points.Count-1) {
				vert.Next = verts[0];
				verts[0].Prev = vert;
			}
			verts.Add(vert);
		}
		ClockwiseifyPolygon(ref verts);
		return verts;
	}

	static private Triangle Clip(ref List<Vertex> verts, int vI) {
		var vert = verts[vI];

		float angle = Vector2.SignedAngle(vert.Prev.Position - vert.Position, vert.Next.Position - vert.Position);
		//print(angle + " -- i:" + vert.Index);		
		//print(IsAnyPointContainedInTri(verts, vert.Prev, vert, vert.Next));
		if (0 < angle && angle < 180 && !IsAnyPointContainedInTri(verts, vert.Prev, vert, vert.Next)) {
			//print(angle);
			vert.Prev.Next = vert.Next;
			vert.Next.Prev = vert.Prev;
			verts.RemoveAt(vI);
			return new Triangle(vert.Prev, vert, vert.Next);
		} else {
			return null;
		}
	}

	static private bool IsAnyPointContainedInTri(List<Vertex> Verts, Vertex A, Vertex B, Vertex C) {
		Vector2 a,b,c;
		a=A.Position;b=B.Position;c=C.Position;
		List<Vector2> points = new List<Vector2>();
		foreach (var vert in Verts)
			points.Add(vert.Position);
		points.Remove(a); points.Remove(b); points.Remove(c); 

		float myArea = RoundTo(GetArea(a, b, c), 4);
		foreach (var point in points) {
			if (IsInTri(point))
				return true;
		}
		return false;

		bool IsInTri(Vector2 d) {
			//print(myArea + " == " + (GetArea(a,b,d) + GetArea(a,c,d) + GetArea(b,c,d)) + " -> " + (myArea == GetArea(a,b,d) + GetArea(a,c,d) + GetArea(b,c,d)));
			return myArea == RoundTo(GetArea(a,b,d) + GetArea(a,c,d) + GetArea(b,c,d), 4);
		}

		float GetArea(Vector2 a, Vector2 b, Vector2 c) {
			return Mathf.Abs((a.x*(b.y-c.y) + b.x*(c.y-a.y)+ c.x*(a.y-b.y))/2.0f); 
		}

		float RoundTo(float f, int places) {
			return Mathf.Round(f*Mathf.Pow(10,places))/Mathf.Pow(10,places);
		}
	}

	static private void ClockwiseifyPolygon(ref List<Vertex> verts) {
		/* Find the vertex with smallest y (and largest x if there are ties). 
		Let the vertex be A and the previous vertex in the list be B and the next vertex in the list be C. 
		Now compute the sign of the cross product of AB and AC. */

		int minYInd = 0;
		for (int i = 0; i < verts.Count; i++) {
			if (verts[i].Position.y < verts[minYInd].Position.y)
				minYInd = i;
		}
		//targ - pos
		Vector2 a = verts[minYInd].Prev.Position;
		Vector2 b = verts[minYInd].Position;
		Vector2 c = verts[minYInd].Next.Position;
		float sign = (b.x * c.y + a.x * b.y + a.y * c.x) - (a.y * b.x + b.y * c.x + a.x * c.y);
		if (sign < 0)
			return;

		//Reverse list
		foreach (var vert in verts) {
			var temp = vert.Prev;
			vert.Prev = vert.Next;
			vert.Next = temp;
		}
	}
}