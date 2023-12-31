using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    public enum PlatformDirection
    {
        HORIZONTAL,
        VERTICAL,
        DIAGONAL_UP,
        DIAGONAL_DOWN,
        CUSTOM
    }

    public PlatformDirection direction;

    [Header("Movement Properties")]
    [Range(1.0f, 20.0f)]
    public float horizontalDistance = 8.0f;
    [Range(1.0f, 20.0f)]
    public float horizontalSpeed = 3.0f;
    [Range(1.0f, 20.0f)]
    public float verticalDistance = 8.0f;
    [Range(1.0f, 20.0f)]
    public float verticalSpeed = 3.0f;
    [Range(0.001f, 0.1f)]
    public float customSpeedFactor = 0.02f;

    [Header("Shrink Properties")]
    [Range(0.1f, 1.0f)]
    public float minScale = 0.5f;
    [Range(1.0f, 2.0f)]
    public float maxScale = 1.5f;
    public float shrinkSpeed = 1.0f;

    [Header("Platform Path Points")]
    public List<Transform> pathPoints;

    [Header("Audio Properties")]
    public AudioClip touchSound;
    public AudioClip shrinkSound;
    private AudioSource audioSource;

    private Vector2 startPoint;
    private Vector2 destinationPoint;
    private List<Vector2> pathList;
    private float timer;
    private int currentPointIndex;

    private bool isPlayerTouching = false;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        currentPointIndex = 0;
        startPoint = transform.position;
        pathList = new List<Vector2>();

        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector2 point = new Vector2(pathPoints[i].localPosition.x + startPoint.x,
                                        pathPoints[i].localPosition.y + startPoint.y);
            pathList.Add(point);
        }
        pathList.Add(transform.position);

        destinationPoint = pathList[currentPointIndex];

        // Initialize AudioSource component
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        // Check if the player is touching the platform
        if (isPlayerTouching)
        {
            StartCoroutine(ShrinkPlatformOverTime(2.0f));
        }
    }

    void FixedUpdate()
    {
        if (direction == PlatformDirection.CUSTOM)
        {
            if (timer <= 1.0f)
            {
                timer += customSpeedFactor;
            }

            if (timer >= 1.0f)
            {
                timer = 0.0f;

                currentPointIndex++;
                if (currentPointIndex >= pathList.Count)
                {
                    currentPointIndex = 0;
                }

                startPoint = transform.position;
                destinationPoint = pathList[currentPointIndex];
            }
        }
    }

    public void Move()
    {
        switch (direction)
        {
            case PlatformDirection.HORIZONTAL:
                transform.position = new Vector2(
                    Mathf.PingPong(horizontalSpeed * Time.time, horizontalDistance) + startPoint.x, startPoint.y);
                break;
            case PlatformDirection.VERTICAL:
                transform.position = new Vector2(startPoint.x,
                    Mathf.PingPong(verticalSpeed * Time.time, verticalDistance) + startPoint.y);
                break;
            case PlatformDirection.DIAGONAL_UP:
                transform.position = new Vector2(
                    Mathf.PingPong(horizontalSpeed * Time.time, horizontalDistance) + startPoint.x,
                    Mathf.PingPong(verticalSpeed * Time.time, verticalDistance) + startPoint.y);
                break;
            case PlatformDirection.DIAGONAL_DOWN:
                transform.position = new Vector2(
                    Mathf.PingPong(horizontalSpeed * Time.time, horizontalDistance) + startPoint.x,
                    startPoint.y - Mathf.PingPong(verticalSpeed * Time.time, verticalDistance));
                break;
            case PlatformDirection.CUSTOM:
                // Shrink effect
                float scaleFactor = Mathf.Lerp(maxScale, minScale, timer);
                transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

                // Movement
                transform.position = Vector2.Lerp(startPoint, destinationPoint, timer);
                break;
        }
    }

    // New method to handle player collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerTouching = true;

            // Play touch sound
            if (touchSound != null)
            {
                audioSource.PlayOneShot(touchSound);
            }
        }
    }

    // New method to handle player leaving the platform
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerTouching = false;
        }
    }

    // New method for shrinking the platform over time
    private IEnumerator ShrinkPlatformOverTime(float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            // Shrink effect
            float scaleFactor = Mathf.Lerp(maxScale, minScale, elapsedTime / duration);
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final scale is set to minScale
        transform.localScale = new Vector3(minScale, minScale, 1f);

        // Play shrink sound
        if (shrinkSound != null)
        {
            audioSource.PlayOneShot(shrinkSound);
        }
    }
}


