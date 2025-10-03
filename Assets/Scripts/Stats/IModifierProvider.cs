using System.Collections.Generic;

namespace RPG.Stats
{
    // 스탯 보정치를 제공하는 인터페이스
    // 무기, 장비, 버프
    public interface IModifierProvider
    {   
        // 절대값 보정
        IEnumerable<float> GetAdditiveModifier(Stat stat);
        // 퍼센트 보정
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}