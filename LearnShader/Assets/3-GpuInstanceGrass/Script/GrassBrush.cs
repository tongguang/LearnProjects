using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class GrassBrush : MonoBehaviour
{
    public GameObject GrassPrefab;

    public Texture GrassTexture = null;

    public Color GrassColor = Color.white;

    public float GrassMinScale = 1.0f;

    public float GrassMaxScale = 1.0f;

    public float BrushRadius = 1.0f;

    public int CreateGrassNum = 1;

    private void Awake()
    {
//        GrassPrefab =
//            AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_RawData/map_editor/map_models/grass/grass.prefab");
    }
}

