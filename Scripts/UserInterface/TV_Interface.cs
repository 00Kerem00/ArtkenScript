using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TV_Interface : MonoBehaviour
{
    public string[] elementNames;
    public TVIF_Element[] currentElements;
    public Image[] elementImages;
    public Text[] elementText;
    public Text title;    
    
    public GameObject textViewer;
    public Text textViewerText;

    public GameObject pictureViewer;
    public Image pvPicture;
    public TVIF_Element[] pictures;
    int displayingPictureID;
    public GameObject zoomedPictureMask;
    public Transform zoomedPicture;
    public string[] localizedPictureTexts;
    public GameObject[] currentLocalizedPictureTexts;

    private List<TVIF_Element> recentFolders = new List<TVIF_Element>();

    #region File Manager
    public void OnClickElementButton(int ID) 
    {
        TVIF_Element selectedElement = currentElements[ID];
        switch (selectedElement.elementType) 
        {
            case "Folder": OpenFolder(selectedElement, false); break;
            case "Picture": OpenPictureViewer(ID); break;
            case "Text": OpenTextViewer(selectedElement.textLines); break;
        }
    }
    public void OnClickBackFolder() 
    {
        if (recentFolders.Count > 1)
        {
            recentFolders.RemoveAt(recentFolders.Count - 1);
            OpenFolder(recentFolders.ToArray()[recentFolders.Count - 1], true);
        }
    }

    public void OpenFolder(TVIF_Element selectedElement, bool back) 
    {
        HideAllElements();
        if (!back)
            recentFolders.Add(selectedElement);
        currentElements = ReadFolder(selectedElement.folderName);
        currentElements = ChehckUSBDiskExists(selectedElement.folderName, currentElements);

        for (int i = 0; i < currentElements.Length; i++) 
        {
            elementImages[i].sprite = Resources.Load(@"TVDatabase\Icons\" + currentElements[i].iconName, typeof(Sprite)) as Sprite;
            elementText[i].text = elementNames[currentElements[i].nameID];
        }

        ShowNonNullElements();
        title.text = elementNames[selectedElement.nameID];
    }
    private TVIF_Element[] ChehckUSBDiskExists(string folderName, TVIF_Element[] current_Elements) 
    {
        if (folderName == "Discs")
        {
            Leo leo = GameObject.Find("Leo").GetComponent<Leo>();
            if (!leo.inventory.ItemExists("USBDisk"))
            {
                List<TVIF_Element> elementList = new List<TVIF_Element>(currentElements);
                elementList.RemoveAt(1);
                current_Elements = elementList.ToArray();
            }
        }

        return current_Elements;
    }

    public void HideAllElements() 
    {
        for (int i = 0; i < elementImages.Length; i++) 
        {
            elementImages[i].gameObject.SetActive(false);
        }
    }
    public void ShowNonNullElements() 
    {
        for (int i = 0; i < currentElements.Length; i++) 
        {
            elementImages[i].gameObject.SetActive(true);
        }
    }
    #endregion
    #region Picture Viewer

    public void OnClick_ClosePictureViewer()
    {
        pictureViewer.SetActive(false);
    }
    public void OpenPictureViewer(int buttonID)
    {
        List<TVIF_Element> pictures = new List<TVIF_Element>();
        foreach (TVIF_Element element in currentElements)
            if (element.elementType == "Picture")
                pictures.Add(element);
        this.pictures = pictures.ToArray();

        displayingPictureID = 0;
        foreach (TVIF_Element pic in pictures) { if (pic.fileName == currentElements[buttonID].fileName) break; displayingPictureID++; }

        pictureViewer.SetActive(true);
        SetPicture(currentElements[buttonID]);

        pvPicture.gameObject.GetComponent<ButtonUpDownManager>().OnDown = OnDownPicture();
        pvPicture.gameObject.GetComponent<ButtonUpDownManager>().OnUp = OnUpPicture();
    }
    public void SetPicture(TVIF_Element picture)
    {
        pvPicture.sprite = Resources.Load(@"TVDatabase\Pictures\" + picture.fileName, typeof(Sprite)) as Sprite;
        zoomedPicture.gameObject.GetComponent<Image>().sprite = pvPicture.sprite;

        SetPictureSize(picture.fileName);

        DestroyLocalizedPictureTexts();
        CreateLocalizedPictureTexts(picture.fileName);
    }

    // PASSAGE NEXT & PREVIOUS PICTURES
    public void OnClick_PassNextPicture() 
    {
        StartCoroutine(passNextPicture());
    }
    private IEnumerator passNextPicture() 
    {
        pvPicture.gameObject.GetComponent<Animation>().Play("HideWeaponIndicator");
        yield return new WaitForSeconds(0.25f);
        
        displayingPictureID++;
        if (displayingPictureID == pictures.Length)
            displayingPictureID = 0;

        SetPicture(pictures[displayingPictureID]);

        pvPicture.gameObject.GetComponent<Animation>().Play("ShowWeaponIndicator");
    }
    public void OnClick_PassPrevPicture()
    {
        StartCoroutine(passPrevPicture());
    }
    private IEnumerator passPrevPicture() 
    {
        pvPicture.gameObject.GetComponent<Animation>().Play("HideWeaponIndicator");
        yield return new WaitForSeconds(0.25f);

        displayingPictureID--;
        if (displayingPictureID == -1)
            displayingPictureID = pictures.Length - 1;

        SetPicture(pictures[displayingPictureID]);

        pvPicture.gameObject.GetComponent<Animation>().Play("ShowWeaponIndicator");
    }

    // LOCALIZED TEXTS
    public void CreateLocalizedPictureTexts(string pictureName) 
    {
        List<GameObject> newTexts = new List<GameObject>();

        Debug.Log(pictureName);
        string path = @"TVDatabase\Pictures\LocalizedTexts\" + pictureName;
        string[] localizedTextStates = TextManager.GetTextArray(path);
        if (localizedTextStates == null)
            return;
        
        Debug.Log("Picture Name: " + pictureName);
        for (int i = 0; i < localizedTextStates.Length / 5; i++) 
        {
            GameObject localizedText = new GameObject("localizedPictureText", typeof(Text));
            SetTextStates(localizedText, new Vector2(float.Parse(localizedTextStates[i * 5]), float.Parse(localizedTextStates[1 + (i * 5)])),
                new Vector2(float.Parse(localizedTextStates[2 + (i * 5)]), float.Parse(localizedTextStates[3 + (i * 5)])), localizedPictureTexts[int.Parse(localizedTextStates[4 + (i * 5)])], false);

            newTexts.Add(localizedText);

            GameObject localizedZoomedText = new GameObject("localizedPictureText", typeof(Text));
            SetTextStates(localizedZoomedText, new Vector2(float.Parse(localizedTextStates[i * 5]), float.Parse(localizedTextStates[1 + (i * 5)])),
                new Vector2(float.Parse(localizedTextStates[2 + (i * 5)]), float.Parse(localizedTextStates[3 + (i * 5)])), localizedPictureTexts[int.Parse(localizedTextStates[4 + (i * 5)])], true);

            newTexts.Add(localizedZoomedText);
        }
        currentLocalizedPictureTexts = newTexts.ToArray();
    }
    public void SetTextStates(GameObject textObject, Vector2 anchoredPosition, Vector2 sizeDelta, string localizedText, bool zoomed) 
    {
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        Text text = textObject.GetComponent<Text>();

        if (zoomed)
            rectTransform.SetParent(zoomedPicture.gameObject.GetComponent<RectTransform>());
        else
            rectTransform.SetParent(pvPicture.gameObject.GetComponent<RectTransform>());
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.localScale = new Vector3(0.05f, 0.05f, 1);

        text.alignment = TextAnchor.MiddleCenter;
        text.font = title.font;
        text.text = localizedText;
        text.resizeTextForBestFit = true;
        text.color = Color.black;
    }
    public void DestroyLocalizedPictureTexts() 
    {
        foreach (GameObject go in currentLocalizedPictureTexts)
            Destroy(go);
    }

    // ZOOM
    private bool pictureHolding = false;
    public IEnumerator OnDownPicture() 
    {
        pictureHolding = true;
        zoomedPictureMask.SetActive(true);
        StartCoroutine(ZoomPicture());

        pvPicture.gameObject.GetComponent<ButtonUpDownManager>().OnDown = OnDownPicture();
        yield return null;
    }
    public IEnumerator OnUpPicture() 
    {
        pictureHolding = false;
        zoomedPictureMask.SetActive(false);

        pvPicture.gameObject.GetComponent<ButtonUpDownManager>().OnUp = OnUpPicture();
        yield return null;
    }
    public IEnumerator ZoomPicture() 
    {
        while (pictureHolding) 
        {
            Vector3 pos = Camera.main.ScreenPointToRay(Input.mousePosition).origin - pvPicture.GetComponent<Transform>().position;
            zoomedPicture.localPosition = new Vector3(pos.x * -11, pos.y * -11, 1);
            yield return new WaitForEndOfFrame();
        }
    }

    // PICTURE SIZER
    public void SetPictureSize(string pictureName) 
    {
        string[] sizeTexts = TextManager.GetTextArray(@"TVDatabase\Pictures\PictureSizes\" + pictureName);
        Vector2 sizeDelta = new Vector2(float.Parse(sizeTexts[0]), float.Parse(sizeTexts[1]));
        pvPicture.gameObject.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        zoomedPicture.gameObject.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        zoomedPictureMask.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
    #endregion
    #region Folder Reader
    public TVIF_Element[] ReadFolder(string folderName) 
    {
        List<TVIF_Element> result = new List<TVIF_Element>();
        string path = @"TVDatabase\Folders\" + folderName;

        string[] folderLines = TextManager.GetTextArray(path);

        for (int i = 0; i < (folderLines.Length / 6); i++) 
        {
            TVIF_Element element = new TVIF_Element();
            Debug.Log("a");
            element.nameID = int.Parse(folderLines[i * 6]);
            Debug.Log("b");
            element.iconName = folderLines[1 + (i * 6)];
            element.elementType = folderLines[2 + (i * 6)];
            element.folderName = folderLines[3 + (i * 6)];
            element.fileName = folderLines[4 + (i * 6)];
            element.textLines = ConvertTextToIntArray(folderLines[5 + (i * 6)]);

            result.Add(element);
        }

        return result.ToArray();
    }
    public int[] ConvertTextToIntArray(string text) 
    {
        List<int> result = new List<int>();
        if (text == "Null")
            return result.ToArray();

        string toBeAddedItem = "";
        foreach (char c in text) 
        {
            if (c != '_')
                toBeAddedItem += c;
            else
            { Debug.Log(toBeAddedItem); result.Add(int.Parse(toBeAddedItem)); toBeAddedItem = ""; }
        }
        return result.ToArray();
    }
    #endregion
    #region Text Viewer
    public void OnClick_CloseTextViewer() 
    {
        textViewer.SetActive(false);
    }
    public void OpenTextViewer(int[] textLines) 
    {
        textViewer.SetActive(true);

        string text = "";
        string[] texts = TextManager.GetTextArrayOnlySelected(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.Scene.Underground_0, "TVIF_TextFiles"), textLines);

        foreach (string s in texts)
            text += s;

        textViewerText.text = text;
    }
    #endregion

    private void Start() 
    {
        elementNames = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.Scene.Underground_0, "TVIF_ElementNames"));
        localizedPictureTexts = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.Scene.Underground_0, "TVIF_LocalizedPictureTexts"));

        TVIF_Element firstElement = new TVIF_Element();
        firstElement.nameID = 7;
        firstElement.folderName = "Discs";
        OpenFolder(firstElement, false);
    }

    public class TVIF_Element
    {
        public int nameID;
        public string iconName;
        public string elementType;
        public string folderName;
        public string fileName;
        public int[] textLines;
    }
}
