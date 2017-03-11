#!/usr/bin/env python3

from bluepy import sensortag
from os.path import expanduser
from time import sleep
import json
import os
import requests
import sdnotify
import serial
import threading
import time

SEND_INTERVAL = 5
POLL_INTERVAL = 1
API_PREFIX = 'https://trashtalkapi.azurewebsites.net/api/trashcan/'
BAUD_RATE = 9600
SENSORTAG_MAC = 'A0:E6:F8:AF:3E:06'

systemd_notifier = sdnotify.SystemdNotifier()

with open(expanduser('~') + '/.config/trashtalk/device_id') as file:
    device_id = file.read().strip()

post_url = API_PREFIX + device_id + '/status'
post_headers = {'Content-Type': 'application/json'}

serial_devices = [
    '/dev/' + file for file in os.listdir('/dev') if file.startswith('ttyUSB')]
serial_connections = [
    serial.Serial(device, BAUD_RATE) for device in serial_devices]

tag = sensortag.SensorTag(SENSORTAG_MAC)
tag.IRtemperature.enable()
tag.accelerometer.enable()

arduino_readings = {
    'distance1': 0,
    'distance2': 0,
    'flame': 0,
    'weight': 0
}


def worker():
    for conn in serial_connections:
        conn.flushInput()

    while True:
        for conn in serial_connections:
            try:
                reading = conn.readline().strip()
                fields = reading.decode().split(':')
                if len(fields) == 2:
                    arduino_readings[fields[0]] = fields[1]
            except UnicodeDecodeError:
                pass


thread = threading.Thread(target=worker)
thread.start()

sleep(2)
systemd_notifier.notify('READY=1')

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
            'lidIsClosed': lid_is_closed,
            'temperature': {
                'ambient': temperature[0],
                'target': temperature[1]
            },
            'weight': arduino_readings['weight']
        })
        print(sensor_data)
        requests.post(url=post_url, headers=post_headers, data=sensor_data)
        last_send_time = time.time()

    last_lid_is_closed = lid_is_closed
    systemd_notifier.notify('WATCHDOG=1')
    sleep(POLL_INTERVAL)
