using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using RPG.Control;


namespace RPG.Combat
{
    //전투 대상임을 표시하는 마커 컴포넌트
    // RequireComponent: 반드시 Health가 함께 붙어 있어야함
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            // 공격 가능 여부 검사
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }

            // 좌클릭 시 공격 명령
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }
            // 전투 상호작용 대상이므로 true 반환(이동 로직 막기)
            return true;
        }
    }
}

