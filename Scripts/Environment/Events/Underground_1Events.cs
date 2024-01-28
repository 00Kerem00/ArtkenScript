using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Underground_1Events : MonoBehaviour
{
    public Animation elevator_0, elevator_1, elevator_2, alarm;
    public ConversationInterface conversationInterface;
    public InteractionManager interactionManager;
    public Panorama panorama_1, panorama_6;
    public UIMessenger uiMessenger;
    public Guard guard_0;
    public Leo leo;
    public GameObject p1Floor;
    public Collider2D p7DownStairsCol0;
    public InteractableObject chargingMachine, closedDoor, leosAntique;
    public Destroyable boxHanger, box;

    #region Spesific Events
    public IEnumerator OnSelectElevator_0() 
    {
        elevator_0.Play("ElevatorOpen");
        elevator_0.gameObject.GetComponent<InteractableObject>().selectEvent = OnSelectElevator_0();
        yield return null;
    }
    public IEnumerator OnUnselectElevator_0() 
    {
        elevator_0.Play("ElevatorClose");
        elevator_0.gameObject.GetComponent<InteractableObject>().unselectEvent = OnUnselectElevator_0();
        yield return null;
    }

    public IEnumerator OnSelectElevator_1()
    {
        elevator_1.Play("ElevatorOpen");
        elevator_1.gameObject.GetComponent<InteractableObject>().selectEvent = OnSelectElevator_1();
        yield return null;
    }
    public IEnumerator OnUnselectElevator_1()
    {
        elevator_1.Play("ElevatorClose");
        elevator_1.gameObject.GetComponent<InteractableObject>().unselectEvent = OnUnselectElevator_1();
        yield return null;
    }

    public IEnumerator OnSelectElevator_2()
    {
        elevator_2.Play("ElevatorOpen");
        elevator_2.gameObject.GetComponent<InteractableObject>().selectEvent = OnSelectElevator_2();
        yield return null;
    }
    public IEnumerator OnUnselectElevator_2()
    {
        elevator_2.Play("ElevatorClose");
        elevator_2.gameObject.GetComponent<InteractableObject>().unselectEvent = OnUnselectElevator_2();
        yield return null;
    }

    public IEnumerator OnGuard_0SeeLeo() 
    {
        yield return new WaitForSeconds(0.2f);
        GameObject fightPanel = GameObject.Find("FightPanel");
        fightPanel.SetActive(false);
        FightPanel.RemoveEnemy("Guard_0", 0);

        guard_0.state = Guard.State.Idle;
        guard_0.anim.SetBool("Running", false);
        guard_0.enabled = false;

        int[] conversation = { 8, 9, 10 };
        int[] conversationCQ = { 3, 1, 3 };

        conversationInterface.SetSecondCharacterImage("Guard_2");
        conversationInterface.StartConversation(conversation, conversationCQ);
        conversationInterface.onFinishConversation = OnFinishConversationBetweenG_And_L();

        yield return new WaitForSeconds(1);
        fightPanel.SetActive(true);
    }
    private IEnumerator OnFinishConversationBetweenG_And_L() 
    {
        guard_0.enabled = true;
        yield return null;
    }
    private IEnumerator OnGuard_0Die() 
    {
        Collectable alarmControl = Collectable.SpawnCollectable(guard_0.tf.position, new Leo.Inventory.Item("AlarmControl", 1, 0, 10));

        alarmControl.GetComponents<BoxCollider2D>()[1].enabled = false;
        yield return new WaitForSeconds(0.5f);
        alarmControl.GetComponents<BoxCollider2D>()[1].enabled = true;
    }

    public IEnumerator OnEnterPanorama_1() 
    {
        p1Floor.SetActive(false); p7DownStairsCol0.enabled = false; yield return new WaitForSeconds(0.5f); p1Floor.SetActive(true);
    }
    public IEnumerator OnEnterPanorama_6() 
    {
        panorama_1.panoramaEnterEvent = OnEnterPanorama_1();
        panorama_6.panoramaEnterEvent = OnEnterPanorama_6();
        yield return null;
    }

    public IEnumerator OnSelectChargingMachine() 
    {
        if (!leo.inventory.ItemExists("AlarmControl") || leo.inventory.GetItem("AlarmControl").availability == 100)
        {
            interactionManager.buttonText.text = interactionManager.interactableNames[1];
            interactionManager.interactButton.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>()[0].interactable = false;
        }
        else
        {
            chargingMachine.sceneEvent = InteractChargingMachine();
            //interactionManager.interactButton.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>()[0].interactable = true;
        }

        chargingMachine.selectEvent = OnSelectChargingMachine();

        yield return null;
    }
    public IEnumerator InteractChargingMachine() 
    {
        leo.inventory.GetItem("AlarmControl").availability = 100;
        uiMessenger.ShowMessage(uiMessenger.textArray[3], 2);
        yield return null;
    }

    public IEnumerator OnSelectClosedDoor() 
    {
        if (!leo.inventory.ItemExists("Key"))
        {
            interactionManager.buttonText.text = interactionManager.interactableNames[2] + " (" + interactionManager.interactionNames[3] + ")";
            interactionManager.interactButton.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>()[0].interactable = false;
        }
        else 
        {
            closedDoor.sceneEvent = InteractClosedDoor();
            interactionManager.interactButton.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>()[0].interactable = true;
        }
        closedDoor.selectEvent = OnSelectClosedDoor();
        yield return null;
    }
    public IEnumerator InteractClosedDoor() 
    {
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_Closing");
        yield return new WaitForSeconds(0.5f);
        Destroy(closedDoor.gameObject);
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_OnChangedPanorama");
    }

    public IEnumerator OnDestroyBoxHanger() 
    {
        box.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(1);
        box.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        box.destroyable = true;
        yield return null;
    }
    public IEnumerator OnDestroyBox() 
    {
        Collectable.SpawnCollectable(box.gameObject.GetComponent<Transform>().position, new Leo.Inventory.Item("Key", 1, 100, 3));
        yield return null;
    }

    public IEnumerator OnGetLeosAntique() 
    {
        conversationInterface.StartConversation(new int[] { 16 }, new int[] { 1 });
        conversationInterface.onFinishConversation = ShowMessage_FindAWayToEscapeFromThisPlace();
        yield return null;
    }
    public IEnumerator ShowMessage_FindAWayToEscapeFromThisPlace() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[8], 3);
        yield return null;
    }
	#endregion

    #region TriggerableEvents
    public void StartUnderground_1Event(int eventNumber) 
    {
        switch (eventNumber) 
        {
            case 0: Underground_1Event_0(); break;
            case 1: Underground_1Event_1(); break;
            case 2: Underground_1Event_2(); break;
        }
    }
    public void StartUnderground_1RewindEvent(int eventNumber) 
    {
        switch (eventNumber) 
        {
            case 1: Underground_1Event_1Rewind(); break;
            case 2: Underground_1Event_2Rewind(); break;
        }
    }

    private void Underground_1Event_0() 
    {
        conversationInterface.StartConversation(new int[] { 4, 5 }, new int[] { 1, 1 });
        conversationInterface.onFinishConversation = ShowMessage_FindAWayToSabogateTheAlarm();
        Destroy(GameObject.Find("EventTrigger_0"));
    }
    private IEnumerator ShowMessage_FindAWayToSabogateTheAlarm() 
    {
        uiMessenger.ShowMessage(uiMessenger.textArray[0], 3);
        yield return null;
    }

    private void Underground_1Event_1() { Debug.Log("alert area enter"); leoIsInAlertArea = true; }
    private void Underground_1Event_1Rewind() { Debug.Log("alert area exit"); leoIsInAlertArea = false; }

    private void Underground_1Event_2() { uiMessenger.ShowMessage(uiMessenger.textArray[4]); }
    private void Underground_1Event_2Rewind() { uiMessenger.HideMessage(); }
    #endregion

    #region Special Item Usage
    private bool leoIsInAlertArea = false;
    private bool alarmSabotaged = false;
    public void UseAlarmControl() 
    {
        if (!alarmSabotaged)
        {
            Debug.Log("Alarm Control Availability: " + leo.inventory.GetItem("AlarmControl").availability + ", Alert Area: " + leoIsInAlertArea);
            if (leo.inventory.GetItem("AlarmControl").availability == 0)
                uiMessenger.ShowMessage(uiMessenger.textArray[1], 2.5f);
            else if (!leoIsInAlertArea)
                uiMessenger.ShowMessage(uiMessenger.textArray[2], 2.5f);
            else
                StartCoroutine(SabotageAlarm());
        }
    }

    private IEnumerator SabotageAlarm() 
    {
        alarmSabotaged = true;
        Debug.Log("Helal Len Alarmý Kapa Gali");
        leo.SeparateLeftHand();
        leo.leftHandSolverTarget_Separate.gameObject.GetComponent<Animation>().Play("LeoLeftHandUseAlarmControl");
        yield return new WaitForSeconds(1);
        StartCoroutine(leo.moveTowardsToJoinedLeftHand());
        alarm.wrapMode = WrapMode.Default;
        alarm.Play("U1AlarmLightTurnOff");
        Destroy(GameObject.Find("AlarmWall"));
    }
    #endregion

    private void Start() 
    {
        elevator_0.gameObject.GetComponent<InteractableObject>().selectEvent = OnSelectElevator_0();
        elevator_0.gameObject.GetComponent<InteractableObject>().unselectEvent = OnUnselectElevator_0();

        elevator_1.gameObject.GetComponent<InteractableObject>().selectEvent = OnSelectElevator_1();
        elevator_1.gameObject.GetComponent<InteractableObject>().unselectEvent = OnUnselectElevator_1();

        elevator_2.gameObject.GetComponent<InteractableObject>().selectEvent = OnSelectElevator_2();
        elevator_2.gameObject.GetComponent<InteractableObject>().unselectEvent = OnUnselectElevator_2();

        guard_0.onSetToChase = OnGuard_0SeeLeo();
        guard_0.onDie = OnGuard_0Die();
        panorama_6.panoramaEnterEvent = OnEnterPanorama_6();
        chargingMachine.selectEvent = OnSelectChargingMachine();
        closedDoor.selectEvent = OnSelectClosedDoor();
        boxHanger.Event = OnDestroyBoxHanger();
        box.Event = OnDestroyBox();
        leosAntique.sceneEvent = OnGetLeosAntique();


        leo.inventory.AddItem("Arrow", 1, 100, 1);
    }
}
