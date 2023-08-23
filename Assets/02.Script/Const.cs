using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const 
{
    public static class Battle
    {
        public static readonly float BATTLE_TIME = 80.0f;
        public static readonly float[] DEAD_MOTION_DELAY = {0.45f,0.36f,0.41f};
        public static readonly float[] ATTACK_MOTION_DELAY = {0.4f,0.44f,0.416f};
        public static readonly float[] ATTACK_LEFT_MOTION = {0.6f,0.433f,0.334f};
    }
    public static class Skill
    {
        public static float UPGRADE_BUTTON_DELAY = 0.15f;
        public static int RESET_COST = 500;
        public static int SP_COST = 150;
        public static float[] EFFECT_INCREASE_AMOUNT = {0.3f,0.66f,0.5f,1.5f,0.33f,0.15f,0.2f};
        public static readonly int[] LEVEL_REQUIREMENT_EXP = {1000, 1300, 1500, 1700, 1800, 1900, 2000, 2050, 2100, 2150,
                                                              2200, 2250, 2300, 2350, 2400, 2500, 2600, 2700, 2800, 2900,
                                                              3000, 3100, 3200, 3300, 3400, 3600, 3900, 4100, 4300, 4500,
                                                              4700, 4900, 5100, 5300, 5500, 5500, 5500, 5500, 5500, 5500,
                                                              5550, 5600, 5650, 5700, 5750, 5800, 5850, 5900, 5950, 6000,
                                                              7000, 7050, 7100, 7150, 7200, 7500, 7550, 7600, 7650, 7700,
                                                              8000, 8050, 8100, 8150, 8200, 8500, 8550, 8600, 8650, 8700,
                                                              8750, 8800, 8850, 8900, 8950, 9000, 9100, 9200, 9300, 9400,
                                                              10000, 10500, 11000, 11500, 12000, 13000, 14000, 15000, 16000, 17000,
                                                              20000, 23000, 26000, 30000, 325000, 350000, 35000, 35000, 40000, 50000,1};
    }
    public static class Data{
        public static readonly string USERDATA_SAVE_PATH = Application.persistentDataPath + "/UserData.json";
        public static readonly string MAGICBOOKDATA_SAVE_PATH = Application.persistentDataPath + "/MagicBookData.json";
        public static readonly string OPTIONDATA_SAVE_PATH = Application.persistentDataPath + "/OptionData.json";
        public static readonly string MATHPIDDATA_SAVE_PATH = Application.persistentDataPath + "/MathpidData.json";
    }
}
