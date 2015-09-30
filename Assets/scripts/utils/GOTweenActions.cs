using UnityEngine;
using System.Collections;

public abstract class GOTweenAction
{
    protected GameObject m_target;
    public GOTweenAction(GameObject target)
    {
        m_target = target;
    }
    public abstract void execute(float t);
}

public class Move : GOTweenAction
{
    Vector3 m_start;
    Vector3 m_destination;
    public Move(GameObject target, Vector3 destination)
        : base(target)
    {
        m_start = m_target.transform.position;
        m_destination = destination;
    }

    public override void execute(float t)
    {
        m_target.transform.position = Vector3.Lerp(m_start, m_destination, t); ;
    }
}

public class MoveLocal : GOTweenAction
{
    Vector3 m_start;
    Vector3 m_destination;

    public MoveLocal(GameObject target, Vector3 destination)
        : base(target)
    {
        m_start = m_target.transform.localPosition;
        m_destination = destination;
    }

    public override void execute(float t)
    {
        m_target.transform.localPosition = Vector3.Lerp(m_start, m_destination, t);
    }
}

public class Scale : GOTweenAction
{
    Vector3 m_start;
    Vector3 m_destination;
    public Scale(GameObject target, Vector3 destination)
        : base(target)
    {
        m_start = m_target.transform.localScale;
        m_destination = destination;
    }

    public override void execute(float t)
    {
        m_target.transform.localScale = Vector3.Lerp(m_start, m_destination, t);
    }
}

public class ScaleWithRebound : GOTweenAction
{
    Vector3 m_start;
    Vector3 m_destination;
    Vector3 m_maxScale;
    float m_ratio;
    public ScaleWithRebound(GameObject target, Vector3 destination, Vector3 maxScale, float ratio = 0.75f)
        : base(target)
    {
        m_start = m_target.transform.localScale;
        m_destination = destination;
        m_maxScale = maxScale;
        m_ratio = ratio;
    }

    public override void execute(float t)
    {
        if (t < m_ratio)
        {
            m_target.transform.localScale = Vector3.Lerp(m_start, m_maxScale,t);
        }
        else
        {
            m_target.transform.localScale = Vector3.Lerp(m_maxScale, m_destination, t);
        }
    }
}

public class SpriteFade: GOTweenAction
{
    SpriteRenderer m_renderer;
    float m_startAlpha;
    float m_finalAlpha;

    public SpriteFade(GameObject target, float finalAlpha)
        : base(target)
    {
        m_renderer = m_target.GetComponent<SpriteRenderer>();
        Debug.Assert(m_renderer != null, "NULL SPRITE RENDERER!!");
        m_startAlpha = m_renderer.color.a;
        m_finalAlpha = finalAlpha;
    }

    public override void execute(float t)
    {
        Debug.Assert(m_renderer != null, "NULL SPRITE RENDERER!!");
        Color c = m_renderer.color;
        c.a = Mathf.Lerp(m_startAlpha, m_finalAlpha, t);
        m_renderer.color = c;
    }
}

public class RotateTo : GOTweenAction
{
    Quaternion m_startRotation;
    Quaternion m_finalRotation;

    public RotateTo(GameObject target, Quaternion finalRotation)
        : base(target)
    {
        m_startRotation = m_target.transform.rotation;
        m_finalRotation = finalRotation;
    }

    public override void execute(float t)
    {
        m_target.transform.rotation = Quaternion.Slerp(m_startRotation, m_finalRotation, t);
    }
}