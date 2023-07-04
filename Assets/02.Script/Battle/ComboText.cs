using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboText : MonoBehaviour
{
    public TextMeshPro comboCountText;
    public TextMeshPro comboText;
    // Start is called before the first frame update

    public void ResetCombo()
    {
        comboCountText.gameObject.SetActive(false);
        comboText.gameObject.SetActive(false);
    }
    public void ComboAnimation(int comboCount)
    {
        comboCountText.gameObject.SetActive(true);
        comboText.gameObject.SetActive(true);
        comboCountText.text = comboCount.ToString();
        StartCoroutine(ComboHighLight());
    }
    private IEnumerator ComboHighLight()
    {
        float time = 0;
        float t = 0.1f;
        while (time < 1)
        {
            time += Time.deltaTime / t;
            comboCountText.fontSize = Mathf.Lerp(16.5f, 23, time);
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
        while (time > 0)
        {
            time -= Time.deltaTime / t;
            comboCountText.fontSize = Mathf.Lerp(16.5f, 23, time);
            yield return null;
        }
    }
}
