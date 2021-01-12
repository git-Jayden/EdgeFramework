using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机淡入淡出
/// </summary>
public class CameraFadeInOut : MonoBehaviour {

	private Texture2D fadeOutTexture;
	public float fadeSpeed = 0.3f;
	public Color FadeColor = new Color(0,0,0);
	private int drawDepath = -1000;
	private float alpha = 1.0f;
	private int fadeDir = -1;
	private bool isFade = false;
	private Rect _rect = new Rect(-10,-10,Screen.width + 10,Screen.height + 10);
	void OnGUI()
	{		
		if (!isFade && alpha == 0)
			return;
		Color _color = GUI.color;
		_color.a = alpha;
		_color.r = FadeColor.r;
		_color.g = FadeColor.g;
		_color.b = FadeColor.b;
		GUI.color = _color;
		GUI.depth = drawDepath;
		GUI.DrawTexture (_rect,fadeOutTexture,ScaleMode.StretchToFill,false);

	}

	// Use this for initialization
	void Start () 
	{
		alpha = 0;
		fadeOutTexture = new Texture2D (1,1);
	}

	public void fadeIn()
	{
		isFade = true;
		fadeDir = 1;
	}


	public void fadeOut()
	{
		isFade = true;
		fadeDir = -1;

	}
	// Update is called once per frame
	private bool toggle = true;
	void Update () 
	{
		
		if (Input.GetKeyDown (KeyCode.F)) {
			if (toggle) {
				fadeIn ();
			} else {
				fadeOut ();
			}
			toggle = !toggle;
		}
		if (isFade == false)
			return;

		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		if (alpha > 1f || alpha < 0f) {
			isFade = false;
		}
		alpha = Mathf.Clamp01 (alpha);
	}
}
