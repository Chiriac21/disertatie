#include <SoftwareSerial.h>

const int EN  = 2;
const int PWM = 3;
const int INA = 7;
const int INB = 8;

const int CLOCK_PIN = 5;
const int DATA_PIN  = 6;
const int CS        = 10;
const int BIT_COUNT = 10; // 0..1023

int Ref = 600;  // will be overwritten when a REF message arrives

const int DEAD_BAND = 2;
const unsigned long STABLE_MS = 200;

double kp = 20.0, ki = 0.02, kd = 0.005;

int reading = 0;
unsigned long lastTime;
double error, errSum, lastErr;

bool targetLatched = false;
unsigned long inBandSince = 0;

// ===== ID & link =====
const char* MY_ID      = "U3";  
const char* BROADCAST  = "ALL";

// SoftwareSerial link on D4 (RX) / D11 (TX) for U1
SoftwareSerial link(4, 11); // RX, TX

// Line buffer for addressed commands
char inBuf[32];
byte inLen = 0;

void setup() {
  // driver motor
  pinMode(EN, OUTPUT);
  pinMode(PWM, OUTPUT);
  pinMode(INA, OUTPUT);
  pinMode(INB, OUTPUT);
  digitalWrite(EN, HIGH);
  brake();

  // encoder
  pinMode(DATA_PIN, INPUT);
  pinMode(CLOCK_PIN, OUTPUT);
  pinMode(CS, OUTPUT);
  digitalWrite(CLOCK_PIN, HIGH);
  digitalWrite(CS, HIGH);

  Serial.begin(115200);
  link.begin(9600); // UART from UNO R4 WiFi

  Serial.print(MY_ID);
  Serial.println(" REF <0..1023>\", or \"ALL REF <...>\"");

  lastTime = millis();
}

void loop() {
  // 1) Check for new addressed command
  pollLinkForAddressedRef();

  // 2) Encoder read
  int val = readPosition();
  if (val == -1) {
    Serial.println("Read ERR!");
    delay(20);
    return;
  }
  reading = val;
  Serial.println(val);

  // 3) When latched
  if (targetLatched) {
    brake();
    Serial.print("LATCH @ "); Serial.println(reading);
    delay(20);
    return;
  }

  // 4) Dead-band and control
  int e = Ref - reading;
  if (abs(e) <= DEAD_BAND) {
    if (inBandSince == 0) inBandSince = millis();
    if (millis() - inBandSince >= STABLE_MS) {
      targetLatched = true;
      brake();
      Serial.print("TARGET LATCHED @ encoder = ");
      Serial.println(reading);
      delay(20);
      return;
    }
  } else {
    inBandSince = 0;
  }

  // 5) PID + motor command
  int pwmCmd = PID_step();
  if (pwmCmd < 0) {
    pwmCmd = -pwmCmd; if (pwmCmd > 255) pwmCmd = 255;
    rotateRight(pwmCmd);
  } else if (pwmCmd > 0) {
    if (pwmCmd > 255) pwmCmd = 255;
    rotateLeft(pwmCmd);
  } else {
    brake();
  }

  delay(10);
}

// Accepts (case-insensitive cmd):
// "ID <value>"
void pollLinkForAddressedRef() {
  while (link.available()) {
    char c = (char)link.read();

    if (c == '\n' || c == '\r') {
      if (inLen > 0) {
        inBuf[inLen] = '\0';
        applyAddressedLine(inBuf);
        inLen = 0;
      }
    } else {
      if (inLen < sizeof(inBuf) - 1) {
        inBuf[inLen++] = c;
      } else {
        // overflow -> reset
        inLen = 0;
      }
    }
  }
}

void applyAddressedLine(const char* line) {
  // Try "<id> <val>"
  char id[8] = {0};
  int value = -1;
  if (sscanf(line, "%7s %d", id, &value) == 2) {
    if (matchesID(id)) {
      applyNewRef(value);
      return;
    }
  }

  // Not for me or invalid -> ignore
}

bool matchesID(const char* id) {
  return (strcasecmp(id, MY_ID) == 0) || (strcasecmp(id, BROADCAST) == 0);
}

void applyNewRef(int value) {
  // clip and apply
  if (value < 0) value = 0;
  if (value > 1023) value = 1023;

  Ref = value;
  targetLatched = false;   // re-enable control to move to new target
  inBandSince = 0;
  errSum = 0;              // optional: reset integrator for a clean step

  Serial.print("Nou Ref primit pentru ");
  Serial.print(MY_ID);
  Serial.print(": ");
  Serial.println(Ref);
}

// ================== MOTOR ==================
void rotateRight(int pwm) {
  analogWrite(PWM, pwm);
  digitalWrite(INA, HIGH);
  digitalWrite(INB, LOW);
}
void rotateLeft(int pwm) {
  analogWrite(PWM, pwm);
  digitalWrite(INA, LOW);
  digitalWrite(INB, HIGH);
}
void brake() {
  analogWrite(PWM, 0);
  digitalWrite(INA, LOW);
  digitalWrite(INB, LOW);
}

// ================== ENCODER ==================
int readPosition() {
  unsigned long s1 = shiftIn(DATA_PIN, CLOCK_PIN, BIT_COUNT);
  unsigned long s2 = shiftIn(DATA_PIN, CLOCK_PIN, BIT_COUNT);
  delayMicroseconds(2);
  if (s1 != s2) return -1;
  return (int)s1; // 0..1023
}
unsigned long shiftIn(const int data_pin, const int clock_pin, const int bit_count) {
  unsigned long data = 0;
  digitalWrite(CS, LOW);
  delayMicroseconds(1);
  for (int i=0; i<bit_count; i++) {
    data <<= 1;
    digitalWrite(clock_pin, LOW);
    delayMicroseconds(1);
    digitalWrite(clock_pin, HIGH);
    delayMicroseconds(1);
    data |= digitalRead(data_pin);
  }
  digitalWrite(CS, HIGH);
  return data;
}

// ================== PID ==================
int PID_step() {
  double now = millis();
  double dt  = (double)(now - lastTime);
  if (dt <= 0) dt = 1.0;

  error   = (double)(Ref - reading);
  errSum += error * dt;

  const double I_CLAMP = 6000.0;
  if (errSum >  I_CLAMP) errSum =  I_CLAMP;
  if (errSum < -I_CLAMP) errSum = -I_CLAMP;

  double dErr  = (error - lastErr) / dt;
  int Output   = (int)(kp * error + ki * errSum + kd * dErr);

  lastErr  = error;
  lastTime = now;
  return Output;
}
