using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{   
    // 플레이어 체력을 UI(Text)로 표시하는 클래스
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        void Awake()
        {   
            // Player태그로 플레이어 오브젝트를 찾아 Health 컴포넌트 참조
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        void Update()
        {   
            // "현재체력 / 최대체력 " 형식으로 표시
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}

