void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  int val = analogRead(A0);
  //Serial.print("Voltage");
  //Serial.print("\t");
  Serial.println(val);
  delay(5);

  //we're interested in events that happen roughly within 15-20 ms
  
}
