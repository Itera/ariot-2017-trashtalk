#define pinUltrasound1Trig 2
#define pinUltrasound1Echo 3
#define pinUltrasound2Trig 4
#define pinUltrasound2Echo 5
#define pinFlame A4

long duration1, duration2, distance1, distance2;

void setup() {
  Serial.begin(57600);
  pinMode(pinUltrasound1Trig, OUTPUT);
  pinMode(pinUltrasound2Trig, OUTPUT);
  pinMode(pinUltrasound1Echo, INPUT);
  pinMode(pinUltrasound2Echo, INPUT);

  delay(5);
}

void loop() {
  // Trigger the first ultrasound sensor
  digitalWrite(pinUltrasound1Trig, LOW);
  delayMicroseconds(2);
  digitalWrite(pinUltrasound1Trig, HIGH);
  delayMicroseconds(10);
  digitalWrite(pinUltrasound1Trig, LOW);

  // Wait for signal from sensor
  duration1 = pulseIn(pinUltrasound1Echo, HIGH, 100000);

  // Trigger the second ultrasound sensor
  digitalWrite(pinUltrasound2Trig, LOW);
  delayMicroseconds(2);
  digitalWrite(pinUltrasound2Trig, HIGH);
  delayMicroseconds(10);
  digitalWrite(pinUltrasound2Trig, LOW);

  // Wait for signal from sensor
  duration2 = pulseIn(pinUltrasound2Echo, HIGH, 100000);

  // Calculate distance in centimeters
  // The speed of sound is in cm/us at 25 C
  float speedOfSound = 0.0346;
  distance1 = duration1 * speedOfSound / 2;
  distance2 = duration2 * speedOfSound / 2;

  Serial.print("distance1:");
  Serial.println(distance1);

  Serial.print("distance2:");
  Serial.println(distance2);

  int flameReading = analogRead(pinFlame);
  Serial.print("flame:");
  Serial.println(flameReading);

  delay(10);
}
