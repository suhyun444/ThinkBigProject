using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const 
{
    public static class Battle
    {
        public static readonly float BATTLE_TIME = 120.0f;
        public static readonly float[] DEAD_MOTION_DELAY = {0.45f};
        public static readonly float[] ATTACK_MOTION_DELAY = {0.4f};
        public static readonly float[] ATTACK_LEFT_MOTION = {0.6f};
    }
    public static class Skill
    {
        public static int[] EFFECT_INCREASE_AMOUNT = {3,3,3,3};
    }
    public static class Data{
        public static readonly string USERDATA_SAVE_PATH = Application.persistentDataPath + "/UserData.json";
        public static readonly string MAGICBOOKDATA_SAVE_PATH = Application.persistentDataPath + "/MagicBookData.json";
        public static readonly string OPTIONDATA_SAVE_PATH = Application.persistentDataPath + "/OptionData.json";
    }
}
