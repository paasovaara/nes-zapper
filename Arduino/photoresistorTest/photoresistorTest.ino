const int BUFFER_SIZE = 4;
int m_buffer[BUFFER_SIZE];
int m_bufIndex = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  int val = readValue();
  //Serial.print("Voltage");
  //Serial.print("\t");
  Serial.println(val);
  delay(5);

  //we're interested in events that happen roughly within 15-20 ms
  
}


int readValue() {
  m_buffer[m_bufIndex] = analogRead(A0);
  m_bufIndex = (m_bufIndex + 1) % BUFFER_SIZE;
  //test if median is better than avg
  int sum = 0;
  for (int n = 0; n < BUFFER_SIZE; ++n) {
    sum += m_buffer[n];
  }
  return sum / BUFFER_SIZE;
}

