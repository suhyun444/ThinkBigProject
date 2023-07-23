using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const 
{
    public static class Battle
    {
        public static readonly float BATTLETIME = 120.0f;
    }
    public static class Skill
    {
        public static int[] effectIncreaseAmount = {3,3,3,3};
    }
    public static class Data{
        public static readonly string USERDATA_SAVE_PATH = Application.persistentDataPath + "/UserData.json";
        public static readonly string MAGICBOOKDATA_SAVE_PATH = Application.persistentDataPath + "/MagicBookData.json";
    }
}
