using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsManager : MonoBehaviour
{
    public static StarsManager instance;
    [SerializeField] int totalStars;
    int currentStars;
    public delegate void StarsChanged(int stars, int maxStars);
    public static event StarsChanged OnStarsChanged;
    private void Awake() {
        instance = this;
    }
    private void Start() {
        currentStars = 0;
        OnStarsChanged?.Invoke(currentStars,totalStars);
    }
    public void AddStar()
    {
        currentStars++;
        OnStarsChanged?.Invoke(currentStars,totalStars);
        if(currentStars>=totalStars) GameManager.GetGameManager().Win();
    }
}
