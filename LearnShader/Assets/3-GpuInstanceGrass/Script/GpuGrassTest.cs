using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GpuGrassTest : MonoBehaviour
{
    public GrassData GrassDataInfo;
    public GrassInstance GrassInstancePrefab;

	void Start ()
    {
        GrassInstancePrefab.SetMapGrassData(GrassDataInfo);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
