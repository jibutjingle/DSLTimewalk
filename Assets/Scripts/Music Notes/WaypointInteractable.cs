using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WaypointInteractable : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasPlayed = false;

    // Flag to control floating/movement
    private bool canFloat = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.Stop(); // Make sure it's stopped
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if it collided with a waypoint tagged object
        if (!hasPlayed && other.CompareTag("Waypoint"))
        {
            //audioSource.Play();
            hasPlayed = true;
            Debug.Log("Audio started on waypoint placement.");

            // Stop floating or movement here
            canFloat = false;

            // Freeze position by disabling Rigidbody movement (optional)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Prevent physics movement
            }
        }
    }

    void Update()
    {
        if (canFloat)
        {
            // Your floating logic here, or call a floating method
            FloatObject();
        }
    }

    private void FloatObject()
    {
        // Example floating logic
        float floatAmplitude = 0.5f;
        float floatFrequency = 1f;
        Vector3 pos = transform.position;
        pos.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude * Time.deltaTime;
        transform.position = pos;
    }
}
