using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Intro : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fadeOut;
    [SerializeField] private SpriteRenderer loadingBG;
    [SerializeField] private SpriteRenderer loadingText;
    private List<Pet> pets = new List<Pet>();
    private bool progressEnd = false;
    private bool onLoad = false;
    private void Start() {
        SpawnPet();
        SpawnPlayerCostume(SaveManager.Instance.GetCostumeTypeData());
        StartCoroutine(LoadMainScene());
    }
    private void Update() {
        if(progressEnd && Input.GetMouseButtonDown(0))
        {
            if(!onLoad)
            {
                onLoad = true;
                Vector3[] petInfo = new Vector3[pets.Count * 2];
                bool[] directionInfo = new bool[pets.Count];
                for(int i=0;i<pets.Count;++i)
                {
                    petInfo[i * 2]=pets[i].transform.position - transform.position;
                    petInfo[i*2 + 1] = pets[i].transform.localScale;
                    directionInfo[i] = pets[i].GetDirection();
                }
                MainManager mainManager = GameObject.FindObjectOfType<MainManager>();
                mainManager.LoadMainSceneFromIntro();
                mainManager.SetPetPosition(petInfo, directionInfo);
                SoundManager.Instance.PlaySoundEffect(Sound.ButtonClick);
                SceneManager.UnloadSceneAsync(0);
            }
        }
    }
    private IEnumerator FadeIn()
    {
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            loadingBG.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, time));
            loadingText.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, time));
            yield return null;
        }
        loadingBG.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
    }

    private IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation op = SceneManager.LoadSceneAsync(1,LoadSceneMode.Additive);
        while (!op.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main"));
        progressEnd = true;
        StartCoroutine(FadeIn());
    }
    private void SpawnPlayerCostume(CostumeType costumeType)
    {
        GameObject costume = (GameObject)Resources.Load<GameObject>("Players/Main/" + costumeType.ToString() + "Main");
        Instantiate(costume,costume.transform.position + transform.position,Quaternion.identity);
    }
    private Vector3 GetSpawnPosition()
    {
        while (true)
        {
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-7.0f, 7.0f), Random.Range(-18.0f, 18.0f), 0);
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
        Pet pet = Instantiate(petObject, spawnPosition, Quaternion.identity).GetComponent<Pet>();
        pet.index = index;
        pets.Add(pet);
    }
}
