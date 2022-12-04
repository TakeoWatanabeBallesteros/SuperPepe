using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsManager : MonoBehaviour,IReset
{
    public static StarsManager instance;
    [SerializeField] int totalStars;
    int currentStars;
    public delegate void StarsChanged(int stars);
    public delegate void MaxStars();
    public static event StarsChanged OnStarsChanged;
    public static event MaxStars OnMaxStars;
    private void Awake() {
        instance = this;
    }
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
        currentStars = 0;
        OnStarsChanged?.Invoke(currentStars);
    }
    public void AddStar()
    {
        currentStars++;
        OnStarsChanged?.Invoke(currentStars);
        if(currentStars>=totalStars) OnMaxStars?.Invoke();
    }
    public void Reset()
    {
        currentStars = 0;
        OnStarsChanged?.Invoke(currentStars);
    }

}
