using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // 0: Factory, 1: Artken, 2: Auction Hall, 3: Railway Station
    public GameObject[] sceneStarters;
    public Animation darkForeground;
    public int currentViewingScene = 0;

    public Text[] buttonTexts;
    public Text[] sceneNames;

    public void OnClick_ContinueGame() 
    {
        StartCoroutine(onClick_ContinueGame());
    }
    private IEnumerator onClick_ContinueGame() 
    {
        darkForeground.Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        sceneStarters[0].SetActive(true);
        darkForeground.Play("FrontImage_OnChangedPanorama");
    }

    public void OnClick_StartNewGame() 
    {

    }
    public void OnClick_Quit() 
    {
        Application.Quit();
    }

    public void OnClickPassToNextScene() 
    {
        StartCoroutine(onClickPassToNextScene());
    }
    private IEnumerator onClickPassToNextScene() 
    {
        currentViewingScene++;
        darkForeground.Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        sceneStarters[currentViewingScene - 1].SetActive(false);
        sceneStarters[currentViewingScene].SetActive(true);
        darkForeground.Play("FrontImage_OnChangedPanorama");
    }

    public void OnClickPassToPreviousScene()
    {
        if (currentViewingScene > 0)
            StartCoroutine(onClickPassToPreviousScene());
        else
            StartCoroutine(HideSceneViewer());
    }
    private IEnumerator onClickPassToPreviousScene()
    {
        currentViewingScene--;
        darkForeground.Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        sceneStarters[currentViewingScene + 1].SetActive(false);
        sceneStarters[currentViewingScene].SetActive(true);
        darkForeground.Play("FrontImage_OnChangedPanorama");
    }
    private IEnumerator HideSceneViewer() 
    {
        darkForeground.Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        sceneStarters[0].SetActive(false);
        darkForeground.Play("FrontImage_OnChangedPanorama");
    }

    public void OnClickPlayButton() 
    {
        switch (currentViewingScene) 
        {
            case 0: GeneralVariables.scene = GeneralVariables.Scene.Factory; break;
            case 1: GeneralVariables.scene = GeneralVariables.Scene.Underground_0; break;
            case 2: GeneralVariables.scene = GeneralVariables.Scene.Underground_1; break;
        }

        StartCoroutine(onClickPlayButton());
    }
    private IEnumerator onClickPlayButton() 
    {
        darkForeground.Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(currentViewingScene + 1);
    }

    public void SetSceneTexts()
    {
        SetButtonTexts();
        SetSceneNames();
    }
    private void SetButtonTexts() 
    {
        string[] texts = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "ButtonTexts"));
        for (int i = 0; i < 4; i++)
            buttonTexts[i].text = texts[i];

        buttonTexts[4].text = texts[4];
        buttonTexts[5].text = texts[4];
        buttonTexts[6].text = texts[4];
        buttonTexts[7].text = texts[4];
    }
    private void SetSceneNames() 
    {
        string[] texts = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "SceneNames"));
        for (int i = 0; i < sceneNames.Length; i++)
            sceneNames[i].text = texts[i];
    }

    private void Start() 
    {
        SetSceneTexts();
    }
}
