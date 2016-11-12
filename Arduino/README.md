# Arduino

This project was implemented using Arduino Uno, but that is not a strict requirement. Source code utilizes some interrupts (timers and IO interrupts) so those need to match. Also both Digital and Analog IO are used.

This implementation targets to output 60Hz displays. Some of the hardcoded filterings assume this, but it is not a restriction. Just change the code if needed.

## HW

See images in /HW folder for system diagrams and teardown pictures. The trigger circuit is identical to the original Zapper but the detector circuit is WAY simplified, using just voltage division. We do the whole detection logic in SW based on the analog input. Original Zapper contained already some detection logic.

See (Bom)[BOM.md] for required components.

## Output

Program outputs a burst of serial messages (9600baud) when the trigger is pulled. 

Current implementation sends the raw data for some time period (f.ex 250ms). This way the receiving end can handle the detection logic based on the game update loop frequency and display lags. Once the specific parameters are found, it makes sense to move the logic to Arduino.

Output frequency should be static (f.ex 60Hz or 120Hz). Design considerations: use separate thread for the reading and possibly timestamp the messages.

All messages are separated by an LF-char.

Message | Description
---------------------
TRIG    | Trigger pulled, burst of values to follow
[0-1023]| Raw analog value read, 0 meaning pitch black and 1023 saturated white.
END     | End of single burst. 


TODO we should create a way for client to request and update parameters from/to the Arduino.
