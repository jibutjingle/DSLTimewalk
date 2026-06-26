using RvSdk.Avatar;
using RvSdk.Component;
using RvSdk.Controller;
using RvSdk.Module;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DreamscapeGrabbable))]
public class PlaceableObject : MonoBehaviour
{
    // ClientToServerTrigger argument prefix for snap requests
    public const string PlaceArgumentPrefix = "Place|";

    const float PlaceRequestTimeoutSeconds = 0.75f;

    [Header("Identity")]
    // must match PlacementZone.acceptedPlaceableId for the target slot
    [SerializeField] string placeableId;

    [Header("Detection")]
    // coarse search — zones farther than this are ignored entirely
    [SerializeField] float zoneSearchRadius = 1f;

    [Header("References")]
    [SerializeField] DreamscapeGrabbable grabbable;
    [SerializeField] ClientToServerTrigger serverTrigger;

    [Header("Events")]
    public UnityEvent onPlaced;

    bool _placeRequestPending;
    float _placeRequestTime;

    public string PlaceableId => placeableId;
    // reads placed state from the shared DreamscapeGrabbable.stateSync integer
    public bool IsPlaced => grabbable != null && grabbable.IsPlaced;

    void Awake()
    {
        if (grabbable == null)
            grabbable = GetComponent<DreamscapeGrabbable>();

        if (serverTrigger == null)
            serverTrigger = GetComponent<ClientToServerTrigger>();
    }

    void OnEnable()
    {
        if (grabbable != null && grabbable.StateSync != null)
            grabbable.StateSync.OnValueChanged.AddListener(OnStateSyncChanged);

        // shares ClientToServerTrigger with DreamscapeGrabbable — different arguments
        if (serverTrigger != null)
            serverTrigger.OnTriggerWithArg.AddListener(HandleServerTriggerWithArg);

        // Debug.Log($"[PlaceableObject] {name}: OnEnable - placeableId='{placeableId}', activeZones={PlacementZone.ActiveZones.Count}");

        if (IsPlaced)
            ApplyPlacedState();
    }

    void OnDisable()
    {
        if (grabbable != null && grabbable.StateSync != null)
            grabbable.StateSync.OnValueChanged.RemoveListener(OnStateSyncChanged);

        if (serverTrigger != null)
            serverTrigger.OnTriggerWithArg.RemoveListener(HandleServerTriggerWithArg);
    }

    void Update()
    {
        if (_placeRequestPending && Time.time - _placeRequestTime >= PlaceRequestTimeoutSeconds)
            _placeRequestPending = false;

        if (!NetworkGate.IsInitialized || !NetworkGate.IsClient)
        {
            // Debug.Log($"[PlaceableObject] {name}: Update skipped - NetworkGate not ready (Initialized={NetworkGate.IsInitialized}, IsClient={NetworkGate.IsClient})");
            return;
        }

        // only the local holder tries to snap — no release required
        if (IsPlaced || grabbable == null || !grabbable.IsHeldByLocalPlayer)
        {
            // Debug.Log($"[PlaceableObject] {name}: Update skipped - IsPlaced={IsPlaced}, grabbableNull={grabbable == null}, IsHeldByLocalPlayer={grabbable != null && grabbable.IsHeldByLocalPlayer}");
            return;
        }

        TrySnapWhileHeld();
    }

    void TrySnapWhileHeld()
    {
        if (_placeRequestPending || serverTrigger == null)
        {
            // Debug.Log($"[PlaceableObject] {name}: TrySnapWhileHeld skipped - placeRequestPending={_placeRequestPending}, serverTriggerNull={serverTrigger == null}");
            return;
        }

        PlacementZone zone = FindBestZone();
        if (zone == null)
        {
            // Debug.Log($"[PlaceableObject] {name}: TrySnapWhileHeld - no zone found");
            return;
        }

        // Debug.Log($"[PlaceableObject] {name}: TrySnapWhileHeld - sending place request for zone '{zone.ZoneId}'");
        _placeRequestPending = true;
        _placeRequestTime = Time.time;
        // same ClientToServerTrigger as grab — different argument string
        serverTrigger.Trigger(PlaceArgumentPrefix + zone.ZoneId);
    }

