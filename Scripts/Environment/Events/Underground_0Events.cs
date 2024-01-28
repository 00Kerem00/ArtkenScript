using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Underground_0Events : MonoBehaviour
{
    public string[] texts;
    public Leo leo;
    public ConversationInterface conversationInterface;
    public Guard guard_0;
    public UIMessenger uiMessenger;
    public InteractionManager interactionManager;
    public InteractableObject closedDoor_0, closedDoor_1, disabledDoor, dynamite, spinningTable;
    public InvestigateUI investigateUI;
    public GameObject fenceDoor;
    public Collider2D[] backFloorColliders;     // c_0, t_0, t_1, t_2
    public Collider2D lockedCabinet, backRightsideWall, p7Floor, p7DownStairsCol0;
    public GameObject fenceMask;
    public SpriteRenderer[] p_9FrontWall;
    public Animation fence;
    public AudioSource ambiance;
    public Panorama panorama_1, panorama_2, panorama_3, panorama_7;
    public Destroyable cable;

    private void Start() 
    {
        texts = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "SceneTexts"));

        guard_0.onSetToChase = OnGuardSeeLeo();
        guard_0.onDie = OnDiedGuard_0();

//        panorama_1.panoramaEnterEvent = OnEnterPanorama_1();
//        panorama_2.panoramaEnterEvent = OnEnterPanorama_2();
//        panorama_3.panoramaEnterEvent = OnEnterPanorama_3();
        panorama_7.panoramaEnterEvent = OnEnterPanorama_7();
