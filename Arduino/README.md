# Arduino

This project was implemented using Arduino Uno, but that is not a strict requirement. Source code utilizes some interrupts (timers and IO interrupts) so those need to match. Also both Digital and Analog IO are used.

This implementation targets to output 60Hz displays. Some of the hardcoded filterings assume this, but it is not a restriction. Just change the code if needed.

## HW

See images in /HW folder for system diagrams and teardown pictures. The trigger circuit is identical to the original Zapper but the detector circuit is WAY simplified, using just voltage division. We do the whole detection logic in SW based on the analog input. Original Zapper contained already some detection logic.

See (Bom)[BOM.md] for required components.
