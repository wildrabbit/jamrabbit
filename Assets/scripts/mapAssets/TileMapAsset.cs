using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

public enum TileMapOrientation
{
    kOrthogonal,
    kIsometric,
    kStaggeredIsometric,
    kHexagonal
}

public enum TileMapRenderOrder
{
    kRightDown,
    kRightUp,
    kLeftDown,
    kLeftUp
}

[System.Serializable]
public class TilesetEntry
{
    public int m_firstGid = 0;
    public TileSetAsset m_tileSet = null;
}

public enum TileImageFormat
{
    kPng,
    kGif,
    kJpg,
    kBmp
}


public class TileMapAsset : ScriptableObject 
{
    public TextAsset m_mapFile = null;
    public string m_mapName = "";

    [SerializeField]
    private string m_mapVersion = "";

    // We need this to serialize generics!
    [System.Serializable]
    public class TilesetDictionary : SerializableDictionaryBase<string, TilesetEntry> { }
    
    [SerializeField]
    private TilesetDictionary m_mapTilesets = new TilesetDictionary();

    [SerializeField]
    private TileMapOrientation m_orientation = TileMapOrientation.kOrthogonal;

    [SerializeField]
    private TileMapRenderOrder m_renderOrder = TileMapRenderOrder.kRightDown;

    [SerializeField]
    private Pair<int> m_mapDimensions = new Pair<int>(0,0);

    [SerializeField]
    private Pair<int> m_tileDimensions = new Pair<int>(32,32);

    [SerializeField]
    private int m_nextObjectId = 0;

    [System.Serializable]
    public class MapLayerDictionary : SerializableDictionaryBase<string, TileMapLayer> { }
    
    [SerializeField]
    private MapLayerDictionary m_mapLayers = new MapLayerDictionary();

    [System.Serializable]
    public class ObjectGroupDictionary: SerializableDictionaryBase<string, TileMapObjectGroup> { }
    
    [SerializeField]
    private ObjectGroupDictionary m_objectGroups = new ObjectGroupDictionary();

    [System.Serializable]
    public class ImageLayerDictionary : SerializableDictionaryBase<string, TileMapImageLayer> { }
    
    [SerializeField]
    private ImageLayerDictionary m_imageLayers = new ImageLayerDictionary();

    [SerializeField]
    private List<TileMapLayerBase> m_allLayers = new List<TileMapLayerBase>();
    public List<TileMapLayerBase> Layers
    {
        get { return m_allLayers; }
    }

    public void OnEnable ()
    {
        if (m_mapName == "")
        {
            Reload();
        }
    }

    public void Clear()
    {
        m_mapTilesets.Clear();
        m_mapLayers.Clear();
        m_objectGroups.Clear();
        m_imageLayers.Clear();

        for (int i = 0; i < m_allLayers.Count; ++i)
        {
            UnityEngine.Object.DestroyImmediate(m_allLayers[i]);
            m_allLayers[i] = null;
        }
        m_allLayers = new List<TileMapLayerBase>();
    }

    public string Reload ()
    {
        Clear();
        if (m_mapFile != null)
        {
            m_mapName = m_mapFile.name.Substring(0, m_mapFile.name.IndexOf('.'));
            name = m_mapName + "_TileMap";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(m_mapFile.text);

            XmlNode mapNode = xmlDoc.GetElementsByTagName("map")[0];
            if (mapNode != null)
            {
                XmlAttributeCollection attribs = mapNode.Attributes;
                m_mapVersion = attribs["version"].Value;
                string[] orientations = {"orthogonal","isometric","staggered","hexagonal"};
                int idx = Array.IndexOf(orientations, attribs["orientation"].Value);
                m_orientation = (TileMapOrientation)idx;

                string[] renderOrders = {"right-down","right-up","left-down","left-up"};
                idx = Array.IndexOf(renderOrders, attribs["renderorder"].Value);
                m_renderOrder = (TileMapRenderOrder)idx;

                m_mapDimensions.first = Convert.ToInt32(attribs["width"].Value);
                m_mapDimensions.second = Convert.ToInt32(attribs["height"].Value);

                m_tileDimensions.first = Convert.ToInt32(attribs["tilewidth"].Value);
                m_tileDimensions.second = Convert.ToInt32(attribs["tileheight"].Value);

                m_nextObjectId = Convert.ToInt32(attribs["nextobjectid"].Value);

                XmlNodeList mapChildren = mapNode.ChildNodes;
                int layerOrder = 0;
                foreach (XmlNode childNode in mapChildren)
                {
                    attribs = childNode.Attributes;
                    if (childNode.Name == "tileset")
                    {
                        TileSetAsset tileset = null;
                        int firstgid = Convert.ToInt32(attribs["firstgid"].Value);
                        if (attribs["source"] != null)
                        {
                            string tilesetId = attribs["source"].Value;
                            tilesetId = tilesetId.Substring(0, tilesetId.IndexOf('.'));
                            tileset = Resources.Load<TileSetAsset>(string.Format("{0}_Tileset",tilesetId));                            
                        }
                        else
                        {
                            tileset = ScriptableObject.CreateInstance<TileSetAsset>();
                            tileset.Load(childNode);
                        }

                        if (tileset != null)
                        {
                            TilesetEntry tsEntry = new TilesetEntry();
                            tsEntry.m_firstGid = firstgid;
                            tsEntry.m_tileSet = tileset;
                            m_mapTilesets[tileset.name] = tsEntry;
                        }
                    }
                    else if (childNode.Name == "layer")
                    {
                        TileMapLayer layer = ScriptableObject.CreateInstance<TileMapLayer>();
                        layer.FromXML(childNode);
                        layer.m_layerOrder = layerOrder++;
                        m_mapLayers[layer.m_name] = layer;
                        m_allLayers.Add(layer);                        
                    }
                    else if (childNode.Name == "objectgroup")
                    {
                        TileMapObjectGroup objGroup = ScriptableObject.CreateInstance<TileMapObjectGroup>();
                        objGroup.FromXML(childNode);
                        objGroup.m_layerOrder = layerOrder++;
                        m_objectGroups[objGroup.m_name] = objGroup;
                        m_allLayers.Add(objGroup);
                    }
                    else if (childNode.Name == "imagelayer")
                    {
                        TileMapImageLayer imgLayer = ScriptableObject.CreateInstance<TileMapImageLayer>();
                        imgLayer.FromXML(childNode);
                        imgLayer.m_layerOrder = layerOrder++;
                        m_imageLayers[imgLayer.m_name] = imgLayer;
                        m_allLayers.Add(imgLayer);
                    }
                }
            }
        }
        return name;
    }

}
