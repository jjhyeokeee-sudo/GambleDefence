using System.Collections.Generic;
using UnityEngine;

// 씬에 배치된 웨이포인트들을 순서대로 연결해서 적 이동 경로를 정의
public class EnemyPath : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new();

    public List<Transform> Waypoints => waypoints;

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        Gizmos.color = Color.yellow;
        foreach (var wp in waypoints)
        {
            if (wp != null)
                Gizmos.DrawWireSphere(wp.position, 0.2f);
        }
    }
}
