# BMSFitness
## Download
You can download latest apk below link.  
<https://drive.google.com/drive/folders/1Al4NRgsqriaBL-8kN-QFQo6oYDXCFwPT>  

SideQuest  
<https://sidequestvr.com/app/942/bmsfitness>  
  

Japanese explanation  
<http://testerchan.hatenadiary.com/entry/2020/05/04/222942>
  

## What is BMSFitness?
It's a fitness game like BOXVR.  
Balls will fly along with the song, so hit it at the right time.  
Also, you can see how many calories you've used.

In this game we use "BMS files".  
A BMS file is a file from a music game that was popular in Japan.  
Many users have made BMSs.  
You can play the game with those files.  

[What is BMS?](https://fileinfo.com/extension/bms)  

## Youtube
You can check this game at youtube.  

[![](https://img.youtube.com/vi/QlTms1lmWTw/0.jpg)](https://www.youtube.com/watch?v=QlTms1lmWTw)
  
**Third person view**  
[![](https://img.youtube.com/vi/HZk35F2jPxk/0.jpg)](https://www.youtube.com/watch?v=HZk35F2jPxk)

* * *  
  
## Contents  
- [Installation](#Installation)  
- [How to play](#How-to-play)
- [How to create music categories](#How-to-create-music-categories)
- [How to play BMS with MPG in the background](#How-to-play-BMS-with-MPG-in-the-background)
- [Avatar mode](#Avatar-mode)
- [Random select mode](#Random-select-mode)
- [About the music folder cache](#About-the-music-folder-cache)
- [History](#History)
  
* * *
  
## Installation  
**1. Please create "bmsfitness" folder under the internal storage of Oculus Quest.**  
![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200504/20200504211600.png)  
  
  

**2. Put folders of BMS in "bmsfitness" folder.**
![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200504/20200504211651.png)  

Songs can be found by googling "bms TITLE" and so on.  
For example, I like these songs.  
<http://www.yamajet.com/archives/289>  
<http://cool-create.cc/bms/index.html>  

  

**3. Please install "bmsfitness.apk", you can find out how to install apk at the following site for example.**  
<https://www.androidcentral.com/how-sideload-apps-oculus-quest>  

4. **You can play this game at "Library -> Unknown Sources -> BMSFitness".** 
  

* * *
  

## How to play
**1. You can move records with the right stick.**  
![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200504/20200504212801.jpg)  
  
  

**2. You can choose the music by picking up a record.**  
Push A button to start the music.  
![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200504/20200504212839.jpg)
  


**3. If you want to return to the music selection screen during the game, you can do so by pressing B button.**

**4. If you hit the ball with your alternate hand, it will be "Excellent". This is 10 times the score of "Great".**


![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200509/20200509144510.jpg)  
  

* * *
  
## How to create music categories
If you have a lot of songs, it will take a while to find the one you want to play.  
So I added a "Category" feature.    
This function allows you to manage songs in categories such as Anime and Game.  

**1. Create a "categories" folder in the "bmsfitness" folder.**  
  
![](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200516/20200516082723.png)  
  

**2. In the "categories" folder, create a folder for categories such as "Anime" and "Game". You can make it with any name you like.**   
  
![](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200516/20200516082844.png)  
![](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200516/20200516090153.png)  
  

**3. While in the game, aim the laser at the area marked "Category : None" on the screen and press the trigger on the right controller.**  
  
![](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200516/20200516083012.png)  
  

**4. A list of categories will appear, from which you can select a category. When "None" is selected, the song directly under the "bmsfitness" folder will be displayed.**  
  
![](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200516/20200516083112.jpg)
  
* * *
  
## How to play BMS with MPG in the background  
MP4 is supported in this game, but MPG is not.  
So, you need to convert mpg to mp4.  
After converting to mp4, you need to modify the #BMP "filename" part in the bms/bme/bml file.  
  
![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200510/20200510095659.png)

* * *

## Avatar mode  
I added the Avatar just because I wanted to shot in third person perspective.  
Use the start button on the left hand side of the oculus quest (the button in the middle) to toggle the avatar display.  
See below for a third-person perspective shot in Oculs Quest.  
<https://developer.oculus.com/documentation/mrc/mr-quest-rigs/>  
  
![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200524/20200524121446.png)
  
This game uses Unity-chan Avatar.  
<https://unity-chan.com/>  
![](https://unity-chan.com/images/imageLicenseLogo.png)  
この作品はユニティちゃんライセンス条項の元に提供されています

* * *

## Random select mode
![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200620/20200620105518.jpg)  
The function to do a random song selection in the selected category was implemented.  
It is located on the left side of your screen.  
Each category has a level, and when you select it, the song is selected at random from the level.  
  
* * *

## About the music folder cache
If the folder structure doesn't change, it is now cached for quick viewing.  
When you add a new song folder, the cache will be cleared and reloaded.  
Also, if you want to clear the cache at will, you delete listFolder.csv and listMusicDict.csv.  
![Picture](https://cdn-ak.f.st-hatena.com/images/fotolife/m/m_training/20200620/20200620110003.png)  
  

* * *
  

## History
- 20200620 : Added random song selection, cached folders to make them appear faster, and tweaked Music ball graphics.
- 20200531 : Set damage low, display play time, remove noise through audio mixier, support extended BPM (09), load video in mp4 no matter what the bms says.
- 20200524 : Added Avator mode, Extend the maximum song time to 100,000 frames, Changed to skip the last data if the data part is not a multiple of 2, BMP declarations are skipped when they are duplicated
- 20200516 : Added category function, added rank function, fixed a bug where MusicBall slips through, fixed a bug where MusicBall flies to another place, set the recovery rate of power low.
- 20200510 : MP4 is now supported.
- 20200509_2 : Bug fix : Fixed moving records bug.(There was something wrong with the way the records were arranged.)
- 20200509 : Added "Excellent", Saving high score, Fixed moving records, Changeing METs(2 panch/sec -> 3 panch/sec), ZERO explanation.
- 20200507 : Corresponded ".bml", Fixed reading music bug at music select scene.
- 20200506 : Changed the calorie UI.
- 20200505 : Fixed Changing BPM channel(03) bug. 
- 20200504 : First release.