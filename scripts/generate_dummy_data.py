import json
import random
from functools import partial
import requests
import numpy as np


def generate_accelorometer_point():
    return random.uniform(-1.5, 1.5)


def generate_distance_points(d1=random.randrange(0,90)):
    d2 = d1 + random.randrange(-5, 5)
    if d2 < 0:
        d2 = 0

    if random.random() > 0.5:
        temp = d1
        d1 = d2
        d2 = temp

    return d1, d2


def generate_flame_point():
    return random.randrange(900, 1023)


def generate_lid_is_closed_point():
    return random.random() > 0.01


def noisy_fi(reading, sd_base):
    print(reading)
    return np.random.normal(reading, sd_base, 1)[0]


def generate_one_signal_series(n=10):

    distance = 85
    target_distance = 5
    distance_step = (target_distance - distance) / n
    temp = 0
    target_temp = 30
    temp_step = (target_temp - temp) / n

    distance_series = [generate_distance_points(noisy_fi(distance + distance_step * i, 2)) for i in range(n)]
    temp_series = [noisy_fi(temp + temp_step * i, 1) for i in range(n)]

    series = []
    for distances, temp_reading in zip(distance_series, temp_series):
        sensor_data = json.dumps({
            'accelerometer': {
                'x': generate_accelorometer_point(),
                'y': generate_accelorometer_point(),
                'z': generate_accelorometer_point()
            },
            'distance': {
                'sensor1': distances[0],
                'sensor2': distances[1]
            },
            'flame': generate_flame_point(),
            'lidIsClosed': generate_lid_is_closed_point(),
            'temperature': {
                'ambient': temp_reading,
                'target': temp_reading
            }
        })
        series.append(sensor_data)

    return series


def generate_one_random_point():
    distances = generate_distance_points()
    sensor_data = json.dumps({
        'accelerometer': {
            'x': generate_accelorometer_point(),
            'y': generate_accelorometer_point(),
            'z': generate_accelorometer_point()
        },
        'distance': {
            'sensor1': distances[0],
            'sensor2': distances[1]
        },
        'flame': generate_flame_point(),
        'lidIsClosed': generate_lid_is_closed_point(),
        'temperature': {
            'ambient': random.randrange(0, 25),
            'target': random.randrange(0, 25)
        }
    })

    return sensor_data


def generate_n_per_guid(guids, n_func=partial(random.randrange, 1, 10)):

    return {guid: generate_one_signal_series(n_func()) for guid in guids}


def load_addresses():
    return requests.get('http://www.aaberge.net/stuff/addresses_oslo.json').json()['adresser']


def get_subset_of_addresses(filters=(lambda x: x['postnr'].lower() in ['283', '378', '379', '281'],), **kwargs):
    result = load_addresses()
    for f in filters:
        result = filter(f, result)
    return list(result)


def make_address_string(location_dict):
    street = location_dict['adressenavn']
    number = location_dict['husnr']
    letter = location_dict.get('bokstav', '')

    return "{} {} {}".format(street, number, letter)


def fetch_guid(address, api_location='https://trashtalkapi.azurewebsites.net/api/trashcan/create'):
    body = {
          "lat": address['nord'],
          "long": address['aust'],
          "address": make_address_string(address)
        }

    return requests.post(api_location, data=body).json()


def post_data_for_one_guid(guid, data, api_prefix = 'https://trashtalkapi.azurewebsites.net/api/trashcan/'):
    post_url = api_prefix + guid + '/status'
    post_headers = {'Content-Type': 'application/json'}
    for sensor_data in data:
        requests.post(url=post_url, headers=post_headers, data=sensor_data)
    print("Posted for: {} \ndata: {}".format(guid, data))


def post_dummy_data(data_map):
    for guid, data in data_map.items():
        post_data_for_one_guid(guid, data)


def generate_and_post_dummy_data():
    addresses = get_subset_of_addresses()

    guids = [fetch_guid(address) for address in addresses]

    data = generate_n_per_guid(guids)

    post_dummy_data(data)


if __name__ == '__main__':
    generate_and_post_dummy_data()

    for sig in generate_one_signal_series():
        print(sig)
