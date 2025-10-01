using System.Collections;
using RPG.Movement;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Combat;
using RPG.Core;
using RPG.Resources;

namespace RPG.Control
{
    // 플레이어 입력 처리 
    // 1) 좌클릭 대상이 CombatTarget이면 공격
    // 2) 아니면 클릭 지점으로 이동
    // 3) 사망 시 입력 무시
    public class PlayerController : MonoBehaviour
    {
        Health health;
        void Awake()
        {
            health = GetComponent<Health>();
        }
        void Update()
        {   
            // 죽었으면 입력 처리 중단
            if (health.IsDead()) return;
            // 전투 상호작용(타겟 클릭) 우선
            if (InteractWithCombat()) return;
            // 이동 상호작용(바닥 클릭)
            if (InteractWithMovement()) return;
        }

        // 전투 상호작용 : 마우스 레이로 맞은 모든 것 검사후 CombatTarget 찾기
        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                // 전투 타겟이 아니면 패스
                if (target == null) continue;

                // 공격 가능 여부 검사
                if (!GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    continue;
                }

                // 좌클릭 시 공격 명령
                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                // 전투 상호작용 대상이므로 true 반환(이동 로직 막기)
                return true;
            }
            // 전투 대상이 전혀 없다면 false
            return false;
        }

        // 이동 상호작용: 바닥 등 충돌 지점으로 이동
        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {   
                // 좌클릭 누르고 있는 동안 계속 이동
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point, 1f);
                }
                //히트한 표면이 있으면 이동 상호작용 처리
                return true;
            }
            // 아무것도 맞지 않음
            return false;
        }

        // 카메라 기준 마우스 레이
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