//        lockedCabinet.gameObject.GetComponent<InteractableObject>().sceneEvent = InteractLockedCabinet();

        closedDoor_0.sceneEvent = UnlockClosedDoor_0();
        closedDoor_0.selectEvent = OnSelectClosedDoor_0();

        closedDoor_1.sceneEvent = InteractClosedDoor_1();
        closedDoor_1.selectEvent = OnSelectClosedDoor_1();

        disabledDoor.sceneEvent = InvestigateDisabledDoor();

        dynamite.sceneEvent = OnCollectDynamite();

        cable.Event = OnDestroyCable();
        Debug.Log("Event Setted");

        leo.inventory.AddItem("Axe", 1, 100, 2);
    }

    #region Specific Events
    // GUARD & LEO CONVERSATION
    public IEnumerator OnGuardSeeLeo() 
    {
        yield return new WaitForSeconds(0.2f);
        GameObject fightPanel = GameObject.Find("FightPanel");
        fightPanel.SetActive(false);
        FightPanel.RemoveEnemy("Guard_0", 0);
        guard_0.state = Guard.State.Idle;
        guard_0.anim.SetBool("Running", false);
        guard_0.enabled = false;
        int[] conversation = { 12, 13, 14, 15, 16, 17 };
        int[] conversationCQ = { 2, 1, 2, 1, 2, 2 };

        conversationInterface.StartConversation(conversation, conversationCQ);
        conversationInterface.onFinishConversation = OnFinishConversationBetweenG_And_L();

        yield return new WaitForSeconds(1);
        fightPanel.SetActive(true);
        yield return null;
    }

    public IEnumerator OnFinishConversationBetweenG_And_L() 
    {
        guard_0.enabled = true;
        yield return null;
    }

    // GUARD
    public IEnumerator OnDiedGuard_0() 
    {
        Debug.Log("Spawn");
        Collectable keyOfGuard = Collectable.SpawnCollectable(guard_0.tf.position, new Leo.Inventory.Item("StoreKey", 1, 100, 3), OnCollectKey());

        keyOfGuard.GetComponents<BoxCollider2D>()[1].enabled = false;
        yield return new WaitForSeconds(0.5f);
        keyOfGuard.GetComponents<BoxCollider2D>()[1].enabled = true;

        yield return new WaitForSeconds(1.5f);
        uiMessenger.ShowMessage(uiMessenger.textArray[7], 3);
    }
    public IEnumerator OnCollectKey() 
    {
        conversationInterface.StartConversation(new int[] { 18 }, new int[] { 1 });
        U_0E_0_textArrayIndex = 1;
        yield return null;
    }

    // CLOSED DOOR 0
    public IEnumerator UnlockClosedDoor_0()
    {
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        Destroy(GameObject.Find("ClosedDoor_0"));
        Destroy(GameObject.Find("EventTrigger_0"));
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_OnChangedPanorama");
    }
    public IEnumerator OnSelectClosedDoor_0() 
    {
        if (!leo.inventory.ItemExists("StoreKey")) 
        {
            interactionManager.buttonText.text = interactionManager.interactableNames[1] + " (" + interactionManager.interactionNames[2] + ")";
            interactionManager.interactButton.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>()[0].interactable = false;
        }
        closedDoor_0.selectEvent = OnSelectClosedDoor_0();
        yield return null;
    }

    // DISABLED DOOR
    private bool barrierExists = true;
    private bool disabledDoorInvestigated = false;
    public IEnumerator InvestigateDisabledDoor() 
    {
        disabledDoorInvestigated = true;
        conversationInterface.StartConversation(new int[] { 6 }, new int[] { 1 });
        conversationInterface.onFinishConversation = OnFinishedDisabledDoorConversation();
        disabledDoor.selectEvent = OnSelectDisabledDoor();
        disabledDoor.interactionType = InteractableObject.InteractionType.PassToPanorama;
        yield return null;
    }
    public IEnumerator OnFinishedDisabledDoorConversation() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[2], 3);
        yield return null;
    }
    public IEnumerator OnSelectDisabledDoor()
    {
        if (!leo.inventory.ItemExists("Dynamite"))  
        {
            interactionManager.buttonText.text = interactionManager.interactableNames[1] + " (" + interactionManager.interactionNames[3] + ")";
            interactionManager.interactButton.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>()[0].interactable = false;
        }
        disabledDoor.selectEvent = OnSelectDisabledDoor();

        yield return null;
    }

    // DYNAMITE
    public IEnumerator OnCollectDynamite() 
    {
        if (disabledDoorInvestigated)
            conversationInterface.StartConversation(new int[] { 19 }, new int[] { 1 });
        else
            conversationInterface.StartConversation(new int[] { 20 }, new int[] { 1 });
        yield return null;
    }

    // LOCKED CABINET
    public IEnumerator InteractLockedCabinet() 
    {
        if (!leo.inventory.ItemExists("CabinetKey"))
        {
            Debug.Log("CabinetKey Does Not Exist");
            investigateUI.hideEvent = CloseLockedCabinet();
            uiMessenger.ShowMessage(uiMessenger.textArray[8]);
        }
        else
        {
            investigateUI.DeepInvestigate(1);
            investigateUI.partQueue = new List<int>();
            investigateUI.partQueue.Add(1);
            Debug.Log("CabinetKey Exists");
        }

        lockedCabinet.gameObject.GetComponent<InteractableObject>().sceneEvent = InteractLockedCabinet();
        yield return null;
    }

    // CLOSED DOOR 1
    public IEnumerator InteractClosedDoor_1() 
    {
        if (leo.inventory.ItemExists("DoorCard")) 
        {
            GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_Closing");
            yield return new WaitForSeconds(0.5f);
            Destroy(GameObject.Find("ClosedDoor_1"));
            GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_OnChangedPanorama");
        }
    }
    public IEnumerator OnSelectClosedDoor_1() 
    {
        if (!leo.inventory.ItemExists("DoorCard"))
        {
            interactionManager.buttonText.text = interactionManager.interactableNames[1] + " (" + interactionManager.interactionNames[10] + ")";
            interactionManager.interactButton.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>()[0].interactable = false;
        }
        closedDoor_1.selectEvent = OnSelectClosedDoor_1();
        yield return null;
    }

    // CABLE
    public IEnumerator OnDestroyCable() 
    {
        Collectable.SpawnCollectable(cable.gameObject.GetComponent<Transform>().position, new Leo.Inventory.Item("Cable", 1, 100, 4));
        yield return null;
    }

    // PANORAMA 7
    public IEnumerator OnEnterPanorama_7()
    {
        p7Floor.gameObject.SetActive(false); p7DownStairsCol0.enabled = false; panorama_7.panoramaEnterEvent = OnEnterPanorama_7(); yield return new WaitForSeconds(0.5f); p7Floor.gameObject.SetActive(true);
    }

    #endregion

    #region TriggerableEvents
    public void StartUnderground_0Event(int eventNumber) 
    {
        switch (eventNumber) 
        {
            case 0: Underground_0Event_0(); break;
            case 1: Underground_0Event_1(); break;
            case 2: Underground_0Event_2(); break;
            case 3: Underground_0Event_3(); break;
            case 4: Underground_0Event_4(); break;
            case 5: Underground_0Event_5(); break;
        }
    }
    public void StartUnderground_0EventRewind(int eventNumber) 
    {
        switch(eventNumber)
        {
            case 0: Underground_0Event_0Rewind(); break;
        }
    }

    private int U_0E_0_textArrayIndex = 0;
    public void Underground_0Event_0() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[U_0E_0_textArrayIndex]);
    }
    public void Underground_0Event_0Rewind() 
    {
        uiMessenger.HideMessage();
    }

    public void Underground_0Event_1() 
    {
        StartCoroutine(underground_0Event_1());
    }
    private IEnumerator underground_0Event_1() 
    {
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_Closing");
        yield return new WaitForSeconds(3);
        leo.tf.position = GameObject.Find("SpecificStartPosition_2").GetComponent<Transform>().position;
        Camera.main.GetComponent<MultiLayer>().currentPanorama = GameObject.Find("Underground_0_Panorama_1").GetComponent<Panorama>();
    }

    public void Underground_0Event_2() // t_0
    {
        backFloorColliders[0].enabled = true;
        backFloorColliders[3].enabled = true;
        backFloorColliders[2].enabled = true;
        backFloorColliders[1].enabled = false;
        AttenuateFence();
    }
    public void Underground_0Event_3() // t_1 
    {
        backFloorColliders[0].enabled = false;
        backFloorColliders[1].enabled = false;
        StartCoroutine(ActivateFence());
    }   
    public void Underground_0Event_4() // t_2
    {
        backFloorColliders[1].enabled = true;
        backFloorColliders[2].enabled = false;
        backFloorColliders[3].enabled = false;
    }

    bool activateFenceGuideShowed = false;
    bool attenuateFenceGuideShowed = false;
    //GameObject activateFenceGuide;
    public void AttenuateFence() 
    { 
        fence.Play("AttenuateFence"); 
        fence.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "FrontOfCharacter";
        foreach (SpriteRenderer sprite in p_9FrontWall)
            sprite.sortingLayerName = "FrontOfCharacter";
        fenceMask.SetActive(false);
        lockedCabinet.enabled = true;
        backRightsideWall.enabled = true;

        if (!attenuateFenceGuideShowed)
        {
            LocalizedTextMesh.SpawnLocalizedText(new int[] { 2, 3 }, new Vector2(307.2f, -32.5f), 1);
            attenuateFenceGuideShowed = true;
            LocalizedTextMesh.DestroyLocalizedText(GameObject.Find("LocalizedText"));
        }
    }
    public IEnumerator ActivateFence() 
    {
        yield return new WaitForSeconds(0.5f);
        fence.Play("ActivateFence"); 
        fence.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        foreach (SpriteRenderer sprite in p_9FrontWall)
            sprite.sortingLayerName = "Default";
        yield return new WaitForSeconds(0.4f);
        fence.Stop(); 
        fence.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        fenceMask.SetActive(true);
        lockedCabinet.enabled = false;
        backRightsideWall.enabled = false;

        if (!activateFenceGuideShowed) 
        {
            LocalizedTextMesh.DestroyLocalizedText(GameObject.Find("LocalizedText"));
            activateFenceGuideShowed = true;
        }
    }

    public void Underground_0Event_5() 
    {
        conversationInterface.StartConversation(new int[] { 25 }, new int[] { 1 });
        conversationInterface.onFinishConversation = ShowMessage_GetOnTopOfColumns();
        Destroy(GameObject.Find("Event Trigger_5"));
    }
    private IEnumerator ShowMessage_GetOnTopOfColumns() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[10], 3);
        GameObject.Find("D_ColumnsInL").GetComponent<Animation>().Play("U0P3_ColumnsInL");
        yield return null;
    }
    #endregion

    #region InvestigatingObjectEvents
    #region TV_Interface
    // TV_INTERFACE
    public IEnumerator OnOpenTV_Interface(InvestigatingObject tv_Interface) 
    {
        Debug.Log("Open TV_Interface");
        tv_Interface.SetEventInteractionButtonClick(0, (delegate { OnClickPassPicture(); }));
        tv_Interface.SetEventInteractionButtonClick(1, (delegate { OnClickPassPicture(); }));
        yield return null;
    }
    private void OnClickPassPicture() 
    {
        Debug.Log("PassPicture");
    }
    #endregion

    #region Spinning Table
    // SPINNING TABLE
    private bool tableRippedOut = false;

    public IEnumerator OnOpenSpinningTable(InvestigatingObject spinningTable) 
    {
        if (!leo.inventory.ItemExists("Crowbar"))
        {
            if (crowbarNeeded)
                uiMessenger.ShowMessage(uiMessenger.textArray[3], 3);
            spinningTable.interactionButtons[1].GetComponent<InteractableObject>().interactionType = InteractableObject.InteractionType.EmptyInteraction;

            Debug.Log("Table Opened");
            joint = GameObject.Find("IST_Joint").GetComponent<Transform>();
            ButtonUpDownManager tableButton = spinningTable.interactionButtons[0].GetComponent<ButtonUpDownManager>();
            tableButton.OnDown = OnDownTableButton(tableButton);
            tableButton.OnUp = OnUpTableButton(tableButton);

            spinningTableAudioSource = CreateAudioSourceOfTableSpin();
            spinningTableAudioSource.volume = 0;
            investigateUI.hideEvent = DestroyAudioSourceOfTableSpin();
            spinningTable.SetEventButtonInteraction(1, LeoSpeak_ShouldDisassembleTable());
        }
        else
        {
            investigateUI.DeepInvestigate(1);
            investigateUI.partQueue = new List<int>();
            investigateUI.partQueue.Add(1);
            this.spinningTable.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(@"Environment\InvestSTEnPanel");
            GameObject.Find("TableOnGround").GetComponent<SpriteRenderer>().enabled = true;


            if (!tableRippedOut)
            {
                Debug.Log("table ripping");
                this.spinningTable.interactionSound = null;
                this.spinningTable.interactionName = 5;
                this.spinningTable.interactionParams = new string[] { "InvestigatingSpinningTable" };
                tableRippedOut = true;
            }
        }

        spinningTable.SetEventInteractionButtonClick(11, (delegate { OnClickPasswordNumberAdd("0"); }));
        spinningTable.SetEventInteractionButtonClick(2, (delegate { OnClickPasswordNumberAdd("1"); }));
        spinningTable.SetEventInteractionButtonClick(3, (delegate { OnClickPasswordNumberAdd("2"); }));
        spinningTable.SetEventInteractionButtonClick(4, (delegate { OnClickPasswordNumberAdd("3"); }));
        spinningTable.SetEventInteractionButtonClick(5, (delegate { OnClickPasswordNumberAdd("4"); }));
        spinningTable.SetEventInteractionButtonClick(6, (delegate { OnClickPasswordNumberAdd("5"); }));
        spinningTable.SetEventInteractionButtonClick(7, (delegate { OnClickPasswordNumberAdd("6"); }));
        spinningTable.SetEventInteractionButtonClick(8, (delegate { OnClickPasswordNumberAdd("7"); }));
        spinningTable.SetEventInteractionButtonClick(9, (delegate { OnClickPasswordNumberAdd("8"); }));
        spinningTable.SetEventInteractionButtonClick(10, (delegate { OnClickPasswordNumberAdd("9"); }));

//        for(int i = 0; i < 10; i++)
//            spinningTable.SetEventInteractionButtonClick(i + 1, (delegate { OnClickPasswordNumberAdd(i.ToString()); }));
        spinningTable.SetEventInteractionButtonClick(12, (delegate { OnClickEnter(); }));
        spinningTable.SetEventInteractionButtonClick(13, (delegate { OnClickDelete(); }));
        passwordPanel = spinningTable.texts[0];
        yield return null;
    }
    private IEnumerator OnDownTableButton(ButtonUpDownManager tableButton) 
    {
        Debug.Log("Spinning");
        tableButtonHolding = true;
        StartCoroutine(HoldingTableButton());
        tableButton.OnDown = OnDownTableButton(tableButton);
        yield return null;
    }
    private IEnumerator OnUpTableButton(ButtonUpDownManager tableButton)
    {
        Debug.Log("Spinning_Cancel");
        tableButtonHolding = false;
        tableButton.OnUp = OnUpTableButton(tableButton);
        yield return null;
    }
    private IEnumerator HoldingTableButton() 
    {
        float firstAngle = joint.eulerAngles.z - GetAngle();

        while (tableButtonHolding) 
        {
            float f = joint.rotation.z;
            joint.rotation = Quaternion.Euler(new Vector3(0, 0, firstAngle + GetAngle()));
//            Debug.Log(Mathf.Abs(f - joint.rotation.z));
            if (Mathf.Abs(f - joint.rotation.z) > 0.002f)
                spinningTableAudioSource.volume = 1;
            else
                spinningTableAudioSource.volume = 0;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(ReleaseTableButton());
        yield return null;
    }
    private IEnumerator ReleaseTableButton() 
    {
        Vector3 rotateUnit;
        bool anglePositive = joint.eulerAngles.z < 180;
        Debug.Log("rotation" + joint.rotation.z);
        Debug.Log("rad2deg rotation" + joint.rotation.z * Mathf.Rad2Deg);
        Debug.Log("deg2rad rotation" + joint.rotation.z * Mathf.Deg2Rad);
        Debug.Log("euler angle" + joint.eulerAngles.z);

        spinningTableAudioSource.volume = 1;
        if (!anglePositive)
        {
            Debug.Log("Negative");
            rotateUnit = new Vector3(0, 0, 2f);
            while (joint.eulerAngles.z > 180)
            {
                joint.Rotate(rotateUnit);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.Log("Positive");
            rotateUnit = new Vector3(0, 0, -2);
            while (joint.eulerAngles.z < 180) 
            {
                joint.Rotate(rotateUnit);
                yield return new WaitForEndOfFrame();
            }
        }
        joint.rotation = Quaternion.Euler(0, 0, 0);
        spinningTableAudioSource.volume = 0;
        yield return null;
    }
    private IEnumerator LeoSpeak_ShouldDisassembleTable() 
    {
        investigateUI.HideInvestigateUI();

        int[] conversation = {21};
        int[] conversationCQ = {1};
        conversationInterface.StartConversation(conversation, conversationCQ);
        conversationInterface.onFinishConversation = ShowUIMessage_FindAToolForDisassebmle();

        this.spinningTable.interactionType = InteractableObject.InteractionType.None;
        crowbarNeeded = true;
        yield return null;
    }
    private IEnumerator ShowUIMessage_FindAToolForDisassebmle() 
    {
        if (!leo.inventory.ItemExists("Crowbar"))
        {
            uiMessenger.ShowMessage(uiMessenger.textArray[3], 3);
            spinningTable.selectEvent = ShowUIMessage_FindAToolForDisassebmle();
        }
        else 
        {
            this.spinningTable.interactionSound = Resources.Load<AudioClip>(@"Audio\Environment\TableRip");
            this.spinningTable.interactionName = 8;
            this.spinningTable.interactionType = InteractableObject.InteractionType.Investigate;
            this.spinningTable.interactionParams = new string[] { "InvestigatingSpinningTable", "1,4" };
            interactionManager.SelectInteractableObject(this.spinningTable.gameObject);
        }
        yield return null;
    }

    // SAFE
    private string inputPassword = "";
    private Text passwordPanel;

    private void OnClickPasswordNumberAdd(string number)
    {
        Debug.Log("Number: " + number);
        inputPassword += number;
        passwordPanel.text = inputPassword;
    }
    private void OnClickDelete() 
    {
        inputPassword = inputPassword.Remove(inputPassword.Length - 1);
        if (inputPassword == "")
            passwordPanel.text = texts[4];
        else
            passwordPanel.text = inputPassword;
    }
    private void OnClickEnter()
    {
        if (inputPassword == password)
            StartCoroutine(PasswordIsCorrect());
        else
            StartCoroutine(PasswordIsWrong());
    }

    private IEnumerator PasswordIsCorrect() 
    {
        passwordPanel.text = texts[5];
        yield return new WaitForSeconds(1);
        investigateUI.DeepInvestigate(2);
    }
    private IEnumerator PasswordIsWrong() 
    {
        passwordPanel.text = texts[5];
        yield return new WaitForSeconds(1);
        passwordPanel.text = inputPassword;
    }

    private float GetAngle() 
    {
        Vector3 touchPosition = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
      //  Debug.Log(touchPosition);
        Debug.Log(joint.eulerAngles.z);

        float x, y;
        float result;
        Vector2 a;
        a = joint.position - touchPosition;
        x = a.x; y = a.y;

        result = Mathf.Atan(x / y) * Mathf.Rad2Deg + 180;
        if (touchPosition.y < joint.position.y) result += 180;

        return result * -1;
    }
    Transform joint;
    bool crowbarNeeded = false;
    bool tableButtonHolding = false;
    AudioSource spinningTableAudioSource;

    private AudioSource CreateAudioSourceOfTableSpin() 
    {
        AudioSource audioSource = new GameObject().AddComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>(@"Audio\Environment\TableSpinningSound");
        audioSource.loop = true;
        audioSource.Play();
        audioSource.name = "SpinningTableAudioSource";
        return audioSource.GetComponent<AudioSource>();
    }
    private IEnumerator DestroyAudioSourceOfTableSpin() 
    {
        Destroy(spinningTableAudioSource.gameObject);
        spinningTableAudioSource = null;
        yield return null;
    }
    #endregion

    #region Maze Box
    // MAZE BOX
    Rigidbody2D ball;
    Vector2 tiltUnit;
    bool coverHolding = false;
    ButtonUpDownManager cover;
    Transform tfCover;
    InteractableObject coverOpener;
    bool firstMazeInv = true;
    bool leoSpokeThereIsAMaze = false;
    bool coverIsLocked = true;
    RectTransform[] coverEdges = new RectTransform[4];         // 0: down, 1: up, 2: right, 3: left

    public IEnumerator OnOpenMazeBox(InvestigatingObject mazeBox) 
    {
        ball = mazeBox.deactivatingObjects[0].gameObject.GetComponent<Rigidbody2D>();
        cover = mazeBox.interactionButtons[0].gameObject.GetComponent<ButtonUpDownManager>();
        coverOpener = mazeBox.interactionButtons[1].gameObject.GetComponent<InteractableObject>();

        tfCover = cover.gameObject.GetComponent<Transform>();
        cover.OnDown = OnDownCover();
        cover.OnUp = OnUpCover();
        coverOpener.sceneEvent = TryToOpenMazeBox();

        coverEdges[0] = mazeBox.deactivatingObjects[2].GetComponent<RectTransform>();
        coverEdges[1] = mazeBox.deactivatingObjects[3].GetComponent<RectTransform>();
        coverEdges[2] = mazeBox.deactivatingObjects[4].GetComponent<RectTransform>();
        coverEdges[3] = mazeBox.deactivatingObjects[5].GetComponent<RectTransform>();
        mazeBox.deactivatingObjects[6].GetComponent<_2DTriggerBox>().enterEvent = OnBallEnterPit();
        yield return null;
    }
    public IEnumerator TryToOpenMazeBox() 
    {
        if (coverIsLocked)
        {
            if (!leoSpokeThereIsAMaze)
            {
                conversationInterface.StartConversation(new int[] { 23, 24 }, new int[] { 1, 1 });
                conversationInterface.onFinishConversation = TryToOpenMazeBox();
                leoSpokeThereIsAMaze = true;
            }
            else
            {
                GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().DeepInvestigate(1);
                if (firstMazeInv)
                {
                    StartCoroutine(GuideMazeWithUIMessenger());
                    firstMazeInv = false;
                }
            }
            coverOpener.sceneEvent = TryToOpenMazeBox();
        }
        else
        {
            GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().DeepInvestigate(2);
        }
        coverOpener.sceneEvent = TryToOpenMazeBox();
        yield return null;
    }
    private IEnumerator GuideMazeWithUIMessenger() 
    {
        yield return new WaitForSeconds(1);
        uiMessenger.ShowMessage(uiMessenger.textArray[5], 3);
        yield return new WaitForSeconds(3.5f);
        uiMessenger.ShowMessage(uiMessenger.textArray[6], 3);
    }
    private IEnumerator OnBallEnterPit() 
    {
        coverIsLocked = false;
        GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().DeepInvestigate(2);
        investigateUI.partQueue.RemoveAt(1);
        Debug.Log("Onu gettim ben goyemmi hekeþe");
        yield return null;
    }

    public IEnumerator OnDownCover() 
    {
        coverHolding = true;
        StartCoroutine(TiltCover());
        cover.OnDown = OnDownCover();
        yield return null;
    }
    private IEnumerator TiltCover() 
    {
        Vector3 downPoint = Camera.main.ScreenPointToRay(Input.mousePosition).origin;

        while (coverHolding) 
        {
            tiltUnit = Camera.main.ScreenPointToRay(Input.mousePosition).origin - downPoint;
            tiltUnit = RestrictTiltUnit(tiltUnit, new Vector2(2, 2));

            if (Vector2.Distance(ball.velocity, new Vector2(0, 0)) < 5)
                ball.AddForce(tiltUnit * new Vector2(3, 3));
            Vector3 localPosition;
            localPosition = ball.gameObject.GetComponent<Transform>().localPosition;
            ball.gameObject.GetComponent<Transform>().localPosition = new Vector3(localPosition.x, localPosition.y, 0);

            tiltUnit = new Vector3(tiltUnit.y, tiltUnit.x, 0);

            tfCover.rotation = Quaternion.Euler(tiltUnit * new Vector3(10, -10, 1));
     //       if (tiltUnit.x < 0) 
     //       {
     //           Debug.Log("x negative");
     //           coverEdges[1].sizeDelta = new Vector2(0, tiltUnit.x * -2);
     //       }
            //Debug.Log(tiltUnit);
            yield return new WaitForEndOfFrame();
        }
    }
    Vector2 RestrictTiltUnit(Vector2 tiltUnit, Vector2 limit) 
    {
        if (tiltUnit.x > limit.x)
            tiltUnit = new Vector2(limit.x, tiltUnit.y);
        if (tiltUnit.y > limit.y)
            tiltUnit = new Vector2(tiltUnit.x, limit.y);
        if (tiltUnit.x < -limit.x)
            tiltUnit = new Vector2(-limit.x, tiltUnit.y);
        if (tiltUnit.y < -limit.y)
            tiltUnit = new Vector2(tiltUnit.x, -limit.y);

        return tiltUnit;
    }
    private IEnumerator ReleaseCover() 
    {
        Vector2 center = new Vector2(0, 0);
        cover.interactable = false;
        while (Vector2.Distance(tiltUnit, center) > 0.5f)
        {
            tiltUnit = Vector2.MoveTowards(tiltUnit, center, 0.3f);
            tfCover.rotation = Quaternion.Euler(tiltUnit * new Vector3(10, -10, 1));
            yield return new WaitForEndOfFrame();
        }
        cover.interactable = true;
        tfCover.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
    public IEnumerator OnUpCover()
    {
        coverHolding = false;
        StartCoroutine(ReleaseCover());
        cover.OnUp = OnUpCover();
        yield return null;
    }
    #endregion

    #region U0 Cabinet_0
    // U0 CABINET_0
    public InvestigatingObject cabinet_0;

    public IEnumerator OnOpenCabinet_0(InvestigatingObject cabinet_0)
    {
        this.cabinet_0 = cabinet_0;
        cabinet_0.interactionButtons[0].gameObject.GetComponent<InteractableObject>().sceneEvent = InvestigateMazeBox();
        Debug.Log("Cabinet");
        yield return null;
    }
    public IEnumerator InvestigateMazeBox() 
    {
        cabinet_0.deactivatingObjects[0].SetActive(false);
        investigateUI.HideInvestigateUI();
//        investigateUI.partQueue.Add(0);
        investigateUI.ShowInvestigateUI("Inves_MazeBox");
        cabinet_0.deactivatingObjects[0].SetActive(false);
        leo.inventory.AddItem("MazeBox", 1, 8);
        investigateUI.hideEvent = LeoSpeak_ITookTheBox();
        Debug.Log("InvestigateBox");
        yield return null;
    }
    public IEnumerator LeoSpeak_ITookTheBox() 
    {
        conversationInterface.StartConversation(new int[] {22}, new int[] {1});
        conversationInterface.onFinishConversation = ShowMessage_BoxIsInTheInventory();
        yield return null;
    }
    public IEnumerator ShowMessage_BoxIsInTheInventory() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[4], 3);
        yield return null;
    }
    #endregion

    #region U0 Cabinet_1
    public IEnumerator OnOpenCabinet_1(InvestigatingObject cabinet_1) 
    {
        Debug.Log("Open Cabinet_1");
        if (!leo.inventory.ItemExists("CabinetKey"))
        {
            Debug.Log("CabinetKey Does Not Exist");
            investigateUI.hideEvent = CloseLockedCabinet();
            uiMessenger.ShowMessage(uiMessenger.textArray[8]);
        }
        else
        {            
            investigateUI.DeepInvestigate(1);
            investigateUI.partQueue = new List<int>();
            investigateUI.partQueue.Add(1);
            Debug.Log("CabinetKey Exists");
        }

        lockedCabinet.gameObject.GetComponent<InteractableObject>().sceneEvent = InteractLockedCabinet();
        yield return null;
    }
    public IEnumerator CloseLockedCabinet() { uiMessenger.HideMessage(); yield return null; }
    #endregion

    #region Fence Door
    // FENCE DOOR
    public Animation[] numberTexts = new Animation[4];
    public Button[] upDownNumberBox = new Button[8];
    int[] currentPasswordFigures = { 0, 0, 0, 0 };
    Text[,] numberBoxTexts = new Text[4, 5];
    string password = "4820";

    public IEnumerator OnOpenFenceDoor(InvestigatingObject fenceDoor) 
    {
        for (int i = 0; i < 4; i++ )
            numberTexts[i] = fenceDoor.deactivatingObjects[i].GetComponent<Animation>();

        for (int i = 0; i < 8; i++)
            upDownNumberBox[i] = fenceDoor.interactionButtons[i];

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 5; j++)
                numberBoxTexts[i, j] = fenceDoor.texts[(i * 5) + j];

        upDownNumberBox[0].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickUpNumBox_0();
        upDownNumberBox[1].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickDownNumBox_0();
        upDownNumberBox[2].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickUpNumBox_1();
        upDownNumberBox[3].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickDownNumBox_1();
        upDownNumberBox[4].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickUpNumBox_2();
        upDownNumberBox[5].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickDownNumBox_2();
        upDownNumberBox[6].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickUpNumBox_3();
        upDownNumberBox[7].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickDownNumBox_3();

        yield return null;
    }

    public IEnumerator OnClickUpNumBox_0()
    {
        StartCoroutine(OnClickNumBoxArrow(true, 0));
        upDownNumberBox[0].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickUpNumBox_0();
        Debug.Log("Click Up NumBox_0");
        yield return null;
    }
    public IEnumerator OnClickDownNumBox_0()
    {
        StartCoroutine(OnClickNumBoxArrow(false, 0));
        upDownNumberBox[1].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickDownNumBox_0();
        yield return null;
    }

    public IEnumerator OnClickUpNumBox_1()
    {
        StartCoroutine(OnClickNumBoxArrow(true, 1));
        upDownNumberBox[2].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickUpNumBox_1();
        yield return null;
    }
    public IEnumerator OnClickDownNumBox_1()
    {
        StartCoroutine(OnClickNumBoxArrow(false, 1));
        upDownNumberBox[3].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickDownNumBox_1();
        yield return null;
    }

    public IEnumerator OnClickUpNumBox_2()
    {
        StartCoroutine(OnClickNumBoxArrow(true, 2));
        upDownNumberBox[4].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickUpNumBox_2();
        yield return null;
    }
    public IEnumerator OnClickDownNumBox_2()
    {
        StartCoroutine(OnClickNumBoxArrow(false, 2));
        upDownNumberBox[5].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickDownNumBox_2();
        yield return null;
    }

    public IEnumerator OnClickUpNumBox_3()
    {
        StartCoroutine(OnClickNumBoxArrow(true, 3));
        upDownNumberBox[6].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickUpNumBox_3();
        yield return null;
    }
    public IEnumerator OnClickDownNumBox_3()
    {
        StartCoroutine(OnClickNumBoxArrow(false, 3));
        upDownNumberBox[7].gameObject.GetComponent<InteractableObject>().selectEvent = OnClickDownNumBox_3();
        yield return null;
    }
    public IEnumerator OnClickNumBoxArrow(bool up, int numberIndex)
    {
        int buttonIndex = numberIndex * 2; if (!up) buttonIndex++;
        upDownNumberBox[buttonIndex].interactable = false;
        if (up)
        {
            currentPasswordFigures[numberIndex]--;
            if (currentPasswordFigures[numberIndex] == -1)
                currentPasswordFigures[numberIndex] = 9;

            numberTexts[numberIndex].Play("Inves_EncrLock_NumberTextUp");
        }
        else
        {
            currentPasswordFigures[numberIndex]++;
            if (currentPasswordFigures[numberIndex] == 10)
                currentPasswordFigures[numberIndex] = 0;
            numberTexts[numberIndex].Play("Inves_EncrLock_NumberTextDown");
        }

        yield return new WaitForSeconds(0.2f);

        float[] numBoxXLocs = { -39, -12.9f, 12.9f, 39 };
        numberTexts[numberIndex].gameObject.GetComponent<RectTransform>().localPosition = new Vector2(numBoxXLocs[numberIndex], 0);
        numberBoxTexts[numberIndex, 0].text = (currentPasswordFigures[numberIndex] - 2).ToString();
        numberBoxTexts[numberIndex, 1].text = (currentPasswordFigures[numberIndex] - 1).ToString();
        numberBoxTexts[numberIndex, 2].text = (currentPasswordFigures[numberIndex]).ToString();
        numberBoxTexts[numberIndex, 3].text = (currentPasswordFigures[numberIndex] + 1).ToString();
        numberBoxTexts[numberIndex, 4].text = (currentPasswordFigures[numberIndex] + 2).ToString();
        Debug.Log(currentPasswordFigures[numberIndex]);

        if (currentPasswordFigures[numberIndex] == 0) 
        {
            numberBoxTexts[numberIndex, 0].text = "8";
            numberBoxTexts[numberIndex, 1].text = "9";
        }
        else if (currentPasswordFigures[numberIndex] == 1) numberBoxTexts[numberIndex, 0].text = "9";
        else if (currentPasswordFigures[numberIndex] == 9) numberBoxTexts[numberIndex, 3].text = "0";
        else if (currentPasswordFigures[numberIndex] == 8) numberBoxTexts[numberIndex, 4].text = "0";

        upDownNumberBox[buttonIndex].interactable = true;
        StartCoroutine(CheckPassword());
    }
    private IEnumerator CheckPassword() 
    {        
        string password = currentPasswordFigures[0].ToString() + currentPasswordFigures[1].ToString() + currentPasswordFigures[2].ToString() + currentPasswordFigures[3].ToString();
        if (this.password == password)
        {
            Debug.Log("Þifre doðru emmolu");
            yield return new WaitForSeconds(1);

            fenceDoor.SetActive(false);
            investigateUI.HideInvestigateUI();
            backFloorColliders[1].enabled = true;
            if (!attenuateFenceGuideShowed)
            {
                LocalizedTextMesh.SpawnLocalizedText(new int[] { 0, 1 }, new Vector2(307.2f, -32.5f), 0.5f);
            }
        }
    }
    #endregion
    #endregion

    #region Special Item Usage
    public void UseDynamite() 
    {
        Debug.Log("Hayuvleeeh");
    }
    public void UseMazeBox() 
    {
        GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().ShowInvestigateUI("Inves_MazeBox");
    }
    #endregion
}
