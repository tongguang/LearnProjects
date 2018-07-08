using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour {

    // Use this for initialization
    AndroidJavaClass jc;
    AndroidJavaObject jo;
    void Start () {
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }
	

    void OnGUI()
    {
        GUI.skin.textArea.fontSize = 50;
        GUI.skin.button.fontSize = 50;

        // add
        if (GUI.Button(new Rect(50, 50, 200, 200), "test1"))
        {
            jo.Call("TestFun1");
        }

        // showMessage
        if (GUI.Button(new Rect(50, 300, 200, 200), "test2"))
        {
        }
    }

    void OnJavaMessage(string messageStr)
    {
        Debug.Log(messageStr);
    }
}
