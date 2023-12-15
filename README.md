# <img src='https://github.com/kris701/TopBarSharp/assets/22596587/6b275939-4c25-4453-934e-c9f8ac7101b0' width='25'> TopBarSharp

Ever wanted some widgets on your desktop, but gets annoyed that they are in the way most of the time and making so you cant see your wallpaper?
Well here is the solution! TopBarSharp allows you to select a window, and make it hide itself in the top of your screen.
When you take your cursor up into the small visible part, it will pop back down so you can see it in its entirety.
When you start the program, it will appear as a tray item on the taskbar:

![image](https://github.com/kris701/TopBarSharp/assets/22596587/53e7c042-45ae-4ec1-abb4-3a82034bed23)

Click "Target" button to select a new window to target, you now have 3 seconds to get your cursor over the window you want (generally its a good idea to hover the cursor over the top bar of a window)
When its selected, the label under the button will show the current target, if it looks right, click start and the window will pop into the top of your screen.

![image](https://github.com/kris701/TopBarSharp/assets/22596587/edfb4362-e3d0-475e-b171-7041048e57e5)

To make the window reappear, simply hover your cursor over the little bit thats visible or by bringing the window into focus.

![image](https://github.com/kris701/TopBarSharp/assets/22596587/2c9e9687-f059-4a13-bbc1-56716c319a48)

The window will pop back up, when your cursor goes outside the bounds of the window.
Your selected window will be saved, so the next time the program starts it will try and find the same window again (based on .exe name and window title). When it starts, it will try 10 times with a 5 sec delay in between to find the window, if it cannot find the window in that time, it will give up.

And thats it! Hope you enjoy it!
