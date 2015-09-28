using UnityEngine;
using System.Collections;
using System.Xml;
using System;

[System.Serializable]
public class TileMapImageLayer : TileMapLayerBase
{
    public string m_name = "";

    [System.Serializable]
    public class IntPair : Pair<int> 
    {
        public IntPair(int x, int y) : base(x, y) { }
    }

    public IntPair m_position = new IntPair(0, 0);
    public float m_opacity = 1.0f;

    public TileImageFormat m_format = TileImageFormat.kPng;
    public Sprite m_image = null;
    public string m_imagePath = "";

    public bool m_useTransparentColour = false;
    public Color m_transparentColour = Color.white;

    public IntPair m_imgDimensions = new IntPair(0, 0);

    public int m_layerOrder = 0;

    public override int GetOrder()
    {
        return m_layerOrder;
    }

    public override string GetName()
    {
        return m_name;
    }

    public override GameObject Build()
    {
        GameObject img = new GameObject();
        img.name = m_name;
        SpriteRenderer spRenderer = img.AddComponent<SpriteRenderer>();
        spRenderer.sprite = m_image;
        Color c = spRenderer.color;
        c.a = m_opacity;
        spRenderer.color = c;
        img.transform.position = Vector2.zero;
        img.transform.position.Set(m_position.first, m_position.second, 0.0f);

        return img;
    }

    public override void FromXML(XmlNode node)
    {
        if (node == null || node.Name != "imagelayer")
        {
            return;
        }
        XmlAttributeCollection attrs = node.Attributes;
        m_name = attrs["name"].Value;
        m_position.first = (attrs["x"] != null && attrs["x"].Value != null) ? Convert.ToInt32(attrs["x"].Value) : 0;
        m_position.second = (attrs["y"] != null && attrs["y"].Value != null) ? Convert.ToInt32(attrs["y"].Value) : 0;
        m_opacity = (attrs["opacity"] != null && attrs["opacity"].Value != null) ? (float)Convert.ToDouble(attrs["opacity"].Value) : 1.0f;

        string[] formats = { "png", "gif", "jpg", "bmp" };
        foreach (XmlNode child in node.ChildNodes)
        {
            attrs = child.Attributes;
            if (child.Name == "image")
            {
                int idx = Array.IndexOf(formats, attrs["format"]);
                if (idx >= 0)
                {
                    m_format = (TileImageFormat)idx;
                }
                m_imagePath = attrs["source"].Value;
                if (m_imagePath != "")
                {
                    int extensionPoint = m_imagePath.LastIndexOf(".");
                    string resourceId = m_imagePath.Substring(0, extensionPoint);
                    m_image = Resources.Load<Sprite>(resourceId);
                    m_imgDimensions.Set(m_image.texture.width, m_image.texture.height);
                }
                m_useTransparentColour = false;
                if (attrs["trans"] != null)
                {
                    m_useTransparentColour = true;
                    string colorStr = attrs["trans"].Value;
                    int sharpIdx = colorStr.IndexOf("#");
                    if (sharpIdx >= 0)
                    {
                        colorStr = colorStr.Substring(sharpIdx + 1);
                    }
                    UInt32 colorInt = Convert.ToUInt32(colorStr, 16);
                    m_transparentColour.r = (float)((colorInt & 0x00ff0000) >> 16) / 255.0f;
                    m_transparentColour.g = (float)((colorInt & 0x0000ff00) >> 8) / 255.0f;
                    m_transparentColour.b = (float)((colorInt & 0x000000ff)) / 255.0f;
                }
                if ((attrs["width"] != null && attrs["width"].Value != null))
                {
                    m_imgDimensions.first = Convert.ToInt32(attrs["width"].Value);
                }
                if ((attrs["height"] != null && attrs["height"].Value != null))
                {
                    m_imgDimensions.second = Convert.ToInt32(attrs["height"].Value);
                }
            }
        }
    }
}