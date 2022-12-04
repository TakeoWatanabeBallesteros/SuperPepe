using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour, IReset
{
    Animator anim;
    [SerializeField] float timeToFreezeGame;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject lifesObject;
    [SerializeField] GameObject bowserBackground;
    [SerializeField] GameObject winBackground;
    [SerializeField] GameObject winText;
    [SerializeField] GameObject loseText;
    [SerializeField] TextMeshProUGUI lifesText;
    int lifesLeft;
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
    }
    private void OnEnable() {
        anim = GetComponent<Animator>();
        GameManager.OnGameOverEvent += OpenGameOverMenu;
        StarsManager.OnMaxStars += OpenWinMenu;
    }
    private void OnDisable() {
        GameManager.OnGameOverEvent -= OpenGameOverMenu;
        StarsManager.OnMaxStars -= OpenWinMenu;
    }
    void OpenGameOverMenu(int lifes)
    {
        lifesLeft = lifes;
        anim.SetTrigger("Show");
        restartButton.SetActive(lifes > 0);
        lifesObject.SetActive(true);
        bowserBackground.SetActive(true);
        loseText.SetActive(true);
        winBackground.SetActive(false);
        winText.SetActive(false);
        UpdateLifes(lifes);
        StartCoroutine(StopTime());
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void OpenWinMenu()
    {
        anim.SetTrigger("Show");
        restartButton.SetActive(false);
        lifesObject.SetActive(false);
        winBackground.SetActive(true);
        winText.SetActive(true);
        bowserBackground.SetActive(false);
        loseText.SetActive(false);
        StartCoroutine(StopTime());
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Reset()
    {
        anim.SetTrigger("Hide");
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void RestartButton()
    {
        GameManager.GetGameManager().ResetGame();
        StopAllCoroutines();
    }
    public void ReturnMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    IEnumerator StopTime()
    {
        yield return new WaitForSeconds(timeToFreezeGame);
        Time.timeScale = 0f;
    }
    void UpdateLifes(int lifes)
    {
        string lifesString = lifes.ToString();
        string finalText = "";
        foreach (char letter in lifesString)
        {
            int index = int.Parse(letter.ToString()) == 0 ? 9 : int.Parse(letter.ToString()) - 1;
            finalText = finalText + "<sprite index=" + index.ToString() + ">";
        }
        lifesText.text = finalText;
    }
    public void ChangeLifes()
    {
        UpdateLifes(lifesLeft - 1);
    }
}
