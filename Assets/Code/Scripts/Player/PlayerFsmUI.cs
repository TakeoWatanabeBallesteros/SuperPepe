using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerFsmUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lastState;
    [SerializeField] private TMP_Text stateText;

    public void StateChanged(string lastState, string actualState)
    {
        this.lastState.text = $"Last State: {lastState}";
        stateText.text = $"Actual State: {actualState}";
    }
}
