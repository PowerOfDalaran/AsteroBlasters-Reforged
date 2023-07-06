/// <summary>
/// Class used to contain messeges for <c>MessageSystem</c>
/// </summary>
public class Message
{
    public string messageValue;
    public int miliseconds;

    public Message(string value, int displayTime) 
    { 
        messageValue = value;
        miliseconds = displayTime;
    }
}
