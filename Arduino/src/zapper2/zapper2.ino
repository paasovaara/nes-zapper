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
  int triggerPrev = triggerPressed;
  triggerPressed = digitalRead(inPinTrigger);
  if (triggerPressed != triggerPrev) {
    Serial.print("Trigger state changed: ");
    Serial.println(triggerPressed);
  /*  if (triggerPressed == LOW) {
      readDetector();
      digitalWrite(outPinLed, LOW);
      delay(15);
      readDetector();
      digitalWrite(outPinLed, HIGH);
      delay(15);
      readDetector();
      digitalWrite(outPinLed, LOW);
      delay(15);
      readDetector();
      
    }*/
  }
  readDetector();
  digitalWrite(outPinLed, HIGH);
  
  /*
  if (triggerPressed == LOW) {
    int detectorPrev = detectorValue;
    detectorValue = analogRead(inPinDetector);    
    Serial.print("     Detector: ");
    Serial.print(detectorValue);
    Serial.print("   diff=");
    Serial.println(detectorValue - detectorPrev);
  }
  int ledState = !triggerPressed ? HIGH : LOW;
  digitalWrite(outPinLed, ledState);
*/
  
}

void readDetector() {
  int detectorPrev = detectorValue;
  detectorValue = digitalRead(inPinDetector);    
  if (detectorValue != detectorPrev) {
    Serial.print("     Detector Changed!: ");
    Serial.println(detectorValue);
    
  }
  
}

