using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGuide : MonoBehaviour
{
    public Image imageViewer;
    public Text commentText, guideTitle;
    public Button continueButton;
    public Animation playerGuide, skeleton;
    public int partIndex;
    public string[] comments;
    public string guideName;
    public Leo leo;

    public void SetComments(string guideName) 
    {
        comments = TextManager.GetTextArray(@"Texts\" + GeneralVariables.language + @"\PlayerGuide\" + guideName);
        this.guideName = guideName;
    }

    public void OpenPlayerGuide(string guideName) 
    {
        HideBackUI();
        gameObject.SetActive(true);
        SetComments(guideName);
        partIndex = 0;

        commentText.text = comments[partIndex];
        guideTitle.text = TextManager.GetTextArray(@"Texts\" + GeneralVariables.language + @"\PlayerGuide\GuideTitles")[GetTitleIndexOfGuide(guideName)];
        imageViewer.sprite = Resources.Load<Sprite>(@"UserInterface\PlayerGuide\" + guideName + "_" + partIndex);
        playerGuide.Play("ShowWeaponIndicator");        
    }

    public void ClosePlayerGuide() 
    {
        StartCoroutine(closePlayerGuide());
    }
    private IEnumerator closePlayerGuide() 
    {
        if (backUIWillShow)
            ShowBackUI();
        playerGuide.Play("HideWeaponIndicator");
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }

    public void OnClickContinueButton() 
    {
        partIndex++;
        if (partIndex < comments.Length)
            StartCoroutine(onClickContinueButton());
        else
            ClosePlayerGuide();
    }
    private IEnumerator onClickContinueButton() 
    {
        skeleton.Play("HideWeaponIndicator");
        continueButton.interactable = false;
        yield return new WaitForSeconds(0.25f);
        continueButton.interactable = true;
        commentText.text = comments[partIndex];
        imageViewer.sprite = Resources.Load<Sprite>(@"UserInterface\PlayerGuide\" + guideName + "_" + partIndex);
        skeleton.Play("ShowWeaponIndicator");
    }

    private static int GetTitleIndexOfGuide(string guideName) 
    {
        switch (guideName) 
        {
            case "InteractionGuide": return 0;
            case "WeaponsGuide": return 1;
            case "ArrowUsageGuide": return 2;
            case "InvestigatingGuide": return 3;
        }
        return 0;
    }

    private void HideBackUI()
    {
        if (!backUIAppearing)
            backUIWillShow = false;
        else
        {
            backUIWillShow = true;
            leo.currentController.GetComponent<Animation>().Play("HideWeaponIndicator");
            GameObject.Find("HealthIndicator").GetComponent<Animation>().Play("HideWeaponIndicator");
            if (leo.weaponIndicator.gameObject.GetComponent<CanvasGroup>().alpha == 1)
                leo.weaponIndicator.gameObject.GetComponent<Animation>().Play("HideWeaponIndicator");
        }
    }
    private void ShowBackUI()
    {
        leo.currentController.GetComponent<Animation>().Play("ShowWeaponIndicator");
        GameObject.Find("HealthIndicator").GetComponent<Animation>().Play("ShowWeaponIndicator");
        if (leo.weaponIndicator.gameObject.GetComponent<CanvasGroup>().alpha == 0)
            leo.weaponIndicator.gameObject.GetComponent<Animation>().Play("ShowWeaponIndicator");
    }
    private bool backUIAppearing { get { return leo.currentController.active; } }
    private bool backUIWillShow = true;

}
