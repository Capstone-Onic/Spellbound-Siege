using System.Collections.Generic;
using UnityEngine;

public class PathSystem : MonoBehaviour
{
    public List<Transform> waypoints;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //웨이포인트 선 그리기
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
        }
        //마지막 웨이포인트 표시
        if (waypoints.Count > 0)
        {
            Gizmos.DrawSphere(waypoints[waypoints.Count - 1].position, 0.2f);
        }
    }
}
