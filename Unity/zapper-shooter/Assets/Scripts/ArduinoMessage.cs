using System;

public class ArduinoMessage
{
    private string m_msg;

    /**
     *  Currently the message is just a string but nothing stops us creating a more detailed class later, either binary or string-form
     */
    public string Message {
        get { return m_msg; }
        set { m_msg = value; }
    }
    //TODO add timestamp

    public ArduinoMessage (String msg)
    {
        m_msg = msg;
    }

    public override string ToString() {
        return Message;
    }
}
