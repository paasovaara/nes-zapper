const int BUFFER_SIZE = 8;
int m_buffer[BUFFER_SIZE];
int m_bufIndex = 0;

const int OUTPUT_SAMPLE_TIMEOUT_MS = 17;//60Hz = 17ms

unsigned long m_outputTimer = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  m_outputTimer = millis();

  for (int n = 0; n < BUFFER_SIZE; ++n) {
    m_buffer[n] = 1023;//pullup mode
  }
  // Timer0 is already used for millis() - we'll just interrupt somewhere
  // in the middle and call the "Compare A" function below
  OCR0A = 0xAF;
  TIMSK0 |= _BV(OCIE0A);
}

// Interrupt is called once a millisecond (Timer0 frequency)
SIGNAL(TIMER0_COMPA_vect) 
{
  readSample();

  unsigned long curTime = millis();
  
  if (curTime - m_outputTimer >= OUTPUT_SAMPLE_TIMEOUT_MS) {
    m_outputTimer = curTime;
    
    int val = readBuffer();
    Serial.println(val);
  }
}

void loop() {
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

