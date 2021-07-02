using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private const float dotThreshold = 0.8f;
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.Normalize();
        return Vector3.Dot(transform.forward, dir) >= dotThreshold;
    }
}
