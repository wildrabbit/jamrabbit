using UnityEngine;
using System.Collections;


public class Level : MonoBehaviour
{
    private GameObject[] m_layers;
    public void InitLayers(int numLayers)
    {
        m_layers = new GameObject[numLayers];
    }
    public void SetLayerAt(int idx, GameObject layer)
    {
        if (idx < 0 || idx >= m_layers.Length) return;
        if (layer == null) return;

        layer.transform.SetParent(transform);
        m_layers[idx] = layer;
    }

    public void Cleanup()
    {
        foreach (GameObject layer in m_layers)
        {
            GameObject.Destroy(layer);
        }
        m_layers = new GameObject[0];
    }
}
