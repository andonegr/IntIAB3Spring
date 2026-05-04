using UnityEngine;
using TMPro;

public class SimpleBehaviorController : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField inputField;
    Renderer targetRenderer;
    public TMP_Text statusText;

    [Header("Movement settings")]
    public float moveSpeed = 0f;
    public float rotateSpeed = 50f;
    public bool randomMovement = false;

    private Vector3 moveDirection = Vector3.forward;

    private void Start()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Basic forward movement
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Optional rotation
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        // Optional random direction changes
        if (randomMovement)
        {
            transform.Translate(
                new Vector3(Mathf.Sin(Time.time), 0, Mathf.Cos(Time.time))
                * moveSpeed * 0.3f * Time.deltaTime
            );
        }
    }

    public void ApplyBehavior()
    {
        if (inputField == null) return;

        string input = inputField.text.ToLower().Trim();

        // Default values
        moveSpeed = 0f;
        rotateSpeed = 50f;
        randomMovement = false;
        moveDirection = Vector3.forward;
        transform.localScale = Vector3.one;

        if (targetRenderer != null)
            targetRenderer.material.color = Color.white;

        string currentMode = "default";

        if (input.Contains("fast"))
        {
            moveSpeed = 4f;
            rotateSpeed = 100f;
            if (targetRenderer != null)
                targetRenderer.material.color = Color.yellow;
            currentMode = "fast";
        }
        else if (input.Contains("calm"))
        {
            moveSpeed = 0.5f;
            rotateSpeed = 20f;
            if (targetRenderer != null)
                targetRenderer.material.color = Color.blue;
            currentMode = "calm";
        }
        else if (input.Contains("aggressive"))
        {
            moveSpeed = 5f;
            rotateSpeed = 150f;
            if (targetRenderer != null)
                targetRenderer.material.color = Color.red;
            currentMode = "aggressive";
        }
        else if (input.Contains("random"))
        {
            moveSpeed = 2f;
            rotateSpeed = 80f;
            randomMovement = true;
            if (targetRenderer != null)
                targetRenderer.material.color = new Color(Random.value, Random.value, Random.value);
            currentMode = "random";
        }
        else if (input.Contains("heavy"))
        {
            moveSpeed = 0.7f;
            rotateSpeed = 10f;
            transform.localScale = new Vector3(2f, 2f, 2f);
            if (targetRenderer != null)
                targetRenderer.material.color = Color.gray;
            currentMode = "heavy";
        }
        else if (input.Contains("light"))
        {
            moveSpeed = 2.5f;
            rotateSpeed = 120f;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            if (targetRenderer != null)
                targetRenderer.material.color = Color.cyan;
            currentMode = "light";
        }
        else
        {
            transform.localScale = Vector3.one;
        }

        if (statusText != null)
            statusText.text = "Current mode: " + currentMode;
    }
}