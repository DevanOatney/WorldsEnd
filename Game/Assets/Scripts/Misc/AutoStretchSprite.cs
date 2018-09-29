using UnityEngine;
using System.Collections;
[RequireComponent(typeof(SpriteRenderer))]

public class AutoStretchSprite : MonoBehaviour {

	/// <summary> Do you want the sprite to maintain the aspect ratio? </summary>
	public bool KeepAspectRatio = true;
	/// <summary> Do you want it to continually check the screen size and update? </summary>
	public bool ExecuteOnUpdate = true;
	float m_fUpdateTimer = 0.0f;
	float m_fUpdateBucket = 0.3f;
    GameObject m_goPathfinder;

	void Start ()
    {
		Resize(KeepAspectRatio);
	}

	void FixedUpdate () 
	{
        if (ExecuteOnUpdate)
		{
			m_fUpdateTimer += Time.deltaTime;
			if(m_fUpdateTimer >= m_fUpdateBucket)
			{
				m_fUpdateTimer = 0.0f;
				Resize(KeepAspectRatio);
			}
		}
	}

	/// <summary>
	/// Resize the attached sprite according to the camera view
	/// </summary>
	/// <param name="keepAspect">bool : if true, the image aspect ratio will be retained</param>
	void Resize(bool keepAspect = false)
	{

		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		//transform.localScale = new Vector3(1, 1, 1);

		// example of a 640x480 sprite
		float width = sr.sprite.bounds.size.x; // 4.80f
		float height = sr.sprite.bounds.size.y; // 6.40f

		// and a 2D camera at 0,0,-10
		float worldScreenHeight = Camera.main.orthographicSize * 2f; // 10f
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width; // 10f

		Vector3 imgScale = new Vector3(1f, 1f, 1f);

		// do we scale according to the image, or do we stretch it?
		if (keepAspect)
		{
			Vector2 ratio = new Vector2(width / height, height / width);
			if ((worldScreenWidth / width) > (worldScreenHeight / height))
			{
				// wider than tall
				imgScale.x = worldScreenWidth / width;
				imgScale.y = imgScale.x * ratio.y; 
			}
			else
			{
				// taller than wide
				imgScale.y = worldScreenHeight / height;
				imgScale.x = imgScale.y * ratio.x;             
			}
		}
		else
		{
			imgScale.x = worldScreenWidth / width;
			imgScale.y = worldScreenHeight / height;
		}

        // apply change
        //Something in the ratio has changed.
        if (transform.localScale != imgScale)
        {
            transform.localScale = imgScale;
            if (m_goPathfinder == null)
                m_goPathfinder = GameObject.Find("Pathfinder");
            m_goPathfinder.GetComponent<CGrid>().GridResized(gameObject);
        }
		
	}
}
