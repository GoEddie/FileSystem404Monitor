# File System 404 Monitor

This watches all file open / creates and waits for failures, if we get a failure and no success then the filename is displayed along with which paths and error codes occured.

# What? Why?

If you are trying to find out which files an application cannot open or create you can use the great procmon (sysinternals) but sometimes windows searches lots of folders trying to find a file and then succeeds or maybe fails, because of all these false positives it is hard to set a filter in procmon and find any files which are missing or cannot be opened for some reason.

# Why did you do it?

I did it because I got fed up of apps saying "Can't find file" or some such nonsense without teling you more detail or without having to spend an inordinate amount of time in procmon trying to figure out why the bleeping app does whatever it does.

# Support

Email me, I am sure you can find me somehow and I will help you!

Ed

