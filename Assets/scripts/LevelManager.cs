using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelBuilder
{
    public Level Build(TileMapAsset level)
    {
        GameObject root = new GameObject(level.m_mapName);
        Level levelRoot = root.AddComponent<Level>();

        int numLayers = level.Layers.Count;
        levelRoot.InitLayers(numLayers);
        for (int i = 0; i < numLayers; ++i)
        {
            levelRoot.SetLayerAt(i,level.Layers[i].Build());
        }

        return levelRoot;
    }
}

public class LevelManager : MonoBehaviour 
{
    private LevelBuilder builder = new LevelBuilder();
    private Level m_levelRoot = null;

    public TileMapAsset map;
    
    public void BuildLevel(TileMapAsset level)
    {
        DestroyLevel();
        m_levelRoot = builder.Build(level);

    }

    public void DestroyLevel()
    {
        if (m_levelRoot != null)
        {
            m_levelRoot.Cleanup();
            Destroy(m_levelRoot);
        }
    }

	// Use this for initialization
	void Start () 
    {
        BuildLevel(map);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
