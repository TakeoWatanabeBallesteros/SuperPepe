using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameOver : MonoBehaviour, IReset
{
    Animator anim;
    [SerializeField] float timeToFreezeGame;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject menuButton;
    [SerializeField] GameObject pressImage;
    [SerializeField] GameObject lifesObject;
    [SerializeField] GameObject bowserBackground;
    [SerializeField] GameObject winBackground;
    [SerializeField] GameObject winText;
    [SerializeField] GameObject loseText;
    [SerializeField] TextMeshProUGUI lifesText;
    int lifesLeft;
    bool canExitMenu;
    bool subscribeEnabled;
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
        subscribeEnabled = false;
    }
    private void OnEnable() {
        anim = GetComponent<Animator>();
        GameManager.OnGameOverEvent += OpenGameOverMenu;
        GameManager.OnWin += OpenWinMenu;
    }
    private void OnDisable() {
        GameManager.OnGameOverEvent -= OpenGameOverMenu;
        GameManager.OnWin -= OpenWinMenu;
    }
    void OpenGameOverMenu(int lifes)
    {
        lifesLeft = lifes;
        anim.SetTrigger("Show");
        canExitMenu = false;
        restartButton.SetActive(true);
        menuButton.SetActive(true);
        pressImage.SetActive(false);
        subscribeEnabled = false;
        if(lifes <= 0)
        {
            restartButton.SetActive(false);
            menuButton.SetActive(false);
            pressImage.SetActive(true);
            subscribeEnabled = true;
        }
        lifesObject.SetActive(true);
        bowserBackground.SetActive(true);
        loseText.SetActive(true);
        winBackground.SetActive(false);
        winText.SetActive(false);
        UpdateLifes(lifes);
    }
    void OpenWinMenu()
    {
        anim.SetTrigger("Show");
        canExitMenu = false;
        restartButton.SetActive(false);
        menuButton.SetActive(false);
        pressImage.SetActive(true);
        lifesObject.SetActive(false);
        winBackground.SetActive(true);
        winText.SetActive(true);
        bowserBackground.SetActive(false);
        loseText.SetActive(false);
        subscribeEnabled = true;
    }
    public void CanExitToMenu()
    {
        //method called by animation event
        canExitMenu = true;
        if(subscribeEnabled) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(restartButton);
    }
    public void UnfreezeTime()
    {
        //method called by animation event
        Time.timeScale = 1f;
    }
    public void Reset()
    {
        anim.SetTrigger("Hide");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void RestartButton()
    {
        GameManager.GetGameManager().ResetGame();
        StopAllCoroutines();
        GameManager.GetGameManager().ChangeActionMap("Player");
    }
    public void ReturnMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void ReturnMenuSubscribed()
    {
        if(!canExitMenu || !subscribeEnabled) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void StopTime()
    {
        //method called by animation event
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.GetGameManager().ChangeActionMap("UI");
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
