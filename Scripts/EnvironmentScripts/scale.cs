using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scale : MonoBehaviour {
	public Texture2D texture;
	public MeshRenderer mesh;
	public Material mat;
	public Texture2D tex;
	public float x;
	public float y;
	public float z;
	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshRenderer> ();
		mat = mesh.material;
		tex = (Texture2D)mat.mainTexture;
		Texture2D clone = Instantiate (tex);
		TextureScale.Bilinear (clone, tex.width * 2, tex.height * 2);
	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
