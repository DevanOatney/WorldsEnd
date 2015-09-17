using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAudioHelper : MonoBehaviour
{
	public int m_cSFXPlayers = 12;	//Number of total players to use
	public int m_cSFXCycle = 4;	//How many track to play on a player before moving to the next one
    private AudioSource[] m_audiosourceSFX = null;
    private AudioSource[] m_aAudiosourceMusic = null;
	private int m_iActiveMusic = 0;
	private CAudioHelper.EMusic m_eMusicActive = CAudioHelper.EMusic.Nil;

	private int m_iSFXPlayerCur = 0;
	private int m_iSFXCycleCur = 0;

    public enum ESound
    {
        //Farm
        Max,
        Nil = -1
    };

    public enum EMusic
    {
        Max,
        Nil = -1
    };

    static CAudioHelper ms_instance = null;

    public static CAudioHelper Instance
    {
        get
        {
            return ms_instance;
        }
    }

	private static float ms_rMusicDefault = 0.5f;
	private float m_rMusicVolume = ms_rMusicDefault;

    void Start( )
    {
        if (ms_instance == null)
        {
            ms_instance = this;
            vSetUpMasterInstance();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void vSetUpMasterInstance( )
    {
		m_audiosourceSFX = new AudioSource[m_cSFXPlayers];

		for (int t_i = 0; t_i < m_cSFXPlayers; ++t_i)
		{
        	GameObject t_go = new GameObject("SFX_Player_" + t_i);
        	m_audiosourceSFX[t_i] = t_go.AddComponent<AudioSource>( );
			m_audiosourceSFX[t_i].priority = 255;	//Lowest priority
			t_go.transform.parent = gameObject.transform;
		}

		//Music
		m_aAudiosourceMusic = new AudioSource[2];
        GameObject t_go2 = new GameObject("Music_1");
		m_aAudiosourceMusic[0] = t_go2.AddComponent<AudioSource>( );
		m_aAudiosourceMusic[0].priority = 0; //Highest priority
		GameObject t_go3 = new GameObject("Music_2");
		m_aAudiosourceMusic[1] = t_go3.AddComponent<AudioSource>( );
		m_aAudiosourceMusic[1].priority = 0; //Highest priority
        
        t_go2.transform.parent = gameObject.transform;
		t_go3.transform.parent = gameObject.transform;
    }

    public List<AudioClip> m_listaudioclipSFXs = new List<AudioClip>( );
	public List<float> m_listSFXVolume = new List<float>();
    public List<AudioClip> m_listaudioclipMusic = new List<AudioClip>( );


	public int iFromName(string p_strName)
	{
		int t_i = -1;
		if (System.Enum.IsDefined(typeof(ESound), p_strName))
		{
			ESound t_eSound = (ESound)System.Enum.Parse(typeof(ESound), p_strName);
			t_i = (int)t_eSound;
		}

		return t_i;
	}

    public void vEnsureListSizes( )
    {
        while( m_listaudioclipSFXs.Count < (int)ESound.Max )
        {
            m_listaudioclipSFXs.Add( null );
        }

		while (m_listSFXVolume.Count < (int)ESound.Max)
		{
			m_listSFXVolume.Add(1.0f);
		}

        while( m_listaudioclipMusic.Count < (int)EMusic.Max )
        {
            m_listaudioclipMusic.Add( null );
        }
    }

	
    public void vPlaySFX( CAudioHelper.ESound p_eSound )
    {
		if (p_eSound == ESound.Nil)
		{
			return;
		}

		m_audiosourceSFX[m_iSFXPlayerCur].PlayOneShot( m_listaudioclipSFXs[(int)p_eSound], m_listSFXVolume[(int)p_eSound] );
		if (++m_iSFXCycleCur >= m_cSFXCycle)
		{
			m_iSFXCycleCur = 0;
			if (++m_iSFXPlayerCur >= m_cSFXPlayers)
			{
				m_iSFXPlayerCur = 0;
			}
		}
    }

	public void vPlaySFX( CAudioHelper.ESound[] p_aeSound )
	{
		int t_c = p_aeSound.Length;
		int t_i = Random.Range(0, t_c - 1);
		vPlaySFX(p_aeSound[t_i]);
	}

	public void vStopSFX()
	{
		for (int t_i = 0; t_i < m_cSFXPlayers; ++t_i)
		{
			m_audiosourceSFX[t_i].Stop();
		}
	}

    public void vPlayMusic (CAudioHelper.EMusic p_eMusic, bool p_fLoop, bool p_fImmediate)
    {
		if (p_eMusic == m_eMusicActive)
		{
			return;
		}

		m_eMusicActive = p_eMusic;

		if (p_fImmediate)
		{
			m_aAudiosourceMusic[m_iActiveMusic].clip = m_listaudioclipMusic[(int)p_eMusic];
			m_aAudiosourceMusic[m_iActiveMusic].volume = 1.0f;
			m_aAudiosourceMusic[m_iActiveMusic].Play( );
			m_aAudiosourceMusic[m_iActiveMusic].loop = p_fLoop;
			return;
		}

		if (m_aAudiosourceMusic[m_iActiveMusic].isPlaying)
		{
			CAudioFade t_afOut = m_aAudiosourceMusic[m_iActiveMusic].gameObject.AddComponent<CAudioFade>();
			t_afOut.vSetup(0.0f, 1.5f);
		}

		m_iActiveMusic = 1 - m_iActiveMusic;

		m_aAudiosourceMusic[m_iActiveMusic].clip = m_listaudioclipMusic[(int)p_eMusic];
		m_aAudiosourceMusic[m_iActiveMusic].volume = 0.0f;
		m_aAudiosourceMusic[m_iActiveMusic].Play( );
		m_aAudiosourceMusic[m_iActiveMusic].loop = p_fLoop;
		CAudioFade t_afIn = m_aAudiosourceMusic[m_iActiveMusic].gameObject.AddComponent<CAudioFade>();
		t_afIn.vSetup(m_rMusicVolume, 3.0f);
    }

	public void vQueueMusicChange(CAudioHelper.EMusic p_eMusic, bool p_fLoop)
	{
		float t_rTimeTillEnd = m_aAudiosourceMusic[m_iActiveMusic].clip.length - m_aAudiosourceMusic[m_iActiveMusic].time;
		double t_rTimeChange = AudioSettings.dspTime + t_rTimeTillEnd;
		m_aAudiosourceMusic[m_iActiveMusic].SetScheduledEndTime(t_rTimeChange);

		m_iActiveMusic = 1 - m_iActiveMusic;
		
		m_aAudiosourceMusic[m_iActiveMusic].clip = m_listaudioclipMusic[(int)p_eMusic];
		m_aAudiosourceMusic[m_iActiveMusic].PlayScheduled(t_rTimeChange);
		m_aAudiosourceMusic[m_iActiveMusic].loop = p_fLoop;
	}
}
