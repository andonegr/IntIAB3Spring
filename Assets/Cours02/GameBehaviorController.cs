using UnityEngine;
using TMPro;

public class GameBehaviorController : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField inputField;
    public Renderer targetRenderer;
    public Transform followTarget;
    public TMP_Text statusText;

    [Header("Movement settings")]
    public float moveSpeed = 0f;
    public float rotateSpeed = 50f;
    public float orbitDistance = 3f;

    private string currentMode = "default";

    void Update()
    {
        if (currentMode == "chase" && followTarget != null)
        {
            Vector3 direction = (followTarget.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else if (currentMode == "flee" && followTarget != null)
        {
            Vector3 direction = (transform.position - followTarget.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else if (currentMode == "orbit" && followTarget != null)
        {
            transform.RotateAround(followTarget.position, Vector3.up, rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    public void ApplyBehavior()
    {
        string input = inputField.text.ToLower().Trim();

        ResetBehavior();

        if (input.Contains("fast"))
        {
            moveSpeed = 4f;
            rotateSpeed = 120f;
            SetColor(Color.yellow);
            currentMode = "fast";
        }
        else if (input.Contains("calm"))
        {
            moveSpeed = 0.5f;
            rotateSpeed = 20f;
            SetColor(Color.blue);
            currentMode = "calm";
        }
        else if (input.Contains("aggressive"))
        {
            moveSpeed = 5f;
            rotateSpeed = 180f;
            SetColor(Color.red);
            currentMode = "aggressive";
        }
        else if (input.Contains("chase"))
        {
            moveSpeed = 3f;
            rotateSpeed = 80f;
            SetColor(Color.red);
            currentMode = "chase";
        }
        else if (input.Contains("flee"))
        {
            moveSpeed = 3f;
            rotateSpeed = 80f;
            SetColor(Color.cyan);
            currentMode = "flee";
        }
        else if (input.Contains("orbit"))
        {
            moveSpeed = 0f;
            rotateSpeed = 60f;
            SetColor(Color.magenta);
            currentMode = "orbit";
        }

        if (statusText != null)
            statusText.text = "Current mode: " + currentMode;
    }

    void ResetBehavior()
    {
        moveSpeed = 1f;
        rotateSpeed = 50f;
        currentMode = "default";
        transform.localScale = Vector3.one;
        SetColor(Color.white);
    }

    void SetColor(Color color)
    {
        if (targetRenderer != null)
            targetRenderer.material.color = color;
    }

    public void ApplyMode(string mode)
    {
        inputField.text = mode;
        ApplyBehavior();
    }
}