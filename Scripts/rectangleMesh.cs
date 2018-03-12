using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rectangleMesh : MonoBehaviour {

	// Use this for initialization
	void Start () {

        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh m = mf.mesh;

        Vector3[] rectangle = new Vector3[]
        {
            new Vector3(-1, 1,  0),
            new Vector3(1, 1, 0),
            new Vector3(1, -1, 0),
            new Vector3(-1, -1, 0)
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 3, 0
        };

        Vector2[] uvs = new Vector2[]
        {
            //front 0,0 is bottom left 1,1 is top right.
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,0)
        };

        m.Clear();
        m.vertices = rectangle;
        m.triangles = triangles;
        m.uv = uvs;
        m.RecalculateNormals();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
