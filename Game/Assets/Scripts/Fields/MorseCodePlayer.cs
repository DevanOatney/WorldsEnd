using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class MorseCodePlayer : MonoBehaviour {
	public AudioClip dotSound;
	public AudioClip dashSound;
	float spaceDelay = 0.1f;
	float letterDelay = 0.05f;

	DCScript m_dcPersistantData;
	
	// International Morse Code Alphabet
	private string[] alphabet = 
    //A     B       C       D      E    F       G
	{".-", "-...", "-.-.", "-..", ".", "..-.", "--.",
    //H       I     J       K      L       M     N
	 "....", "..", ".---", "-.-", ".-..", "--", "-.", 
	//O      P       Q       R      S      T    U
	 "---", ".--.", "--.-", ".-.", "...", "-", "..-",
	//V       W      X       Y       Z
	 "...-", ".--", "-..-", "-.--", "--..",
	//0        1        2        3        4        
	 "-----", ".----", "..---", "...--", "....-", 
	//5        6        7        8        9     
	 ".....", "-....", "--...", "---..", "----."};

	// Use this for initialization
	void Start () 
	{
		m_dcPersistantData = GameObject.Find("PersistantData").GetComponent<DCScript>();
	}
	
	public void PlayMorseCodeMessage(string message)
	{
		StartCoroutine("_PlayMorseCodeMessage", message);
	}
	public void StopMorseCodeMessage()
	{
		GetComponent<AudioSource>().Stop();
		StopCoroutine("_PlayMorseCodeMessage");
	}
	
	private IEnumerator _PlayMorseCodeMessage(string message)
	{
		//cut the message in half because it takes to long to play an entire message
		message = message.Substring(0, (int)(message.Length*0.3f));
		// Remove all characters that are not supported by Morse code...
		Regex regex = new Regex("[^A-z0-9 ]");
		message = regex.Replace(message.ToUpper(), "");
		
		// Convert the message into Morse code audio... 
		foreach(char letter in message)
		{
			if (letter == ' ')
				yield return new WaitForSeconds(spaceDelay);
			else
			{
				int index = letter - 'A';
				if (index < 0)
					index = letter - '0' + 26;
				string letterCode = alphabet[index];
				foreach(char bit in letterCode)
				{
					// Dot or Dash?
					AudioClip       sound = dotSound;
					if (bit == '-')	sound = dashSound;

                    // Play the audio clip and wait for it to end before playing the next one.
                    if (sound != null)
                    {
                        GetComponent<AudioSource>().PlayOneShot(sound, 0.5f + m_dcPersistantData.m_fSFXVolume);
                        yield return new WaitForSeconds(sound.length + letterDelay);
                    }
				}
			}
		}
	}
}
