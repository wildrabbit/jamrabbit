using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class TiledMenuItems
{
    [MenuItem("Assets/Create/Tiled/New tileset")]
    public static void CreateTileset ()
    {
        CreateAsset<TileSetAsset>("NewTileSet");
    }

    [MenuItem("Assets/Create/Tiled/New tilemap")]
    public static void CreateTileMap()
    {
        CreateAsset<TileMapAsset>("NewTileMap");
    }

    static private void CreateAsset<T>(String name) where T : ScriptableObject
    {
        var dir = "Assets/";
        var selected = Selection.activeObject;
        if (selected != null)
        {
            var assetDir = AssetDatabase.GetAssetPath(selected.GetInstanceID());
            if (assetDir.Length > 0 && AssetDatabase.IsValidFolder(assetDir))
                dir = assetDir + "/";
        }
        ScriptableObject asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, dir + name + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
