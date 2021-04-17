#define RED 3
#define GREEN 9
#define BLUE 10
#define LED_COUNT 360

void setup() {
  Serial.begin(500000);
  pinMode(RED, OUTPUT);
  pinMode(GREEN, OUTPUT);
  pinMode(BLUE, OUTPUT);
  digitalWrite(RED, LOW);
  digitalWrite(GREEN, LOW);
  digitalWrite(BLUE, LOW);
}

void loop() {
  char buff[LED_COUNT];
  bool nd = false;
  int c = 0;
  while(c<LED_COUNT){
    if(Serial.available()>0){
      buff[c] = Serial.read();
      c++ ;
    }
  }
  nd = true;
  if(nd) {
    analogWrite(RED, buff[1]);
    analogWrite(GREEN, buff[0]);
    analogWrite(BLUE, buff[2]);
  }
  
}
