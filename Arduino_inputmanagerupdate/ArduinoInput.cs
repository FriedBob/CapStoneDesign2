using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using UnityEngine.Events;

public class ArduinoInput : MonoBehaviour
{
    SerialPort Arduino = new SerialPort();
    [SerializeField] private Text ButtonText;
    [SerializeField] private InputField PortNameField;
    bool isReceive = false;
    string receivedata;
    [SerializeField] public UnityEvent JumpEvent;
    [SerializeField] public UnityEvent MoveEvent;
    [SerializeField] public UnityEvent StopMoveEvent;
    [SerializeField] public UnityEvent StopJumpEvent;

    void ArduinoConnect(){
        if(PortNameField.text == string.Empty){
            Debug.Log("Port Name Empty");
            return;
        }

        Arduino.PortName = PortNameField.text;
        Arduino.BaudRate = 9600;
        try{
            Arduino.Open();
        }
        catch{
            Debug.Log("연결실패");
            return;
        }
        ButtonText.text = "DISCONNECT";
    }

    void ArduinoDisconnect(){
        if(Arduino.IsOpen) Arduino.Close();
        ButtonText.text = "CONNECT";
    }

    public void OnButtonClick(){
        if(!Arduino.IsOpen){
            ArduinoConnect();
            
        }else{
            ArduinoDisconnect();
        }
    }

    void ArduinoDataReceive(object sender, SerialDataReceivedEventArgs e){
      
    }

    
    void Start()
    {
        StartCoroutine(updating());
        Arduino.DataReceived += ArduinoDataReceive;
    }

    bool prevmove;
    bool prevjump;

    Queue<char> i_queue = new Queue<char>();
    Queue<string> s_queue = new Queue<string>();

    IEnumerator updating()
    {
        while(true){
        yield return new WaitForSeconds(0.01f);
        if(Arduino.IsOpen){
            Arduino.Write("T");
        }
        }
    }

    void Update()
    {
        if(Arduino.IsOpen&&Arduino.BytesToRead>0)
            {
                int size = Arduino.BytesToRead;
                for(int i = 0; i<size; i++)
                {
                    char buffer = (char)Arduino.ReadByte();
                    if(buffer!=0x0d){
                        i_queue.Enqueue(buffer);
                    }else{
                        char[] ccc = i_queue.ToArray();
                        i_queue.Clear();
                        s_queue.Enqueue(new string(ccc));
                    }
                }
                
            }
        
        if(Arduino.IsOpen){
            if(s_queue.Count > 0)                
             {
                receivedata = s_queue.Dequeue();
                Debug.Log(receivedata);
                string[] datas = receivedata.Split(',');
                int pressure = int.Parse(datas[1]);
                bool ismove = int.Parse(datas[0]) > 0;

                if(prevmove != ismove){
                    prevmove = ismove;
                    if(ismove) MoveEvent.Invoke();
                    else StopMoveEvent.Invoke();
                }
                if(prevjump != (pressure > 100)){
                    prevjump = (pressure > 100);
                    if(prevjump) StopJumpEvent.Invoke();
                    else JumpEvent.Invoke();
                }
            }
        }
    }
}
