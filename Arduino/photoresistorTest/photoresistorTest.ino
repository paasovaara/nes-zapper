const int BUFFER_SIZE = 4;
/** Input */
int m_buffer[BUFFER_SIZE];
int m_bufIndex = 0;

const int PIN_TRIGGER_INPUT = 2; // interrupt 0 is associated with pin 2

/** Output */
const int OUTPUT_SAMPLE_PERIOD_MS = 17;  // 60Hz = 17ms
unsigned long m_outputTimer = 0;

const bool SEND_ALWAYS = false; //debug flag

/** State */
enum State {
  Idle, Triggered
};
State m_state = Idle;

//we might need to add some detection to the third frame also, spike from white target to normal.
enum HitState {
  StartDetecting, BlankDetected, Miss, Hit
};
HitState m_hitState = StartDetecting;

const int DETECTOR_THRESHOLD_BLANK = 10; // This needs testing. Optimally we would have an input "knob" where we could adjust it in runtime
const int DETECTOR_THRESHOLD_TARGET = -5; // This needs testing. Optimally we would have an input "knob" where we could adjust it in runtime
int m_prevDetector = 0;

const int TRIGGER_STATE_LENGTH_MS = 250;  // Burst length. TODO reduce to reasonable, test how fast user can pull
unsigned long m_triggeredTimer = 0;

/** Implementation begins */

void setup() {
  Serial.begin(9600);
  m_outputTimer = millis();
  m_triggeredTimer =  millis();
  
  for (int n = 0; n < BUFFER_SIZE; ++n) {
    m_buffer[n] = 1023;//pullup mode
  }
  // Timer0 is already used for millis() - we'll just interrupt somewhere
  // in the middle and call the "Compare A" function below
  OCR0A = 0xAF;
  TIMSK0 |= _BV(OCIE0A);

  // Configure trigger input as interrupt also
  pinMode(PIN_TRIGGER_INPUT, INPUT_PULLUP);
  attachInterrupt(0, triggerCallback, FALLING);
}

/** This is the actual main loop. 
    Interrupt is called once a millisecond (Timer0 frequency) 
  */
SIGNAL(TIMER0_COMPA_vect) 
{
  readSample();

  updateState();
  
  sendOutput();
}

void loop() {
}

/** Trigger pressed, we need to start monitoring the state(s) and sending output */
void triggerCallback() {
  if (m_state == Triggered) {
    //Serial.println("Alredy Triggered..");
    return;
  }
  m_state = Triggered;
  m_hitState = StartDetecting;
  m_triggeredTimer =  millis();
  m_prevDetector = readBuffer();
  Serial.println("TRIG");
}

void updateState() {
  if (m_state == Triggered) {    
    if (millis() - m_triggeredTimer > TRIGGER_STATE_LENGTH_MS) {
      m_state = Idle;
      Serial.println("END");
    }
  }
  else {
    // update always to prevent possible overflows
    m_triggeredTimer =  millis();
  }
  // TODO raise led output when in triggered state
}

/**
 * Detects the hits by looking at changes in input. We have a pullup resistor on detector
 * so black equals positive spike, white as negative.
 */
void detectHit() {
  int detector = readBuffer();
  int diffToPrev = detector - m_prevDetector;
  bool stateChanged = false;
  
  if (m_hitState == StartDetecting) {
    //looking for black screen => spike upwards
    if (diffToPrev >= DETECTOR_THRESHOLD_BLANK) {
      m_hitState = BlankDetected;
      stateChanged = true;
      //Serial.print("TO BLANK DETECTED STATE, diff:");
      //Serial.println(diffToPrev);
    }
  }
  else if (m_hitState == BlankDetected) {
    //looking for white target => spike downwards
    if (diffToPrev <= DETECTOR_THRESHOLD_TARGET) {
      m_hitState = Hit;
      //Serial.print("=> HIT! diff:");
      //Serial.println(diffToPrev);
    }
    else {
      m_hitState = Miss;
      //Serial.print("<= MISS... diff:");
      //Serial.println(diffToPrev);
    }
    stateChanged = true;
  }
  /*
  if (!stateChanged) {
    Serial.print("no change, diff:");
    Serial.println(diffToPrev);
  }*/
  
  m_prevDetector = detector;
}

void sendOutput()  {
  unsigned long curTime = millis();

  bool isSendState = m_state == Triggered || SEND_ALWAYS;
  if (isSendState) {
    
    if (curTime - m_outputTimer >= OUTPUT_SAMPLE_PERIOD_MS) {
      m_outputTimer = curTime;
      
      int val = readBuffer();
      //Normalize here to send 0 on black and 1023 on white. 
      int output = 1023 - val;
      Serial.println(output);
      //we might have to start the counter for detectHit() from the actual trigger..
      //other option is not to be so super strict but allow some lag for the detection. might be more robust 
      //detectHit();
    }  
  }
}

/** Read single sample. Do not send this out */
int readSample() {
  int value = m_buffer[m_bufIndex] = analogRead(A0);
  m_bufIndex = (m_bufIndex + 1) % BUFFER_SIZE;
  return value;
}

/** Use this for actual logic and output */
int readBuffer() {
  //TODO: test if median is better than avg
  int sum = 0;
  for (int n = 0; n < BUFFER_SIZE; ++n) {
    sum += m_buffer[n];
  }
  return sum / BUFFER_SIZE;
}

