using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{   
    //플레이어가 현재 타겟팅 중인 적의 체력을 UI(Text)로 표시
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;
        void Awake()
        {   
            // Player 태그로 플레이어를 찾고 Fighter를 캐싱
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        void Update()
        {   
            //타겟이 없으면 "N/A" 출력 
            if (fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            //타겟이 있으면 (현재 / 최대 체력) 표시
            Health health = fighter.GetTarget();
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}




