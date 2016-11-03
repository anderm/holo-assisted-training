using UnityEngine;
using System.Collections;

/// <summary>
/// FocusedObjectMessageReceiver class shows how to handle messages sent by FocusedObjectMessageSender.
/// This particular implementatoin controls object appearance by changing its color when focused.
/// </summary>
public class FocusedObjectMessageReceiver : MonoBehaviour
{
    [Tooltip("Object color changes to this when focused.")]
    public Color FocusedColor = Color.red;
    public bool IsHighlighted { get; set; }

    private Material material;
    private Shader highLightShader;
    private Shader normalShader;

    private Color originalColor;
    private bool showHighlights;
    public bool isSnappable;

    private void Start()
    {
        material = GetComponent<Renderer>().material;

        if ( this.material.color != null && this.material.HasProperty("_Color") )
        {
            originalColor = material.color;
        }
        
        // Get the shaders
        normalShader = GetComponent<Renderer>().material.shader;
        highLightShader = Shader.Find("OutlinedDiffBump");
    }

    public void OnGazeEnter()
    {
        if (material != null && material.color != null)
        {
            material.color = FocusedColor;
        }

    }

    public void OnGazeLeave()
    {
        if (material != null && material.color != null)
        {
            material.color = originalColor;
        }
    }

    public void OnStartHighlight()
    {

        GetComponent<Renderer>().material.shader = highLightShader;

        IsHighlighted = true;
    }

    public void OnEndHighlight()
    {
        GetComponent<Renderer>().material.shader = normalShader;

        IsHighlighted = false;
    }

    public void OnRotate()
    {
        transform.Rotate(0, 0, 180);
    }
}