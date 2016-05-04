using UnityEngine;
using System.Collections;

public class FadeInOutSound : MonoBehaviour {
	public enum Fade {In, Out}

	public float musicFadeTime = 4.0f;

 

	public IEnumerator  FadeAudio (float timer,Fade fadeType) 
	{
		GameObject GO = GameObject.Find("PersistantData");
		if(GO != null)
		{
			Component script = GO.GetComponent("DCScript");
			if(script != null)
			{
    			float start = (float)(fadeType == Fade.In? 0.0 : 0.5f + ((DCScript)script).m_fMusicVolume);
    			float end = (float)(fadeType == Fade.In? 0.5f + ((DCScript)script).m_fMusicVolume : 0.0);
    			float i = 0.0f;
   				 float step = (float)1.0/timer;
    			while (i <= 1.0) 
				{
				
       				 i += step * Time.deltaTime;
				
					GetComponent<AudioSource>().volume = Mathf.Lerp(start, end, i);
				
        			yield return new WaitForSeconds(step * Time.deltaTime);
				
    			}
			}
		}
	}
}
