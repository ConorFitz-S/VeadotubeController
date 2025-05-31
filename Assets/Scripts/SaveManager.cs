using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    void LoadData()
    {
        if(PlayerPrefs.HasKey("handleCount"))
        {
            int handleCount = int.Parse(PlayerPrefs.GetString("handleCount"));
            string[] handleStateNames = PlayerPrefs.GetString("handleStateNames").Split(',');
            string[] handleStateValues = PlayerPrefs.GetString("handleStateValues").Split(',');

            for (int i = 0; i < handleCount; i++)
            {
                SliderHandle newHandle = Instantiate(SliderBody.Instance.handlePrefab, SliderBody.Instance.handleParent);
                newHandle.StateName = handleStateNames[i];
                newHandle.StateValue = int.Parse(handleStateValues[i]);
                newHandle.ValueText.text = newHandle.StateValue.ToString();
                SliderBody.Instance.handles.Add(newHandle);
            }

            SliderBody.Instance.minValue = PlayerPrefs.GetFloat("sliderMinValue");
            SliderBody.Instance.maxValue = PlayerPrefs.GetFloat("sliderMaxValue");
            
            SliderBody.Instance.SetMinValueInput(SliderBody.Instance.minValue);
            SliderBody.Instance.SetMaxValueInput(SliderBody.Instance.maxValue);
            
            for(int i = 0; i < SliderBody.Instance.handles.Count; i++)
            {
                SliderBody.Instance.handles[i].SetPosition(SliderBody.Instance.handles[i].StateValue);
            }
            Debug.Log("Game loaded");
        }
    }

    public void SaveGame()
    {
        // Implement save logic here
        //Get all handles and their values
        List<string> handleStateNames = new List<string>();
        List<string> handleStateValues = new List<string>();
        
        foreach (var handle in SliderBody.Instance.handles)
        {
            handleStateNames.Add(handle.StateName);
            handleStateValues.Add(handle.StateValue.ToString());
        }

        string handleCount = SliderBody.Instance.handles.Count.ToString();
        PlayerPrefs.SetString("handleCount", handleCount);
        PlayerPrefs.SetString("handleStateNames", string.Join(",", handleStateNames));
        PlayerPrefs.SetString("handleStateValues", string.Join(",", handleStateValues));
        PlayerPrefs.SetFloat("sliderMinValue", SliderBody.Instance.minValue);
        PlayerPrefs.SetFloat("sliderMaxValue", SliderBody.Instance.maxValue);
        
        PlayerPrefs.Save();
        
        Debug.Log("Game saved");
    }
}
