using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class SnapToPosition : MonoBehaviour
{

    public bool isInteracting;
    public GameObject mySnapPosition;
    public float distanceToSnap;
    private Rigidbody rb;
    public bool isSnapped, isLerping;

    private float timeStamp;
    public float lerpTime;
    private bool animate;
    private Vector3 myLerpStartPos;
    private Quaternion myLerpStartRot;
    private FocusedObjectMessageReceiver focusedObjectReceiver;
    private Interpolator interpolator;
    private Vector3 animationStartingPosition;
    private bool animateUp;
    private Vector3 initialPosition;
    //private Material normalMat;
    private Material highGreen;
    private Material highNotReady;
    private uint attempts = 0;

    public bool Animate
    {
        get
        {
            return animate;
        }

        set
        {
            animate = value;

            if (value && interpolator != null)
            {
                interpolator.SlowPositionAnimation = true;
                this.animationStartingPosition = this.transform.position;
                interpolator.SetTargetPosition(this.transform.position + Vector3.down * 0.25f);
                animateUp = true;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        highGreen = Resources.Load("Materials/highGreen") as Material;
        highNotReady = Resources.Load("Materials/highNotReady") as Material;

        interpolator = GetComponent<Interpolator>();
        if (interpolator != null)
        {
            interpolator.InterpolationDone += Interpolator_InterpolationDone;
        }

        rb = GetComponent<Rigidbody>();
        focusedObjectReceiver = this.GetComponent<FocusedObjectMessageReceiver>();
    }

    private void Interpolator_InterpolationDone()
    {
        if (!animate)
        {
            return;
        }

        interpolator.enabled = true;
        interpolator.SetTargetPosition(this.transform.position + (animateUp ? Vector3.up : Vector3.down) * 0.25f);

        animateUp = !animateUp;
    }

    void OnSelect()
    {
        if (focusedObjectReceiver == null || this.isSnapped)
        {
            return;
        }

        if (focusedObjectReceiver.IsHighlighted)
        {
            mySnapPosition.GetComponent<Renderer>().enabled = true;
            mySnapPosition.SendMessage("OnStartHighlight");
            StartInteraction();
            focusedObjectReceiver.OnEndHighlight();
        }
        else if (isInteracting && Vector3.Distance(transform.position, mySnapPosition.transform.position) < distanceToSnap)
        {
            EndInteractionBySnapping();
        }
        else if (isInteracting)
        {
            EndInteraction();
            focusedObjectReceiver.OnStartHighlight();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var headPosition = Camera.main.transform.position;

        if (isInteracting)
        {
            if (Vector3.Distance(transform.position, mySnapPosition.transform.position) < distanceToSnap)
            {
                mySnapPosition.GetComponent<Renderer>().material = highGreen;
                //Debug.Log("InSnapArea!!");
            }
            else
            {
                mySnapPosition.GetComponent<Renderer>().material = highNotReady;
            }

            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                1.2f, Physics.DefaultRaycastLayers))
            {
                this.transform.position = hitInfo.point - (gazeDirection.normalized * 0.07f);
            }
            else
            {
                this.transform.position = headPosition + gazeDirection * 1.2f;
            }

            // Rotate this object to face the user.
            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            this.transform.rotation = toQuat;
        }
        // Did the object fall under the floor? Reset
        else if (Vector3.Distance(this.transform.position, headPosition) > 5.0f && this.transform.position.y < -5.0f)
        {
            Reset();
        }

        if (isLerping)
        {
            DoLerp();
        }
    }

    public void Reset()
    {
        this.isSnapped = false;
        this.animate = false;
        this.gameObject.layer = 0;
        GetComponent<Collider>().enabled = true;
        rb.isKinematic = false;

        if (this.interpolator != null)
        {
            interpolator.StopInterpolating();
            this.interpolator.Reset();
            this.interpolator.enabled = false;
        }
    }

    public void StartInteraction()
    {
        isInteracting = true;

        this.gameObject.layer = 2;
        // GetComponent<Collider>().enabled = false;
    }

    public void EndInteraction()
    {

        isInteracting = false;
        this.gameObject.layer = 0;
        GetComponent<Collider>().enabled = true;

        attempts++;
    }

    public void EndInteractionBySnapping()
    {
        isInteracting = false;
        rb.isKinematic = true;

        isLerping = true;
        myLerpStartPos = transform.position;
        myLerpStartRot = transform.rotation;
        timeStamp = Time.time;

        focusedObjectReceiver.OnEndHighlight();
        mySnapPosition.GetComponent<Renderer>().enabled = false;

        attempts++;
    }

    void DoLerp()
    {

        float fraction = (Time.time - timeStamp) / lerpTime;

        if (fraction >= 1)
        {
            transform.position = mySnapPosition.transform.position;
            transform.rotation = mySnapPosition.transform.rotation;
            isLerping = false;
            isSnapped = true;

            ScoreManager.Instance.OnScoreAttempts(this.attempts); // NB: update score first
            SceneManager.Instance.SendMessage("OnPlaced");
            
            
            if (GetComponent<CopyMeToRight>())
            {
                GetComponent<CopyMeToRight>().StartSpawning(transform.position);
            }
        }
        else
        {
            fraction = Mathf.Sin(fraction * Mathf.PI * 0.5f);

            transform.position = Vector3.Lerp(myLerpStartPos, mySnapPosition.transform.position, fraction);
            transform.rotation = Quaternion.Lerp(myLerpStartRot, mySnapPosition.transform.rotation, fraction);
        }
    }

}
