using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton responsible for managing text messeges displayed on user screen
/// </summary>
public class MessageSystem : MonoBehaviour
{
    public static MessageSystem instance;


    [SerializeField]
    Text messegeText;
    [SerializeField]
    Animator animator;

    List<Message> lowMessages;
    List<Message> mediumMessages;
    List<Message> highMessages;
    bool displayingMessage = false;

    /// <summary>
    /// Enum representing possible priority levels for a message
    /// </summary>
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

        lowMessages = new List<Message>();
        mediumMessages = new List<Message>();
        highMessages = new List<Message>();
    }

    private void FixedUpdate()
    {
        // Checking if message should be displayed
        if (!displayingMessage && (lowMessages.Count > 0 || mediumMessages.Count > 0 || highMessages.Count > 0))
        {
            Message messageToDisplay = GetTopProrityMessage();
            DisplayMessege(messageToDisplay);
        }
    }

    /// <summary>
    /// Method creating new message and assigning it to proper list, basing on its priority
    /// </summary>
    /// <param name="value">Text value of the message</param>
    /// <param name="displayTime">Time in which the message will be displayed</param>
    /// <param name="priority">Level of priority of the message</param>
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

    /// <summary>
    /// Method activating the animations and displaying the text for set amount of time
    /// </summary>
    /// <param name="message">Message object, containing the data of the message</param>
    async void DisplayMessege(Message message)
    {
        messegeText.text = message.messageValue;
        animator.SetTrigger("Show");
        animator.SetBool("NoMessage", false);

        await Task.Delay(message.miliseconds);

        animator.SetTrigger("Hide");
        animator.SetBool("NoMessage", true);
    }

    /// <summary>
    /// Method choosing the top priority message, which is supposed to be displayed
    /// </summary>
    /// <returns>Top priority message</returns>
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
