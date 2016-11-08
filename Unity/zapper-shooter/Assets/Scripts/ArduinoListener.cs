using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

/**
 * For System.IO.Ports namespace you need to enable .Net 2.0 instead of .Net 2.0 subset (which is default).
 * 
 * You can find this setting from "Edit > project settings > player"
 * */

public enum ComPort {
    COM3,
    COM4
}

public class ArduinoListener : MonoBehaviour {
    SerialPort m_serial = null;

    [SerializeField]
    private ComPort m_comPort;

    public string toString(ComPort com) {
        switch (com) {
        case ComPort.COM3: return "COM3";
        case ComPort.COM4: return "COM4";
        }
        return "unknown";
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

    private void handleMessage(string msg) {
        Debug.Log("Read message: " + msg);
    }
}
