const int BUFFER_SIZE = 8;
int m_buffer[BUFFER_SIZE];
int m_bufIndex = 0;

unsigned long m_timer = 0;
int m_counter = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  m_timer = millis();

  // Timer0 is already used for millis() - we'll just interrupt somewhere
  // in the middle and call the "Compare A" function below
  OCR0A = 0xAF;
  TIMSK0 |= _BV(OCIE0A);
}

// Interrupt is called once a millisecond (Timer0 frequency)
SIGNAL(TIMER0_COMPA_vect) 
{
  int val = readValue();
  //we're interested in events that happen roughly within 15-20 ms  
  if (m_counter++ >= 4) {
      Serial.println(val);
      m_counter = 0;    
  }
}

void loop() {
}

int readSample() {
  int value = m_buffer[m_bufIndex] = analogRead(A0);
  m_bufIndex = (m_bufIndex + 1) % BUFFER_SIZE;
  return value;
}

int readValue() {
  int value = readSample();
  //TODO: test if median is better than avg
  int sum = 0;
  for (int n = 0; n < BUFFER_SIZE; ++n) {
    sum += m_buffer[n];
  }
  return sum / BUFFER_SIZE;
}

