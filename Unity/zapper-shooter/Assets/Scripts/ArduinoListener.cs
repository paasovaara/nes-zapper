using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

/**
 * For System.IO.Ports namespace you need to enable .Net 2.0 instead of .Net 2.0 subset (which is default).
 * 
 * You can find this setting from "Edit > project settings > player"
 * */

public enum ComPort {
    COM3,
    COM4
}

/*
 * Use message handler for async callbacks, or clearQueue for a thread safe way to get all pending messages (f.ex from main loop)
 */
public interface IArduinoMessageHandler {
    void messageReceived(ArduinoMessage msg);
}

public class ArduinoListener : MonoBehaviour {
    SerialPort m_serial = null;

    [SerializeField]
    private ComPort m_comPort = ComPort.COM3;

    private List<IArduinoMessageHandler> m_messageHandlers = new List<IArduinoMessageHandler>();

    private List<ArduinoMessage> m_queue = new List<ArduinoMessage>();

    private string toString(ComPort com) {
        switch (com) {
        case ComPort.COM3: return "COM3";
        case ComPort.COM4: return "COM4";
        }
        return "unknown";
    }

    public void addMessageHandler(IArduinoMessageHandler handler) {
        //TODO add possibility to remove a handler
        m_messageHandlers.Add(handler);
    }

	// Use this for initialization
	void Start () {
        string comPort = toString(m_comPort);
        m_serial = new SerialPort(comPort, 9600);

        //m_serial.NewLine //set new line char
        m_serial.Open();
        if (m_serial.IsOpen) {
            Debug.Log("Serial port opened " + comPort);

            Thread t = new Thread(new ThreadStart(serialLoop));
            t.Start();
        }
        else {
            Debug.LogError("Failed to open serial port " + comPort);
        }

	}

    public void OnDestroy() {
        //clean all resources
        Debug.Log("closing serial port");
        //Closing the port will cause exception and terminate the read thread.
        m_serial.Close();
    }
	
    public void serialLoop() {
        try {
            Debug.Log("Starting serial loop");
            while (m_serial.IsOpen) {
                //readline blocks until EOL or until IOException
                string input = m_serial.ReadLine();
                handleMessage(input);
            }
        }
        catch(System.Exception ex) {
            Debug.Log("Serial thread aborted: " + ex.StackTrace);        
        }
        Debug.Log("Stopping serial loop");

    }

    public List<ArduinoMessage> getAndClearQueue() {
        List<ArduinoMessage> msgs = getQueue();
        m_queue.Clear();
        return msgs;
    }

    public List<ArduinoMessage> getQueue() {
        //create deep copy of list. maybe we could also copy the events?
        List<ArduinoMessage> msgs = new List<ArduinoMessage>(m_queue);
        return msgs;
    }

    private void handleMessage(string readLine) {
        Debug.Log("Read message: " + readLine);
        ArduinoMessage msg = new ArduinoMessage(readLine);
        m_queue.Add(msg);
        foreach(IArduinoMessageHandler handler in m_messageHandlers) {
            if (handler != null)
                handler.messageReceived(msg);
        }
    }
}
