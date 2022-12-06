using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StarsDisplay : MonoBehaviour,IReset
{
    [SerializeField] TextMeshProUGUI starsText;
    [SerializeField] TextMeshProUGUI maxStarsText;
    [SerializeField] float timeOnScreen;
    Animator anim;
    int currentStars;
    int maxStars;
    private void OnEnable() {
        StarsManager.OnStarsChanged += UpdateStars;
        anim = GetComponent<Animator>();
    }
    private void OnDisable() {
        StarsManager.OnStarsChanged -= UpdateStars;
    }
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
        StartCoroutine(AnimationTime());
    }

    void UpdateStars(int stars,int maxStars)
    {
        starsText.text = NumberToString(stars);
        maxStarsText.text = NumberToString(maxStars);
        StartCoroutine(AnimationTime());
    }
    IEnumerator AnimationTime()
    {
        anim.SetBool("Show",true);
        yield return new WaitForSeconds(timeOnScreen);
        anim.SetBool("Show",false);
    }
    string NumberToString(int number)
    {
        string numbersString = number.ToString();
        string finalText = "";
        foreach (char letter in numbersString)
        {
            int index = int.Parse(letter.ToString()) == 0 ? 9 : int.Parse(letter.ToString()) - 1;
            finalText = finalText + "<sprite index=" + index.ToString() + ">";
        }
        return finalText;
    }
    public void Reset()
    {
        anim.Play("Hide");
        StopAllCoroutines();
        StartCoroutine(AnimationTime());
    }
}
