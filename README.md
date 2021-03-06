Talking trash
=============

Talking trash is a system for smart trash management. It enables trash cans to
be equiped with multiple different sensors and reports this data to a server.
The server aggregates and analyzes the data to provide useful statistics.
This is then used to map optimal routes for emptying the trash cans
effectively.

The trash cans uses two ultrasound sensors for measuring the distance from the
lid of the trash can to the bottom. These are used to measure how full the
trash can is. The reason for having two of them is in case the trash is
unevenly distributed, so there still is some space on one side. It also uses a
flame sensor in the lid to measure if the trash is taking fire. These sensors
are connected to an Arduino Nano, which runs the code provided in
`arduino/lid. The last sensor in the lid is a TI SensorTag. This has an
accelerometer, which is used to detect if the lid is open or closed.

In the bottom of the trash can, we have some weight sensors. This is useful to
get an idea of the types of trash in the can, and how much it can be
compressed. If the can is full, but very light, we can assume that the
contents can be compacted by the garbage truck, which means we can empty more
trash cans in one round. This sensor will be connected to an Arduino Uno
placed in the bottom of the can, which runs the code provided in
`arduino/weight`.

The two Arduinos and the SensorTag are connected to a Raspberry PI placed on
the back side of the trash can. The Arduinos are connected with USB and
communicates over a serial bus. The SensorTag is connected with Bluetooth. The
Raspberry PI runs the code provided in `raspi` to collect the data and push
it to the server.

####Note: there are a few util scripts in the scripts folder, go chek them out to fill your system with data!

Gettings started
================

Prerequisites:
* Setup a DocumentDb in Azure
* Google Maps ApiKey


TrashTalkDasboard solution
--------------------------

Replace Azure DocumentDB settings in `appsettings.json`:

```
{
  "ConnectionStrings": {
    "DefaultConnection": [LocalDBConnectionString]
  },
  "DatabaseSettings": {
    "Collection": [CollectionName],
    "Database": [DbName],
    "AuthKey": "[AuthKey]",
    "Endpoint": "[EndpointUrl]"
  }
}
```

Replace `googleApiKey` in `Views/HeatMap/Index.cshtml`:

```
<script src="https://maps.googleapis.com/maps/api/js?key=[GoogleApiKey]&libraries=visualization&callback=initMap" async defer></script>
```

TrashTalkApi solution
---------------------

Replace Azure DocumentDB settings in `Web.config`:

```
<appSettings>
  <add key="endpoint" value="[Endpoint URL]" />
  <add key="authKey" value="[AuthKey]" />
  <add key="database" value="trashTalk" />
  <add key="collection" value="[CollectionName]" />
</appSettings>
```


Arduino
-------

To run the Arduinos, first connect the sensors to the pins defined in the
files in the `arduino` directory. We use an Arduino Nano for `lid` and an
Arduino Uno for `weight`. Then use the Arduino SDK to open each of the `.ino`
files and upload them to the Arduinos. The Arduinos will run automatically
when given power, so after uploading you just need to connect them to the
Raspberry Pi with USB.


Raspberry Pi
------------

The Raspberry Pi needs to be registered with the server. To do this, register
a new device in the server and copy the device id to
`~/.config/trashtalk/device_id` on the Pi. Then find the MAC address of your
TI SensorTag and set `SENSORTAG_MAC` in `raspi/client.py` to this address. You
can set the API endpoint with `API_PREFIX`.

To run the program, you need to install some dependencies first. You can do
this by running `pip install -r requirements.txt`. After setting the config
and installing the dependencies, you simply need to run the program with
`./raspi/client.py`.


Android App
-----------

The Android App is set up as an Android Studio project. To run it you can
import the project in Android Studio and install it on a device. You can
change the API endpoint by editing `API_PREFIX` in `MainActivity.java`.
