const int outPinLed = 2;
const int inPinTrigger = 7;
const int inPinDetector = 4;

int triggerPressed = 0;
int detectorValue = 1023;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  
  pinMode(outPinLed, OUTPUT);
  digitalWrite(outPinLed, LOW);

  pinMode(inPinTrigger, INPUT_PULLUP);
  pinMode(inPinDetector, INPUT_PULLUP);
}

void loop() {
  int ledState = LOW;
  while(true) {
    readDetector();
    
    digitalWrite(outPinLed, ledState);
    ledState = ledState == LOW ? HIGH : LOW;
    //delay(3);
    int triggerPrev = triggerPressed;
    triggerPressed = digitalRead(inPinTrigger);
    if (triggerPressed != triggerPrev) {
      Serial.print("Trigger state changed: ");
      Serial.println(triggerPressed);
      if (triggerPressed == LOW) {
        digitalWrite(outPinLed, LOW);
        delay(15);
        digitalWrite(outPinLed, HIGH);
        delay(15);
        digitalWrite(outPinLed, LOW);
        
      }
    }
  }
}

void readDetector() {
  int detectorPrev = detectorValue;
  detectorValue = digitalRead(inPinDetector);    
  if (detectorValue != detectorPrev) {
    Serial.print("     Detector Changed!: ");
    Serial.println(detectorValue);
    
  }
  
}

