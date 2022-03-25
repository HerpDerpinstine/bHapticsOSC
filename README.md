# bHapticsOSC
Application to allow the bHaptics Player to interface over OSC.  
Special Thanks to [bHaptics](https://www.bhaptics.com) for making the bHaptics Gear as well as supporting me wherever needed! :D

- All Connection based Settings are located in ``Connection.ini``

- All Device based Settings are located in ``Devices.ini``

- Discord: https://discord.gg/JDw423Wskf

---

### REQUIREMENTS:

- A Windows PC to run the bHaptics Player and bHapticsOSC application on.
- [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net48)  
  
**VRCHAT USAGE ONLY**    

- A bHaptics Compatible Avatar  

- VRChat's Open-Beta  
Currently Avatar Dynamics is only available on the Open-Beta branch of VRChat.  
You can switch to this branch in VRChat's Properties in Steam or Oculus.
  
**VRCHAT AVATAR CREATION ONLY**

- VRChat's Open-Beta SDK  
Currently Avatar Dynamics is only available on the Open-Beta SDK of VRChat.

- A Custom Avatar with an FX Layer and Expression Parameters.  
Depending on the Devices you choose to Integrate the Expression Parameters will need a certain amount of free memory space.  
Below is a handy chart for how much memory each Device takes up.

| Device | Memory Usage Per Device |
| - | - |
| Head | 6 bits |
| Vest | 40 bits |
| Arm | 6 bits |
| Hand | 3 bits |
| Foot | 3 bits |

---

### USAGE:

1) Download [bHapticsOSC](https://github.com/HerpDerpinstine/bHapticsOSC/releases) from [Releases](https://github.com/HerpDerpinstine/bHapticsOSC/releases).
2) Extract the application from the ZIP Archive.
3) Run ``bHapticsOSC.exe``
4) Start the bHaptics Player and connect your Devices.

---

### VRCHAT USAGE:

- Make sure you are on VRChat's Open-Beta branch.

1) Follow the normal [USAGE](#usage) guide above.
2) Change into a bHapticsOSC Compatible Avatar.
3) Make sure that OSC is enabled in the Avatar 3.0 Radial Menu. (Options -> OSC -> Enable)

---

### VRCHAT AVATAR CREATION:

- Be sure to create a backup of the Avatar's Scene before continuing.

1) Download the Source Code for [Animator As Code](https://github.com/hai-vr/av3-animator-as-code) from [HERE](https://github.com/hai-vr/av3-animator-as-code/archive/refs/heads/main.zip).
2) Extract the ``av3-animator-as-code-main`` folder from the ZIP to the Root of your Project's Assets folder.
3) Download the [bHapticsOSC](https://github.com/HerpDerpinstine/bHapticsOSC/releases) VRCSDK Unity Package from [Releases](https://github.com/HerpDerpinstine/bHapticsOSC/releases).
4) Import the [bHapticsOSC](https://github.com/HerpDerpinstine/bHapticsOSC/releases) VRCSDK Unity Package to your Project.
5) Navigate to ``Assets/bHapticsOSC/VRChat/Prefabs``.

6) Pick a set. Inside the Prefabs folder you will see 2 sets of Prefabs.  
One set is for just the Contacts, One set is for using the included Meshs.  

7) Drag, Position, and Attach the Prefabs from that set's folder onto the Avatar's Armature.
8) Add the ``bHapticsOSC Integration`` component to the Avatar next to it's Avatar Descriptor.

9) Select which Devices you wish to Integrate.  

- Every Device will give you a ``Mesh GameObject`` selection.  
If you used a Device Prefab that has an included Mesh then select it here, if not leave this blank.  

- Some Devices will give you a ``Use ParentConstraints`` option.  
This will attempt to attach ParentConstraints from the Prefab to the Avatar's Bones.  

10) Click the ``APPLY`` button.

---

### LICENSING & CREDITS:

bHapticsOSC is licensed under the GPL-3.0 License. See [LICENSE](https://github.com/HerpDerpinstine/bHapticsOSC/blob/master/LICENSE.md) for the full License.

Third-party Libraries used as Source Code and/or bundled in Binary Form:
- [Animator As Code](https://github.com/hai-vr/av3-animator-as-code) is licensed under the MIT License. See [LICENSE](https://github.com/hai-vr/av3-animator-as-code/blob/main/LICENSE) for the full License.
- [Rug.Osc](https://bitbucket.org/rugcode/rug.osc) is licensed under the MIT License. See [LICENSE](https://bitbucket.org/rugcode/rug.osc/wiki/License) for the full License.
- [bHaptics Haptic Library](https://github.com/bhaptics/haptic-library) is licensed under All rights reserved Copyright (c) 2016-2022 bHaptics Inc.  
See [Terms and Conditions](https://www.bhaptics.com/legals/terms-and-conditions) for the full License. We have Express Permission from bHaptics.
