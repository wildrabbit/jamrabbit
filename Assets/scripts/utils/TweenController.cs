using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TweenController : MonoBehaviour 
{
    private static TweenController m_instance;

    private List<ITween> m_tweens;
    private float m_timer;
    
    public static TweenController Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GameObject("TweenController").AddComponent<TweenController>();
            }
            return m_instance;
        }
    }

    public static void Launch(ITween tween)
    {
        m_instance.m_tweens.Add(tween);
        if (!m_instance.gameObject.activeSelf)
        {
            m_instance.gameObject.SetActive(true);
            m_instance.m_timer = Time.realtimeSinceStartup;
        }
    }

    public static void Stop (ITween tween)
    {
        if (m_instance.m_tweens == null) { return; }
        
        if (m_instance.m_tweens.Contains(tween))
        {
            m_instance.m_tweens.Remove(tween);
        }
        if (m_instance.m_tweens.Count == 0)
        {
            m_instance.gameObject.SetActive(false);
        }
    }

    public static void ClearTweensFromTarget (object target)
    {
        List<ITween> toRemove = m_instance.m_tweens.FindAll(x => x.Target() == target);
        
        if (toRemove!= null && toRemove.Count > 0)
        {
            for (int i = 0; i < toRemove.Count; ++i)
            {
                m_instance.m_tweens.Remove(toRemove[i]);
            }
        }

        if (m_instance.m_tweens.Count == 0)
        {
            m_instance.gameObject.SetActive(false);
        }
    }

	// Use this for initialization
	void Awake() 
    {
	    m_tweens = new List<ITween>();
        m_timer = 0.0f;
        m_instance = this;
        m_instance.gameObject.SetActive(false);
	}

    void Start()
    {
        m_timer = Time.realtimeSinceStartup;
    }
	
	// Update is called once per frame
	void Update () 
    {
        float deltaTime = Time.realtimeSinceStartup - m_timer;
        m_timer = Time.realtimeSinceStartup;

        List<ITween> toRemove = new List<ITween>();
        for (int i = 0; i < m_tweens.Count; ++i)
        {
            m_tweens[i].Update(deltaTime);
            if (m_tweens[i].Finished())
            {
                toRemove.Add(m_tweens[i]);
            }
        }
        for (int i = 0; i < toRemove.Count; ++i)
        {
            m_tweens.Remove(toRemove[i]);
        }

        if (m_tweens.Count == 0)
        {
            gameObject.SetActive(false);
        }

        toRemove.Clear();
        toRemove = null;
	}

    void OnDestroy()
    {
        m_tweens.Clear();        
    }


}
