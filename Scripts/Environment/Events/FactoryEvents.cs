using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryEvents : MonoBehaviour
{
    public string[] texts;
    public Leo leo;
    public CameraFollow cameraFollow;
    public Transform cameraTarget_0;
    public UIMessenger uiMessenger;
    public GameObject eventTrigger_1, eventTrigger_0, conversationStarter_0, eventTrigger_2;
    public GameObject hangingBox, uiMessengerTrigger;
    public GameObject light;
    public GameObject deadlyGap;
    public Panorama panorama_6, panorama_7, panorama_0, panorama_3;
    public ConversationStarter smellConversation;
    public ConversationInterface conversationInterface;
    public WeaponIndicator weaponIndicator;
    public TalkingBaloonManager talkingBaloonManager;
    public InteractionManager interactButton;
    public Animation heaver;
    public AudioSource ambianceAudioSource, musicAudioSource;
    public PlayerGuide playerGuide;

    [Header("Interactables")]
    public InteractableObject arrowBox;
    public InteractableObject closedDoor;
    public Destroyable boxHanger;
    public InteractableObject[] smellObjects;
    public InteractableObject lever;
    public InteractableObject cableExtra;
    public InteractableObject reel;
    public InteractableObject investigatinhTable;

    #region Triggerable Events
    public void FactoryEvent_0() 
    {
        if (!interactionGuideShowed) { interactionGuideShowed = true; Invoke("ShowInteractionGuide", 1); }
        else if (!arrowUsageGuideShowed && leo.inventory.ItemExists("Arrow")) { arrowUsageGuideShowed = true; Invoke("ShowArrowUsageGuide", 1); }

        uiMessenger.ShowMessage(uiMessenger.textArray[0]);
        cameraFollow.target = cameraTarget_0;
        if (eventTrigger_1 != null)
            eventTrigger_1.SetActive(true);
    }
    public void FactoryEvent_0Rewind() 
    {
        uiMessenger.HideMessage();
        cameraFollow.target = GameObject.Find("Leo").GetComponent<Transform>();
    }
    private void ShowInteractionGuide() 
    {
        playerGuide.OpenPlayerGuide("InteractionGuide");
    }
    private void ShowArrowUsageGuide()
    {
        playerGuide.OpenPlayerGuide("ArrowUsageGuide");
    }
    private bool arrowUsageGuideShowed = false;
    private bool interactionGuideShowed = false;

    public void FactoryEvent_1() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[1]);
        Destroy(eventTrigger_1);

        Invoke("FactoryEvent_1_0", 5);
    }
    private void FactoryEvent_1_0() { uiMessenger.HideMessage(); }

    public void FactoryEvent_2() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[3]);
    }
    public void FactoryEvent_2Rewind() 
    {
        uiMessenger.HideMessage();
    }

    public void FactoryEvent_3() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[5]);
    }
    public void FactoryEvent_3Rewind() 
    {
        uiMessenger.HideMessage();
    }

    public void FactoryEvent_4() 
    {
        if (leo.inventory.GetItem("Zippo") != null) 
        {
            GameObject.Find("WeaponIndicator").GetComponent<WeaponIndicator>().ChangeWeapon("Zippo");
            leo.Attack();
        }
    }

    public void FactoryEvent_5() 
    {
        if (leo.inventory.GetItem("Zippo") == null)
            uiMessenger.ShowMessage(uiMessenger.textArray[9]);
        else
            uiMessenger.ShowMessage(uiMessenger.textArray[10]);
    }

    public void FactoryEvent_6() 
    {
        int[] conversation = { 20, 21, 22, 23, 24 };
        int[] conversationCQ = { 1, 1, 1, 1, 1 };
        conversationInterface.StartConversation(conversation, conversationCQ);
        GameObject.Find("TalkingBaloonTrigger_0").GetComponent<BoxCollider2D>().enabled = true;
        GameObject.Find("ArtkenCardLight").GetComponent<Animation>().Play("ArtkenCardLight");

        InteractableObject fireplace_1 = GameObject.Find("Fireplace_1").GetComponent<InteractableObject>();
        fireplace_1.interactionType = InteractableObject.InteractionType.EmptyInteraction;
        fireplace_1.interactable = false;
        fireplace_1.interactionName = 19;
    }
    #endregion

    #region Specific Events
    private bool weaponsGuideShowed = false;
    private IEnumerator OnTookArrow() 
    {       
        leo.talkingBaloon.StartTalking(talkingBaloonManager.textArray[3], 2);
        weaponIndicator.ChangeWeapon();
        yield return new WaitForSeconds(1);
        if (!weaponsGuideShowed)
        {
            playerGuide.OpenPlayerGuide("WeaponsGuide");
            weaponsGuideShowed = true;
        }
    }
    private IEnumerator OnDestroyBoxHanger() 
    {
        hangingBox.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        hangingBox.GetComponent<PolygonCollider2D>().enabled = true;
        FactoryEvent_0Rewind();
        Destroy(eventTrigger_0);
        yield return new WaitForSecondsRealtime(2);
        hangingBox.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
    private IEnumerator OnInteractedConversationStarter_0() 
    {
        uiMessengerTrigger.SetActive(true);
        yield return null;
    }
    private IEnumerator OnFinishSmellConversation() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[7], 5);
        yield return null;
    }
    private IEnumerator UnlockDoor() 
    {
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        Destroy(GameObject.Find("ClosedDoor"));
        Destroy(GameObject.Find("EventTrigger_6"));
        GameObject.Find("CharacterMask_0").GetComponent<SpriteMask>().enabled = true;
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_OnChangedPanorama");
    }
    private IEnumerator OnSelectClosedDoor() 
    {
        Debug.Log("1");
        if (!leo.inventory.ItemExists("Key")) 
        {
            Debug.Log("2");
            Debug.Log(interactButton.gameObject.name);
            interactButton.buttonText.text = interactButton.interactableNames[12] + " (" + interactButton.interactionNames[13] + ")";
            interactButton.interactButton.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>()[0].interactable = false;
        }
        closedDoor.selectEvent = OnSelectClosedDoor();
        yield return null;
    }
    private IEnumerator OnSelectCable() 
    {
        if (leo.inventory.ItemExists("CollectableCable"))
        {
            Debug.Log("Cable Exists");
            interactButton.buttonText.text = interactButton.interactableNames[19] + " (" + interactButton.interactionNames[15] + ")";
            interactButton.interactButton.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else 
        {
            Debug.Log("Cable Does Not Exist");
            interactButton.buttonText.text = interactButton.interactableNames[19] + " (" + interactButton.interactionNames[17] + ")";
            interactButton.interactButton.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        cableExtra.selectEvent = OnSelectCable();

        yield return null;
    }
    private IEnumerator FixCable() 
    {
        cableExtra.gameObject.GetComponent<Animation>().Play("CableExtra");
        leverEnabled = true;
        Destroy(eventTrigger_2);
        uiMessenger.ShowMessage(uiMessenger.textArray[11], 1.5f);
        yield return null;
    }
    private bool leverEnabled;
    private IEnumerator OnSelectLever() 
    {
        if (leverEnabled)
        {
            interactButton.buttonText.text = interactButton.interactableNames[20] + " (" + interactButton.interactionNames[16] + ")";
            interactButton.interactButton.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else 
        {
            interactButton.buttonText.text = interactButton.interactableNames[20] + " (" + interactButton.interactionNames[18] + ")";
            interactButton.interactButton.gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        lever.selectEvent = OnSelectLever();

        yield return null;
    }
    private IEnumerator InteractLever() 
    {
        if (leverEnabled)
        {
            heaver.Play("Heaver");
            Destroy(deadlyGap);
        }
        yield return null;
    }
    private IEnumerator InteractReel() 
    {
        int[] conversation = {25, 26};
        int[] conversationCQ = { 1, 1 };
        conversationInterface.StartConversation(conversation, conversationCQ);
        conversationInterface.onFinishConversation = OnFinishReekStuckConversation();
        yield return null;
    }
    private IEnumerator OnFinishReekStuckConversation() 
    {
        eventTrigger_0.GetComponent<BoxCollider2D>().enabled = false;
        uiMessenger.ShowMessage(uiMessenger.textArray[13], 3);
        yield return new WaitForSeconds(3.5f);
        uiMessenger.ShowMessage(uiMessenger.textArray[14], 3);
        yield return new WaitForSeconds(3.5f);
        eventTrigger_0.GetComponent<BoxCollider2D>().enabled = true;
    }
    private IEnumerator OnInvestigateTable() 
    {
        if (!investigatingGuideShowed) 
        {
            yield return new WaitForSeconds(1);
            playerGuide.OpenPlayerGuide("InvestigatingGuide");
            investigatingGuideShowed = true;
        }
        yield return null;
    }
    private bool investigatingGuideShowed = false;

    private IEnumerator OnEnterDarkPanorama() 
    {
        light.SetActive(true);
        light.GetComponent<Transform>().SetParent(GameObject.Find("Leo").GetComponent<Leo>().weapon.gameObject.GetComponent<Transform>());
        light.GetComponent<Transform>().localPosition = new Vector3(0, 0, -2);
        panorama_6.panoramaEnterEvent = OnEnterDarkPanorama();
        panorama_7.panoramaEnterEvent = OnEnterDarkPanorama();
        yield return null;
    }
    private IEnumerator OnExitDarkPanorama() 
    {
        light.GetComponent<Transform>().SetParent(null);
        light.SetActive(false);
        panorama_6.panoramaExitEvent = OnExitDarkPanorama();
        panorama_7.panoramaExitEvent = OnExitDarkPanorama();
        yield return null;
    }

    //  PANORAMA 0
    private IEnumerator OnPassPanorama_0() { ambianceAudioSource.Stop(); ambianceAudioSource.PlayOneShot(Resources.Load<AudioClip>(@"Audio\Environment\SoftBreezeAmbiance")); panorama_0.panoramaEnterEvent = OnPassPanorama_0(); yield return null; }
    private IEnumerator OnExitPanorama_0() { ambianceAudioSource.Stop(); ambianceAudioSource.PlayOneShot(Resources.Load<AudioClip>(@"Audio\Environment\FactoryAmbiance")); panorama_0.panoramaExitEvent = OnExitPanorama_0(); yield return null; }

    // PANORAMA 3
    private IEnumerator OnPassPanorama_3() // (once)
    {
        musicAudioSource.gameObject.GetComponent<Animation>().Play("MuteMusic");
        yield return new WaitForSeconds(7);
        musicAudioSource.Stop();
        musicAudioSource.volume = 0.5f;
        musicAudioSource.PlayOneShot(Resources.Load<AudioClip>(@"Audio\Soundtracks\ArtkenSoundtrack_2"));
    }

    bool conversationInterfaceDisplayed = false;
    int conversationID = 12;
    private IEnumerator OnSmellObject(InteractableObject smellObject) 
    {
        if (smellObject.gameObject.name == "Fireplace")
        {
            int[] conversation = { 15 };
            int[] conversationCQ = { 1 };
            conversationInterface.StartConversation(conversation, conversationCQ);
            smellObject.interactionType = InteractableObject.InteractionType.Investigate;
            smellObject.interactionName = 4;
        }
        else
        {
            if (conversationID == 14)
                conversationID = 12;
            else
                conversationID++;

            int[] conversation = { conversationID };
            int[] conversationCQ = { 1 }; 
            if (!conversationInterfaceDisplayed)
            {
                conversationInterface.StartConversation(conversation, conversationCQ);
                conversationInterfaceDisplayed = true;
            }
            else
            {
                leo.talkingBaloon.StartTalking(conversationInterface.textArray[conversationID], 1.5f);
            }
            smellObject.interactionType = InteractableObject.InteractionType.None;
        }
        yield return null;
    }
    private void SetSmellObjectEvents() 
    {
        foreach (InteractableObject smellObject in smellObjects)
            smellObject.sceneEvent = OnSmellObject(smellObject);
    }
    #endregion

    #region InvestigatingObjectEvents

    #region Investigating Table
    //  INVESTIGATING TABLE
    private GameObject[] letters;
    private GameObject activeLetter;
    private int letterPaperPointer = 0;
    private bool leoHasSpoken = false;
    private bool leoSeenLetter = false;
    private bool musicHasPlayed = false;

    public IEnumerator OnOpenInvestigatingTable(InvestigatingObject table) 
    {
        if (GameObject.Find("Leo").GetComponent<Leo>().inventory.GetItem("Axe") != null)
            table.deactivatingObjects[1].SetActive(false);
        table.SetEventInteractionButtonClick(4, (delegate { OnClickInteractLeftDrawerClick1(); }));
        table.SetEventInteractionButtonClick(5, (delegate { OnClickButtonPassLetterRight(); }));
        table.SetEventInteractionButtonClick(6, (delegate { OnClickButtonPassLetterLeft(); }));
        table.SetEventBeforeButtonInteraction(7, StartPalm());

        List<GameObject> letters = new List<GameObject>();
        letters.Add(table.deactivatingObjects[3]);
        letters.Add(table.deactivatingObjects[4]);
        letters.Add(table.deactivatingObjects[5]);

        this.letters = letters.ToArray();
        yield return null;
    }
    private void OnClickInteractLeftDrawerClick1() 
    {
        GameObject.Find("ButtonInteractLeftDrawer1").SetActive(false);
    }
    private void OnClickButtonPassLetterRight() 
    {
        if (activeLetter == null)
            activeLetter = letters[0];

        letterPaperPointer++;

        StartCoroutine(ChangePaper());
    }
    private void OnClickButtonPassLetterLeft() 
    {
        if (activeLetter == null)
            activeLetter = letters[0];

        letterPaperPointer--;

        StartCoroutine(ChangePaper());
    }
    private IEnumerator ChangePaper() 
    {
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        if (letterPaperPointer == -1)
            letterPaperPointer = 2;
        if (letterPaperPointer == 3)
            letterPaperPointer = 0;

        activeLetter.SetActive(false);

        activeLetter = letters[letterPaperPointer];
        activeLetter.SetActive(true);

        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_OnChangedPanorama");

        if (letterPaperPointer == 1 && !leoHasSpoken)
            StartCoroutine(SpeakLeoLetterFromMyFather());
    }
    private IEnumerator SpeakLeoLetterFromMyFather() 
    {
        GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().hideEvent = SpeakLeoIDidNotKnow();
        leoHasSpoken = true;
        yield return new WaitForSeconds(1);
        uiMessenger.ShowMessage(uiMessenger.textArray[12], 4);
    }
    private IEnumerator SpeakLeoIDidNotKnow() 
    {
        conversationInterface.StartConversation(new int[] { 19 }, new int[] { 1 });
        yield return null;
    }
    private IEnumerator StartPalm() 
    {
        musicAudioSource.gameObject.GetComponent<Animation>().Play("MuteMusic");
        Invoke("PlayArtkenLetterMusic", 1);
        yield return null;
    }
    private void PlayArtkenLetterMusic() 
    {
        musicAudioSource.gameObject.GetComponent<Animation>().Stop();
        musicAudioSource.Stop();
        musicAudioSource.volume = 0.5f;
        musicAudioSource.PlayOneShot(Resources.Load<AudioClip>(@"Audio\Soundtracks\ArtkenSoundtrack_2"));     
    }
    #endregion
    #region Encrypted Door
    //  ENCRYPTED DOOR
    public IEnumerator OnOpenInvestigateEncryptedDoorUI(InvestigatingObject EDUI) 
    {
        passwordPanel = GameObject.Find("PasswordPanel").GetComponent<UnityEngine.UI.Text>();

        EDUI.SetEventInteractionButtonClick(0, (delegate { OnClickPasswordNumberAdd("0"); }));
        EDUI.SetEventInteractionButtonClick(1, (delegate { OnClickPasswordNumberAdd("1"); }));
        EDUI.SetEventInteractionButtonClick(2, (delegate { OnClickPasswordNumberAdd("2"); }));
        EDUI.SetEventInteractionButtonClick(3, (delegate { OnClickPasswordNumberAdd("3"); }));
        EDUI.SetEventInteractionButtonClick(4, (delegate { OnClickPasswordNumberAdd("4"); }));
        EDUI.SetEventInteractionButtonClick(5, (delegate { OnClickPasswordNumberAdd("5"); }));
        EDUI.SetEventInteractionButtonClick(6, (delegate { OnClickPasswordNumberAdd("6"); }));
        EDUI.SetEventInteractionButtonClick(7, (delegate { OnClickPasswordNumberAdd("7"); }));
        EDUI.SetEventInteractionButtonClick(8, (delegate { OnClickPasswordNumberAdd("8"); }));
        EDUI.SetEventInteractionButtonClick(9, (delegate { OnClickPasswordNumberAdd("9"); }));

        EDUI.SetEventInteractionButtonClick(10, (delegate { OnClickDelete(); }));
        EDUI.SetEventInteractionButtonClick(11, delegate { OnClickEnter(); });
        int a;
        if (int.TryParse(passwordPanel.text, out a))
            inputPassword = passwordPanel.text;
        else
            passwordPanel.text = texts[2];
        yield return null;
    }
    public void OnClickPasswordNumberAdd(string number) 
    {
        inputPassword += number;
        passwordPanel.text = inputPassword;
    }
    public void OnClickDelete() 
    {
        Debug.Log("del");
        inputPassword = inputPassword.Remove(inputPassword.Length - 1);
        if (inputPassword == "")
            passwordPanel.text = texts[2];
        else
            passwordPanel.text = inputPassword;
    }
    public void OnClickEnter() 
    {
        if (inputPassword == password)
            StartCoroutine(PasswordIsCorrect());
        else
            StartCoroutine(PasswordIsWrong());

    }
    private IEnumerator PasswordIsCorrect()
    {
        GameObject.Find("EncryptedDoor").GetComponent<SpriteRenderer>().sprite = Resources.Load("Environment\\EncryptedOpenDoor", typeof(Sprite)) as Sprite;
        GameObject.Find("EncryptedDoor").GetComponent<EdgeCollider2D>().enabled = false;
        passwordPanel.text = texts[0];
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().HideInvestigateUI();
    }
    private IEnumerator PasswordIsWrong() 
    {
        passwordPanel.text = texts[1];
        yield return new WaitForSeconds(0.5f);
        passwordPanel.text = inputPassword;
    }
    private string password = "49305";
    private string inputPassword;
    private UnityEngine.UI.Text passwordPanel;
#endregion
    #region Shelf
    // SHELF
    public IEnumerator OnOpenInvestigatingShelf(InvestigatingObject shelf) 
    {
        uiMessenger.HideMessage();
        shelf.SetEventButtonInteraction(0, OnInteractLighterGas());        

        if (leo.inventory.GetItem("Zippo") == null)
        {
            lighterGasInteractionAvailability = false;
            shelf.interactionButtons[0].interactable = false;
        }
        else 
        {
            lighterGasInteractionAvailability = true;
            shelf.interactionButtons[0].interactable = true;
        }
        yield return null;
    }
    private IEnumerator OnInteractLighterGas() 
    {
        if (lighterGasInteractionAvailability)
        {
            leo.inventory.GetItem("Zippo").availability = 100;
            DatabaseManager.SetLeoInventory(leo.inventory);
            uiMessenger.ShowMessage(uiMessenger.textArray[8], 2);
            Destroy(GameObject.Find("Barrier_0"));
            Destroy(GameObject.Find("EventTrigger_5"));
        }
        yield return null;
    }
    private bool lighterGasInteractionAvailability;
    #endregion
    #region Fireplace
    // FIREPLACE
    public IEnumerator OnOpenInvestigatingFireplace(InvestigatingObject fireplace) 
    {
        fireplace.SetEventButtonInteraction(0, OnInteractHandle());
        yield return null;
    }
    private IEnumerator OnInteractHandle() { StaticCoroutine.DoCoroutine(onInteractHandle()); yield return null; }
    private IEnumerator onInteractHandle() 
    {
        int[] conversation = {10};
        int[] conversationCQ = { 1 };
        GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().HideInvestigateUI();
        yield return new WaitForSeconds(0.5f);
        conversationInterface.StartConversation(conversation, conversationCQ);
        conversationInterface.onFinishConversation = OnFinishHandleConversation();
    }
    private IEnumerator OnFinishHandleConversation() 
    {
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_Closing");
        yield return new WaitForSeconds(1);
        Camera.main.GetComponent<MultiLayer>().currentPanorama = GameObject.Find("Factory_Panorama_7").GetComponent<Panorama>();
        leo.tf.position = GameObject.Find("SpecificStartPosition_2").GetComponent<Transform>().position;

        InteractableObject fireplace = GameObject.Find("Fireplace").GetComponent<InteractableObject>();
        fireplace.interactionType = InteractableObject.InteractionType.PassToPanorama;
        fireplace.interactionName = 10;
        string[] interactionParams = { "Factory_Panorama_7", "SpecificStartPosition_2" };
        fireplace.interactionParams = interactionParams;
    }
    #endregion
    #region Artken Card
    // ARTKEN CARD
    bool talkedAboutCard = false;
    public IEnumerator OnOpenArtkenCard() 
    {
        if(!talkedAboutCard)
        {
            GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().hideEvent = OnHideCard();
            talkedAboutCard = true;
        }
        yield return null;
    }
    public IEnumerator OnHideCard()
    {
        InteractableObject fireplace_1 = GameObject.Find("Fireplace_1").GetComponent<InteractableObject>();
        fireplace_1.interactionType = InteractableObject.InteractionType.PassToPanorama;
        fireplace_1.interactable = true;
        fireplace_1.interactionName = 11;

        int[] conversation = {11, 16};
        int[] conversationCQ = { 1, 1 };
        conversationInterface.StartConversation(conversation, conversationCQ);
        yield return null;
    }
    #endregion

    #endregion

    private void Start()
    {
        texts = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "SceneTexts"));
        SetSceneTexts();

        arrowBox.sceneEvent = OnTookArrow();
        boxHanger.Event = OnDestroyBoxHanger();
        conversationStarter_0.GetComponent<ConversationStarter>().sceneEvent = OnInteractedConversationStarter_0();
        smellConversation.onFinishConversation = OnFinishSmellConversation();
        closedDoor.sceneEvent = UnlockDoor();
        closedDoor.selectEvent = OnSelectClosedDoor();
        investigatinhTable.sceneEvent = OnInvestigateTable();

        lever.sceneEvent = InteractLever();
        lever.selectEvent = OnSelectLever();

        cableExtra.sceneEvent = FixCable();
        cableExtra.selectEvent = OnSelectCable();

        reel.sceneEvent = InteractReel();

        panorama_6.panoramaEnterEvent = OnEnterDarkPanorama();
        panorama_6.panoramaExitEvent = OnExitDarkPanorama();

        panorama_7.panoramaEnterEvent = OnEnterDarkPanorama();
        panorama_7.panoramaExitEvent = OnExitDarkPanorama();

        panorama_0.panoramaEnterEvent = OnPassPanorama_0();
        panorama_0.panoramaExitEvent = OnExitPanorama_0();

        //panorama_3.panoramaEnterEvent = OnPassPanorama_3();

        SetSmellObjectEvents();
    }
    private void SetSceneTexts() 
    {
        GameObject.Find("NextButtonText").GetComponent<UnityEngine.UI.Text>().text = texts[3];
    }
}
