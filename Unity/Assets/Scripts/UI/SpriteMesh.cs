using UnityEngine;

internal class SpriteMesh
{
	internal Mesh mesh
	{
		get { return this.m_mesh; }
	}

	internal Vector3[] vertices
	{
		get { return this.m_vertices; }
	}

	internal Color[] colors
	{
		get { return this.m_colors; }
	}

	internal Vector2[] uvs
	{
		get { return this.m_uvs; }
	}

	private Mesh m_mesh;

	private Vector3[] m_vertices;
	private Color[] m_colors;
	private Vector2[] m_uvs;
	private int[] m_tris;

	internal SpriteMesh()
	{
		this.m_vertices = new Vector3[4];
		this.m_colors = new Color[4];
		this.m_uvs = new Vector2[4];
		this.m_tris = new int[6];
	}

	// FIXME: for now just create a quad
	void Initialize()
	{
		if (this.m_mesh == null)
		{
			// FIXME: retrieve the shared mesh from a mesh filter
			this.m_mesh = new Mesh();
		}

		// Assign to mesh
		this.m_mesh.Clear();
		this.m_mesh.vertices = this.m_vertices;
		this.m_mesh.uv = this.m_uvs;
		this.m_mesh.colors = this.m_colors;
		this.m_mesh.triangles = this.m_tris;

		/*
		var vertices = new Vector3[4];
		vertices[0].x = -10.0f;
		vertices[0].y = -10.0f;
		vertices[1].x = -10.0f;
		vertices[1].y = 10.0f;
		vertices[2].x = 10.0f;
		vertices[2].y = 10.0f;
		vertices[3].x = 10.0f;
		vertices[3].y = -10.0f;

		// Create triangles
		var tris = new int[6];
		// Clock-wise:
		tris[0] = 0;	//	0_ 1			0 ___ 3
		tris[1] = 3;	//  | /		Verts:	 |	/|
		tris[2] = 1;	// 2|/				1|/__|2

		tris[3] = 3;	//	  3
		tris[4] = 2;	//   /|
		tris[5] = 1;	// 5/_|4
		*/
	}
}
