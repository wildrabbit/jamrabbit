using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileSetAsset))]
public class TileSetEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reload"))
        {
            string oldName = target.name;
            string newName = (target as TileSetAsset).Reload();
            if (newName != oldName)
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), newName);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
