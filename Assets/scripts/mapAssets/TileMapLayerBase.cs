using UnityEngine;
using System.Collections;
using System.Xml;
using System;

[System.Serializable]
public abstract class TileMapLayerBase : ScriptableObject
{
    public abstract string GetName();
    public abstract int GetOrder();
    public abstract void FromXML(XmlNode node);
    public abstract GameObject Build();
}
