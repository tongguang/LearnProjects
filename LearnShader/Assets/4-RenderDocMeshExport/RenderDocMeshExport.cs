using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RenderDocMeshExport
{
    public class RenderDocMeshStr
    {
        public string vtx;
        public string IDX;
        public string positionX;
        public string positionY;
        public string positionZ;
        public string colorX = null;
        public string colorY;
        public string colorZ;
        public string colorW;
        public string normalX = null;
        public string normalY;
        public string normalZ;
        public string tangentX = null;
        public string tangentY;
        public string tangentZ;
        public string tangentW;
        public string texcoord0X = null;
        public string texcoord0Y;
        public string texcoord1X = null;
        public string texcoord1Y;

        public int StringParseToInt(string str)
        {
            int val;
            if (int.TryParse(str, out val))
            {
                return val;
            }

            return int.MinValue;
        }

        public float StringParseToFloat(string str)
        {
            float val;
            if (float.TryParse(str, out val))
            {
                return val;
            }

            return float.MinValue;
        }
    }

    public class RenderDocMesh
    {
        public int vtx;
        public int IDX;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float colorX = float.MinValue;
        public float colorY;
        public float colorZ;
        public float colorW;
        public float normalX = float.MinValue;
        public float normalY;
        public float normalZ;
        public float tangentX = float.MinValue;
        public float tangentY;
        public float tangentZ;
        public float tangentW;
        public float texcoord0X = float.MinValue;
        public float texcoord0Y;
        public float texcoord1X = float.MinValue;
        public float texcoord1Y;
    }

    public static void RenderDocCsvToMesh(string srcPath, string outDir)
    {
        if (!File.Exists(srcPath))
        {
            Debug.LogError("未找到文件 " + srcPath);
            return;
        }
        if (!srcPath.EndsWith(".csv"))
        {
            Debug.LogError("不是csv文件 " + srcPath);
            return;
        }
        var csvPath = srcPath;
        var allStr = File.ReadAllText(csvPath);
        allStr = allStr.Replace("in_POSITION0.x", "positionX");
        allStr = allStr.Replace("in_POSITION0.y", "positionY");
        allStr = allStr.Replace("in_POSITION0.z", "positionZ");
        allStr = allStr.Replace("in_COLOR0.x", "colorX");
        allStr = allStr.Replace("in_COLOR0.y", "colorY");
        allStr = allStr.Replace("in_COLOR0.z", "colorZ");
        allStr = allStr.Replace("in_COLOR0.w", "colorW");
        allStr = allStr.Replace("in_NORMAL0.x", "normalX");
        allStr = allStr.Replace("in_NORMAL0.y", "normalY");
        allStr = allStr.Replace("in_NORMAL0.z", "normalZ");
        allStr = allStr.Replace("in_NORMAL0.w", "normalW");
        allStr = allStr.Replace("in_TANGENT0.x", "tangentX");
        allStr = allStr.Replace("in_TANGENT0.y", "tangentY");
        allStr = allStr.Replace("in_TANGENT0.z", "tangentZ");
        allStr = allStr.Replace("in_TANGENT0.w", "tangentW");
        allStr = allStr.Replace("in_TEXCOORD0.x", "texcoord0X");
        allStr = allStr.Replace("in_TEXCOORD0.y", "texcoord0Y");
        allStr = allStr.Replace("in_TEXCOORD1.x", "texcoord1X");
        allStr = allStr.Replace("in_TEXCOORD1.y", "texcoord1Y");

        File.WriteAllText(csvPath, allStr);

        Mesh mesh = new Mesh();
        var datas = UtilCsv.LoadObjects<RenderDocMeshStr>(csvPath);
        List<RenderDocMesh> realDatas = new List<RenderDocMesh>();
        foreach (var renderDocMeshStr in datas)
        {
            realDatas.Add(new RenderDocMesh()
            {
                vtx = renderDocMeshStr.StringParseToInt(renderDocMeshStr.vtx),
                IDX = renderDocMeshStr.StringParseToInt(renderDocMeshStr.IDX),
                positionX = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.positionX),
                positionY = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.positionY),
                positionZ = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.positionZ),
                colorX = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.colorX),
                colorY = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.colorY),
                colorZ = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.colorZ),
                colorW = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.colorW),
                normalX = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.normalX),
                normalY = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.normalY),
                normalZ = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.normalZ),
                tangentX = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.tangentX),
                tangentY = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.tangentY),
                tangentZ = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.tangentZ),
                tangentW = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.tangentW),
                texcoord0X = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.texcoord0X),
                texcoord0Y = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.texcoord0Y),
                texcoord1X = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.texcoord1X),
                texcoord1Y = renderDocMeshStr.StringParseToFloat(renderDocMeshStr.texcoord1Y),
            });
        }

        List<Vector3> positionList = new List<Vector3>();
        List<Color> colorList = null;
        List<Vector3> normalList = null;
        List<Vector4> tangentList = null;
        List<Vector2> uv0List = null;
        List<Vector2> uv1List = null;

        // 重新算三角形顶点索引
        HashSet<int> idxSet = new HashSet<int>();
        foreach (var renderDocMesh in realDatas)
        {
            idxSet.Add(renderDocMesh.IDX);
        }
        var idxList = idxSet.ToList();
        idxList.Sort();
        var newIdxDict = new Dictionary<int, int>();
        for (int i = 0; i < idxList.Count; i++)
        {
            newIdxDict.Add(idxList[i], i);
        }
        foreach (var renderDocMesh in realDatas)
        {
            renderDocMesh.IDX = newIdxDict[renderDocMesh.IDX];
        }

        Dictionary<int, RenderDocMesh> renderDocMeshDict = new Dictionary<int, RenderDocMesh>();

        foreach (var renderDocMesh in realDatas)
        {
            if (!renderDocMeshDict.ContainsKey(renderDocMesh.IDX))
            {
                renderDocMeshDict.Add(renderDocMesh.IDX, renderDocMesh);
            }
        }

        // 计算顶点
        var keys = renderDocMeshDict.Keys.ToList();
        keys.Sort();
        foreach (var key in keys)
        {
            var renderDocMesh = renderDocMeshDict[key];
            positionList.Add(new Vector3(renderDocMesh.positionX, renderDocMesh.positionY, renderDocMesh.positionZ));
            if (Math.Abs(renderDocMesh.colorX - float.MinValue) > 0.00001f)
            {
                if (colorList == null)
                {
                    colorList = new List<Color>();
                }
                colorList.Add(new Color(renderDocMesh.colorX, renderDocMesh.colorY, renderDocMesh.colorZ, renderDocMesh.colorW));
            }
            if (Math.Abs(renderDocMesh.normalX - float.MinValue) > 0.00001f)
            {
                if (normalList == null)
                {
                    normalList = new List<Vector3>();
                }
                normalList.Add(new Vector3(renderDocMesh.normalX, renderDocMesh.normalY, renderDocMesh.normalZ));
            }
            if (Math.Abs(renderDocMesh.tangentX - float.MinValue) > 0.00001f)
            {
                if (tangentList == null)
                {
                    tangentList = new List<Vector4>();
                }
                tangentList.Add(new Vector4(renderDocMesh.tangentX, renderDocMesh.tangentY, renderDocMesh.tangentZ, renderDocMesh.tangentW));
            }
            if (Math.Abs(renderDocMesh.texcoord0X - float.MinValue) > 0.00001f)
            {
                if (uv0List == null)
                {
                    uv0List = new List<Vector2>();
                }
                uv0List.Add(new Vector2(renderDocMesh.texcoord0X, renderDocMesh.texcoord0Y));
            }
            if (Math.Abs(renderDocMesh.texcoord1X - float.MinValue) > 0.00001f)
            {
                if (uv1List == null)
                {
                    uv1List = new List<Vector2>();
                }
                uv1List.Add(new Vector2(renderDocMesh.texcoord1X, renderDocMesh.texcoord1Y));
            }
        }

        // 生成三角形
        List<int> indices = new List<int>();
        foreach (var renderDocMesh in realDatas)
        {
            indices.Add(renderDocMesh.IDX);
        }

        mesh.SetVertices(positionList);
        mesh.SetNormals(normalList);
        mesh.SetColors(colorList);
        mesh.SetTangents(tangentList);
        mesh.SetUVs(0, uv0List);
        mesh.SetUVs(1, uv1List);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

        CreateFolderIfNeed(outDir);
        string fileName = Path.GetFileNameWithoutExtension(srcPath);

        string meshOutPath = Path.Combine(outDir, fileName + ".asset");
        AssetDatabase.CreateAsset(mesh, meshOutPath);
    }

    public static void CreateFolderIfNeed(string path)
    {
        if (path.EndsWith("/"))
        {
            path = path.Substring(0, path.Length - 1);
        }
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }
        var parentPath = Path.GetDirectoryName(path);
        CreateFolderIfNeed(parentPath);
        AssetDatabase.CreateFolder(parentPath, Path.GetFileName(path));
    }
}
