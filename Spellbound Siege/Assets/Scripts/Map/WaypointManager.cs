using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform[] waypoints;

    public Transform GetWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Length)
        {
            return waypoints[index];
        }
        return null;
    }

    public Vector3[] GetAllWaypointPositions()
    {
        Vector3[] positions = new Vector3[waypoints.Length];

        for (int i = 0; i < waypoints.Length; i++)
        {
            positions[i] = waypoints[i].position;
        }

        return positions;
    }
}
