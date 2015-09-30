using UnityEngine;
using System.Collections.Generic;



public interface ITween
{
    object Target();
    bool Finished();
    void Update(float t);
}

public class GOTween: ITween
{
    public delegate void TweenCallback(GameObject target);
    public delegate float EaseFunction(float timeRatio);

    public List<GOTweenAction> m_actions = new List<GOTweenAction>();
    
    public GOTween AddAction(GOTweenAction action)
    {
        m_actions.Add(action);
        return this;
    }
    public GOTween MoveTo(Vector3 newPosition)
    {
        return AddAction(new Move(m_target, newPosition));
    }
    public GOTween MoveLocalTo(Vector3 newPosition)
    {
        return AddAction(new MoveLocal(m_target, newPosition));
    }
    
    public GOTween ScaleTo(Vector3 newScale)
    {
        return AddAction(new Scale(m_target, newScale));
    }

    public GOTween ScaleWithRebound(Vector3 maxScale, Vector3 finalScale, float inDurationRatio = 0.75f)
    {
        return AddAction(new ScaleWithRebound(m_target, finalScale, maxScale, inDurationRatio));
    }

    public GOTween SpriteFade(float alpha)
    {
        return AddAction(new SpriteFade(m_target, alpha));
    }

    public GOTween RotateTo(Quaternion targetRotation)
    {
        return AddAction(new RotateTo(m_target, targetRotation));
    }

    public GOTween Rotate2D(float targetAngle)
    {
        return AddAction(new RotateTo(m_target, Quaternion.AngleAxis(targetAngle, Vector3.forward)));
    }
    
    protected GameObject m_target;
    private float m_elapsed;
    private float m_duration;

    private bool m_deleted;
    private EaseFunction m_ease;
    public EaseFunction Ease { get { return m_ease; } }
    protected TweenCallback m_completeCallback;
    protected TweenCallback m_updateCallback;

    public GOTween(GameObject target,
        float duration,
        EaseFunction easeFunction = null,
        TweenCallback completeCallback = null,
        TweenCallback updateCallback = null)
    {
        if (easeFunction == null)
        {
            m_ease = EaseFunctions.LinearNone;
        }
        else
        {
            m_ease = easeFunction;
        }
        
        m_target = target;
        m_duration = duration;
        m_elapsed = 0.0f;
        m_completeCallback = completeCallback;
        m_updateCallback = updateCallback;
    }

    public object Target ()
    {
        return m_target;
    }

    public bool Finished()
    {
        return Mathf.Approximately(m_elapsed, m_duration);
    }

    public void Update(float deltaTime)
    {
        m_elapsed += deltaTime;
		if (m_elapsed >= m_duration) 
		{
			m_elapsed = m_duration;
            if (m_completeCallback != null) 
			{
				m_completeCallback(m_target);
			}				
		}
        else
        {
            for (int i = 0; i < m_actions.Count; ++i)
            {
                m_actions[i].execute(m_ease(m_elapsed / m_duration));
            }

            if (m_updateCallback != null)
            {
                m_updateCallback(m_target);
            }
		
        }
    }
}
