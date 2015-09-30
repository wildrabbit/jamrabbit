using UnityEngine;
using System.Collections;

public class CarrotTest : MonoBehaviour 
{
    private bool m_taken = false;

	// Use this for initialization
	void Start () 
{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void KillMe ()
    {
        if (!m_taken)
        {
            m_taken = false;

            GOTween tween = new GOTween(this.gameObject, 1.0f, EaseFunctions.Sine.EaseIn, (x) => Destroy(x)).ScaleTo(new Vector3(15.0f, 15.0f, 1.0f)).SpriteFade(0.2f);
            GOTween otherTween = new GOTween(this.gameObject, 0.8f, EaseFunctions.QuartInOut).MoveLocalTo(2 * Vector2.up);
            TweenController.Launch(tween);
            TweenController.Launch(otherTween);
            m_taken = true;
            tween = null;
            otherTween = null;
        }
    }
}