    PlacementZone FindBestZone()
    {
        PlacementZone best = null;
        float bestDistSq = float.MaxValue;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        float searchRadiusSq = zoneSearchRadius * zoneSearchRadius;

        // Debug.Log($"[PlaceableObject] {name}: FindBestZone - placeableId='{placeableId}', activeZones={PlacementZone.ActiveZones.Count}, searchRadius={zoneSearchRadius}, pos={position}");

        for (int i = 0; i < PlacementZone.ActiveZones.Count; i++)
        {
            PlacementZone zone = PlacementZone.ActiveZones[i];
            if (!zone.AcceptsPlaceable(this))
            {
                // Debug.Log($"[PlaceableObject] {name}: Zone '{zone.ZoneId}' rejected - AcceptsPlaceable=false (my placeableId='{placeableId}', occupied={zone.IsOccupied}, occupiedSyncValue={zone.OccupiedSyncValue})");
                continue;
            }

            float distSq = (zone.SnapPosition - position).sqrMagnitude;
            float dist = Mathf.Sqrt(distSq);
            if (distSq > searchRadiusSq)
            {
                // Debug.Log($"[PlaceableObject] {name}: Zone '{zone.ZoneId}' rejected - outside search radius (dist={dist:F2}, max={zoneSearchRadius})");
                continue;
            }

            // zoneSearchRadius is loose — snapDistance on the zone is the tight check
            if (!zone.IsWithinSnapRange(position, rotation))
            {
                float snapDist = Vector3.Distance(position, zone.SnapPosition);
                float angle = Quaternion.Angle(rotation, zone.SnapRotation);
                // Debug.Log($"[PlaceableObject] {name}: Zone '{zone.ZoneId}' rejected - outside snap range (snapDist={snapDist:F2}, max={zone.SnapDistance}, angle={angle:F1})");
                continue;
            }

            // Debug.Log($"[PlaceableObject] {name}: Zone '{zone.ZoneId}' is a valid candidate (dist={dist:F2})");

            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = zone;
            }
        }

        // Debug.Log($"[PlaceableObject] {name}: FindBestZone result = {(best != null ? best.ZoneId : "null")}");
        return best;
    }

    void HandleServerTriggerWithArg(string triggerName, AvatarController avatar, string argument)
    {
        if (!NetworkController.IsServer || avatar == null)
        {
            // Debug.Log($"[PlaceableObject] {name}: HandleServerTriggerWithArg skipped - IsServer={NetworkController.IsServer}, avatarNull={avatar == null}");
            return;
        }

        if (!argument.StartsWith(PlaceArgumentPrefix))
            return;

        string targetZoneId = argument.Substring(PlaceArgumentPrefix.Length);
        // Debug.Log($"[PlaceableObject] {name}: HandleServerTriggerWithArg - place request for zone '{targetZoneId}'");

        PlacementZone zone = PlacementZone.FindByZoneId(targetZoneId);
        if (zone == null)
        {
            // Debug.Log($"[PlaceableObject] {name}: HandleServerTriggerWithArg - zone '{targetZoneId}' not found on server");
            return;
        }

        TryPlaceOnServer(avatar, zone);
    }

    void TryPlaceOnServer(AvatarController avatar, PlacementZone zone)
    {
        // Debug.Log($"[PlaceableObject] {name}: TryPlaceOnServer - zone '{zone.ZoneId}'");

        if (IsPlaced || grabbable == null)
        {
            // Debug.Log($"[PlaceableObject] {name}: TryPlaceOnServer rejected - IsPlaced={IsPlaced}, grabbableNull={grabbable == null}");
            return;
        }

        if (!zone.AcceptsPlaceable(this))
        {
            // Debug.Log($"[PlaceableObject] {name}: TryPlaceOnServer rejected - zone does not accept this placeable (occupied={zone.IsOccupied}, occupiedSyncValue={zone.OccupiedSyncValue})");
            return;
        }

        if (!grabbable.IsHeldBy(avatar))
        {
            // Debug.Log($"[PlaceableObject] {name}: TryPlaceOnServer rejected - not held by requesting avatar (holder={grabbable.HolderHash})");
            return;
        }

        // Debug.Log($"[PlaceableObject] {name}: TryPlaceOnServer success - placing in zone '{zone.ZoneId}'");
        grabbable.SetPlacedOnServer(zone);
        zone.SetOccupiedOnServer();
    }

    void OnStateSyncChanged(int value)
    {
        // Debug.Log($"[PlaceableObject] {name}: OnStateSyncChanged - value={value}, isPlaced={PlaceableObjectNetworkState.IsPlaced(value)}");

        if (!PlaceableObjectNetworkState.IsPlaced(value))
            return;

        _placeRequestPending = false;
        // runs on every client when stateSync becomes a placed value
        ApplyPlacedState();
        onPlaced.Invoke();
    }

    void ApplyPlacedState()
    {
        if (grabbable == null || grabbable.StateSync == null)
        {
            // Debug.Log($"[PlaceableObject] {name}: ApplyPlacedState skipped - grabbableNull={grabbable == null}, stateSyncNull={grabbable == null || grabbable.StateSync == null}");
            return;
        }

        PlacementZone zone = PlacementZone.FindByPlacedStateValue(grabbable.StateSync.Value);
        if (zone == null)
        {
            // Debug.Log($"[PlaceableObject] {name}: ApplyPlacedState - no zone for placed state value {grabbable.StateSync.Value}");
            return;
        }

        // Debug.Log($"[PlaceableObject] {name}: ApplyPlacedState - snapping to zone '{zone.ZoneId}'");
        grabbable.SnapToWorldPose(zone.SnapPosition, zone.SnapRotation);
    }

#if UNITY_EDITOR
    public void AutoAssignComponents()
    {
        grabbable = GetComponent<DreamscapeGrabbable>();
        serverTrigger = GetComponent<ClientToServerTrigger>();
    }
#endif
}
