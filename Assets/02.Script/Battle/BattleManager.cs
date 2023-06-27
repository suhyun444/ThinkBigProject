using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Monster monster;
    [SerializeField] private MathpidManager mathpidManager;
    public string answer;
    public void BindAnswer(string answer)
    {
        this.answer = answer;
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mathpidManager.SelectAnswer(true);
        }
    }
    public void CheckAnswer(string answer)
    {
        bool isCorrect = this.answer == answer;
        Debug.Log("isCorrect : " + isCorrect.ToString());
        player.Act(isCorrect);
        monster.Act(isCorrect);
        mathpidManager.SelectAnswer(isCorrect);
    }
}
