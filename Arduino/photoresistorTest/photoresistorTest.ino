const int BUFFER_SIZE = 4;
int m_buffer[BUFFER_SIZE];
int m_bufIndex = 0;

const int PIN_TRIGGER_INPUT = 2; // interrupt 0 is associated with pin 2

const int OUTPUT_SAMPLE_PERIOD_MS = 17;  // 60Hz = 17ms
unsigned long m_outputTimer = 0;

enum State {
  Idle, Triggered
};
State m_state = Idle;
const int TRIGGER_STATE_LENGTH_MS = 500;  // Burst length
unsigned long m_triggeredTimer = 0;

const bool SEND_ALWAYS = false; //debug flag

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

// Interrupt is called once a millisecond (Timer0 frequency)
SIGNAL(TIMER0_COMPA_vect) 
{
  readSample();

  updateState();
  
  sendOutput();
}

void loop() {
}

void triggerCallback() {
  m_state = Triggered;
  m_triggeredTimer =  millis();
}

void updateState() {
  if (m_state == Triggered) {
    if (millis() - m_triggeredTimer > TRIGGER_STATE_LENGTH_MS) {
      m_state = Idle;
    }
  }
  else {
    // update always to prevent possible overflows
    m_triggeredTimer =  millis();
  }
  // TODO raise led output when in triggered state
}

void sendOutput()  {
  unsigned long curTime = millis();

  bool isSendState = m_state == Triggered || SEND_ALWAYS;
  if (isSendState) {
    if (curTime - m_outputTimer >= OUTPUT_SAMPLE_PERIOD_MS) {
      m_outputTimer = curTime;
      
      int val = readBuffer();
      Serial.println(val);
    }  
  }
}

int readSample() {
  int value = m_buffer[m_bufIndex] = analogRead(A0);
  m_bufIndex = (m_bufIndex + 1) % BUFFER_SIZE;
  return value;
}

int readBuffer() {
  //TODO: test if median is better than avg
  int sum = 0;
  for (int n = 0; n < BUFFER_SIZE; ++n) {
    sum += m_buffer[n];
  }
  return sum / BUFFER_SIZE;
}

