using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using CustomUtils;

public class MainManager : MonoBehaviour
{
    [SerializeField] private CustomButton nameButton;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro crystalText;
    [SerializeField] private Material expBarProgressMaterial;
    [SerializeField] private TextMeshPro levelText;
    [SerializeField] private SpriteRenderer fadeIn;
    [SerializeField] private SetNamePopup setNamePopup;
    private GameObject playerCostume;
    private Pet[] pets = new Pet[4];
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(FadeIn());
        levelText.text = SaveManager.Instance.GetLevelData().ToString();
        expBarProgressMaterial.SetFloat("_FillAmount",(float)SaveManager.Instance.GetExpAmountData() / (float)Const.Skill.LEVEL_REQUIREMENT_EXP[SaveManager.Instance.GetLevelData()]);
        if(SaveManager.Instance.GetNameData() == "") setNamePopup.OpenSetNameUI();
        else Debug.Log(SaveManager.Instance.GetNameData());
        nameButton.BindClickEvent(setNamePopup.OpenUpdateNameUI);
        SpawnPet();
        SpawnPlayerCostume(SaveManager.Instance.GetCostumeTypeData());
    }
    private void SpawnPlayerCostume(CostumeType costumeType)
    {
        GameObject costume = (GameObject)Resources.Load<GameObject>("Players/Main/"+costumeType.ToString() + "Main");
        playerCostume = Instantiate(costume);
        nameButton.transform.position = playerCostume.transform.GetChild(0).transform.position;
        nameButton.transform.position = new Vector3(nameButton.transform.position.x,nameButton.transform.position.y,-0.5f);
    }
    public void ChangePlayerCostime(CostumeType costumeType)
    {
        Destroy(playerCostume);
        SpawnPlayerCostume(costumeType); 
    }
    private IEnumerator FadeIn()
    {
        fadeIn.gameObject.SetActive(true);
        float time = 0.0f;
        float t = 0.5f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / t;
            fadeIn.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, time));
            yield return null;
        }
        fadeIn.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        crystalText.text = Util.SplitIntByComma(SaveManager.Instance.GetCrystalData());
        nameText.text = SaveManager.Instance.GetNameData();
    }

    private Vector3 GetSpawnPosition()
    {
        while (true)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-7.0f, 7.0f), Random.Range(-18.0f, 18.0f), 0);
            RaycastHit2D[] raycastHits2D = Physics2D.RaycastAll(spawnPosition, transform.forward, 999);
            for(int i=0;i<raycastHits2D.Length;i++)
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
        for(int i=0;i<petList.Count;i++)
        {
            if(petList[i] != -1)
                InstantitePetObject(i,petList[i]);
        }
    }
    public void ChangePet(int index,int id)
    {
        SaveManager.Instance.ChangePetList(index,id);
        SaveManager.Instance.SaveData();
        if(pets[index] != null)
            Destroy(pets[index].gameObject);
        if(id != -1)
            InstantitePetObject(index,id);
    }
    private void InstantitePetObject(int index,int id)
    {
        GameObject petObject = Resources.Load<GameObject>("Pets/Pet" + id.ToString());
        Vector3 spawnPosition = GetSpawnPosition();
        pets[index] = Instantiate(petObject, spawnPosition, Quaternion.identity).GetComponent<Pet>();
        pets[index].index = index;
    }
}
