using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private GameObject handle;
    [SerializeField] private Material fillMaterial;
    private Vector3 prevMousePosition;
    bool onDrag = false;
    private float ratio = 1.0f;

    public void Open(float volume)
    {
        ratio = volume;
        float nextX = (maxX - minX) * ratio + minX;
        handle.transform.position = new Vector3(nextX, handle.transform.position.y, -1);
        fillMaterial.SetFloat("_FillAmount", ratio);
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f));
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(mousePosition, transform.forward, 999);
            if (raycastHit2D.collider != null && raycastHit2D.transform.CompareTag("Handle"))
            {
                prevMousePosition = mousePosition;
                onDrag = true;
            }
        }
        else if (onDrag && Input.GetMouseButton(0))
        {
            if (Mathf.Abs(prevMousePosition.x - mousePosition.x) > 0.1f)
            {
                float nextX = Mathf.Clamp(mousePosition.x,minX,maxX);
                handle.transform.position = new Vector3(nextX,handle.transform.position.y,-1);
                ratio = (nextX - minX) / (maxX - minX);
                fillMaterial.SetFloat("_FillAmount",ratio);
                prevMousePosition = mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            onDrag = false;
        }
    }
    public float GetVolume()
    {
        return ratio;
    }
}
