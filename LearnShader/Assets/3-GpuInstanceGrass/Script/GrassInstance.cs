﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrassInstance : MonoBehaviour
{
    public class SubmitInstance
    {
        public Matrix4x4[] Matrixs;
        public MaterialPropertyBlock PropertyBlock = new MaterialPropertyBlock();
    }

    public Vector2Int BlockSize = new Vector2Int(30, 30);

    public Camera RenderCamera;

    [SerializeField]
    List<Material> _GrassMaterialList = new List<Material>();

    [SerializeField]
    Mesh _CurMesh;

    [SerializeField]
    Texture2D _CombineTexture;

    int GrassLevel = 0;

    Vector2 RolePosition = Vector2.zero;

    GrassData GrassDataInfo;

    List<Material> _RealUseGrassMaterialList = new List<Material>();

    Material _CurMaterial;

    List<SubmitInstance> _CurSubmitInstances = new List<SubmitInstance>();

    Vector2Int[] _CurShowBlockCells = new Vector2Int[9];

    int[] _ShowBlockIndexCalcInts = new[] { -1, 0, 1 };
    List<Matrix4x4> _TempMatrix4x4s = new List<Matrix4x4>();
    List<Vector4> _TempVector4s1 = new List<Vector4>();
    List<Vector4> _TempVector4s2 = new List<Vector4>();
    Dictionary<string, Vector4> _TextureNameToUV0Offset = new Dictionary<string, Vector4>();

    private bool _ShowBlockDirty = false;
    private bool _SubmitInstanceDirty = false;

    private int _ColorPropID = Shader.PropertyToID("_Color");
    private int _UV0OffsetPropID = Shader.PropertyToID("_UV0Offset");
    private int _MainTexPropID = Shader.PropertyToID("_MainTex");


    public Vector2 testPos;
    public List<Texture2D> testTexture2Ds;

    public void Test1()
    {
    }

    public void Test2()
    {
        SetRolePosition(testPos.x, testPos.y);
    }

    public void Test3()
    {
        SetTextures(testTexture2Ds);
    }

    public void SetGrassLevel(int level)
    {
        GrassLevel = level;
        _CurMaterial = null;
        if (_RealUseGrassMaterialList.Count >= GrassLevel)
        {
            _CurMaterial = _RealUseGrassMaterialList[GrassLevel];
        }
    }

    public void SetMapGrassInfo(GrassData grassData)
    {
        GrassDataInfo = grassData;
        _ShowBlockDirty = true;
        _SubmitInstanceDirty = true;
    }

    public void SetTextures(List<Texture2D> textures)
    {
        var rects = _CombineTexture.PackTextures(textures.ToArray(), 0, 1024, true);
        _TextureNameToUV0Offset.Clear();
        for (int i = 0; i < textures.Count; i++)
        {
            _TextureNameToUV0Offset.Add(textures[i].name, new Vector4(rects[i].min.x, rects[i].min.y, rects[i].max.x, rects[i].max.y));
        }

        foreach (var material in _RealUseGrassMaterialList)
        {
            material.SetTexture(_MainTexPropID, _CombineTexture);
        }
    }

    public void SetRolePosition(float x, float z)
    {
        RolePosition.x = x;
        RolePosition.y = z;
        _ShowBlockDirty = true;
    }

    public void Clear()
    {
        SetMapGrassInfo(null);
    }

    void Awake()
    {
        foreach (var material in _GrassMaterialList)
        {
            _RealUseGrassMaterialList.Add(Instantiate(material));
        }
        for (int i = 0; i < _CurShowBlockCells.Length; i++)
        {
            _CurShowBlockCells[i] = new Vector2Int(-1, -1);
        }
        _CombineTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false, false);
        SetGrassLevel(0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShowBlock();
        UpdateSubmitInstance();
        UpdateGrassDraw();
    }

    void UpdateShowBlock()
    {
        if (!_ShowBlockDirty)
        {
            return;
        }
        _ShowBlockDirty = false;

        int cellX4, cellY4;
        PosToBlockCell(RolePosition.x, RolePosition.y, out cellX4, out cellY4);
        if (_CurShowBlockCells[4].x == cellX4 && _CurShowBlockCells[4].y == cellY4)
        {
            return;
        }
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                int index = j * 3 + i;
                var oldX = _CurShowBlockCells[index].x;
                var oldY = _CurShowBlockCells[index].y;
                var newX = cellX4 + _ShowBlockIndexCalcInts[i];
                var newY = cellY4 + _ShowBlockIndexCalcInts[j];
                if (oldX != newX || oldY != newY)
                {
                    _CurShowBlockCells[index].x = newX;
                    _CurShowBlockCells[index].y = newY;
                    _SubmitInstanceDirty = true;
                }
            }
        }
    }

    void UpdateSubmitInstance()
    {
        if (!_SubmitInstanceDirty)
        {
            return;
        }
        _SubmitInstanceDirty = false;

        SubmitInstance curSubmitInstance = null;
        _TempMatrix4x4s.Clear();
        _TempVector4s1.Clear();
        _TempVector4s2.Clear();
        _CurSubmitInstances.Clear();
        if (GrassDataInfo != null)
        {
            foreach (var curShowBlockCell in _CurShowBlockCells)
            {
                if (curShowBlockCell.x < 0 || curShowBlockCell.y < 0)
                {
                    continue;
                }

                var index = BlockCellToIndex(curShowBlockCell.x, curShowBlockCell.y);
                if (index >= GrassDataInfo.BlockList.Count)
                {
                    continue;
                }

                var grassBlock = GrassDataInfo.BlockList[index];
                foreach (var grassItem in grassBlock.GrassItems)
                {
                    if (curSubmitInstance == null)
                    {
                        curSubmitInstance = new SubmitInstance();
                        _CurSubmitInstances.Add(curSubmitInstance);
                    }
                    _TempMatrix4x4s.Add(grassItem.TransformMatrix);
                    _TempVector4s1.Add(grassItem.ColorInfo);
                    _TempVector4s2.Add(_TextureNameToUV0Offset[grassItem.TextureName]);
                    if (_TempMatrix4x4s.Count >= 1023)
                    {
                        curSubmitInstance.Matrixs = _TempMatrix4x4s.ToArray();
                        curSubmitInstance.PropertyBlock.SetVectorArray(_ColorPropID, _TempVector4s1);
                        curSubmitInstance.PropertyBlock.SetVectorArray(_UV0OffsetPropID, _TempVector4s2);
                        _TempMatrix4x4s.Clear();
                        _TempVector4s1.Clear();
                        _TempVector4s2.Clear();
                        curSubmitInstance = null;
                    }
                }
            }
            if (curSubmitInstance != null)
            {
                curSubmitInstance.Matrixs = _TempMatrix4x4s.ToArray();
                curSubmitInstance.PropertyBlock.SetVectorArray(_ColorPropID, _TempVector4s1);
                curSubmitInstance.PropertyBlock.SetVectorArray(_UV0OffsetPropID, _TempVector4s2);
                _TempMatrix4x4s.Clear();
                _TempVector4s1.Clear();
                _TempVector4s2.Clear();
                curSubmitInstance = null;
            }
        }
    }

    void UpdateGrassDraw()
    {
        if (!_CurMaterial)
        {
            return;
        }
        if (!_CurMesh)
        {
            return;
        }
        if (!RenderCamera)
        {
            return;
        }
        foreach (var curSubmitInstance in _CurSubmitInstances)
        {
            //                Graphics.DrawMeshInstanced(CurMesh, 0, _CurMaterial, curSubmitInstance.Matrixs, curSubmitInstance.Matrixs.Length);
//                MaterialPropertyBlock properties = new MaterialPropertyBlock();
//                properties.SetVectorArray("_Color", new List<Vector4>() { Color.red, Color.red });
            Graphics.DrawMeshInstanced(
                _CurMesh,
                0,
                _CurMaterial,
                curSubmitInstance.Matrixs,
                curSubmitInstance.Matrixs.Length,
                curSubmitInstance.PropertyBlock,
                ShadowCastingMode.Off,
                false,
                0,
                null);
        }
    }

    private void PosToBlockCell(float x, float z, out int cellX, out int cellY)
    {
        cellX = Mathf.FloorToInt(1.0f * x / BlockSize.x);
        cellY = Mathf.FloorToInt(1.0f * z / BlockSize.y);
    }

    private int BlockCellToIndex(int cellX, int cellY)
    {
        int index = 0;
        if (GrassDataInfo != null)
        {
            index = cellY * GrassDataInfo.BlockXMax + cellX;
        }

        return index;
    }

    private int PosToBlockIndex(float x, float z)
    {
        int cellX, cellY;
        PosToBlockCell(x, z, out cellX, out cellY);
        return BlockCellToIndex(cellX, cellY);
    }
}