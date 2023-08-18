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
        public static float UPGRADE_BUTTON_DELAY = 0.2f;
        public static int RESET_COST = 1000;
        public static float[] EFFECT_INCREASE_AMOUNT = {0.3f,0.66f,0.33f,1.5f,0.33f,0.15f,0.2f};
        public static readonly int[] LEVEL_REQUIREMENT_EXP = {2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000,
                                                              2500, 2500, 2500, 2500, 2500, 3000, 3500, 4000, 4500, 5000,
                                                              10000, 11000, 12000, 13000, 14000, 15000, 18000, 21000, 24000, 27000,
                                                              30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000,
                                                              30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000,
                                                              30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000,
                                                              30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000,
                                                              30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000,
                                                              30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000,
                                                              30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000,1};
    }
    public static class Data{
        public static readonly string USERDATA_SAVE_PATH = Application.persistentDataPath + "/UserData.json";
        public static readonly string MAGICBOOKDATA_SAVE_PATH = Application.persistentDataPath + "/MagicBookData.json";
        public static readonly string OPTIONDATA_SAVE_PATH = Application.persistentDataPath + "/OptionData.json";
        public static readonly string MATHPIDDATA_SAVE_PATH = Application.persistentDataPath + "/MathpidData.json";
    }
}
