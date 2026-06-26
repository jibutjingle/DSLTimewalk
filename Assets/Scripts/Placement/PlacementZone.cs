using System.Collections.Generic;
using RvSdk.Component;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NetworkSyncedInteger))]
public class PlacementZone : MonoBehaviour
{
    const int NotOccupied = 0;
    const int Occupied = 1;

    // runtime lookups built in OnEnable — used by PlaceableObject
    static readonly Dictionary<int, PlacementZone> ZonesByIdHash = new();
    static readonly List<PlacementZone> AllActiveZones = new();

    [Header("Identity")]
    // unique name sent in place requests
    [SerializeField] string zoneId;

    [Header("Matching")]
    // must match PlaceableObject.placeableId on the piece that belongs here
    [Tooltip("Leave empty to accept any placeable. Otherwise only matching PlaceableObject ids snap here.")]
    [SerializeField] string acceptedPlaceableId;

    [Header("Snap")]
    // empty child transform where the object should end up
    [SerializeField] Transform snapPose;
    // how close the held object must be before PlaceableObject requests a snap
    [SerializeField] float snapDistance = 0.5f;
    [SerializeField] float rotationToleranceDeg = 180f;

    [Header("Events")]
    public UnityEvent onOccupied;

    NetworkSyncedInteger _occupiedSync;
    int _zoneIdHash;
    // written into DreamscapeGrabbable.stateSync when something is placed here
    int _placedStateValue;
    bool _wasOccupied;

    public string ZoneId => zoneId;
    public string AcceptedPlaceableId => acceptedPlaceableId;
    public int ZoneIdHash => _zoneIdHash;
    public int PlacedStateValue => _placedStateValue;
    public float SnapDistance => snapDistance;
    public Vector3 SnapPosition => snapPose != null ? snapPose.position : transform.position;
    public Quaternion SnapRotation => snapPose != null ? snapPose.rotation : transform.rotation;
    public bool IsOccupied => _occupiedSync != null && _occupiedSync.Value == Occupied;
    public int OccupiedSyncValue => _occupiedSync != null ? _occupiedSync.Value : NotOccupied;
    public static IReadOnlyList<PlacementZone> ActiveZones => AllActiveZones;

    void Awake()
    {
        _occupiedSync = GetComponent<NetworkSyncedInteger>();
        _zoneIdHash = GetStableIdHash(zoneId);
        _placedStateValue = PlaceableObjectNetworkState.EncodePlaced(zoneId);
    }

    void OnEnable()
    {
        if (_zoneIdHash != 0)
            ZonesByIdHash[_zoneIdHash] = this;

        AllActiveZones.Add(this);

        if (_occupiedSync != null)
        {
            _wasOccupied = IsOccupied;
            _occupiedSync.OnValueChanged.AddListener(OnOccupiedSyncChanged);
        }
    }

    void OnDisable()
    {
        if (_zoneIdHash != 0)
            ZonesByIdHash.Remove(_zoneIdHash);

        AllActiveZones.Remove(this);

        if (_occupiedSync != null)
            _occupiedSync.OnValueChanged.RemoveListener(OnOccupiedSyncChanged);
    }

    void OnOccupiedSyncChanged(int value)
    {
        bool occupied = value == Occupied;
        if (occupied && !_wasOccupied)
            onOccupied.Invoke();

        _wasOccupied = occupied;
    }

    public static PlacementZone FindByIdHash(int zoneIdHash)
    {
        ZonesByIdHash.TryGetValue(zoneIdHash, out PlacementZone zone);
        return zone;
    }

    public static PlacementZone FindByZoneId(string targetZoneId)
    {
        if (string.IsNullOrEmpty(targetZoneId))
            return null;

        for (int i = 0; i < AllActiveZones.Count; i++)
        {
            PlacementZone zone = AllActiveZones[i];
            if (zone.ZoneId == targetZoneId)
                return zone;
        }

        return null;
    }

    public static PlacementZone FindByPlacedStateValue(int placedStateValue)
    {
        for (int i = 0; i < AllActiveZones.Count; i++)
        {
            PlacementZone zone = AllActiveZones[i];
            if (zone.PlacedStateValue == placedStateValue)
                return zone;
        }

        return null;
    }

    public bool AcceptsPlaceable(PlaceableObject placeable)
    {
        if (placeable == null)
            return false;

        if (IsEffectivelyOccupied(placeable))
            return false;

        // empty acceptedPlaceableId accepts any piece
        if (string.IsNullOrEmpty(acceptedPlaceableId))
            return true;

        return placeable.PlaceableId == acceptedPlaceableId;
    }

    bool IsEffectivelyOccupied(PlaceableObject placeable)
    {
        if (_occupiedSync == null || _occupiedSync.Value == NotOccupied)
            return false;

        // only exactly 1 counts as occupied — not any non-zero value
        if (_occupiedSync.Value != Occupied)
            return false;

        DreamscapeGrabbable grabbable = placeable.GetComponent<DreamscapeGrabbable>();

        if (placeable.PlaceableId == acceptedPlaceableId
            && !placeable.IsPlaced
            && grabbable != null
            && grabbable.IsHeldByAnyone)
            return false;

        return true;
    }

    public bool IsWithinSnapRange(Vector3 position, Quaternion rotation)
    {
        if (Vector3.Distance(position, SnapPosition) > snapDistance)
            return false;

        // set rotationToleranceDeg below zero to ignore rotation
        if (rotationToleranceDeg < 0f)
            return true;

        return Quaternion.Angle(rotation, SnapRotation) <= rotationToleranceDeg;
    }

    internal void SetOccupiedOnServer()
    {
        // server only — called from PlaceableObject.TryPlaceOnServer
        if (_occupiedSync != null)
            _occupiedSync.Value = Occupied;
    }

    void OnValidate()
    {
        if (!string.IsNullOrEmpty(zoneId))
        {
            _zoneIdHash = GetStableIdHash(zoneId);
            _placedStateValue = PlaceableObjectNetworkState.EncodePlaced(zoneId);
        }
    }

    static int GetStableIdHash(string id)
    {
        return string.IsNullOrEmpty(id) ? 0 : id.GetHashCode();
    }
}
