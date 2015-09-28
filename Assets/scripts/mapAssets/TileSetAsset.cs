using UnityEngine;
using System;
using System.Xml;
using System.Collections;

public class TileSetAsset : ScriptableObject
{
    public TextAsset m_textFile;

    [SerializeField]
    private string m_tileSetName = "";
    public string TileSetName
    {
        get
        {
            return m_tileSetName;
        }
    }

    [SerializeField]
    private Texture2D m_image = null;
    public Texture2D Image
    {
        get
        {
            return m_image;
        }
    }

    [SerializeField]
    private Pair<int> m_tileDimensions = new Pair<int>(0, 0);
    public Pair<int> TileDimensions
    {
        get
        {
            return m_tileDimensions;
        }
    }

    [SerializeField]
    private string m_imagePath = "";
    public string ImagePath
    {
        get
        {
            return m_imagePath;
        }
    }

    [SerializeField]
    private Pair<int> m_imageDimensions = new Pair<int>(0, 0);
    public Pair<int> ImageDimensions
    {
        get
        {
            return m_imageDimensions;
        }
    }

    public void OnEnable ()
    {
        if (m_tileSetName == "")
        {
            Reload();
        }
    }

    public void Load(XmlNode node)
    {
        if (node != null && node.Name == "tileset")
        {
            m_tileSetName = node.Attributes["name"].Value;
            name = m_tileSetName + "_Tileset";
            m_tileDimensions.first = Convert.ToInt32(node.Attributes["tilewidth"].Value);
            m_tileDimensions.second = Convert.ToInt32(node.Attributes["tileheight"].Value);
            node = node.ChildNodes[0];
            if (node != null && node.Name == "image")
            {
                XmlAttributeCollection atts = node.Attributes;
                m_imageDimensions.first = Convert.ToInt32(node.Attributes["width"].Value);
                m_imageDimensions.second = Convert.ToInt32(node.Attributes["height"].Value);
                m_imagePath = node.Attributes["source"].Value;
            }

            if (m_imagePath != "")
            {
                int extensionPoint = m_imagePath.LastIndexOf(".");
                string resourceId = m_imagePath.Substring(0, extensionPoint);
                m_image = Resources.Load<Texture2D>(resourceId);
            }
        }
    }

    public string Reload()
    {
        if (m_textFile != null)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(m_textFile.text);
            
            XmlNode node = xmlDoc.GetElementsByTagName("tileset")[0];
            Load(node);
        }
        return name;
    }

}