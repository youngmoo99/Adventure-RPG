using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{   
    // 플레이어 현재 레벨을 UI(Text)로 표시하는 클래스
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;

        private void Awake()
        {   
            // Player 태그에서 BaseStats 컴포넌트를 찾는다
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {   
            // 현재 레벨을 정수로 UI에 출력
            GetComponent<Text>().text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}