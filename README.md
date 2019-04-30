# Installation
---
* ### Download the [Install Script](https://github.com/JeremyRuffell/AquiraHelpTopics/releases/latest/download/Install_Script.ps1)
*P.S.* Make Sure you click **KEEP**.
 ![github-small](https://user-images.githubusercontent.com/36174931/56937039-d2d05300-6b4e-11e9-9f1d-11dc15113cfd.png)

* ### Change Your PowerShell Execution Policy
Open PowerShell and **Run as administrator**
![image](https://user-images.githubusercontent.com/36174931/56937628-d5807780-6b51-11e9-961d-0d63da276029.png)

Run this command in *PowerShell* to view your current **Execution Policy**
```Get-ExecutionPolicy -Scope CurrentUser```

If your Current Execution Policy is **"Unrestricted"** you can ignore this step.

We need to change the Current Users **Execution Policy** to **"Unrestricted"**, to do this run *this command* ``` Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope CurrentUser``` and Enter ```Y``` when prompted.

![image](https://user-images.githubusercontent.com/36174931/56937924-6146d380-6b53-11e9-95d8-a53ff3d5d89f.png)

* ### Run the Install Script
Locate the *Install Script* genreally located in your **Downloads Folder**. To run the script **Right Click > Run with PowerShell**
![image](https://user-images.githubusercontent.com/36174931/56938091-1ed1c680-6b54-11e9-9d3c-a67abc2a3dd8.png)

When you get prompted with a *Security warning* enter ```R``` and continue. 

*AquiraHelpTopics* will now insall.

Once Installed **Press Enter** to exit the installer.

* ### Run with Command Prompt

To run *AquiraHelpTopics* open Command Prompt and type `AquiraHelpTopics` and press **Enter**

If an update is needed the Application will Automatically install the update for you and Restart. 

* #### Voila AquiraHelpTopics is now Installed and running the latest Version.
![image](https://user-images.githubusercontent.com/36174931/56938363-90f6db00-6b55-11e9-8d1c-25be10db3ec6.png)

* ### Issues or Bugs

If you are having any issues or find a bug you can either create an [Issue](https://github.com/JeremyRuffell/AquiraHelpTopics/issues) or email me at **jruffell@rcs.co.nz**
