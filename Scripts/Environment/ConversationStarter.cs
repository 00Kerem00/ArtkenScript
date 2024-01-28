using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationStarter : MonoBehaviour
{
    public int[] conversation;
    public int[] conversationCharacterQueue;
    public ConversationInterface conversationInterface;
    public IEnumerator sceneEvent;
    public IEnumerator onFinishConversation;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo")
        {
            if (onFinishConversation != null)
                conversationInterface.onFinishConversation = onFinishConversation;

            if (conversationCharacterQueue.Length == 0)
            {
                conversationCharacterQueue = new int[conversation.Length];
                for (int i = 0; i < conversation.Length; i++)
                    conversationCharacterQueue[i] = 1;
            }

            conversationInterface.StartConversation(conversation, conversationCharacterQueue);
            if (sceneEvent != null)
                StartCoroutine(sceneEvent);
            Destroy(gameObject);
        }
    }
}
