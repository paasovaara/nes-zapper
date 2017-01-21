# NES Zapper

This is an attempt to revive the original NES Zapper (aka the Duck Hunt gun) to be used with modern display technology and game development tools.

## Background

The basic operating principle of NES Zapper is simple, yet extremely practical. It's fair to say it's a priceless invention of awesome engineers of their time. 

There are better resources on internet to explain it, but basically there's an optical detector on the gun which is synced to the game and the CRT display. The game will show a pre-defined pattern of black and white frames when the trigger is pulled. 

The solution utilizes heavily some characteristics of CRT displays: 
1. Exact timing
2. Possibly the emitted wavelength(?)

There have been several attempts "all over the internet" to make the gun work, but very few (if none) have been reported as fully successfull. Mostly this is caused by the first item on the list, exact timing. Modern LCD's and projectors have way bigger lag than original CRT displays. Also the lag is not consistent between HW providers. Therefore the delays and timing that were hardcoded to the games do not work on modern display technology.

## Possible solution offered by this project

Goal of this project is not to play Duck Hunt on LCD displays. It is to create a new game using Unity3d that utilizes the same operating principle of the gun & the system. This gives us the opportunity to control the timings and delays as we wish.

Project started out as trying to use the gun "as-is", but it was not feasible because of the item 2 on the list. We could not get the detector to work properly with standard LCD displays at all. Hypothesis is that the wavelength or the signal strength was not suitable for this original HW. Therefore we had to modify the gun.

## Project implementation

Folder /Arduino contains the required Arduino source code. Further instructions are found in that folder.

Folder /Unity contains the corresponding Unity3d project. Further instructions are found in that folder.

## License & Trademarks

NES, NES Zapper and Duck Hunt are trademarks of [Nintendo](http://www.nintendo.com/) corporation. 

All of the source code found in this project are licensed under the [MIT license](LICENSE). Some of the assets might be licensed under different terms. Those assets and their licenses are detailed  [here](LICENSE-EXT-ASSETS). 

Licenses do NOT include some of the artwork found in the project folders, which are modified screenshots of the original Duck Hunt game. The assets are used here as non-essential part of this non-profit research project and are treated as fair use. 
