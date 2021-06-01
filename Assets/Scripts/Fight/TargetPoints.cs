using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetSpots
{
    BODY,
    HEAD,
    FEET,
    ABOVE
}

public class TargetPoints : MonoBehaviour
{
    [SerializeField] Transform bodyPosition;
    [SerializeField] Transform headPosition;
    [SerializeField] Transform feetPosition;
    [SerializeField] Transform abovePosition;

    public Transform GetPosition(TargetSpots target)
    {
        switch (target)
        {
            case TargetSpots.BODY:
                return bodyPosition;
            case TargetSpots.HEAD:
                return headPosition;
            case TargetSpots.FEET:
                return feetPosition;
            case TargetSpots.ABOVE:
                return abovePosition;
            default:
                Debug.Log("Big Error: This should never occur!!"); 
                return transform;
        }
    }
}
