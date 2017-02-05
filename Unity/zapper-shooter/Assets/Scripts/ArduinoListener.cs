using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.IO;

/**
 * For System.IO.Ports namespace you need to enable .Net 2.0 instead of .Net 2.0 subset (which is default).
 * 
 * You can find this setting from "Edit > project settings > player"
 * */

public enum ArduinoStatus {
    UNINITIALIZED,
    INITIALIZED_AND_RUNNING,
    INIT_FAILED
}

public enum ComPort {
    COM3,
    COM4
}

/*
 * Use message handler for async callbacks, or clearQueue for a thread safe way to get all pending messages (f.ex from main loop)
 */
public interface IArduinoMessageHandler {
    void messageReceived(ArduinoMessage msg);
    void statusChanged(ArduinoStatus newStatus);
}

public class ArduinoListener : MonoBehaviour {
    SerialPort m_serial = null;
    ArduinoStatus m_status = ArduinoStatus.UNINITIALIZED;

    [SerializeField]
    private ComPort m_comPort = ComPort.COM3;

    private List<IArduinoMessageHandler> m_messageHandlers = new List<IArduinoMessageHandler>();

    private List<ArduinoMessage> m_queue = new List<ArduinoMessage>();
    private Mutex m_mutex = new Mutex();

    private string toString(ComPort com) {
        switch (com) {
        case ComPort.COM3: return "COM3";
        case ComPort.COM4: return "COM4";
        }
        return "unknown";
    }

    ArduinoStatus Status {
        get { return m_status; }
    }

    public void addMessageHandler(IArduinoMessageHandler handler) {
        //TODO add possibility to remove a handler
        m_messageHandlers.Add(handler);
    }

	// Use this for initialization
	void Initialize () {
        string comPort = toString(m_comPort);

        //m_serial.NewLine //set new line char        
        try {
            m_serial = new SerialPort(comPort, 115200);
            m_serial.Open();
            if (m_serial.IsOpen) {
                Debug.Log("Serial port opened " + comPort);
                m_status = ArduinoStatus.INITIALIZED_AND_RUNNING;

                Thread t = new Thread(new ThreadStart(serialLoop));
                t.Start();
            }
            else {
                m_status = ArduinoStatus.INIT_FAILED;
            }
        }
        catch (IOException ioe) {
            m_status = ArduinoStatus.INIT_FAILED;
            Debug.LogError(ioe.StackTrace);
        }

        if (m_status != ArduinoStatus.INITIALIZED_AND_RUNNING) {
            Debug.LogError("Failed to initialize serial port " + comPort);
        }
        broadcastStatus(m_status);
    }

    void Update() {
        //Use update loop for init instead of Start(), so we don't have a race
        //condition if some messageHandlers register themselves in update.
        //Of course clients can always ask for the status also themselves.
        if (m_status == ArduinoStatus.UNINITIALIZED) {
            Initialize();
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
        m_mutex.WaitOne();
        m_queue.Clear();
        m_mutex.ReleaseMutex();

        return msgs;
    }

    public List<ArduinoMessage> getQueue() {
        //create deep copy of list. maybe we should also copy the events?
        m_mutex.WaitOne();
        List<ArduinoMessage> msgs = new List<ArduinoMessage>(m_queue);
        m_mutex.ReleaseMutex();

        return msgs;
    }

    private void handleMessage(string readLine) {
        //Debug.Log("Read message: " + readLine);
        ArduinoMessage msg = new ArduinoMessage(readLine);
        m_mutex.WaitOne();
        m_queue.Add(msg);
        m_mutex.ReleaseMutex();

        foreach(IArduinoMessageHandler handler in m_messageHandlers) {
            if (handler != null)
                handler.messageReceived(msg);
        }
    }

    private void broadcastStatus(ArduinoStatus status) {
        foreach (IArduinoMessageHandler handler in m_messageHandlers) {
            if (handler != null)
                handler.statusChanged(status);
        }
    }
}
