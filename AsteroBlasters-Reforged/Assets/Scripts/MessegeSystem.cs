using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MessegeSystem : MonoBehaviour
{
    public static MessegeSystem instance;


    [SerializeField]
    Text messegeText;
    [SerializeField]
    Animator animator;

    List<Message> lowMessages;
    List<Message> mediumMessages;
    List<Message> highMessages;
    bool displayingMessage = false;

    public enum MessagePriority
    {
        Low,
        Medium,
        High
    }

    private void Awake()
    {
        // Checking if another instance of this class don't exist yet and deleting itself if that is the case
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!displayingMessage && (lowMessages.Count > 0 || mediumMessages.Count > 0 || highMessages.Count > 0))
        {
            Message messageToDisplay = GetTopProrityMessage();
            DisplayMessege(messageToDisplay);
        }
    }

    public void AddMessage(string value, int displayTime, MessagePriority priority)
    {
        Message newMessage = new Message(value, displayTime);

        switch (priority)
        {
            case MessagePriority.Low:
                lowMessages.Add(newMessage);
                break;
            case MessagePriority.Medium:
                mediumMessages.Add(newMessage);
                break;
            case MessagePriority.High:
                highMessages.Add(newMessage);
                break;
            default: 
                break;
        }
    }

    async void DisplayMessege(Message message)
    {
        messegeText.text = message.messageValue;
        animator.SetTrigger("Show");
        animator.SetBool("NoMessage", false);

        await Task.Delay(message.miliseconds);

        animator.SetTrigger("Hide");
        animator.SetBool("NoMessage", true);
    }

    Message GetTopProrityMessage()
    {
        Message resultMessage;

        if (highMessages.Count > 0)
        {
            resultMessage = highMessages[0];
            highMessages.RemoveAt(0);
        }
        else if (mediumMessages.Count > 0)
        {
            resultMessage = mediumMessages[0];
            mediumMessages.RemoveAt(0);
        }
        else
        {
            resultMessage = lowMessages[0];
            lowMessages.RemoveAt(0);
        }

        return resultMessage;
    }
}
