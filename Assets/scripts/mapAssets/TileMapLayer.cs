using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using Unity.IO.Compression;
using System.Text;


[System.Serializable]
public class TileInfo
{
    public uint m_gid = 0;
    public bool m_horizontalFlip = false;
    public bool m_verticalFlip = false;
    public bool m_diagonalFlip = false;
}

public enum TileEncodingType
{
    kNone,
    kCSV,
    kBase64
}

public enum TileCompressionType
{
    kNone,
    kGzip,
    kZlib
}

[System.Serializable]
public class TileData
{
    public TileEncodingType m_encoding = TileEncodingType.kNone;
    public TileCompressionType m_compression = TileCompressionType.kNone;
    public List<TileInfo> m_tiles = new List<TileInfo>();
}

[System.Serializable]
public class TileMapLayer : TileMapLayerBase
{
    private const UInt32 FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
    private const UInt32 FLIPPED_VERTICALLY_FLAG = 0x40000000;
    private const UInt32 FLIPPED_DIAGONALLY_FLAG = 0x20000000;

    public string m_name = "";
    public Pair<int> m_layerDimensions = new Pair<int>(0, 0);
    public Dictionary<string, object> m_properties = new Dictionary<string, object>();
    public TileData m_data = null;
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
        if (node != null && node.Name == "layer")
        {
            XmlAttributeCollection attrs = node.Attributes;
            m_name = attrs["name"].Value;
            m_layerDimensions.first = Convert.ToInt32(attrs["width"].Value);
            m_layerDimensions.second = Convert.ToInt32(attrs["height"].Value);
            foreach(XmlNode child in node.ChildNodes)
            {
                if (child.Name == "properties")
                {
                    foreach (XmlNode propertyNode in child)
                    {
                        if (propertyNode.Name != "property")
                            continue;
                        XmlAttributeCollection propertyAtts = propertyNode.Attributes;
                        m_properties[propertyAtts["name"].Value] = propertyAtts["value"].Value;
                    }
                }
                else if (child.Name == "data")
                {
                    m_data = new TileData();
                    attrs = child.Attributes;
                    if (attrs["encoding"]!= null)
                    {
                        string[] encodings = { "", "csv", "base64" };
                        string encodingValue = attrs["encoding"].Value;
                        int encodingIdx = Array.IndexOf(encodings, encodingValue);
                        if (encodingIdx >= 0)
                        {
                            m_data.m_encoding = (TileEncodingType)encodingIdx;
                        }

                        string[] compressions = { "", "gzip", "zlib" };
                        string compression = attrs["compression"].Value;
                        int compressionIdx = Array.IndexOf(compressions, compression);
                        if (compressionIdx >= 0)
                        {
                            m_data.m_compression = (TileCompressionType)compressionIdx;
                        }

                        switch(m_data.m_encoding)
                        {
                            case TileEncodingType.kCSV:
                                {
                                    string text = child.InnerText;
                                    string[] values = text.Split(',');
                                    foreach (string v in values)
                                    {
                                        uint value = Convert.ToUInt32(v);
                                        TileInfo info = new TileInfo(); 
                                        info.m_horizontalFlip = (value & FLIPPED_HORIZONTALLY_FLAG) != 0;
                                        info.m_verticalFlip = (value & FLIPPED_VERTICALLY_FLAG) != 0;
                                        info.m_diagonalFlip = (value & FLIPPED_DIAGONALLY_FLAG) != 0;
                                        value = value & ~(FLIPPED_DIAGONALLY_FLAG | FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG);
                                        info.m_gid = value;
                                        m_data.m_tiles.Add(info);
                                    }
                                    break;
                                }
                            case TileEncodingType.kBase64:
                                {
                                    byte[] bytes = null;
                                    switch(m_data.m_compression)
                                    {
                                        case TileCompressionType.kNone:
                                            {
                                                bytes = Convert.FromBase64String(child.InnerText);                                                
                                                break;
                                            }
                                        case TileCompressionType.kGzip:
                                            {
                                                //Transform string into byte[]
                                                string str = child.InnerText;
                                                byte[] byteArray = new byte[str.Length];
                                                int indexBA = 0;
                                                foreach (char item in str.ToCharArray())
                                                {
                                                    byteArray[indexBA++] = (byte)item;
                                                }
                                                
                                                MemoryStream ms = new MemoryStream(byteArray);
                                                GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);

                                                byteArray = new byte[byteArray.Length];
                                                int rBytes = gzip.Read(byteArray, 0, byteArray.Length);

                                                StringBuilder sb = new StringBuilder(rBytes);
                                                for (int i = 0; i < rBytes; ++i)
                                                {
                                                    sb.Append((char)byteArray[i]);
                                                }

                                                gzip.Close();
                                                ms.Close();
                                                gzip.Dispose();
                                                ms.Dispose();

                                                bytes = Convert.FromBase64String(sb.ToString());
                                                break;
                                            }
                                        case TileCompressionType.kZlib:
                                            {
                                                //Transform string into byte[]
                                                string str = child.InnerText;
                                                byte[] byteArray = new byte[str.Length];
                                                int indexBA = 0;
                                                foreach (char item in str.ToCharArray())
                                                {
                                                    byteArray[indexBA++] = (byte)item;
                                                }

                                                MemoryStream ms = new MemoryStream(byteArray);
                                                DeflateStream zlib = new DeflateStream(ms, CompressionMode.Decompress);

                                                byteArray = new byte[byteArray.Length];
                                                int rBytes = zlib.Read(byteArray, 0, byteArray.Length);

                                                StringBuilder sb = new StringBuilder(rBytes);
                                                for (int i = 0; i < rBytes; ++i)
                                                {
                                                    sb.Append((char)byteArray[i]);
                                                }

                                                zlib.Close();
                                                ms.Close();
                                                zlib.Dispose();
                                                ms.Dispose();

                                                bytes = Convert.FromBase64String(sb.ToString());
                                                break;
                                            }
                                    }
                                    for (int i = 0; i < bytes.Length; i += 4)
                                    {
                                        uint value = (uint)bytes[i] | ((uint)bytes[i + 1] << 8) | ((uint)bytes[i + 2] << 16) | ((uint)bytes[i + 3] << 24);
                                        TileInfo info = new TileInfo();
                                        info.m_horizontalFlip = (value & FLIPPED_HORIZONTALLY_FLAG) != 0;
                                        info.m_verticalFlip = (value & FLIPPED_VERTICALLY_FLAG) != 0;
                                        info.m_diagonalFlip = (value & FLIPPED_DIAGONALLY_FLAG) != 0;
                                        value = value & ~(FLIPPED_DIAGONALLY_FLAG | FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG);
                                        info.m_gid = value;
                                        m_data.m_tiles.Add(info);
                                    }
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                    else
                    {
                        m_data.m_encoding = TileEncodingType.kNone;
                        m_data.m_compression = TileCompressionType.kNone;

                        m_data.m_tiles.Clear();
                        foreach(XmlNode tileNode in child.ChildNodes)
                        {
                            if (tileNode.Name != "tile")
                            {
                                continue;
                            }
                            TileInfo info = new TileInfo();
                            uint value = Convert.ToUInt32(tileNode.Attributes["gid"].Value);

                            info.m_horizontalFlip = (value & FLIPPED_HORIZONTALLY_FLAG) != 0;
                            info.m_verticalFlip = (value & FLIPPED_VERTICALLY_FLAG) != 0;
                            info.m_diagonalFlip = (value & FLIPPED_DIAGONALLY_FLAG) != 0;
                            value = value & ~(FLIPPED_DIAGONALLY_FLAG | FLIPPED_HORIZONTALLY_FLAG |FLIPPED_VERTICALLY_FLAG);
                            info.m_gid = value;
                            m_data.m_tiles.Add(info);
                        }
                    }
                }
            }
        }
    }
}
