using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance;
    [SerializeField] private List<Transform> crosshairs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }    
    }

    private void Start()
    {
        GetCrosshairs();
    }

    private void GetCrosshairs()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            crosshairs.Add(transform.GetChild(i));
        }
    }

    public void SetCrosshair(Transform crosshair)
    {
        foreach (Transform child in crosshairs)
        {
            child.gameObject.SetActive(false);
        }
        crosshair.gameObject.SetActive(true);
    }
}
