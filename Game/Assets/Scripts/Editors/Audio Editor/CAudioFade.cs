using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class CAudioFade : MonoBehaviour 
{
	private float m_rFadeDuration = 0.0f;
	private float m_rFadeTarget = 0.0f;
	private float m_rFadeDelta = 0.0f;

	public void vSetup(float p_rTarget, float p_rDuration)
	{
		m_rFadeTarget = p_rTarget;
		m_rFadeDuration = p_rDuration;
		float t_rVolumeCur = GetComponent<AudioSource>().volume;
		m_rFadeDelta = (m_rFadeTarget - t_rVolumeCur) / m_rFadeDuration;
	}
	
	void FixedUpdate ()
	{
		if (m_rFadeDuration > 0.0f)
		{
			GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume + (Time.deltaTime * m_rFadeDelta);
			m_rFadeDuration -= Time.deltaTime;
			if (m_rFadeDuration <= 0.0f)
			{
				GetComponent<AudioSource>().volume = m_rFadeTarget;
				if (m_rFadeTarget == 0.0f)
				{
					GetComponent<AudioSource>().Stop();
				}
			}
		}
		else
		{
			Destroy (this);
		}
	}
}