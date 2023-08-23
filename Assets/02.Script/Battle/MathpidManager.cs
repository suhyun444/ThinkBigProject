using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WjChallenge;

public enum CurrentStatus { WAITING, DIAGNOSIS, LEARNING }
public class MathpidManager : MonoBehaviour
{
    [SerializeField] MathpidConnector conn;
    [SerializeField] CurrentStatus currentStatus;
    public CurrentStatus CurrentStatus => currentStatus;

    [SerializeField] BattleManager battleManager;
    [SerializeField] QuestionBox textEquation;           //���� �ؽ�Ʈ(��TextDraw�� ���� �ʿ�)  //식
    string correctAnswer;
    string wrongAnswer;

    [Header("Status")]
    int currentQuestionIndex;
    bool isSolvingQuestion;
    float questionSolveTime;


    private void OnEnable()
    {
        Setup();
        textEquation.BindShowProblemCallBack(()=>questionSolveTime = 0);
    }
    private void Setup()
    {
        switch (currentStatus)
        {
            case CurrentStatus.WAITING:
                break;
        }

        if (conn != null)
        {
            conn.onGetDiagnosis.AddListener(() => GetDiagnosis());
            conn.onGetLearning.AddListener(() => GetLearning(0));
        }
        else Debug.LogError("Cannot find Connector");
        if(SaveManager.Instance.GetMemberIdData() == "null")
        {
            ButtonEvent_ChooseDifficulty(0);
        }
        else
        {
            currentStatus = CurrentStatus.LEARNING;
            conn.Learning_GetQuestion();
        }
    }

    private void Update()
    {
        if (isSolvingQuestion) questionSolveTime += Time.deltaTime;
        if (currentQuestionIndex >= 8)
        {
            conn.Learning_GetQuestion();
        }
    }

    /// <summary>
    /// ������ ���� �޾ƿ���
    /// </summary>
    private void GetDiagnosis()
    {
        switch (conn.cDiagnotics.data.prgsCd)
        {
            case "W":
                MakeQuestion(conn.cDiagnotics.data.textCn,
                            conn.cDiagnotics.data.qstCn,
                            conn.cDiagnotics.data.qstCransr,
                            conn.cDiagnotics.data.qstWransr);
                break;
            case "E":
                SaveManager.Instance.SetAuthorizationData(conn.GetAutorization());
                SaveManager.Instance.SetMemberIdData(conn.GetMBR_ID());
                SaveManager.Instance.SaveMathpidData();
                Debug.Log("������ ��! �н� �ܰ�� �Ѿ�ϴ�.");
                currentStatus = CurrentStatus.LEARNING;
                break;
        }
    }

    /// <summary>
    ///  n ��° �н� ���� �޾ƿ���
    /// </summary>
    private void GetLearning(int _index)
    {
        if (_index == 0) currentQuestionIndex = 0;

        MakeQuestion(conn.cLearnSet.data.qsts[_index].textCn,
                    conn.cLearnSet.data.qsts[_index].qstCn,
                    conn.cLearnSet.data.qsts[_index].qstCransr,
                    conn.cLearnSet.data.qsts[_index].qstWransr);
    }

    /// <summary>
    /// �޾ƿ� �����͸� ������ ������ ǥ��
    /// </summary>
    private void MakeQuestion(string textCn, string qstCn, string qstCransr, string qstWransr)
    {
        
        correctAnswer = qstCransr;
        wrongAnswer = qstWransr.Split(',')[0];
        battleManager.BindAnswer(correctAnswer);
        textEquation.BindQuestion(qstCn);

        isSolvingQuestion = true;
    }

    /// <summary>
    /// ���� ������ �¾Ҵ� �� üũ
    /// </summary>
    public float SelectAnswer(bool isCorrect) // 버튼으로 정답맞추기
    {
        string ansrCwYn = isCorrect ? "Y" : "N";
        string ansText = isCorrect ? correctAnswer : wrongAnswer;
        float ret = questionSolveTime;

        switch (currentStatus)
        {
            case CurrentStatus.DIAGNOSIS:
                isSolvingQuestion = false;
                conn.Diagnosis_SelectAnswer(ansText, ansrCwYn, (int)(questionSolveTime * 1000));
                currentQuestionIndex++;

                questionSolveTime = 0;
                break;

            case CurrentStatus.LEARNING:

                isSolvingQuestion = false;
                conn.Learning_SelectAnswer(currentQuestionIndex, ansText, ansrCwYn, (int)(questionSolveTime * 1000));
                currentQuestionIndex++;
                if(currentQuestionIndex < 8)
                    GetLearning(currentQuestionIndex);

                questionSolveTime = 0;
                break;
        }
        return ret;
    }


    #region Unity ButtonEvent
    public void ButtonEvent_ChooseDifficulty(int a) // 난이도 초기 설정
    {
        currentStatus = CurrentStatus.DIAGNOSIS;
        conn.FirstRun_Diagnosis(a);
    }
    public void ButtonEvent_GetLearning() //문제 받아오기 버튼
    {
        conn.Learning_GetQuestion();
    }
    #endregion
}
