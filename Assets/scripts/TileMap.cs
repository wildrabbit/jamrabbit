using UnityEngine;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.Text;
using System.IO;

public enum TilemapRenderStrategy
{
    kQuad,
    kMesh
}

[RequireComponent (typeof(MeshFilter))]
public class TileMap : MonoBehaviour 
{
    public TilemapRenderStrategy renderStrategy = TilemapRenderStrategy.kQuad;
	public Texture2D srcTex;
	public int texTileW = 32;
	public int texTileH = 32;

	public TextAsset xmlMap;

	//--------------------

	int numRows = 100;
	int numCols = 100;

	//--
	private Mesh mesh;
	List<int> tileData;
	List<Vector2> indexes;

	void generateTileAtlas()
	{
		indexes = new List<Vector2> ();
		int nRows = srcTex.height / texTileH;
		int nCols = srcTex.width / texTileW;
		
		float vRatio = 1 / (float)nRows;
		float uRatio = 1 / (float)nCols;

		Vector2 tmp;
		for (int r = 0; r < nRows; r++) 
		{
			for (int c = 0; c < nCols; c++)
			{
				tmp = new Vector2();
				tmp.x = c * uRatio;
				tmp.y = 1 - (r + 1) * vRatio;
				indexes.Add(tmp);
			}
		}
	}

	void generateMesh()
	{
		XmlDocument doc = new XmlDocument ();
		doc.LoadXml (xmlMap.text);
		XmlNodeList layers = doc.GetElementsByTagName ("layer");

		tileData = new List<int> ();
		
		if (layers.Count > 0) 
		{
			XmlNode node = layers.Item(0);
			XmlAttributeCollection atts = node.Attributes;
			Debug.Log(atts["width"].Value);

			numCols = Convert.ToInt32(node.Attributes["width"].Value);
			numRows = Convert.ToInt32(node.Attributes["height"].Value);

			XmlNode data = node.ChildNodes[0];
			if (data != null)
			{
				XmlNodeList tiles = data.ChildNodes;
				XmlNode tileNode;
				for (int i = 0; i < tiles.Count; i++)
				{
					tileNode = tiles[i];
					tileData.Add(Convert.ToInt32(tileNode.Attributes["gid"].Value) - 1);
				}
			}
		}

		List<Vector3> vertices = new List<Vector3> ();
		List<int> triangles = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();

		float startX = transform.position.x;
		float startY = transform.position.y;
		float startZ = transform.position.z;

		int currentTile = 0;

		float vRatio =  texTileH/(float)srcTex.height;
		float uRatio = texTileW/(float)srcTex.width;

		for (int r = 0; r < numRows; r++) 
		{
			for (int c = 0; c < numCols; c++)
			{
				vertices.Add (new Vector3(startX + c , startY - r, startZ));
				vertices.Add (new Vector3(startX + c + 1 , startY - r, startZ));
				vertices.Add (new Vector3(startX + c + 1, startY - (r + 1), startZ));
				vertices.Add (new Vector3(startX + c , startY - (r + 1), startZ));
				triangles.Add(vertices.Count - 4);
				triangles.Add (vertices.Count - 3);
				triangles.Add (vertices.Count - 2);
				triangles.Add(vertices.Count - 4);
				triangles.Add(vertices.Count - 2);
				triangles.Add (vertices.Count - 1);

				//currentTile = UnityEngine.Random.Range(0,indexes.Count-6);
				currentTile = tileData[r * numCols + c];

				uvs.Add(new Vector2(indexes[currentTile].x, indexes[currentTile].y + vRatio));
				uvs.Add(new Vector2(indexes[currentTile].x + uRatio, indexes[currentTile].y + vRatio));
				uvs.Add(new Vector2(indexes[currentTile].x + uRatio, indexes[currentTile].y));
				uvs.Add(new Vector2(indexes[currentTile].x, indexes[currentTile].y));
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();
		mesh.Optimize();
	}
	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter>().mesh;
		(GetComponent<Renderer>() as MeshRenderer).material.SetTexture("_MainTex", srcTex);
		generateTileAtlas();
		generateMesh();

		Camera.main.transform.Translate (new Vector3 (numCols / 2, -numRows / 2, 0));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
