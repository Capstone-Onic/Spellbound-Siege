using System.Collections.Generic;
using UnityEngine;

public class PathSystem : MonoBehaviour
{
    public List<Transform> waypoints;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //��������Ʈ �� �׸���
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
        }
        //������ ��������Ʈ ǥ��
        if (waypoints.Count > 0)
        {
            Gizmos.DrawSphere(waypoints[waypoints.Count - 1].position, 0.2f);
        }
    }
}
