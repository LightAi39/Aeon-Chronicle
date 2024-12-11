using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour, ISaveable
{
    private string _guid;
    public string GUID => _guid;
    
    [SerializeField] private string prefabID;
    public string PrefabID => prefabID;

    
    [SerializeField]
    private int movementPoints = 20;
    public int MovementPoints { get => movementPoints; }

    [SerializeField]
    private float movementDuration = 1, rotationDuration = 0.3f;

    private GlowHighlight glowHighlight;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();

    public event Action<Unit> MovementFinished;

    private void Awake()
    {
        glowHighlight = GetComponent<GlowHighlight>();
        
        if (string.IsNullOrEmpty(_guid))
        {
            _guid = Guid.NewGuid().ToString();
        }
    }

    public void Deselect()
    {
        glowHighlight.ToggleGlow(false);
    }

    public void Select()
    {
        glowHighlight.ToggleGlow();
    }

    public void MoveThroughPath(List<Vector3> currentPath)
    {
        pathPositions = new Queue<Vector3>(currentPath);
        Vector3 firstTarget = pathPositions.Dequeue();
        StartCoroutine(RotationCoroutine(firstTarget, rotationDuration));
    }

    private IEnumerator RotationCoroutine(Vector3 endPosition, float rotationDuration)
    {
        Quaternion startRotation = transform.rotation;
        endPosition.y = transform.position.y;
        Vector3 direction = endPosition - transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);

        if (Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1.0f) == false)
        {
            float timeElapsed = 0;
            while (timeElapsed < rotationDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / rotationDuration; // 0-1
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpStep);
                yield return null;
            }
            transform.rotation = endRotation;
        }
        StartCoroutine(MovementCoroutine(endPosition));
    }

    private IEnumerator MovementCoroutine(Vector3 endPosition)
    {
        Vector3 startPosition = transform.position;
        endPosition.y = startPosition.y;
        float timeElapsed = 0;

        while (timeElapsed < movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, lerpStep);
            yield return null;
        }
        transform.position = endPosition;

        if (pathPositions.Count > 0)
        {
            Debug.Log("Selecting the next position!");
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(), rotationDuration));
        }
        else
        {
            Debug.Log("Movement finished!");
            MovementFinished?.Invoke(this);
        }
    }

    public ISaveData SaveData()
    {
        return new UnitData
        {
            Position = transform.localPosition,
            Rotation = transform.localRotation,
            Scale = transform.localScale,
            GUID = _guid,
            ParentGUID = transform.parent != null ? transform.parent.GetComponent<ISaveableComponent>()?.GUID : null,
            MovementPoints = movementPoints,
            PrefabID = prefabID
        };
        
        // TODO: save model used
    }

    public void LoadData(ISaveData data)
    {
        var unitData = data as UnitData;
        if (unitData == null) throw new Exception("unitData is null");
        transform.localPosition = unitData.Position;
        transform.localRotation = unitData.Rotation;
        transform.localScale = unitData.Scale;
        _guid = unitData.GUID;
        movementPoints = unitData.MovementPoints;
        prefabID = unitData.PrefabID;
    }

    [Serializable]
    private class UnitData : ISaveData
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string PrefabID { get; set; }
        public string GUID { get; set; }
        public string ParentGUID { get; set; }
        
        public int MovementPoints { get; set; }
    }
}
