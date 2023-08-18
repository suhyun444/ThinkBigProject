using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Intro : MonoBehaviour
{
    [SerializeField] private PetPosition petPosition;
    [SerializeField] GameObject startButton;
    [SerializeField] private SpriteRenderer fadeOut;
    [SerializeField] private TextMeshPro progressText;
    private bool progressEnd = false;
    private bool enterMainScene = false;
    private bool onLoad = false;
    private void Awake() {
        SpawnPet();
        SpawnPlayerCostume(SaveManager.Instance.GetCostumeTypeData());
        StartCoroutine(LoadBattleScene());
    }
    private void Update() {
        if(progressEnd && Input.GetMouseButtonDown(0))
        {
            if(!onLoad)
            {
                StartCoroutine(FadeOut());
            }
        }
    }
    private IEnumerator FadeOut()
    {
        onLoad = true;
        fadeOut.gameObject.SetActive(true);
        float time = 0.0f;
        float t = 0.4f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / t;
            fadeOut.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, time));
            yield return null;
        }
        enterMainScene = true;
    }

    private IEnumerator LoadBattleScene()
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation op = SceneManager.LoadSceneAsync(1,LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        while (!op.isDone)
        {
            Debug.Log(op.progress);
            progressText.text = op.progress.ToString();
            if(!progressEnd && op.progress >= 0.9f)
            {
                progressEnd = true;
                startButton.SetActive(true);
            }
            if (enterMainScene)
            {
                op.allowSceneActivation = true;
                yield break;
            }
            yield return null;
        }
        SceneManager.UnloadScene(0);
    }
    private void SpawnPlayerCostume(CostumeType costumeType)
    {
        GameObject costume = (GameObject)Resources.Load<GameObject>("Players/Main/" + costumeType.ToString() + "Main");
        Instantiate(costume);
    }
    private Vector3 GetSpawnPosition()
    {
        while (true)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-7.0f, 7.0f), Random.Range(-18.0f, 18.0f), 0);
            RaycastHit2D[] raycastHits2D = Physics2D.RaycastAll(spawnPosition, transform.forward, 999);
            for (int i = 0; i < raycastHits2D.Length; i++)
            {
                if (raycastHits2D[i].transform.CompareTag("PetBoundary"))
                {
                    return spawnPosition;
                }
            }
        }
    }
    private void SpawnPet()
    {
        List<int> petList = SaveManager.Instance.GetPetList();
        for (int i = 0; i < petList.Count; i++)
        {
            if (petList[i] != -1)
                InstantitePetObject(i, petList[i]);
        }
    }
    private void InstantitePetObject(int index, int id)
    {
        GameObject petObject = Resources.Load<GameObject>("Pets/Pet" + id.ToString());
        Vector3 spawnPosition = GetSpawnPosition();
        Instantiate(petObject, spawnPosition, Quaternion.identity).GetComponent<Pet>().index = index;
        petPosition.petPositions[index] = spawnPosition;
    }
}
