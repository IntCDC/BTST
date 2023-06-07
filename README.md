# Been There, Seen That (BTST): Visualization of 3D eye tracking data from real world environments
This Project allows you to analyze 3D gaze and movement data sets recorded using the HoloLens 2. 
It provides you with a gaze replay visualization, which is linked to a space-time cube visualization to show an overview of the behavioral data and inspect important events in more detail.   

<p align="center">
<img src="https://github.com/IntCDC/BTST/blob/main/Img/TeaserUI.PNG" width=70% height=70%>
</p>

## Requirements ##
We recommend using Unity version 2020.3.24f to run this project.

## Setup ##
Once the project is opened in Unity, drag the scenes Gaze Replay and STC to the Hierachy and Unload the scene Gaze Replay. The Scenes can be found within the project Folder in Assets/Scenes.
<p align="center">
<img src="https://github.com/IntCDC/BTST/blob/main/Img/ProjectOverview.png" width=70% height=70%>
</p>

## Background ##
As mentioned above the data was recorded using the HoloLens 2. We generated a Unity project and utilized the [ARETT package](https://github.com/AR-Eye-Tracking-Toolkit/ARETT)  from Kapp et al., 2020 to record the gaze data of different participants. 
Afterwards the [ARETT-R-Package](https://github.com/AR-Eye-Tracking-Toolkit/ARETT-R-Package)  was used to obtain fixation data, which were then utilzed for the visualization. 
To avoid copyright infringement we replaced the photogrammetry mesh with a standard cube and added the artwork collections, to visualize the contextual information.  
For more details regarding the individual visualization techniques, we refer to our paper.
