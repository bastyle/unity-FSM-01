using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities 
{
    public static bool IsAligned(this Vector3 v1, Vector3 v2, Vector3 v3 ,float Epsilon=0.01f)
    {
        Vector3 heading2V2 = v2 - v1;
        heading2V2.Normalize();

        float diff = Vector3.Distance(heading2V2, v3);

        if (diff < Epsilon)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool WaypointReached(this Vector3 v1, Vector3 v2, float Epsilon = 0.01f)
    {
        

        float diff = Vector3.Distance(v1, v2);

        if (diff < Epsilon)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool SeeEnemy(this Vector3 v1, Vector3 v2, Vector3 v3, float cutoff)
    {
        //v3 is a unit vector
        Vector3 T2Eheading = v2 - v1;
        T2Eheading.Normalize();
        float cosTheta = Vector3.Dot(v3, T2Eheading);
        return (cosTheta > cutoff);
    }
}
