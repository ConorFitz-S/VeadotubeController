using System.Collections.Generic;
using UnityEngine;

public class HandleHandler : MonoBehaviour
{
    public SliderHandle handlePrefab; // Prefab for the handle
    
    public List<SliderHandle> handles = new List<SliderHandle>(); // List to keep track of created handles
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void CreateHandle()
    {
        Vector3 position = (SliderBody.Instance.GetMinPosition() + SliderBody.Instance.GetMaxPosition())/2;
        SliderHandle newHandle = Instantiate(handlePrefab, position, Quaternion.identity, transform);
        handles.Add(newHandle);
        newHandle.transform.localPosition = new Vector3(0, -50, 0);
        //.transform.SetParent(transform); // Set the parent to this handler for organization
    }
}
