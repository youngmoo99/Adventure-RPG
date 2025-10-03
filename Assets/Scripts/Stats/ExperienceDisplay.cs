using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{   
    // 플레이어의 현재 경험치를 UI(Text)로 표시
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        void Awake()
        {   
            // Player태그에서 experience 컴포넌트 획득
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        void Update()
        {   
            // 소수점 없이 정수로 표기
            GetComponent<Text>().text = String.Format("{0:0}", experience.GetPoints());
        }
    }
}

