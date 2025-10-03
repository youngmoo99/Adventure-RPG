using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    // 플레이어가 접촉하면 해당 무기를 장착시키는 픽업 아이템
    // 일정 시간 숨겼다가 리스폰하는 기능 포함
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {   
        // 획득 시 장착시킬 무기
        [SerializeField] Weapon weapon = null;
        // 리스폰까지의 시간
        [SerializeField] float respawnTime = 5;

        //플레이어가 콜라이더에 들어오면 발동
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.GetComponent<Fighter>());
            }
        }

        private void Pickup(Fighter fighter)
        {
            // 플레이어의 Fighter에 무기 장착
            fighter.EquipWeapon(weapon);

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

        // 픽업 표시/비표시 토글(콜라이더 + 모든 자식 오브젝트)
        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.GetComponent<Fighter>());
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }


    }
}