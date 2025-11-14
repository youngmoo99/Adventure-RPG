using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    // 플레이어가 접촉하거나 클릭하면 해당 무기를 장착시키는 픽업 아이템
    // - 체력 회복 옵션(healthToRestore)
    // - 일정 시간 숨겼다가 리스폰
    // - IRaycastable로 커서/클릭 상호작용 지원
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {   
        // 획득 시 장착시킬 무기
        [SerializeField] WeaponConfig weapon = null;

        // HP 회복량
        [SerializeField] float healthToRestore = 0;

        // 리스폰까지의 시간
        [SerializeField] float respawnTime = 5;

        // 플레이어가 콜라이더에 들어오면 자동 픽업
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        // 실제 픽업 처리(무기 장착 + 회복 + 숨김/리스폰)
        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
                // 플레이어의 Fighter에 무기 장착
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }

            // 픽업을 숨겼다가 일정 시간 후 다시 나타나도록 처리
            StartCoroutine(HideForSeconds(respawnTime));
        }

        // 숨김 -> 대기 -> 표시 코루틴
        private IEnumerator HideForSeconds(float seconds)
        {   
            // 콜라이더/자식 비활성화 (숨김)
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);

            // 다시 보이도록
            ShowPickup(true);
        }

        // 픽업 표시/비표시 토글(콜라이더 + 모든 자식)
        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        // IRaycastable: 마우스 클릭으로도 픽업 가능
        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);
            }
            // 상호작용 대상이므로 true
            return true;
        }

        // 커서 형태(줍기)
        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}