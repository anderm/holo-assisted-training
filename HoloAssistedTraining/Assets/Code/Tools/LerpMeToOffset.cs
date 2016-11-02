using UnityEngine;
using System.Collections;

public class LerpMeToOffset : MonoBehaviour
{
    private Vector3 offset;
    private bool lerpNow;
    private float timeStamp;
    private float lerpTime = 1f;
    private Vector3 targetPos;
    private Vector3 origPos;
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (lerpNow)
        {
            float fraction = (Time.time - timeStamp) / lerpTime;
            if (fraction >= 1)
            {
                gameObject.GetComponent<SnapToPosition>().Animate = true;
                transform.position = targetPos;
                if (GetComponent<Animator>())
                {
                    GetComponent<Animator>().enabled = true;
                }
                this.enabled = false;
            }
            else
            {
                fraction = Mathf.Sin(fraction * Mathf.PI * 0.5f);
                transform.position = Vector3.Lerp(origPos, targetPos, fraction);
            }
        }
    }

    public void SetOffset(float offsettooo)
    {
        lerpNow = true;
        origPos = transform.position;
        targetPos = transform.position + -transform.forward * offsettooo;
        timeStamp = Time.time;
    }
}
