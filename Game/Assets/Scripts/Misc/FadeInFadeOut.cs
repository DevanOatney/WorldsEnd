using UnityEngine;
using System.Collections;

public class FadeInFadeOut : MonoBehaviour {

	// FadeInOut
 
public Texture2D fadeOutTexture;
public float fadeSpeed = 0.3f;
 
int drawDepth = -1000;
 
 private float alpha = 1.0f; 
 
private int fadeDir = -1;
 
 
//--------------------------------------------------------------------
 
void OnGUI()
{
 
	alpha += fadeDir * fadeSpeed * Time.deltaTime;	
	alpha = Mathf.Clamp01(alpha);	
 	GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
 
	GUI.depth = drawDepth;
 
	GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
}
 
//--------------------------------------------------------------------
 
public void fadeIn(){
	fadeDir = -1;	
}
 
//--------------------------------------------------------------------
 
public void fadeOut(){
	fadeDir = 1;	
}
 
void Start()
	{
	alpha=1;
	fadeIn();
}
}
