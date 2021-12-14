#define PRESSURE A0
#define CYCLESEN 4


int P_Value = 0;
bool C_bool = false;

bool T = false;

unsigned long P_time = 0;
bool P_cb = false;

bool isMoving = false;


void setup() {
  //핀 모드 설정
  pinMode(PRESSURE, INPUT);
  pinMode(CYCLESEN, INPUT);

  //시리얼 통신 설정
  Serial.begin(9600);



}

void loop() {

  
  PinRead();
  MovingCheck();
  SerialRead();
  if(T){
    SerialWrite();
    T = false;
  }
  
}



void PinRead()
{
  //전압값 읽음. 0 ~ 1024
  P_Value = analogRead(PRESSURE);

  C_bool = digitalRead(CYCLESEN);

}

void SerialRead()
{
  
  while (Serial.available() > 0)
  {
    T = true;
    Serial.read();
  }
}

void SerialWrite()
{
  int m = isMoving ? 1 : 0;
  Serial.print(m);
  Serial.print(',');
  Serial.print(P_Value);
  Serial.write(0x0d);
}

void MovingCheck()
{
  if (P_cb != C_bool)
  {
    isMoving = true;
    P_cb = C_bool;
    P_time = millis();
  }


  if ((millis() - P_time) > 100)
  {
    isMoving = false;
  }

  
}



