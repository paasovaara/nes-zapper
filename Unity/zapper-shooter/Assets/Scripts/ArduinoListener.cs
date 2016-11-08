using UnityEngine;
using System.Collections;
using System.IO.Ports;

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
        }
        else {
            Debug.LogError("Failed to open serial port " + comPort);
        }

	}
	
	// Update is called once per frame
	void Update () {
        if (m_serial.IsOpen) {
            Debug.Log("Reading serial port");
            string input = m_serial.ReadLine();//by default this blocks, so put this to separate thread, not to main loop
        }

	}
}
