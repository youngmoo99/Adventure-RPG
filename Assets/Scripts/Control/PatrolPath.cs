using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        // 빈 오브젝트 아래의 자식들을 웨이포인트로 사용 
        // 에디터 뷰에서 구체/선으로 경로를 시각화
        const float waypointGizmoRadius = 0.3f;

        // 씬 뷰에 기즈모 그리기
        void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);

                // 웨이포인트(구) 표시
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                // 다음 웨이포인트로 연결선 표시(루프)
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        // 다음 웨이포인트 인덱스(마지막이면 0으로 루프)
        public int GetNextIndex(int i)
        {
            if (i + 1 >= transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        //i번째 웨이포인트 위치(자식 트랜스폼)
        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
