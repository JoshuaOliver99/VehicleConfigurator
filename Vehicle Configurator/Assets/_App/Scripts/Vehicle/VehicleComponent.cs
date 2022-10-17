using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] VehicleRegions vehicleComponent;

    [Header("Customization")]
    [SerializeField] private bool isOptionable = true;
    public bool IsOptionable { get => isOptionable;  private set => isOptionable = value; }


    public CustomizationOption[] options;

    private int currOption = 0; // Note: -1 indicates nothing

    public float totalPrice = 0;

    public delegate void CostUpdate();
    public static event CostUpdate OnCostUpdate;

    void Start()
    {
        //// Error Handling
        //if (vehicleComponent == null) Debug.LogWarning($"{name} {nameof(vehicleComponent)} == null. Component not identified!");

        
        foreach (var option in options)
        {
            // Disable all options
            option.gameObject.SetActive(false);

            // Set default material is paintable
            if (option.isPaintable)
                UpdateActiveMaterial();
        }


        if (isOptionable == true)
            currOption = -1;
        else 
            currOption = 0;

        SetActiveOption(currOption);
    }



    private void SetActiveOption(int index)
    {
        // Deactivate others
        foreach (var option in options)
            option.gameObject.SetActive(false);

        //// if nothing
        //if (index < 0)
        //{
        //    UpdateTotalPrice();
        //    return;
        //}

        // if (options exist && not option "nothing")
        if (options.Length > 0 && index > -1)
            options[index].gameObject.SetActive(true);

        UpdateTotalPrice();
    }


    private void SetActiveMaterial(int index)
    {
        options[currOption].gameObject.GetComponent<Renderer>().material = options[currOption].materialOptions[index].material;
    }
    private void UpdateActiveMaterial()
    {
        options[currOption].gameObject.GetComponent<Renderer>().material = options[currOption].materialOptions[options[currOption].currMaterial].material;
            
        
        UpdateTotalPrice();
    }


    private void UpdateTotalPrice()
    {
        totalPrice = 0;

        if (currOption != -1)
        {
            totalPrice += options[currOption].price;

            if (options[currOption].materialOptions.Length > 0)
                totalPrice += options[currOption].materialOptions[options[currOption].currMaterial].price; // (add material price) Note: this is completely unreadable. 
        }
        
        OnCostUpdate?.Invoke();
    }



    public void NextOption()
    {
        currOption++;

        if (currOption >= options.Length)
        {
            if (isOptionable)
                currOption = -1;
            else
                currOption = 0;
        }  

        SetActiveOption(currOption);
    }
    public void PrevOption()
    {
        currOption--;

        if(isOptionable)
        {
            if(currOption < -1)
                currOption = options.Length - 1;
        }
        else
        {
            if (currOption < 0)
                currOption = options.Length - 1;
        }

        SetActiveOption(currOption);
    }


    public void NextMaterial()
    {
        if (currOption < 0)
            return;

        if (!options[currOption].isPaintable)
            return;

        options[currOption].currMaterial++;

        if (options[currOption].currMaterial >= options[currOption].materialOptions.Length)
            options[currOption].currMaterial = 0;

        UpdateActiveMaterial();
    }
    public void PrevMaterial()
    {
        if (currOption < 0)
            return;

        if (!options[currOption].isPaintable)
            return;

        options[currOption].currMaterial--;

        if (options[currOption].currMaterial < 0)
            options[currOption].currMaterial = options[currOption].materialOptions.Length - 1;

        UpdateActiveMaterial();
    }



    #region Returning
    public CustomizationOption GetCurrOption()
    {
        if (currOption >= 0)
            return options[currOption];
        else 
            return null;
    }
    #endregion
}

[System.Serializable]
public class CustomizationOption
{
    [Header("Item")]
    public string name;
    //public string description;
    public GameObject gameObject;
    public float price;

    [Header("Marerial Options")]
    public bool isPaintable = false;
    public CustomizableMaterial[] materialOptions;
    public int currMaterial = 0;


    public CustomizableMaterial CurrMaterial 
    { 
        get => materialOptions[currMaterial]; 
        private set => materialOptions[currMaterial] = value;
    }
}

[System.Serializable]
public class CustomizableMaterial
{ 
    public Material material;
    public float price;
}
