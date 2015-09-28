using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileMapAsset))]
public class TileMapEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reload"))
        {
            TileMapAsset map = (target as TileMapAsset);

            string oldName = map.name;
            string newName = map.Reload();
            if (newName != oldName)
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(map), newName);
            }

            for (int i = 0; i < map.Layers.Count; ++i)
            {
                if (map.Layers[i] != null)
                {
                    map.Layers[i].hideFlags = HideFlags.HideInHierarchy;
                    AssetDatabase.AddObjectToAsset(map.Layers[i], map);
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}