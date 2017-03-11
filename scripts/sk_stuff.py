
import numpy as np
from sklearn.svm import SVR
from sklearn.metrics import mean_squared_error
from scripts.generate_dummy_data import generate_one_signal_series
import json
from scripts.azure_connection import fetch_azure_data

def numpyify_data(input, label_key='FillGrade'):
    y = np.asarray([x[label_key] for x in input])
    X = np.zeros((len(input), 3))

    for a in range(len(input)):
        X[a,0] = input[a]['Distance']['Sensor1']
        X[a,1] = input[a]['Weight']
        X[a,2] = input[a]['Flame']

    return X, y


#azure docu-store is being tricky, lets generate the data here!
def fetch_data_super_dummy():
    data = []

    while len(data) < 10000:
        data.extend(generate_one_signal_series())

    data = [json.loads(d) for d in data]

    for d in data:
        d['FillGrade'] = 1 - ( d['distance']['sensor1'] / 85)

    cutoff = int(len(data)*0.8)
    return data[0:cutoff], data[cutoff:]


def fetch_data_azure():
    data = fetch_azure_data()
    cutoff = int(len(data)*0.8)
    return data[0:cutoff], data[cutoff:]


def run():
    train_set, test_set = fetch_data_azure()

    X_train, y_train = numpyify_data(train_set)
    X_test, y_test = numpyify_data(test_set)

    svr = SVR()
    svr.fit(X_train, y_train)

    y_predict_train = svr.predict(X_train)
    y_predict = svr.predict(X_test)


    mse_train = mean_squared_error(y_predict_train, y_train)**0.5
    mse = mean_squared_error(y_predict, y_test)**0.5
    print("RMSE TEST: {}".format(mse))
    print("RMSE TRAIN: {}".format(mse_train))

if __name__ == '__main__':
    run()