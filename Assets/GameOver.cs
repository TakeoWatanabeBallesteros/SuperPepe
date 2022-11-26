using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour, IReset
{
    Animator anim;
    [SerializeField] float timeToFreezeGame;
    [SerializeField] GameObject restartButton;
    private void Start() {
        GameManager.GetGameManager().AddResetObject(this);
    }
    private void OnEnable() {
        anim = GetComponent<Animator>();
        GameManager.OnGameOverEvent += OpenGameOverMenu;
    }
    private void OnDisable() {
        GameManager.OnGameOverEvent -= OpenGameOverMenu;
    }
    void OpenGameOverMenu(bool hasLifes)
    {
        anim.SetTrigger("Show");
        restartButton.SetActive(hasLifes);
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
}
