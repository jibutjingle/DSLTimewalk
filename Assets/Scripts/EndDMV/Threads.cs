// using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Threads : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    [Header("Shape")]
    public int segments = 16;
    public float arcHeight = 0.5f;

    [Header("Growth")]
    public float growDuration = 2f;
    // public bool autoStart = true;

    [Header("Intensity vs Distance")]
    public float farDistance = 10f;
    public float closeDistance = 2f;
    public float minIntensity = 1f;
    public float maxIntensity = 4f;
    public float minWidth = 0.05f;
    public float maxWidth = 0.1f;

    LineRenderer _line;
    Material _matInstance;
    float _growT = 0f;
    bool _growing = false;
    bool _shrinking = false;
    int _fillID;
    int _intensityID;

    void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = segments;

        _matInstance = _line.material = new Material(_line.material);
        _fillID = Shader.PropertyToID("_Fill");
        _intensityID = Shader.PropertyToID("_Intensity");
        // Debug.Log($"THREAD CREATED on {gameObject.scene.name} | InstanceID: {GetInstanceID()}");
    }

    // void OnEnable()
    // {
    //     if (autoStart) StartGrowth();
    // }

    //make threads grow
    public void Initialize(Transform start, Transform end)
    {
        startPoint = start;
        endPoint = end;
        StartGrowth();
    }

    //make threads shrink
    public void Deinitialize(Transform start, Transform end)
    {
        startPoint = start;
        endPoint = end;
        EndGrowth();
    }


    public void EndGrowth()
    {
        _growing = false;
        _shrinking = true;
    }

    public void StartGrowth()
    {
        // _growT = 0f;
        _shrinking = false;
        _growing = true;
    }

    void LateUpdate()
    {
        if (startPoint == null || endPoint == null) return;

        UpdateCurvePositions();

        if (_growing)
        {
            _growT += Time.deltaTime / growDuration;
            maxIntensity = 4f;
            if (_growT >= 1f)
            {
                _growT = 1f;
                _growing = false;
            }
        }
        else if (_shrinking)
        {
            _growT -= Time.deltaTime / growDuration;
            if (_growT <= 0f)
            {
                _growT = 0f;
                _shrinking = false;
                maxIntensity = 0f;
            }
        }

        if (_matInstance != null)
        {
            _matInstance.SetFloat(_fillID, _growT);
        }

        float distance = Vector3.Distance(startPoint.position, endPoint.position);
        float t = Mathf.InverseLerp(farDistance, closeDistance, distance);

        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        float width = Mathf.Lerp(minWidth, maxWidth, t);

        if (_matInstance != null)
        {
            _matInstance.SetFloat(_intensityID, intensity);
        }
        _line.startWidth = width;
        _line.endWidth = width;
    }

    void UpdateCurvePositions()
    {
        Vector3 p0 = startPoint.position;
        Vector3 p2 = endPoint.position;
        Vector3 mid = (p0 + p2) * 0.5f;

        Vector3 up = Vector3.up;
        Vector3 p1 = mid + up * arcHeight;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);

            Vector3 a = Vector3.Lerp(p0, p1, t);
            Vector3 b = Vector3.Lerp(p1, p2, t);
            Vector3 point = Vector3.Lerp(a, b, t);

            _line.SetPosition(i, point);
        }
    }
}
