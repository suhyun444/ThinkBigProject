using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Sound
{
    ButtonClick,
    Warning,
    MagicSuccess,
    MagicFailed,
    Erase,
    JarAttack,
    EarnCrystal,
    Hitted,
    MonsterDead,
    Buy,
    OpenResultPopup,
    GameStart,
    WitchAttack,
    MagicianAttack,
    HarvestPetCrystal,
    ThiefAttack,
    MagicBookReward,
    BattleBGM,
    MainBGM
}

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Object/Sound")]
public class SoundData : ScriptableObject
{
    public int count;
    public List<Sound> nameList = new List<Sound>();
    public List<AudioClip> clipList = new List<AudioClip>();
}
