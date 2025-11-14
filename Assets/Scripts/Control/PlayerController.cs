using RPG.Combat;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    // 플레이어 입력을 해석하여:
    // - UI 상호작용 우선
    // - 컴포넌트(IRaycastable) 상호작용
    // - 이동 상호작용(NavMesh)
    // - 커서 아이콘 업데이트
    public class PlayerController : MonoBehaviour
    {
        Health health;

        // 커서 매핑 구조체(인스펙터에서 타입별 텍스처/ 핫스팟 설정)
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        // 타입별 커서 세트
        [SerializeField] CursorMapping[] cursorMappings = null;

        // 레이캐스트 지점-> NavMesh 투영 허용 거리
        [SerializeField] float maxNavMeshProjectionDistance = 1f;

        // SphereCast 반경(멀티타겟 감지용)
        [SerializeField] float raycastRadius = 1f;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Update()
        {   
            // 1) UI 위면 UI 커서 설정 후 종료
            if (InteractWithUI()) return;

            // 2) 사망 시 커서만 None으로 갱신하고 종료
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            // 3) 전투/픽업 등 IRaycastable 상호작용 우선
            if (InteractWithComponent()) return;
            // 4) 이동 상호작용
            if (InteractWithMovement()) return;

            // 5) 어떤 것도 아니면 커서 기본값
            SetCursor(CursorType.None);
        }

        // UI 위에 마우스가 있는가
        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        // IRaycastable 컴포넌트들과 상호작용(가까운 순서대로)
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        // 한 대상과 상호작요하면 종료
                        return true;
                    }
                }
            }
            return false;
        }

        // 마우스 레이 기준 SphereCastAll 후 거리순 정렬(가까운 것부터)
        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        // NavMesh 상의 지점으로 이동 상호작용
        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {   
                // 목적지까지 실제 이동 가능해야함
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                // 좌클릭 유지 시 이동 명령
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        // 화면 클릭 지점 → 레이 → 충돌 지점 → NavMesh로 투영
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            
            // 화면 클릭 지점에서 레이캐스트
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            // 충돌 지점을 NavMesh로 투영(허용 반경 내)
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            return true;
        }

        // 커서 아이콘 설정
        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        // 타입에 맞는 커서 매핑 찾기(없으면 첫 요소 반환)
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        // 현재 마우스 위치에서 카메라 기준 레이 생성
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}