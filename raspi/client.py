#!/usr/bin/env python3

from bluepy import sensortag
from os.path import expanduser
from time import sleep
import json
import requests
import serial
import threading
import time

SEND_INTERVAL = 5
POLL_INTERVAL = 1
API_PREFIX = 'https://trashtalkapi.azurewebsites.net/api/trashcan/'
SERIAL_DEVICE = '/dev/ttyUSB0'
BAUD_RATE = 9600
SENSORTAG_MAC = 'A0:E6:F8:AF:3E:06'

with open(expanduser('~') + '/.config/trashtalk/device_id') as file:
    device_id = file.read().strip()

post_url = API_PREFIX + device_id + '/status'
post_headers = {'Content-Type': 'application/json'}

ultrasound = serial.Serial(SERIAL_DEVICE, BAUD_RATE)

tag = sensortag.SensorTag(SENSORTAG_MAC)
tag.IRtemperature.enable()
tag.accelerometer.enable()

arduino_readings = {
    'distance1': 0,
    'distance2': 0,
    'flame': 0
}


def worker():
    ultrasound.flushInput()
    while True:
        reading = ultrasound.readline().strip()
        try:
            fields = reading.decode().split(':')
            if len(fields) == 2:
                arduino_readings[fields[0]] = int(fields[1])
        except UnicodeDecodeError:
            pass


thread = threading.Thread(target=worker)
thread.start()

sleep(2)

last_send_time = time.time() - SEND_INTERVAL
last_lid_is_closed = False

while True:
    accelerometer = tag.accelerometer.read()

    lid_is_closed = (
        -0.1 < accelerometer[0] < 0.1 and
        -1.1 < accelerometer[2] < -0.9)

    if (last_send_time + SEND_INTERVAL < time.time() or
            not last_lid_is_closed and lid_is_closed):
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
            'lid_is_closed': lid_is_closed,
            'temperature': {
                'ambient': temperature[0],
                'target': temperature[1]
            }
        })
        print(sensor_data)
        requests.post(url=post_url, headers=post_headers, data=sensor_data)
        last_send_time = time.time()

    last_lid_is_closed = lid_is_closed
    sleep(POLL_INTERVAL)
