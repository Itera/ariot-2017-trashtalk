import json
import random
from collections import defaultdict
from functools import partial

def generate_accelorometer_point():
    return random.uniform(-1.5, 1.5)


def generate_distance_points():
    d1 = random.randrange(0,90)
    d2 = d1 + random.randrange(-5, 5)
    if d2 < 0:
        d2 = 0

    if random.random() > 0.5:
        temp = d1
        d1 = d2
        d2 = temp

    return d1, d2


def generate_flame_point():
    return random.randrange(600, 1000)


def generate_lid_is_closed_point():
    return random.random() > 0.01


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
            'ambient': 20,
            'target': 20
        }
    })

    return sensor_data


def generate_n_per_guid(guids, n_func=partial(random.randrange, 1, 10)):
    return {guid: [generate_one_random_point() for n in range(n_func())] for guid in guids}


if __name__ == '__main__':
    print(generate_n_per_guid(('a','b')))
