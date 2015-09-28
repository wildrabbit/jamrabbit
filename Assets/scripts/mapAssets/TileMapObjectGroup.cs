using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;


[System.Serializable]
public class TileObject
{
    public int m_id = 0;
    public string m_name = "";
    public string m_type = "";
    public Pair<int> m_position = new Pair<int>(0, 0);
    public Pair<int> m_dimensions = new Pair<int>(0, 0);
    public Dictionary<string, object> m_properties = new Dictionary<string, object>();
}

[System.Serializable]
public class TileMapObjectGroup : TileMapLayerBase
{
    public string m_name = "";
    public List<TileObject> m_objects = new List<TileObject>();
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
        return null;
    }

    public override void FromXML(XmlNode node)
    {
        if (node == null || node.Name != "objectgroup")
        {
            return;
        }
        XmlAttributeCollection attrs = node.Attributes;
        m_name = attrs["name"].Value;

        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.Name == "object")
            {
                attrs = child.Attributes;
                TileObject obj = new TileObject();
                obj.m_id= Convert.ToInt32(attrs["id"].Value);
                obj.m_name = (attrs["name"] != null) ? attrs["name"].Value : "";
                obj.m_type = (attrs["type"] != null) ? attrs["type"].Value : "";
                obj.m_position.first = Convert.ToInt32(attrs["x"].Value);
                obj.m_position.second = Convert.ToInt32(attrs["y"].Value);
                obj.m_dimensions.first = attrs["width"] == null ? 0 : Convert.ToInt32(attrs["width"].Value);
                obj.m_dimensions.second = attrs["height"] == null ? 0 : Convert.ToInt32(attrs["height"].Value);
                foreach (XmlNode propsNode in child.ChildNodes)
                {
                    if (propsNode.Name == "properties")
                    {
                        foreach(XmlNode property in propsNode.ChildNodes)
                        {
                            attrs = property.Attributes;
                            if (property.Name == "property")
                            {
                                obj.m_properties[attrs["name"].Value] = attrs["value"].Value;
                            }
                        }
                    }
                }
                m_objects.Add(obj);
                
            }
        }
    }
}
