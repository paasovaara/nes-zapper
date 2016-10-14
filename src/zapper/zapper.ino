const int outPinLed = 2;
const int inPinTrigger = 7;

int triggerPressed = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  
  pinMode(outPinLed, OUTPUT);
  digitalWrite(outPinLed, LOW);

  pinMode(inPinTrigger, INPUT);

}

void loop() {
  int triggerPrev = triggerPressed;
  triggerPressed = digitalRead(inPinTrigger);
  if (triggerPressed != triggerPrev) {
    Serial.print("Trigger state changed: ");
    Serial.println(triggerPressed);
  }

  int ledState = triggerPressed ? HIGH : LOW;
  digitalWrite(outPinLed, ledState);

}
