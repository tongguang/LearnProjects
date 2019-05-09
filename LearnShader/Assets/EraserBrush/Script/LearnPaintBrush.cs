using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LearnPaintBrush : MonoBehaviour
{
    public Texture MaskTexture;
    public RenderTexture TargetRenderTexture;

    public Texture2D TargetTexture;

    private RectTransform m_RectTransform;

    private Canvas m_Canvas;

    private Vector2 m_LocalPos;
    // Use this for initialization
    void Start () {
        m_RectTransform = GetComponent<RectTransform>();
        m_Canvas = GetComponentInParent<Canvas>();
        TargetRenderTexture = new RenderTexture(200, 200, 24);
        TargetTexture = new Texture2D(200, 200, TextureFormat.RGBA32, false);
        GetComponent<RawImage>().texture = TargetTexture;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform, Input.mousePosition,
                m_Canvas.worldCamera, out m_LocalPos);

            var cache = RenderTexture.active;
            RenderTexture.active = TargetRenderTexture;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, 200, 200, 0);
            Graphics.DrawTexture(new Rect(m_LocalPos.x - 25, -m_LocalPos.y - 25, 50, 50), MaskTexture);
            TargetTexture.ReadPixels(new Rect(0, 0, 200, 200), 0, 0);
            TargetTexture.Apply(false);
            GL.PopMatrix();
            Debug.Log(m_LocalPos);
            RenderTexture.active = cache;
        }
    }
}
