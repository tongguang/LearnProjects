public class RenderDocMeshExport
{
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
        public float texcoord0X = float.MinValue;
        public float texcoord0Y;
        public float texcoord1X = float.MinValue;
        public float texcoord1Y;
    }

    public static void RenderDocCsvToMesh()
    {
        var csvPath = "Assets/RenderDoc/mesh.csv";
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
        allStr = allStr.Replace("in_TEXCOORD0.x", "texcoord0X");
        allStr = allStr.Replace("in_TEXCOORD0.y", "texcoord0Y");
        allStr = allStr.Replace("in_TEXCOORD1.x", "texcoord1X");
        allStr = allStr.Replace("in_TEXCOORD1.y", "texcoord1Y");
        File.WriteAllText(csvPath, allStr);

        Mesh mesh = new Mesh();
        var datas = UtilCsv.LoadObjects<RenderDocMesh>(csvPath);
        List<Vector3> positionList = new List<Vector3>();
        List<Color> colorList = null;
        List<Vector3> normalList = null;
        List<Vector2> uv0List = null;
        List<Vector2> uv1List = null;

        Dictionary<int, RenderDocMesh> renderDocMeshDict = new Dictionary<int, RenderDocMesh>();

        foreach (var renderDocMesh in datas)
        {
            if (!renderDocMeshDict.ContainsKey(renderDocMesh.IDX))
            {
                renderDocMeshDict.Add(renderDocMesh.IDX, renderDocMesh);
            }
        }

        var keys = renderDocMeshDict.Keys.ToList();
        keys.Sort();
        var minIndice = keys[0];
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

        List<int> indices = new List<int>();
        foreach (var renderDocMesh in datas)
        {
            indices.Add(renderDocMesh.IDX - minIndice);
        }

        mesh.SetVertices(positionList);
        mesh.SetNormals(normalList);
        mesh.SetColors(colorList);
        mesh.SetUVs(0, uv0List);
        mesh.SetUVs(1, uv1List);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

        AssetDatabase.CreateAsset(mesh, "Assets/RenderDoc/mesh.asset");
    }
}
