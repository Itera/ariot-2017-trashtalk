#!/usr/bin/env python3

from bluepy import sensortag
from time import sleep
import json
import requests
import serial
import threading

SEND_INTERVAL = 5
API_URL = 'https://trashtalkapi.azurewebsites.net/api/trashcan/status'
SERIAL_DEVICE = '/dev/ttyUSB0'
SENSORTAG_MAC = 'A0:E6:F8:AF:3E:06'

ultrasound = serial.Serial(SERIAL_DEVICE, 57600)

tag = sensortag.SensorTag(SENSORTAG_MAC)
tag.IRtemperature.enable()
tag.accelerometer.enable()

arduino_readings = {
    'distance1': 0,
    'distance2': 0,
    'flame': 0
}


def worker():
    while True:
        reading = ultrasound.readline().strip()
        fields = reading.decode().split(':')
        arduino_readings[fields[0]] = int(fields[1])


thread = threading.Thread(target=worker)
thread.start()

sleep(5)

while True:
    accelerometer = tag.accelerometer.read()
    temperature = tag.IRtemperature.read()
    sensor_data = json.dumps({
        'accelerometer': {
            'x': accelerometer[0],
            'y': accelerometer[1],
            'z': accelerometer[2]
        },
        'distance': {
            'sensor1': arduino_readings['distance1'],
            'sensor2': arduino_readings['distance2']
        },
        'flame': arduino_readings['flame'],
        'temperature': {
            'ambient': temperature[0],
            'target': temperature[1]
        }
    })
    print(sensor_data)
    requests.post(url=API_URL, data=sensor_data)
    sleep(SEND_INTERVAL)
